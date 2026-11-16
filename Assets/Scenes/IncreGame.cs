using Game.Common;
using Game.Upgrades;
using GamePackages.Audio;
using GamePackages.Core;
using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Rnd = UnityEngine.Random;

namespace IncreGame
{
    public class IncreGame : MonoBehaviour
    {
        [SerializeField, IsntNull] Camera gameCamera;
        [SerializeField, IsntNull] Camera canvasCamera;
        [SerializeField, IsntNull] GuiHit guiHit;
        [SerializeField, IsntNull] Transform cursor;
        [SerializeField, IsntNull] Transform p1;
        [SerializeField, IsntNull] Transform p2;
        [SerializeField, IsntNull] Transform firstEnemyPos;
        [SerializeField, IsntNull] UpgradeTree upgradeTree;

        [Space]
        [SerializeField] int startSpawn = 1;
        [SerializeField] float respawnRange = 2;
        [SerializeField] float offsetTocenter = 0.5f;
        [SerializeField] float woodFlyRange = 1;

        [Label("На сколько деревьев надо накопить, чтобы открыть магазин")]
        [SerializeField] int excessMoneyToOpenShop;

        [Label("Cколько деревьев надо посадить, чтобы открыть прокачку")]
        [SerializeField] int enemyesToOpenUpgrades;
        [SerializeField, IsntNull] AppSounds appSounds;

        [Header("Pause")]
        [SerializeField, IsntNull] GameObject pausePanel;
        [SerializeField, IsntNull] Button addMoneyButton;
        [SerializeField, IsntNull] Button restartButton;

        [Header("Enemy")]
        [SerializeField] float spawnDelay;
        [SerializeField] float enemySpeed;
        [SerializeField] float newEnemySpawnDistace;
        [SerializeField] float enemySpawnDistace;
        [SerializeField, IsntNull] Enemy enemy1Prefab;
        [SerializeField, IsntNull] Enemy enemy2Prefab;
        [SerializeField, IsntNull] Enemy enemy3Prefab;
        [SerializeField, IsntNull] Enemy enemy4Prefab;

        [Header("Boss")]
        [SerializeField] float bossRadius;
        [SerializeField, IsntNull] Transform boss;


        [Header("Push")]
        [SerializeField] float pushTime;
        [SerializeField] float pushSpeed;
        [SerializeField, IsntNull] SoundsSet pushHit;
        [SerializeField, IsntNull] SoundsSet bossHit;

        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 addEnemy1Upgrade;
        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 addEnemy2Upgrade;
        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 addEnemy3Upgrade;
        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 addEnemy4Upgrade;

        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 openEnemyUpgrade02;
        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 openEnemyUpgrade03;
        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 openEnemyUpgrade04;

        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 spawnEnemyUpgrade01;
        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 spawnEnemyUpgrade02;
        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 spawnEnemyUpgrade03;
        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 spawnEnemyUpgrade04;

        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 damageEnemyUpgrade01;
        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 damageEnemyUpgrade02;
        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 damageEnemyUpgrade03;
        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 damageEnemyUpgrade04;

        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 truckCapacityUpgrade01;
        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 truckCapacityUpgrade02;
        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 truckCapacityUpgrade03;

        [Foldout("Upgrade"), SerializeField, IsntNull] UpgradeData2 addRangeUpgrade;


        [Foldout("GUI"), SerializeField, IsntNull] TMP_Text truckAmountText;
        [Foldout("GUI"), SerializeField, IsntNull] Button openUpgradeButton;
        [Foldout("GUI"), SerializeField, IsntNull] TMP_Text playerMoneyText;
        [Foldout("GUI"), SerializeField, IsntNull] GameObject leftPanel;
        [Foldout("GUI"), SerializeField, IsntNull] GameObject topPanel;
        [Foldout("GUI"), SerializeField, IsntNull] GameObject shopPanel;
        [Foldout("GUI"), SerializeField, IsntNull] GameObject upgradePanel;
        [Foldout("GUI"), SerializeField, IsntNull] GameObject upgradesMenuButton;

        [Foldout("GUI"), SerializeField, IsntNull] Transform playerMoneyRoot;
        [Foldout("GUI"), SerializeField, IsntNull] AnimationCurve moneyAnimation;

