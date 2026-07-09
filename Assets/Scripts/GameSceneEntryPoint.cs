using Game.CoreGame;
using Game.CoreGame.Gui;
using Game.SortedTiles;
using GamePackages.Core;
using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

        [SerializeField, IsntNull] ShopItem shopItem03;
        [SerializeField, IsntNull] WeaponTowerAI tower03;

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


            // == Buildings ==
            shopButtonsStates = new ShopButtonState[towerShopView.MaxButtonAmount];
            SetupRangeWeaponTower(1, injector, tower01, shopItem01);
            SetupRangeWeaponTower(3, injector, tower03, shopItem03);
            SetupNoAttackTower(2, injector, tower02, shopItem02, newTower => { });

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

        void SetupRangeWeaponTower(int numberInShop, Injector injector, WeaponTowerAI towerPrefab, ShopItem shopItem)
        {
            RangeWeaponComponent weaponComponent = towerPrefab.GetComponent<RangeWeaponComponent>();
            TowerWithAtackRangeBrush towerBrush = new(weaponComponent.AttackRange, shopItem.Sprite);
            injector.Inject(towerBrush, towerPreview);

            shopButtonsStates[numberInShop - 1] = new ShopButtonState(cost: shopItem.Cost, sprite: shopItem.Sprite, onClick: () => buildPlayerInput.SetBrush(towerBrush));

            towerBrush.ClickBuild += cell =>
            {
                if (playerBank >= lastSelectedState.Cost)
                {
                    lastSelectedState.wasBuilded = true;
                    playerBank -= lastSelectedState.Cost;
                    WeaponTowerAI newTower = InstantiateTower(towerPrefab, cell);
                    newTower.Init(enemyOnBoard);

                    buildPlayerInput.StopBuilding();
                    towerShopView.Darw(playerBank, shopButtonsStates);
                    towerShopView.Show();
                }
            };
        }

        void SetupNoAttackTower<T>(int numberInShop, Injector injector, T towerPrefab, ShopItem shopItem, UnityAction<T> initNewTower) where T : MonoBehaviour
        {
            NoAttackTowerBrush towerBrush = new(shopItem.Sprite);
            injector.Inject(towerBrush, towerPreview);

            shopButtonsStates[numberInShop - 1] = new ShopButtonState(cost: shopItem.Cost, sprite: shopItem.Sprite, onClick: () => buildPlayerInput.SetBrush(towerBrush));

            towerBrush.ClickBuild += cell =>
            {
                if (playerBank >= lastSelectedState.Cost)
                {
                    lastSelectedState.wasBuilded = true;
                    playerBank -= lastSelectedState.Cost;
                    T newTower = InstantiateTower(towerPrefab, cell);
                    initNewTower.Invoke(newTower);

                    buildPlayerInput.StopBuilding();
                    towerShopView.Darw(playerBank, shopButtonsStates);
                    towerShopView.Show();
                }
            };
        }

        T InstantiateTower<T>(T towerPrefab, Vector2Int cell) where T : MonoBehaviour
        {
            T newTower = Instantiate(towerPrefab);
            newTower.transform.position = gridWrapper.CellToWorld(cell);
            newTower.gameObject.SetActive(true);
            sortedTilesSystem.LinkGameObject(newTower.gameObject);
            buildingsOnBoard[cell] = newTower.gameObject;
            return newTower;
        }
    }
}
