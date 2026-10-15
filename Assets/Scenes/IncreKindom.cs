using GamePackages.Audio;
using GamePackages.Core;
using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using NaughtyAttributes;

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace IncreKindom
{
    public class IncreKindom : MonoBehaviour
    {
        [SerializeField, IsntNull] Camera gameCamera;
        [SerializeField, IsntNull] Camera canvasCamera;
        [SerializeField, IsntNull] GuiHit guiHit;
        [SerializeField, IsntNull] AppSounds appSounds;

        [SerializeField] int startSpawn = 5;

        [Header("Cursor")]
        [SerializeField, IsntNull] Transform cursor;

        [Header("Field")]
        [SerializeField, IsntNull] Transform p1;
        [SerializeField, IsntNull] Transform p2;

        [Header("Custle")]
        [SerializeField] float custleRadius;
        [SerializeField, IsntNull] Transform custleCenter;

        [Header("Enemy")]
        [SerializeField] float spawnDelay;
        [SerializeField] float enemySpeed;
        [SerializeField] float newEnemySpawnDistace;
        [SerializeField] float enemySpawnDistace;
        [SerializeField, IsntNull] Enemy enemy1Prefab;
        [SerializeField, IsntNull] Enemy enemy2Prefab;

        [Header("Boss")]
        [SerializeField] float bossRadius;
        [SerializeField, IsntNull] Transform boss;


        [Header("Push")]
        [SerializeField] float pushTime;
        [SerializeField] float pushSpeed;
        [SerializeField, IsntNull] SoundsSet pushHit;
        [SerializeField, IsntNull] SoundsSet bossHit;

        [Header("Upgrade")]
        [SerializeField, IsntNull] UpgradeData2 addEnemy1Upgrade;
        [SerializeField, IsntNull] UpgradeData2 addEnemy2Upgrade;
        [SerializeField, IsntNull] UpgradeData2 addDamageUpgrade;
        [SerializeField, IsntNull] UpgradeData2 addRangeUpgrade;
        [SerializeField, IsntNull] UpgradeData2 growSeedUpgrade;

        [Header("GUI")]
        [SerializeField, IsntNull] TMP_Text playerMoneyText;
        [SerializeField, IsntNull] Transform playerMoneyRoot;
        [SerializeField, IsntNull] AnimationCurve moneyAnimation;
        [SerializeField, IsntNull] UpgradeButton addEnemy1Button;
        [SerializeField, IsntNull] UpgradeButton addEnemy2Button;
        [SerializeField, IsntNull] UpgradeButton addDamageButton;
        [SerializeField, IsntNull] UpgradeButton addRangeButton;
        [SerializeField, IsntNull] UpgradeButton growSeedButton;

        [Header("FlyMoney")]
        [SerializeField, IsntNull] RectTransform moneyGuiPoint;
        [SerializeField, IsntNull] FlyMoneyAnimation moneyInFlyPrefab;


        [Header("Seeding")]
        //[SerializeField, IsntNull] int seedingCost1cost;
        [SerializeField, IsntNull] RectTransform seedingSpawn;
        [SerializeField, IsntNull] FlySeedlingAnimator flySeedlingAnimator;

        Rect fieldRect;
        Vector2 cursorPos;
        Vector2 bossPos;
        Vector2 moneyWorldPoint;
        Vector2 buySeedingButtonWorldPoint;

        Vector2 oldCursorPos;
        int playerMoney;
        int growOrder1;
        int growOrder2;
        float timeNextGrow;
        float moneyAnimationStart;
        DynamicObjectPool pool;

        List<Enemy> enemyes;
        List<UpgradeRule> upgradeList;

        void Start()
        {
            enemyes = new();
            upgradeList = new();
            pool = DynamicObjectPool.GetInst();

            bossPos = boss.position;
            boss.localScale = Vector3.one * (bossRadius * 2);

            fieldRect = new Rect((Vector2)p1.position, (Vector2)(p2.position - p1.position));
            playerMoney = 0;

            playerMoneyText.text = "0";
            enemy1Prefab.gameObject.SetActive(false);
            enemy2Prefab.gameObject.SetActive(false);
            moneyInFlyPrefab.gameObject.SetActive(false);
            flySeedlingAnimator.gameObject.SetActive(false);

            appSounds.Init(new AppAudioAccountData());
            AppSounds.SetAsSceneSound(appSounds);

            UpgradeRule AddRule(UpgradeRule rule)
            {
                rule.upgradeData.ResetLevet();
                upgradeList.Add(rule);
                rule.Init();
                return rule;
            }

            AddRule(new UpgradeRule()
            {
                game = this,
                button = addEnemy1Button,
                upgradeData = addEnemy1Upgrade,
                //ApplyUpgrade = upgrade => SpawnNewEnmey(enemy1Prefab)
                ApplyUpgrade = upgrade => SpawnSeeding(enemy1Prefab)
            });

            AddRule(new UpgradeRule()
            {
                game = this,
                button = addEnemy2Button,
                upgradeData = addEnemy2Upgrade,
                //ApplyUpgrade = upgrade => SpawnNewEnmey(enemy2Prefab)
                ApplyUpgrade = upgrade => SpawnSeeding(enemy2Prefab)
            });

            AddRule(new UpgradeRule()
            {
                game = this,
                button = addDamageButton,
                upgradeData = addDamageUpgrade,
                ApplyUpgrade = _ => { }
            });

            AddRule(new UpgradeRule()
            {
                game = this,
                button = growSeedButton,
                upgradeData = growSeedUpgrade,
                ApplyUpgrade = _ => { }
            });

            AddRule(new UpgradeRule()
            {
                game = this,
                button = addRangeButton,
                upgradeData = addRangeUpgrade,
                ApplyUpgrade = upgrade => SetupRange(upgrade)
            }).CallApplyUpgrade();

            moneyWorldPoint = GetWorldPoint(moneyGuiPoint);
            buySeedingButtonWorldPoint = GetWorldPoint(seedingSpawn);

            DrawGui();

            for (int i = 0; i < startSpawn; i++)
            {
                SpawnNewEnmey(enemy1Prefab);
            }
        }

        Vector2 GetWorldPoint(RectTransform rectTransform)
        {
            Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);
            return gameCamera.ScreenPointToWorldPointOnPlane(screenPoint, Plaine.XY);
        }

        [SerializeField] float cursorSpeedFacor;

        void Update()
        {
            float dt = Time.deltaTime;
            float time = Time.time;
            Vector2 cursodVelocity = (cursorPos - oldCursorPos) * Time.deltaTime;

            // == Attack ===
            //int maxAttack = attackPeriodUpgrade.IntValue;
            int damage = addDamageUpgrade.IntValue;
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
                            float hpChange = Mathf.Min(damage, (int)enemy.health);
                            enemy.health -= hpChange;
                            //AddMoney(enemy.Revard);

                            var newFlyMoney = pool.Get(moneyInFlyPrefab);
                            newFlyMoney.Init(Mathf.RoundToInt(enemy.Revard * hpChange), enemy.transform.position, moneyWorldPoint);
                            newFlyMoney.gameObject.SetActive(true);
                            newFlyMoney.FlyFinish += MoneyInFly_FinishFly;

                            enemy.pushEndTime = Time.time + pushTime;
                            AppSounds.Play(pushHit);

                            if (enemy.health > 0)
                            {
                                enemy.State = EnemyState.Push;
                            }
                            else
                            {
                                //enemy.State = EnemyState.PushDeath;

                                HideEnemy(enemy);
                                //growOrder1++;
                                //if (growOrder1 == 1)
                                //{
                                //    float growDelay = 1f / growSeedUpgrade.Value;
                                //    timeNextGrow = Time.time + growDelay;
                                //}


                            }
                        }

                        Vector2 dir = cursodVelocity * cursorSpeedFacor + fromCursorToEnemy.normalized;
                        enemy.pushDir = dir.normalized;

                        //maxAttack--;
                        //if (maxAttack <= 0)
                        //    break;
                    }
                }

                /*
                if (enemy.State is EnemyState.Hide)
                {
                    if (Time.time >= enemy.pushEndTime)
                    {
                        RespawnEnemyRandomInRect(enemy);
                    }
                }
                */

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
            UpdateDrafeMove();


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
        }