        [Foldout("GUI"), SerializeField, IsntNull] UpgradeButton addEnemy1Button;
        [Foldout("GUI"), SerializeField, IsntNull] UpgradeButton addEnemy2Button;
        [Foldout("GUI"), SerializeField, IsntNull] UpgradeButton addEnemy3Button;
        [Foldout("GUI"), SerializeField, IsntNull] UpgradeButton addEnemy4Button;


        [Header("FlyMoney")]
        [SerializeField, IsntNull] Transform moneyGuiPoint;
        [SerializeField, IsntNull] FlyMoneyAnimation moneyInFlyPrefab;


        [Foldout("Truck"), SerializeField, IsntNull] GameObject truck1;
        [Foldout("Truck"), SerializeField, IsntNull] GameObject truck2;
        [Foldout("Truck"), SerializeField, IsntNull] GameObject truck3;
        [Foldout("Truck"), SerializeField, IsntNull] ProgressBarWithText truskProgressBar;

        [Header("Seeding")]
        //[SerializeField, IsntNull] int seedingCost1cost;
        [SerializeField, IsntNull] RectTransform seedingSpawn;
        [SerializeField, IsntNull] FlySeedlingAnimator flySeedlingAnimator;

        [Header("Wood")]
        [SerializeField] float woodRadius;
        [SerializeField, IsntNull] GameObject woodPrefab;
        [SerializeField, IsntNull] FlySeedlingAnimator woodFlyPrefab;

        [Space]
        [SerializeField, IsntNull] DebugSettings debugSettings;

        Rect fieldRect;
        Vector2 cursorPos;
        Vector2 bossPos;
        Vector2 moneyWorldPoint;
        Vector2 buySeedingButtonWorldPoint;

        Vector2 oldCursorPos;
        int playerMoney;
        int truckMoney;
        int growOrder1;
        float timeNextGrow;
        float moneyAnimationStart;
        DynamicObjectPool pool;

        List<Enemy> enemyes;
        List<SpriteRenderer> woods;
        List<UpgradeRule> upgradeList;

        [Range(0, 50), SerializeField]
        float drafeSpeed;
        bool step2 => true;

        bool isPause;
        static bool ReloadAfterUpgrade { get; set; }

