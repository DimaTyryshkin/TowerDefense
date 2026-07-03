using Game.CoreGame;
using Game.CoreGame.Gui;
using Game.SortedTiles;
using GamePackages.Core;
using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using UnityEngine;

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
        [SerializeField, IsntNull] ShopItem tower01;

        Injector injector;
        GridWrapper gridWrapper;
        BuildingsOnBoardColelction buildingsOnBoard;
        HealthComponentOnBoardCollection enemyOnBoard;


        [Space]
        [SerializeField] int starMoney;
        Currency playerBank;
        ShopButtonState lastSelectedState;


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
            TowerWithAtackRangeBrush towerBrush = new TowerWithAtackRangeBrush(tower01.Tower.AttackRange, tower01.Sprite);

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
            injector.Inject(towerBrush, towerPreview);

            // == Buildings ==

            ShopButtonState[] shopButtonsStates = new ShopButtonState[towerShopView.MaxButtonAmount];
            shopButtonsStates[0] = new ShopButtonState(cost: tower01.Cost, sprite: tower01.Sprite, onClick: () => buildPlayerInput.SetBrush(towerBrush));
            shopButtonsStates[1] = new ShopButtonState(cost: tower01.Cost, sprite: tower01.Sprite, onClick: () => buildPlayerInput.SetBrush(towerBrush));
            shopButtonsStates[2] = new ShopButtonState(cost: tower01.Cost, sprite: tower01.Sprite, onClick: () => buildPlayerInput.SetBrush(towerBrush));
            shopButtonsStates[3] = new ShopButtonState(cost: tower01.Cost, sprite: tower01.Sprite, onClick: () => buildPlayerInput.SetBrush(towerBrush));

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
                playerBank += new Currency(2);
                towerShopView.Darw(playerBank, shopButtonsStates);
                towerShopView.Show();
            };

            enemySpawner.GameOver += () =>
            {
                gameOver.SetActive(true);
            };

            towerBrush.ClickBuild += cell =>
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

            towerShopView.Darw(playerBank, shopButtonsStates);
        }


        private void InstantiateTower01(Vector2Int cell)
        {
            TowerAI newTower = gridWrapper.grid.transform.InstantiateAsChild(tower01.Tower);
            newTower.transform.position = gridWrapper.CellToWorld(cell);
            newTower.gameObject.SetActive(true);
            sortedTilesSystem.LinkGameObject(newTower.gameObject);
            buildingsOnBoard[cell] = newTower.gameObject;
            newTower.Init(enemyOnBoard);
        }
    }
}