#endif

        [Range(0, 50), SerializeField]
        float drafeSpeed;

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
                        float growDelay = 1f / growSeedUpgrade.Value;
                        timeNextGrow = Time.time + growDelay;
                        return;
                    }
                }

                index = 0;
            }
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

        void RespawnEnemyWithDelay(Enemy enemey, float delay)
        {
            StartCoroutine(RespawnEnemyWithDelay_Coroutine(enemey, delay));
        }

        void HideEnemy(Enemy enemy)
        {
            enemy.State = EnemyState.Hide;
            enemy.pushEndTime = Time.time + 1;
            enemy.transform.position = new Vector3(+9999, 0, 0);
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
            moneyAnimationStart = Time.time;
            playerMoney += add;
            DrawGui();
        }

        [Button]
        void AddMoney()
        {
            playerMoney += 1000;
            DrawGui();
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

        void SpawnSeeding(Enemy prefab)
        {
            var posAndDir = GetPosForEnemy();

            var newItem = pool.Get(flySeedlingAnimator);
            newItem.Init(prefab, buySeedingButtonWorldPoint, posAndDir.pos);
            newItem.gameObject.SetActive(true);
            newItem.FlyFinish += Seeding_OnLand;
        }

        private void Seeding_OnLand(FlySeedlingAnimator animator)
        {
            Enemy enemy = GetEnemyForRespawn(animator.EnemyPrefab.Type);
            if (!enemy)
                enemy = InstantiateNewEnmey(animator.EnemyPrefab);

            SetEnemyMainState(enemy, animator.EndPos, Vector2.one);

            pool.Return(animator.gameObject);
            animator.gameObject.SetActive(false);
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
        }

        (Vector2 pos, Vector2 dir) GetPosForEnemy()
        {
            if (drafeSpeed > 0)
            {
                Vector2 p1 = new Vector2(fieldRect.xMax, fieldRect.yMin);
                Vector2 p2 = new Vector2(fieldRect.xMax, fieldRect.yMax);
                return
                    (new Vector2(fieldRect.xMax, Random.Range(fieldRect.yMin, fieldRect.yMax)),
                    (new Vector2(-5, 0) + Random.onUnitCircle).normalized);
            }
            else
            {
                return (fieldRect.RandomPoint(), Vector2.zero);
            }

            // 2 enemy.transform.position = fieldRect.RandomPointOnSide();

        }

        void SetupRange(UpgradeData2 upgrade)
        {
            cursor.localScale = Vector3.one * (upgrade.Value * 2);
        }

        void DrawGui()
        {
            playerMoneyText.text = playerMoney.ToString();

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
            public IncreKindom game;
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

            public void DrawButton(int playerNomey)
            {
                int cost = upgradeData.IntCost;
                button.Draw(playerNomey >= cost, cost, upgradeData.Level.level, upgradeData.MaxLevel.level);
            }
        }
    }
}