        IEnumerator Start()
        {
            if (!ReloadAfterUpgrade)
                GameFactory.Storage.Clear();

            pausePanel.SetActive(false);
            openUpgradeButton.gameObject.SetActive(false);
            addMoneyButton.onClick.AddListener(AddMoney);
            restartButton.onClick.AddListener(() =>
            {
                ReloadAfterUpgrade = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });

            openUpgradeButton.onClick.AddListener(() =>
            {
                isPause = true;
                upgradeTree.UpgradePoints = truckCapacity;
                upgradeTree.Show();
            });

            upgradeTree.Close += () =>
            {
                ReloadAfterUpgrade = true;
                //GameFactory.Data.Save();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            };

            enemyes = new();
            woods = new();
            upgradeList = new();
            pool = DynamicObjectPool.GetInst();

            bossPos = boss.position;
            boss.localScale = Vector3.one * (bossRadius * 2);

            fieldRect = new Rect((Vector2)p1.position, (Vector2)(p2.position - p1.position));
            playerMoney = 0;

            truskProgressBar.Draw(0, 100);
            playerMoneyText.text = "0";
            truckAmountText.text = "0";
            enemy1Prefab.gameObject.SetActive(false);
            enemy2Prefab.gameObject.SetActive(false);
            enemy3Prefab.gameObject.SetActive(false);
            enemy4Prefab.gameObject.SetActive(false);
            woodPrefab.gameObject.SetActive(false);
            woodFlyPrefab.gameObject.SetActive(false);
            moneyInFlyPrefab.gameObject.SetActive(false);
            flySeedlingAnimator.gameObject.SetActive(false);
            topPanel.SetActive(true);
            leftPanel.SetActive(true);
            shopPanel.SetActive(true);
            upgradePanel.SetActive(false);

            appSounds.Init(new AppAudioAccountData());
            AppSounds.SetAsSceneSound(appSounds);

            SetupRules();

            GetWorldPositions();

            addEnemy1Upgrade.SetLevel(spawnEnemyUpgrade01.IntValue);
            addEnemy2Upgrade.SetLevel(spawnEnemyUpgrade02.IntValue);
            addEnemy3Upgrade.SetLevel(spawnEnemyUpgrade03.IntValue);
            addEnemy4Upgrade.SetLevel(spawnEnemyUpgrade04.IntValue);

            Debug.Log($"openEnemyUpgrade02.IsOpen={openEnemyUpgrade02.IsOpen}");
            addEnemy1Button.gameObject.SetActive(true);
            addEnemy2Button.gameObject.SetActive(openEnemyUpgrade02.IsOpen);
            addEnemy3Button.gameObject.SetActive(openEnemyUpgrade03.IsOpen);
            addEnemy4Button.gameObject.SetActive(openEnemyUpgrade04.IsOpen);

            for (int i = 0; i < spawnEnemyUpgrade01.IntValue; i++)
                SpawnNewEnmey(enemy1Prefab);

            for (int i = 0; i < spawnEnemyUpgrade02.IntValue; i++)
                SpawnNewEnmey(enemy2Prefab);

            for (int i = 0; i < spawnEnemyUpgrade03.IntValue; i++)
                SpawnNewEnmey(enemy3Prefab);

            for (int i = 0; i < spawnEnemyUpgrade04.IntValue; i++)
                SpawnNewEnmey(enemy4Prefab);

            enemyes[0].transform.position = (Vector2)firstEnemyPos.position;

            DrawGui();

            yield return null;
            GetWorldPositions();
        }


        void GetWorldPositions()
        {
            moneyWorldPoint = GuiHit.GetWorldPoint(moneyGuiPoint, gameCamera);
            buySeedingButtonWorldPoint = GuiHit.GetWorldPoint(seedingSpawn, gameCamera);
        }

        [SerializeField] float cursorSpeedFacor;

        void Update()
        {
            if (isPause)
                return;

            float dt = Time.deltaTime;
            float time = Time.time;
            Vector2 cursodVelocity = (cursorPos - oldCursorPos) * Time.deltaTime;

            // == Attack ===
            //int maxAttack = attackPeriodUpgrade.IntValue;
            float radius = addRangeUpgrade.Value;

            float scale = moneyAnimation.Evaluate(Time.time - moneyAnimationStart);
            playerMoneyRoot.SetLocalScale(new Vector3(scale, scale, scale));

            foreach (var enemy in enemyes)
            {
                if (enemy.State is EnemyState.Main or EnemyState.Push)
                {
                    Vector2 fromCursorToEnemy = (Vector2)enemy.transform.position - cursorPos;
                    if (fromCursorToEnemy.magnitude < radius + enemy.Radius)
                    {
                        if (enemy.State is EnemyState.Main)
                        {

                            int hpChange = GetDamage(enemy);
                            enemy.health -= hpChange;

                            // === Летят бревна
                            for (int i = 0; i < hpChange * enemy.Revard; i++)
                            {
                                var newFlyWood = pool.Get(woodFlyPrefab);
                                newFlyWood.transform.Rotate(0, 0, Rnd.Range(0, 360));
                                newFlyWood.Init(null, enemy.transform.position, GetPosForWood(enemy.transform.position));
                                newFlyWood.gameObject.SetActive(true);
                                newFlyWood.FlyFinish += WoodInFly_Landing;
                            }

                            // === сразу летят деньги
                            //var newFlyMoney = pool.Get(moneyInFlyPrefab);
                            //newFlyMoney.Init(Mathf.RoundToInt(enemy.Revard * hpChange), enemy.transform.position, moneyWorldPoint);
                            //newFlyMoney.gameObject.SetActive(true);
                            //newFlyMoney.FlyFinish += MoneyInFly_FinishFly;



                            enemy.pushEndTime = Time.time + pushTime;
                            AppSounds.Play(pushHit);

                            if (enemy.health > 0)
                            {
                                enemy.State = EnemyState.Push;
                            }
                            else
                            {
                                //enemy.State = EnemyState.PushDeath;

                                // === Летят бревна только после смерти
                                //for (int i = 0; i < enemy.Revard; i++)
                                //{
                                //    var newFlyWood = pool.Get(woodFlyPrefab);
                                //    newFlyWood.transform.Rotate(0, 0, Random.Range(0, 360));
                                //    newFlyWood.Init(null, enemy.transform.position, GetPosForWood(enemy.transform.position));
                                //    newFlyWood.gameObject.SetActive(true);
                                //    newFlyWood.FlyFinish += WoodInFly_Landing;
                                //}

                                HideEnemy(enemy);

                                //growOrder1++;
                                //if (growOrder1 == 1)
                                //{
                                //    float growDelay = 1f / growSeedUpgrade.Value;
                                //    timeNextGrow = Time.time + growDelay;
                                //} 
                            }
                        }

                        //Vector2 dir = cursodVelocity * cursorSpeedFacor + fromCursorToEnemy.normalized;
                        //enemy.pushDir = dir.normalized;

                        //maxAttack--;
                        //if (maxAttack <= 0)
                        //    break;
                    }
                }

                // === Bos ===
                /*
                if (enemy.State is EnemyState.Main or EnemyState.Push)
                {
                    Vector2 toEnemy = (Vector2)enemy.transform.position - bossPos;
                    if (toEnemy.magnitude < bossRadius + enemy.Radius)
                    {
                        if (enemy.State is EnemyState.Push)
                        {
                            AppSounds.Play(bossHit);
                            //enemy.health -= damage;
                            AddMoney(enemy.Revard);
                            //enemy.pushEndTime = Time.time + pushTime;
                        }

                        Vector2 dir = toEnemy.normalized;
                        enemy.pushDir = dir.normalized;

                        //maxAttack--;
                        //if (maxAttack <= 0)
                        //    break;
                    }
                }
                */
            }

            // === Move ===
            //UpdateMove();
            //UpdateGrow();

            UpdateRespawnFromHide();

            UpdateDrafeMove();

            UpdateWood();


            // === 
            foreach (var enemy in enemyes)
            {
                if (enemy.State is EnemyState.Push)
                {
                    if (Time.time > enemy.pushEndTime)
                        enemy.State = EnemyState.Main;
                }
            }

        }

        private void MoneyInFly_FinishFly(FlyMoneyAnimation flyMoney)
        {
            AddMoney(flyMoney.Money);
            pool.Return(flyMoney.gameObject);
        }

        private void LateUpdate()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                isPause = !isPause;
                pausePanel.SetActive(isPause);
            }

