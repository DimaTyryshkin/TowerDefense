using Game.CoreGame;
using Game.CoreGame.Gui;
using Game.SortedTiles;
using GamePackages.Core;
using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    class GameSceneEntryPoint : MonoBehaviour
    {
        [SerializeField, IsntNull] Grid grid;
        [SerializeField, IsntNull] GuiHit guihit;
        [SerializeField, IsntNull] Camera gameCamera;
        [SerializeField, IsntNull] GameObject gameOver;
        [SerializeField, IsntNull] RangeVisualizer rangeVfx;
        [SerializeField, IsntNull] EnemySpawner enemySpawner;
        [SerializeField, IsntNull] SpriteRenderer towerPreview;
        [SerializeField, IsntNull] TowerShopView towerShopView;
        [SerializeField, IsntNull] TargetForEnemy targetForEnemy;
        [SerializeField, IsntNull] BuildingPlayerInput buildPlayerInput;
        [SerializeField, IsntNull] SortedTilesSystem sortedTilesSystem;

        [Header("Buildings")]
        [SerializeField, IsntNull] ShopItem shopItem01;
        [SerializeField, IsntNull] WeaponTowerAI tower01;

        [SerializeField, IsntNull] ShopItem shopItem02;
        [SerializeField, IsntNull] TowerCurrencyGenerator tower02;

        Injector injector;
        GridWrapper gridWrapper;
        BuildingsOnBoardColelction buildingsOnBoard;
        HealthComponentOnBoardCollection enemyOnBoard;


        [Space]
        [SerializeField] int starMoney;
        Currency playerBank;
        ShopButtonState lastSelectedState;
        ShopButtonState[] shopButtonsStates;


        private void Start()
        {
            towerPreview.gameObject.SetActive(false);
            gameOver.SetActive(false);

            rangeVfx.StopAndCelar();

            enemyOnBoard = new();
            buildingsOnBoard = new();
            gridWrapper = new GridWrapper(grid);
            playerBank = new Currency(starMoney);
            HealthComponentOnBoardCollection targetsForEnmey = new();

            RangeWeaponComponent weapon01 = tower01.GetComponent<RangeWeaponComponent>();
            Assert.IsNotNull(weapon01);
            TowerWithAtackRangeBrush tower01Brush = new(weapon01.AttackRange, shopItem01.Sprite);

            NoAttackTowerBrush tower02Brush = new(shopItem02.Sprite);

            targetsForEnmey.Add(targetForEnemy.HealthComponent);
            targetForEnemy.HealthComponent.Init();

            injector = new Injector();
            injector.Register(guihit);
            injector.Register(rangeVfx);
            injector.Register(gameCamera);
            injector.Register(gridWrapper);
            injector.Register(buildingsOnBoard);
            injector.Register(sortedTilesSystem).LinkTilesFromTileMaps();

            injector.Inject(towerShopView);
            injector.Inject(buildPlayerInput, towerPreview).Init();
            injector.Inject(enemySpawner, enemyOnBoard, targetsForEnmey).Init();
            injector.Inject(tower01Brush, towerPreview);
            injector.Inject(tower02Brush, towerPreview);

            // == Buildings ==

            shopButtonsStates = new ShopButtonState[towerShopView.MaxButtonAmount];
            shopButtonsStates[0] = new ShopButtonState(cost: shopItem01.Cost, sprite: shopItem01.Sprite, onClick: () => buildPlayerInput.SetBrush(tower01Brush));
            shopButtonsStates[1] = new ShopButtonState(cost: shopItem02.Cost, sprite: shopItem02.Sprite, onClick: () => buildPlayerInput.SetBrush(tower02Brush));
            shopButtonsStates[2] = new ShopButtonState(cost: shopItem01.Cost, sprite: shopItem01.Sprite, onClick: () => buildPlayerInput.SetBrush(tower01Brush));
            shopButtonsStates[3] = new ShopButtonState(cost: shopItem01.Cost, sprite: shopItem01.Sprite, onClick: () => buildPlayerInput.SetBrush(tower01Brush));

            // == SetupFlow ==

            towerShopView.ClickButon += (ShopButtonView button) =>
            {
                if (playerBank >= button.State.Cost)
                {
                    towerShopView.Hide();
                    lastSelectedState = button.State;
                    button.State.OnClick.Invoke();
                }
            };

            towerShopView.ClickStartWave += () =>
            {
                towerShopView.Hide();
                enemySpawner.StartWave();
            };

            buildPlayerInput.CancelBuilding += () =>
            {
                lastSelectedState = null;
                towerShopView.Show();
            };

            enemySpawner.WaveEnd += () =>
            {
                StartCoroutine(OnWaveEnd());
            };

            enemySpawner.GameOver += () =>
            {
                gameOver.SetActive(true);
            };

            tower01Brush.ClickBuild += cell =>
            {
                if (playerBank >= lastSelectedState.Cost)
                {
                    lastSelectedState.wasBuilded = true;
                    playerBank -= lastSelectedState.Cost;
                    InstantiateTower01(cell);

                    buildPlayerInput.StopBuilding();
                    towerShopView.Darw(playerBank, shopButtonsStates);
                    towerShopView.Show();
                }
            };

            tower02Brush.ClickBuild += cell =>
            {
                if (playerBank >= lastSelectedState.Cost)
                {
                    lastSelectedState.wasBuilded = true;
                    playerBank -= lastSelectedState.Cost;
                    InstantiateTower02(cell);

                    buildPlayerInput.StopBuilding();
                    towerShopView.Darw(playerBank, shopButtonsStates);
                    towerShopView.Show();
                }
            };

            towerShopView.Darw(playerBank, shopButtonsStates);
        }

        IEnumerator OnWaveEnd()
        {
            yield return new WaitForSeconds(0.3f);

            playerBank += new Currency(2);
            foreach (TowerCurrencyGenerator t in buildingsOnBoard.GetAllByType<TowerCurrencyGenerator>())
            {
                t.ShowEffect();
                playerBank += t.MoneyAddPedWaveAmount;
                yield return new WaitForSeconds(0.3f);
            }

            towerShopView.Darw(playerBank, shopButtonsStates);
            towerShopView.Show();
        }

        private void InstantiateTower01(Vector2Int cell)
        {
            WeaponTowerAI newTower = Instantiate(tower01);
            newTower.transform.position = gridWrapper.CellToWorld(cell);
            newTower.gameObject.SetActive(true);
            sortedTilesSystem.LinkGameObject(newTower.gameObject);
            buildingsOnBoard[cell] = newTower.gameObject;
            newTower.Init(enemyOnBoard);
        }

        private void InstantiateTower02(Vector2Int cell)
        {
            TowerCurrencyGenerator newTower = Instantiate(tower02);
            newTower.transform.position = gridWrapper.CellToWorld(cell);
            newTower.gameObject.SetActive(true);
            sortedTilesSystem.LinkGameObject(newTower.gameObject);
            buildingsOnBoard[cell] = newTower.gameObject;
        }
    }
}
