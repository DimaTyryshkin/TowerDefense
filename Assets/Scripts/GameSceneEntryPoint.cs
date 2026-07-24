using Game.Common;
using Game.CoreGame;
using Game.CoreGame.Gui;
using Game.SortedTiles;
using Game.Upgrades;
using GamePackages.Core;
using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    class GameSceneEntryPoint : MonoBehaviour
    {
        [SerializeField, IsntNull] Grid grid;
        [SerializeField, IsntNull] GuiHit guihit;
        [SerializeField, IsntNull] Camera gameCamera;
        [SerializeField, IsntNull] GameOverPanel gameOver;
        [SerializeField, IsntNull] Button restart;
        [SerializeField, IsntNull] UpgradeTree upgradeTree;
        [SerializeField, IsntNull] RangeVisualizer rangeVfx;
        [SerializeField, IsntNull] EnemySpawner enemySpawner;
        [SerializeField, IsntNull] SpriteRenderer towerPreview;
        [SerializeField, IsntNull] TowerShopView towerShopView;
        //[SerializeField, IsntNull] TargetForEnemy targetForEnemy;
        [SerializeField, IsntNull] BuildingPlayerInput buildPlayerInput;
        [SerializeField, IsntNull] SortedTilesSystem sortedTilesSystem;
        [SerializeField, IsntNull] DebugPanel debugPanel;
        [SerializeField, IsntNull] BombTimer bombTimer;

        [Header("Buildings")]
        [SerializeField, IsntNull] BuildingsCollection buildingsCollection;

        [Header("Gui")]
        [SerializeField, IsntNull] TMP_Text waveNumber;

        Injector injector;
        GridWrapper gridWrapper;
        BuildingsOnBoardColelction buildingsOnBoard;
        HealthComponentOnBoardCollection enemyOnBoard;


        [SerializeField] float startTimeScele = 5;
        [Space]
        [SerializeField, IsntNull] UpgradeData startMoney;
        [SerializeField, IsntNull] UpgradeData waveReward;
        [SerializeField, IsntNull] UpgradeData startUpgrade;

        Currency playerBank;
        ShopButtonState lastSelectedState;
        ShopButtonState[] shopButtonsStates;

        int StartMoney => startMoney.IntValue;
        int WeveReward => waveReward.IntValue;

        int bombCounter;

        private void Start()
        {
            startUpgrade.SetOne();
            towerPreview.gameObject.SetActive(false);
            gameOver.Hide();

            rangeVfx.StopAndCelar();

            enemyOnBoard = new();
            buildingsOnBoard = new();
            gridWrapper = new GridWrapper(grid);
            playerBank = new Currency(StartMoney);
            HealthComponentOnBoardCollection targetsForEnmey = new();

            //targetsForEnmey.Add(targetForEnemy.DamageReceiver);
            //targetForEnemy.DamageReceiver.Health.Init();

            injector = new Injector();
            injector.Register(guihit);
            injector.Register(rangeVfx);
            injector.Register(gameCamera);
            injector.Register(gridWrapper);
            injector.Register(buildingsOnBoard);
            injector.Register(sortedTilesSystem).LinkTilesFromTileMaps();// не регистрировать

            injector.Inject(towerShopView);
            injector.Inject(buildPlayerInput, towerPreview).Init();
            injector.RegisterAndInject(enemySpawner, enemyOnBoard, targetsForEnmey).ResetWaves();
            injector.Inject(debugPanel);


            // == Buildings ==
            shopButtonsStates = new ShopButtonState[towerShopView.MaxButtonAmount];

            int n = 1;
            foreach (var buildingInfo in buildingsCollection.buildigs)
            {
                bool isOpeend = buildingInfo.upgradeData.Value > 0;
                var shopItem = buildingInfo.shopItem;
                var towerAi = shopItem.GetComponent<WeaponTowerAI>();
                var rangeWeapon = shopItem.GetComponent<RangeWeaponComponent>();
                if (rangeWeapon)
                {
                    SetupRangeWeaponTower(n, isOpeend, injector, towerAi, shopItem);
                }
                else
                {
                    SetupNoAttackTower(n, isOpeend, injector, shopItem, shopItem, newTower =>
                    {
                        // newTower.GetCompnent<T>().Init()
                    });
                }

                if (isOpeend)
                    n++;
            }

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
                if (enemySpawner.StartWave())
                {
                    DrawWaveNumber();
                    Time.timeScale = startTimeScele;
                    towerShopView.Hide();
                }
            };

            buildPlayerInput.CancelBuilding += () =>
            {
                lastSelectedState = null;
                towerShopView.Show();
            };

            enemySpawner.WaveEnd += () =>
            {
                if (bombCounter == 0)
                    return;

                ResetTimeScale();
                StartCoroutine(OnWaveEnd());
            };

            enemySpawner.EnemyFinishMove += () =>
            {
                if (bombCounter == 0)
                    return;

                bombCounter--;
                bombTimer.SetCount(bombCounter);

                if (bombCounter == 0)
                {
                    ResetTimeScale();
                    gameOver.Show();
                }
            };

            gameOver.Click += () =>
            {
                gameOver.Hide();
                upgradeTree.Show();
            };

            upgradeTree.Close += () =>
            {
                GameFactory.Data.Save();
                RestartGame();
            };

            // === Debug ===


            debugPanel.AddButton("Restart", () =>
            {
                GameFactory.Storage.Clear();
                RestartGame();
            });


            //debugPanel.AddButton("Wave-10", () =>
            //{
            //    playerBank += new Currency(WeveReward * (10 - enemySpawner.WaveIndex));
            //    enemySpawner.DebugSetwaveIndex(10);
            //    DrawWaveNumber();
            //    towerShopView.Draw(playerBank, shopButtonsStates);
            //});

            //debugPanel.ClickAddMoney += () =>
            //{
            //    playerBank += new Currency(10);
            //    towerShopView.Draw(playerBank, shopButtonsStates);
            //}; 


            restart.onClick.AddListener(() =>
            {
                //targetForEnemy.Debug_RestertGame();
                gameOver.Hide();
                towerShopView.Show();
            });

            // === Start Game === 
            bombCounter = 10;
            bombTimer.SetCount(bombCounter);
            enemySpawner.ResetWaves();
            towerShopView.Draw(playerBank, shopButtonsStates);
        }

        void DrawWaveNumber()
        {
            waveNumber.text = (enemySpawner.WaveIndex + 1).ToString();
        }

        void RestartGame()
        {
            int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(activeSceneIndex);
        }

        IEnumerator OnWaveEnd()
        {
            yield return new WaitForSeconds(0.3f);

            playerBank += new Currency(WeveReward);
            foreach (TowerCurrencyGenerator t in buildingsOnBoard.GetAllByType<TowerCurrencyGenerator>())
            {
                t.ShowEffect();
                playerBank += t.MoneyAddPedWaveAmount;
                yield return new WaitForSeconds(0.3f);
            }

            towerShopView.Draw(playerBank, shopButtonsStates);
            towerShopView.Show();
        }

        void SetupRangeWeaponTower(int numberInShop, bool isAvailableInShop, Injector injector, WeaponTowerAI towerPrefab, ShopItem shopItem)
        {
            Assert.IsNotNull(towerPrefab);
            Assert.IsNotNull(shopItem);

            RangeWeaponComponent weaponComponent = towerPrefab.GetComponent<RangeWeaponComponent>();
            TowerWithAtackRangeBrush towerBrush = new(() => weaponComponent.AttackRange, shopItem.Sprite);
            injector.Inject(towerBrush, towerPreview);

            if (isAvailableInShop)
                shopButtonsStates[numberInShop - 1] = new ShopButtonState(cost: shopItem.Cost, sprite: shopItem.Sprite, onClick: () => buildPlayerInput.SetBrush(towerBrush));

            towerBrush.ClickBuild += cell =>
            {
                if (playerBank >= lastSelectedState.Cost)
                {
                    //lastSelectedState.wasBuilded = true;
                    playerBank -= lastSelectedState.Cost;
                    WeaponTowerAI newTower = InstantiateTower(towerPrefab, cell);
                    newTower.Init(enemyOnBoard);
                    newTower.GetComponent<RangeWeaponComponent>().TargetnInRange += ResetTimeScale;

                    buildPlayerInput.StopBuilding();
                    towerShopView.Draw(playerBank, shopButtonsStates);
                    towerShopView.Show();
                    OnBuildAnyTower();
                }
            };
        }

        void ResetTimeScale()
        {
            if (Time.timeScale > 1)
                Time.timeScale = 1;
        }

        void OnBuildAnyTower()
        {
            GameFactory.Data.upgradePoints++;
        }

        void SetupNoAttackTower<T>(int numberInShop, bool isAvailableInShop, Injector injector, T towerPrefab, ShopItem shopItem, UnityAction<T> initNewTower)
            where T : MonoBehaviour
        {
            NoAttackTowerBrush towerBrush = new(shopItem.Sprite);
            injector.Inject(towerBrush, towerPreview);

            shopButtonsStates[numberInShop - 1] = new ShopButtonState(cost: shopItem.Cost, sprite: shopItem.Sprite, onClick: () => buildPlayerInput.SetBrush(towerBrush));

            towerBrush.ClickBuild += cell =>
            {
                if (playerBank >= lastSelectedState.Cost)
                {
                    //lastSelectedState.wasBuilded = true;
                    playerBank -= lastSelectedState.Cost;
                    T newTower = InstantiateTower(towerPrefab, cell);
                    initNewTower.Invoke(newTower);

                    buildPlayerInput.StopBuilding();
                    towerShopView.Draw(playerBank, shopButtonsStates);
                    towerShopView.Show();
                    OnBuildAnyTower();
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