            if (isPause)
                return;

            //if (guiHit.IsGuiUnderPointer)
            //    return;

            oldCursorPos = cursorPos;
            Vector2 mousePos = Mouse.current.position.ReadValue();
            cursorPos = gameCamera.ScreenPointToWorldPointOnPlane(mousePos, Plaine.XY);
            cursor.position = cursorPos;
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            GizmosExtension.DrawRect(fieldRect);
            //Gizmos.DrawWireSphere(transform.position, enemySpawnDistace);
            //Gizmos.DrawWireSphere(transform.position, newEnemySpawnDistace);
            //Gizmos.DrawWireSphere(custleCenter.position, custleRadius);


            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(woodPrefab.transform.position, woodRadius);
        }
#endif
        void SetupRules()
        {
            UpgradeRule AddRule(UpgradeRule rule)
            {
                rule.game = this;
                rule.upgradeData.ResetLevet();
                upgradeList.Add(rule);
                rule.Init();
                return rule;
            }

            AddRule(new UpgradeRule()
            {
                button = addEnemy1Button,
                upgradeData = addEnemy1Upgrade,
                ApplyUpgrade = upgrade =>
                {
                    if (upgrade.IsOpen)
                        SpawnSeeding(enemy1Prefab);

                    //upgradesMenuButton.SetActive(enemyes.Count > enemyesToOpenUpgrades);
                    //bool isOpenUpgrades = addEnemy1Upgrade.Level.level > enemyesToOpenUpgrades;
                    //addDamageButton.gameObject.SetActive(isOpenUpgrades);
                    //addRangeButton.gameObject.SetActive(isOpenUpgrades);
                    //addEnemy2Button.gameObject.SetActive(isOpenUpgrades);
                }
            });

            AddRule(new UpgradeRule()
            {
                button = addEnemy2Button,
                upgradeData = addEnemy2Upgrade,
                ApplyUpgrade = upgrade =>
                {
                    if (upgrade.IsOpen)
                        SpawnSeeding(enemy2Prefab);

                    //addEnemy3Button.gameObject.SetActive(upgrade.IsOpen);
                }
            }).CallApplyUpgrade();

            AddRule(new UpgradeRule()
            {
                button = addEnemy3Button,
                upgradeData = addEnemy3Upgrade,
                ApplyUpgrade = upgrade =>
                {
                    if (upgrade.IsOpen)
                        SpawnSeeding(enemy3Prefab);

                    //addEnemy4Button.gameObject.SetActive(upgrade.IsOpen);
                    //openTruskButton.gameObject.SetActive(upgrade.IsOpen);
                }
            }).CallApplyUpgrade();

            AddRule(new UpgradeRule()
            {
                button = addEnemy4Button,
                upgradeData = addEnemy4Upgrade,
                ApplyUpgrade = upgrade =>
                {
                    if (upgrade.IsOpen)
                        SpawnSeeding(enemy4Prefab);

                    //openTruskButton.gameObject.SetActive(upgrade.IsOpen);
                }
            });

            // === Upgrades ===

            SetupRange(addRangeUpgrade);

            //AddRule(new UpgradeRule()
            //{
            //    button = openTruskButton,
            //    upgradeData = openTruskUpgrade,
            //    ApplyUpgrade = upgrade =>
            //    {
            //        trusk1.gameObject.SetActive(upgrade.IsOpen);
            //        truskProgressBar.gameObject.SetActive(upgrade.IsOpen);
            //    }
            //}).CallApplyUpgrade();

            SetupTruck();
        }

