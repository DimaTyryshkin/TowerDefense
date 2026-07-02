using Game.CoreGame;
using Game.CoreGame.Gui;
using Game.SortedTiles;
using GamePackages.Core;
using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    class ShopItemState
    {
        internal ShopItem shopItem;
        internal bool wasBuilded;
    }

    class GameSceneEntryPoint : MonoBehaviour
    {
        [SerializeField, IsntNull] Camera gameCamera;
        [SerializeField, IsntNull] TargetForEnemy targetForEnemy;
        [SerializeField, IsntNull] GameObject gameOver;
        [SerializeField, IsntNull] GuiHit guihit;
        [SerializeField, IsntNull] EnemySpawner enemySpawner;
        [SerializeField, IsntNull] BuildTowerBrush buildTowerBrush;
        [SerializeField, IsntNull] BuildPlayerInput buildPlayerInput;
        [SerializeField, IsntNull] TowerShopView towerShopView;
        [SerializeField, IsntNull] SortedTilesSystem sortedTilesSystem;

        [Space]
        [SerializeField, IsntNull] ShopItem towerShopItem;

        Injector injector;


        [SerializeField] int starMoney;
        Currency playerBank;
        ShopItemState lastSelectedState;


        private void Start()
        {
            gameOver.SetActive(false);

            HealthComponentOnBoardCollection enemyes = new();
            HealthComponentOnBoardCollection targetsForEnmey = new();

            targetsForEnmey.Add(targetForEnemy.HealthComponent);
            targetForEnemy.HealthComponent.Init();


            injector = new Injector();
            injector.Register(gameCamera);
            injector.Register(guihit);
            injector.Register(sortedTilesSystem);

            injector.Inject(towerShopView);
            injector.Inject(buildTowerBrush, enemyes).Init();
            injector.Inject(buildPlayerInput, buildTowerBrush).Init();
            injector.Inject(enemySpawner, enemyes, targetsForEnmey).Init();



            // == SetupFlow ==
            playerBank = new Currency(starMoney);

            ShopItemState[] shopStates = new ShopItemState[towerShopView.MaxButtonAmount];
            shopStates[0] = new ShopItemState { shopItem = towerShopItem };
            shopStates[1] = new ShopItemState { shopItem = towerShopItem };
            shopStates[2] = new ShopItemState { shopItem = towerShopItem };
            shopStates[3] = new ShopItemState { shopItem = towerShopItem };
            shopStates[4] = new ShopItemState { shopItem = towerShopItem };

            towerShopView.Darw(playerBank, shopStates);
            towerShopView.SelectTower += (TowerButtonView button) =>
            {
                if (playerBank >= button.ShopItemState.shopItem.Cost)
                {
                    towerShopView.Hide();
                    lastSelectedState = button.ShopItemState;
                    buildPlayerInput.SetTower(button.ShopItemState.shopItem.Tower);
                }
            };

            towerShopView.ClickStartWave += () =>
            {
                towerShopView.Hide();

                enemySpawner.StartWave();

            };

            buildPlayerInput.CancelBuild += () =>
            {
                lastSelectedState = null;
                towerShopView.Show();
            };

            buildPlayerInput.Build += () =>
            {
                Assert.IsNotNull(lastSelectedState);
                playerBank -= lastSelectedState.shopItem.Cost;
                lastSelectedState.wasBuilded = true;
                towerShopView.Darw(playerBank, shopStates);
                towerShopView.Show();
            };

            enemySpawner.WaveEnd += () =>
            {
                playerBank += new Currency(2);
                towerShopView.Darw(playerBank, shopStates);
                towerShopView.Show();
            };

            enemySpawner.GameOver += () =>
            {
                gameOver.SetActive(true);
            };

            //enemySpawner.StartWave();
        }

        private void EnemySpawner_WaveStart()
        {
            throw new System.NotImplementedException();
        }
    }
}
