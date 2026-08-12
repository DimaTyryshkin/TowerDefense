using Game.Assets.Scripts.CoreGame;
using Game.Common;
using Game.CoreGame;
using Game.CoreGame.Gui;
using Game.SortedTiles;
using Game.Upgrades;
using GamePackages.Core;
using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    class GameSceneEntryPoint : MonoBehaviour
    {
        [SerializeField, IsntNull] World world;
        [SerializeField, IsntNull] GuiHit guihit;
        [SerializeField, IsntNull] Camera gameCamera;
        [SerializeField, IsntNull] GameOverPanel gameOver;
        [SerializeField, IsntNull] EnemySpawner enemySpawner;
        [SerializeField, IsntNull] Button restart;
        [SerializeField, IsntNull] UpgradeTree upgradeTree;
        [SerializeField, IsntNull] RangeVisualizer rangeVfx;
        [SerializeField, IsntNull] SpriteRenderer towerPreview;
        [SerializeField, IsntNull] TowerShopView towerShopView;
        //[SerializeField, IsntNull] TargetForEnemy targetForEnemy;
        [SerializeField, IsntNull] BuildingPlayerInput buildPlayerInput;
        [SerializeField, IsntNull] SortedTilesSystem sortedTilesSystem;
        [SerializeField, IsntNull] DebugPanel debugPanel;
        [SerializeField, IsntNull] ContextMenuSystem contextMenuSystem;
        [SerializeField, IsntNull] ContextMenuGui contextMenuGui;
        [SerializeField, IsntNull] ContextMenuSystem ContextMenuSystem;
        [SerializeField, IsntNull] WeaponStatisticSystem weaponStatisticSystem;

        [Header("Buildings")]
        [SerializeField, IsntNull] BuildingsCollection buildingsCollection;
        [SerializeField, IsntNull] Transform blocksRoot;

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
            Dictionary<Vector2Int, bool> blocks = new Dictionary<Vector2Int, bool>();

            foreach (var block in blocksRoot.GetComponentsInChildren<Transform>())
            {
                if (blocksRoot == block)
                    continue;

                Vector2Int cell = (Vector2Int)world.grid.WorldToCell(block.transform.position);
                blocks.Add(cell, true);
                block.position = world.grid.CellToWorld((Vector3Int)cell);
            }

            //
            startUpgrade.SetOne();
            towerPreview.gameObject.SetActive(false);
            gameOver.Hide();

            rangeVfx.StopAndCelar();

            enemyOnBoard = new();
            buildingsOnBoard = new();
            gridWrapper = new GridWrapper(world.grid);
            playerBank = new Currency(StartMoney);
            HealthComponentOnBoardCollection targetsForEnmey = new();

            //targetsForEnmey.Add(targetForEnemy.DamageReceiver);
            //targetForEnemy.DamageReceiver.Health.Init();

            injector = new Injector();
            injector.Register(blocks);
            injector.Register(guihit);
            injector.Register(rangeVfx);
            injector.Register(gameCamera);
            injector.Register(gridWrapper);
            injector.Register(buildingsOnBoard);
            injector.Register(sortedTilesSystem).LinkTilesFromTileMaps();// не регистрировать

            injector.Inject(towerShopView);
            injector.Inject(contextMenuSystem);
            injector.Inject(buildPlayerInput, towerPreview).Init();

            injector.RegisterAndInject(enemySpawner, world.enemySpawnPoints, enemyOnBoard, targetsForEnmey).ResetWaves();
            injector.Inject(debugPanel);


            // == Buildings ==
            shopButtonsStates = new ShopButtonState[towerShopView.MaxButtonAmount];

            int n = 1;
            foreach (var buildingInfo in buildingsCollection.buildigs)
            {
                bool isOpeend = buildingInfo.upgradeData.Value > 0;
                var shopItem = buildingInfo.shopItem;
                var towerAi = shopItem.GetComponent<WeaponTowerComposer>();
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
                contextMenuGui.Hide();
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
                    towerShopView.Hide();
                    contextMenuGui.Hide();
                    Time.timeScale = startTimeScele;
                }
            };

            buildPlayerInput.ClickOnTower += (tower) =>
            {
                if (enemySpawner.InWave)
                    return;

                ContextMenuSystem.ShowAtWorldPoint(tower.transform.position, contextMenuGui.RectTransform);
                contextMenuGui.Show(tower);
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

            enemySpawner.EnemySpawed += (enemyHealth) =>
            {
                weaponStatisticSystem.OnSpawnEnmey(enemyHealth);
            };

            enemySpawner.EnemyFinishMove += () =>
            {
                if (bombCounter == 0)
                    return;

                bombCounter--;
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

            contextMenuGui.ClickSoldTower += (shopItem) =>
            {
                if (enemySpawner.InWave)
                    return;

                contextMenuGui.Hide();
                playerBank += shopItem.Cost;
                buildingsOnBoard.RemveValue(shopItem.gameObject);
                towerShopView.Draw(playerBank, shopButtonsStates);
                Destroy(shopItem.gameObject);
            };

            // === Debug ===

            debugPanel.AddButton("AddMoney", () =>
            {
                playerBank += new Currency(10);
                towerShopView.DrawPlayerBank(playerBank);
            });

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
            enemySpawner.ResetWaves();
            contextMenuGui.Hide();
            towerShopView.Draw(playerBank, shopButtonsStates);
        }

        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame)
                return;

            contextMenuGui.Hide();
        }

        void DrawWaveNumber()
        {
            waveNumber.text = $"{enemySpawner.WaveIndex + 1} / {enemySpawner.WaveTotalAmount}";
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

        void SetupRangeWeaponTower(int numberInShop, bool isAvailableInShop, Injector injector, WeaponTowerComposer towerPrefab, ShopItem shopItem)
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
                    WeaponTowerComposer newTower = InstantiateTower(towerPrefab, cell);
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
            buildingsOnBoard[cell] = newTower.gameObject;
            return newTower;
        }


    }
}