        void UpdateDrafeMove()
        {
            Vector3 drafe = new Vector3(-drafeSpeed * Time.deltaTime, 0, 0);
            foreach (var enemy in enemyes)
            {
                if (enemy.State == EnemyState.Hide)
                    continue;

                Vector3 p = enemy.transform.position;
                p = p + drafe;
                if (p.x < fieldRect.xMin)
                    p.x = fieldRect.xMax;

                enemy.transform.position = p;
            }
        }

        void UpdateRespawnFromHide()
        {
            foreach (var enemy in enemyes)
            {
                if (enemy.State is EnemyState.Hide)
                {
                    if (Time.time >= enemy.pushEndTime)
                    {
                        RespawnEnemy(enemy);
                    }
                }
            }
        }

        void UpdateMove()
        {
            foreach (var enemy in enemyes)
            {
                /*
                if (enemy.State == EnemyState.Main)
                {
                    Vector2 dirToCustle = custleCenter.position - enemy.transform.position;

                    if (dirToCustle.magnitude < custleRadius)
                    {
                        RespawnEnemy(enemy);
                    }
                    else
                    {
                        enemy.transform.position += (Vector3)(dirToCustle.normalized * enemySpeed * Time.deltaTime);
                    }
                }
                */

                if (enemy.State is EnemyState.Push)
                {
                    enemy.transform.position += (Vector3)(enemy.pushDir * pushSpeed * Time.deltaTime);

                    if (Time.time > enemy.pushEndTime)
                    {
                        /*
                        bool isPushOut = !fieldRect.Contains((Vector2)enemy.transform.position);

                        */

                        enemy.State = EnemyState.Main;
                    }
                }

                if (enemy.State is EnemyState.PushDeath)
                {
                    if (Time.time > enemy.pushEndTime)
                        KillEnmey(enemy);
                }

                if (enemy.State is EnemyState.Main)
                {
                    enemy.transform.position += (Vector3)(enemy.pushDir * enemySpeed * Time.deltaTime);
                }


                Vector2 p = enemy.transform.position;
                //Vector2 dir = enemy.pushDir;

                // 1 Бесконечное поле
                /*
                if (p.x < fieldRect.xMin)
                    p.x = fieldRect.xMax;
                else if (p.x > fieldRect.xMax)
                    p.x = fieldRect.xMin;
                else if (p.y < fieldRect.yMin)
                    p.y = fieldRect.yMax;
                else if (p.y > fieldRect.yMax)
                    p.y = fieldRect.yMin;
                */

                // 2 Респавн
                /*
                if (p.x < fieldRect.xMin ||
                    p.x > fieldRect.xMax ||
                    p.y < fieldRect.yMin ||
                    p.y > fieldRect.yMax)
                    RespawnEnemyRandom(enemy);
                */

                // 3 Отскок от стены

                if (p.x < fieldRect.xMin)
                {
                    enemy.pushDir = Vector2.Reflect(enemy.pushDir, Vector2.right);
                    p.x = fieldRect.xMin;
                }
                else if (p.x > fieldRect.xMax)
                {
                    enemy.pushDir = Vector2.Reflect(enemy.pushDir, Vector2.left);
                    p.x = fieldRect.xMax;
                }
                else if (p.y < fieldRect.yMin)
                {
                    enemy.pushDir = Vector2.Reflect(enemy.pushDir, Vector2.up);
                    p.y = fieldRect.yMin;
                }
                else if (p.y > fieldRect.yMax)
                {
                    enemy.pushDir = Vector2.Reflect(enemy.pushDir, Vector2.down);
                    p.y = fieldRect.yMax;
                }

                enemy.transform.position = p;
            }
        }

        int index;
        void UpdateGrow()
        {
            if (Time.time >= timeNextGrow && growOrder1 > 0)
            {
                for (; index < enemyes.Count; index++)
                {
                    var enemy = enemyes[index];
                    if (enemy.State == EnemyState.PushDeath)
                    {
                        RespawnEnemy(enemy);

                        growOrder1--;
                        float growDelay = 1f; //1f / growSeedUpgrade.Value;
                        timeNextGrow = Time.time + growDelay;
                        return;
                    }
                }

                index = 0;
            }
        }

        void UpdateWood()
        {
            float radius = addRangeUpgrade.Value;

            foreach (var wood in woods)
            {
                Vector2 fromCursorToEnemy = (Vector2)wood.transform.position - cursorPos;
                if (fromCursorToEnemy.magnitude < radius + woodRadius)
                {
                    var newFlyMoney = pool.Get(moneyInFlyPrefab);
                    newFlyMoney.Init(1, wood.transform.position, moneyWorldPoint);
                    newFlyMoney.gameObject.SetActive(true);
                    newFlyMoney.FlyFinish += MoneyInFly_FinishFly;

                    wood.gameObject.SetActive(false);
                }
            }
        }

        [SerializeField] float respawnDelay = 1;

        void RespawnEnemyWithDelay(Enemy enemey, float delay)
        {
            StartCoroutine(RespawnEnemyWithDelay_Coroutine(enemey, delay));
        }

        void HideEnemy(Enemy enemy)
        {
            enemy.State = EnemyState.Hide;
            enemy.pushEndTime = Time.time + respawnDelay;
            //HideTransform(enemy.transform);
        }

        void HideTransform(Transform transform)
        {
            transform.position = new Vector3(+9999, 0, 0);
        }

        IEnumerator RespawnEnemyWithDelay_Coroutine(Enemy enemey, float delay)
        {
            yield return new WaitForSeconds(delay);
            RespawnEnemy(enemey);
        }

        void KillEnmey(Enemy enemy)
        {
            AddMoney(enemy.Revard);
            HideEnemy(enemy);
            //RespawnEnemyWithDelay(enemy, spawnDelay);
            //RespawnEnemy(enemy);
        }

        void AddMoney(int add)
        {
            add *= debugSettings.RewadrFactor;
            topPanel.SetActive(true);
            //if (playerMoney >= addEnemy1Upgrade.GetSumCost(excessMoneyToOpenShop - 1))
            //    leftPanel.SetActive(true);

            moneyAnimationStart = Time.time;
            playerMoney += add;
            truckMoney += add;
            DrawGui();
        }

        void SetupTruck()
        {
            truck1.gameObject.SetActive(false);
            truck2.gameObject.SetActive(false);
            truck3.gameObject.SetActive(false);
            truskProgressBar.gameObject.SetActive(true);

            if (truckCapacityUpgrade03.IsOpen)
            {
                truck3.gameObject.SetActive(true);
                truckCapacity = truckCapacityUpgrade03.IntValue;
                return;
            }

            if (truckCapacityUpgrade02.IsOpen)
            {
                truck2.gameObject.SetActive(true);
                truckCapacity = truckCapacityUpgrade02.IntValue;
                return;
            }

            truck1.gameObject.SetActive(true);
            truckCapacity = truckCapacityUpgrade01.IntValue;
            return;
        }

        [Button]
        void AddMoney()
        {
            AddMoney(10000 / debugSettings.RewadrFactor);
        }

        void SpawnNewEnmey(Enemy prefab)
        {
            var enemy = InstantiateNewEnmey(prefab);
            RespawnEnemy(enemy);
        }

        Enemy InstantiateNewEnmey(Enemy prefab)
        {
            var enemy = Instantiate(prefab);
            enemy.gameObject.SetActive(true);
            enemy.ApplyRadius();
            enemyes.Add(enemy);
            return enemy;
        }

        SpriteRenderer InstantiateNewWood(GameObject prefab)
        {
            var item = Instantiate(prefab);
            item.gameObject.SetActive(true);
            woods.Add(item.GetComponent<SpriteRenderer>());
            return item.GetComponent<SpriteRenderer>();
        }

        void SpawnSeeding(Enemy prefab)
        {
            var posAndDir = GetPosForEnemy();

            var newItem = pool.Get(flySeedlingAnimator);
            newItem.Init(prefab, buySeedingButtonWorldPoint, posAndDir.pos);
            //newItem.Init(prefab, GuiHit.GetWorldPoint(seedingSpawn, gameCamera), posAndDir.pos);
            newItem.gameObject.SetActive(true);
            newItem.FlyFinish += Seeding_OnLand;
        }

        private void Seeding_OnLand(FlySeedlingAnimator animator)
        {
            //Enemy enemy = GetEnemyForRespawn(animator.EnemyPrefab.Type);
            //if (!enemy)
            //    enemy = InstantiateNewEnmey(animator.EnemyPrefab);

            Enemy enemy = InstantiateNewEnmey(animator.EnemyPrefab);
            SetEnemyMainState(enemy, animator.EndPos, Vector2.one);

            pool.Return(animator.gameObject);
            animator.gameObject.SetActive(false);
        }

        bool woodDoNotStay => true;
        private void WoodInFly_Landing(FlySeedlingAnimator flyAnimator)
        {
            if (woodDoNotStay)
            {
                var newFlyMoney = pool.Get(moneyInFlyPrefab);
                newFlyMoney.Init(1, flyAnimator.EndPos, moneyWorldPoint);
                newFlyMoney.gameObject.SetActive(true);
                newFlyMoney.FlyFinish += MoneyInFly_FinishFly;
            }
            else
            {
                var wood = GetWoodForRespawn();
                if (!wood)
                    wood = InstantiateNewWood(woodPrefab);

                wood.transform.position = flyAnimator.EndPos;
                wood.sortingOrder = GameUtils.SortingOrderFromY(wood.transform.position.y - woodRadius);
                wood.gameObject.SetActive(true);
            }

            pool.Return(flyAnimator.gameObject);
        }

        Enemy GetEnemyForRespawn(int enemyType)
        {
            foreach (var enemy in enemyes)
            {
                if (enemy.State == EnemyState.Hide &&
                    enemy.Type == enemyType)
                {
                    return enemy;
                }
            }

            return null;
        }

        SpriteRenderer GetWoodForRespawn()
        {
            foreach (var wood in woods)
            {
                if (wood.transform.position.x > 1000)
                    return wood;
            }

            return null;
        }

        void RespawnEnemy(Enemy enemy)
        {
            var posAndDir = GetPosForEnemy();
            SetEnemyMainState(enemy, posAndDir.pos, posAndDir.dir);
        }

        void SetEnemyMainState(Enemy enemy, Vector2 pos, Vector2 dir)
        {
            enemy.transform.position = pos;
            enemy.pushDir = dir;
            enemy.health = enemy.StartHealth;
            enemy.State = EnemyState.Main;
            enemy.ApplySortingOrder();
        }

        (Vector2 pos, Vector2 dir) GetPosForEnemy()
        {
            if (drafeSpeed > 0)
            {
                Vector2 p1 = new Vector2(fieldRect.xMax, fieldRect.yMin);
                Vector2 p2 = new Vector2(fieldRect.xMax, fieldRect.yMax);
                return
                    (new Vector2(fieldRect.xMax, Rnd.Range(fieldRect.yMin, fieldRect.yMax)),
                    (new Vector2(-5, 0) + Rnd.onUnitCircle).normalized);
            }
            else
            {
                if (enemyes.Count > 0)
                {
                    int index = 0;
                    int offset = Rnd.Range(0, enemyes.Count);
                    for (int i = 0; i < enemyes.Count; i++)
                    {
                        int realIndex = (i + offset) % enemyes.Count;
                        if (enemyes[realIndex].State == EnemyState.Main)
                        {
                            index = realIndex;
                            break;
                        }
                    }

                    Vector2 p = enemyes[index].transform.position;
                    p -= p.normalized * offsetTocenter;
                    p = ClampRectWithBounce(fieldRect, p + Rnd.insideUnitCircle * respawnRange);

                    return (p, Vector2.zero);
                }

                return (fieldRect.RandomPoint(), Vector2.zero);
            }

            // 2 enemy.transform.position = fieldRect.RandomPointOnSide();

        }

        Vector2 ClampRectWithBounce(Rect rect, Vector2 p)
        {
            if (p.x < rect.xMin)
                p.x = rect.xMin + (rect.xMin - p.x);
            if (p.x > rect.xMax)
                p.x = rect.xMax - (p.x - rect.xMax);
            if (p.y < rect.yMin)
                p.y = rect.yMin + (rect.yMin - p.y);
            if (p.y > rect.yMax)
                p.y = rect.yMax - (p.y - rect.yMax);

            return p;
        }


        int flyWoodSide = 1;
        private int truckCapacity;

        Vector2 GetPosForWood(Vector2 treePos)
        {
            //return fieldRect.Clamp(treePos + Random.onUnitCircle * woodFlyRange);

            float dx = Rnd.Range(woodFlyRange * 0.1f, woodFlyRange);
            flyWoodSide *= -1;
            return fieldRect.Clamp(treePos + new Vector2(dx * flyWoodSide, -0.3f));
        }

        void SetupRange(UpgradeData2 upgrade)
        {
            cursor.localScale = Vector3.one * (upgrade.Value * 2);
        }

        int GetDamage(Enemy enemy)
        {
            if (enemy.Type == 1)
                return enemy.health <= damageEnemyUpgrade01.IntValue ? enemy.health : 1;

            if (enemy.Type == 2)
                return enemy.health <= damageEnemyUpgrade02.IntValue ? enemy.health : 1;

            if (enemy.Type == 3)
                return enemy.health <= damageEnemyUpgrade03.IntValue ? enemy.health : 1;

            if (enemy.Type == 4)
                return enemy.health <= damageEnemyUpgrade04.IntValue ? enemy.health : 1;

            throw new NotImplementedException();
        }

        void DrawGui()
        {
            playerMoneyText.text = playerMoney.ToString();
            truckAmountText.text = GameUtils.IntToSting(truckMoney);

            truskProgressBar.Draw(truckMoney, truckCapacity);
            if (truckMoney >= truckCapacity)
                openUpgradeButton.gameObject.SetActive(true);

            foreach (UpgradeRule item in upgradeList)
            {
                item.DrawButton(playerMoney);
            }
        }

        [System.Serializable]
        class UpgradeRule
        {
            public UpgradeButton button;
            public UpgradeData2 upgradeData;
            public IncreGame game;
            public UnityAction<UpgradeData2> ApplyUpgrade;

            public void Init()
            {
                button.Click += () =>
                {
                    if (Keyboard.current.shiftKey.isPressed)
                    {
                        while (game.playerMoney >= upgradeData.IntCost && upgradeData.NotMaxLevel)
                            OnClick();
                    }
                    else
                    {
                        OnClick();
                    }

                    game.DrawGui();
                };

                button.DrawLabel(upgradeData.GuiName);
            }

            public void DrawAllForEditor()
            {
#if UNITY_EDITOR
                Undo.RegisterCompleteObjectUndo(button, "DrawButton");
                button.DrawLabel(upgradeData.GuiName);
                DrawButton(int.MaxValue);
#endif
            }


            void OnClick()
            {
                game.playerMoney -= upgradeData.IntCost;
                upgradeData.AddLevel();
                CallApplyUpgrade();
            }

            public void CallApplyUpgrade()
            {
                ApplyUpgrade.Invoke(upgradeData);
            }

            public void DrawButton(int playerMomey)
            {
                int cost = upgradeData.IntCost;
                button.Draw(playerMomey >= cost, cost, upgradeData.Level.level, upgradeData.MaxLevel.level);
            }
        }
    }
}
