using GamePackages.Audio;
using GamePackages.Core;
using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace IncreKindom
{
    public class IncreKindom : MonoBehaviour
    {
        [SerializeField, IsntNull] Camera gameCamera;
        [SerializeField, IsntNull] GuiHit guiHit;
        [SerializeField, IsntNull] AppSounds appSounds;

        [Header("Cursor")]
        [SerializeField] float startCursorRadius;
        [SerializeField, IsntNull] Transform cursor;

        [Header("Field")]
        [SerializeField, IsntNull] Transform p1;
        [SerializeField, IsntNull] Transform p2;

        [Header("Custle")]
        [SerializeField] float custleRadius;
        [SerializeField, IsntNull] Transform custleCenter;

        [Header("AddEnemy")]
        [SerializeField] float upgradeCostFactor;
        [SerializeField] float upgradeStartCost;

        [Header("Enemy")]
        [SerializeField] float spawnDelay;
        [SerializeField] float enemySpeed;
        [SerializeField] float newEnemySpawnDistace;
        [SerializeField] float enemySpawnDistace;
        [SerializeField, IsntNull] Enemy enemy1Prefab;
        [SerializeField, IsntNull] Enemy enemy2Prefab;

        [Header("Push")]
        [SerializeField] float pushTime;
        [SerializeField] float pushSpeed;
        [SerializeField, IsntNull] SoundsSet pushHit;

        [Header("GUI")]
        [SerializeField, IsntNull] TMP_Text playerMoneyText;
        [SerializeField, IsntNull] UpgradeButton addEnemy1Button;
        [SerializeField, IsntNull] UpgradeButton addEnemy2Button;
        [SerializeField, IsntNull] UpgradeButton addDamageButton;
        [SerializeField, IsntNull] UpgradeButton addRangeButton;

        Rect fieldRect;
        Vector2 cursorPos;
        float cursorActualRadius;
        int playerMoney;
        int addEnemy1UpgradeLevel;
        int addEnemy2UpgradeLevel;
        int addDamageUpgradeLevel;
        int addRangeUpgradeLevel;
        List<Enemy> enemyes;

        void Start()
        {
            fieldRect = new Rect((Vector2)p1.position, (Vector2)(p2.position - p1.position));
            addEnemy1UpgradeLevel = 1;
            addEnemy2UpgradeLevel = 0;
            addDamageUpgradeLevel = 0;
            playerMoney = 0;
            enemyes = new List<Enemy>();
            playerMoneyText.text = "0";
            enemy1Prefab.gameObject.SetActive(false);
            enemy2Prefab.gameObject.SetActive(false);

            appSounds.Init(new AppAudioAccountData());
            AppSounds.SetAsSceneSound(appSounds);


            SetRange();
            addEnemy1Button.Click += () => AddEnemy1Upgrade();
            addEnemy2Button.Click += () => AddEnemy2Upgrade();
            addDamageButton.Click += () => AddDamageUpgrade();
            addRangeButton.Click += () => AddRangeUpgrade();

            DrawGui();
            SpawnNewEnmey(enemy1Prefab);
        }

        void Update()
        {
            foreach (var enemy in enemyes)
            {
                if (enemy.State != EnemyState.Main)
                    continue;

                Vector2 fromCursorToEnemyDir = (Vector2)enemy.transform.position - cursorPos;
                if (fromCursorToEnemyDir.magnitude < cursorActualRadius + enemy.Radius)
                {
                    AppSounds.Play(pushHit);
                    enemy.health -= addDamageUpgradeLevel + 1;
                    if (enemy.health > 0)
                    {
                        enemy.State = EnemyState.Push;
                    }
                    else
                    {
                        enemy.State = EnemyState.PushDeath;
                    }

                    enemy.pushDir = fromCursorToEnemyDir;
                    enemy.pushEndTime = Time.time + pushTime;
                }
            }

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
                    enemy.transform.position += (Vector3)(enemy.pushDir.normalized * pushSpeed * Time.deltaTime);

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
                    enemy.transform.position += (Vector3)(enemy.pushDir.normalized * enemySpeed * Time.deltaTime);
                }


                Vector2 p = enemy.transform.position;
                if (p.x < fieldRect.xMin)
                    p.x = fieldRect.xMax;

                if (p.x > fieldRect.xMax)
                    p.x = fieldRect.xMin;

                if (p.y < fieldRect.yMin)
                    p.y = fieldRect.yMax;

                if (p.y > fieldRect.yMax)
                    p.y = fieldRect.yMin;

                enemy.transform.position = p;
            }
        }



        void RespawnEnemyWithDelay(Enemy enemey, float delay)
        {
            StartCoroutine(RespawnEnemyWithDelay_Coroutine(enemey, delay));
        }

        void HideEnemy(Enemy enemy)
        {
            enemy.State = EnemyState.Hide;
            enemy.transform.position = new Vector3(-999999, 0, 0);
        }

        IEnumerator RespawnEnemyWithDelay_Coroutine(Enemy enemey, float delay)
        {
            yield return new WaitForSeconds(delay);
            RespawnEnemy(enemey);
        }


        private void LateUpdate()
        {
            //if (guiHit.IsGuiUnderPointer)
            //    return;

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

        void KillEnmey(Enemy enemy)
        {
            playerMoney += enemy.Revard;
            HideEnemy(enemy);
            RespawnEnemyWithDelay(enemy, spawnDelay);
            DrawGui();
        }

        [Button]
        void AddMoney()
        {
            playerMoney += 100;
            DrawGui();
        }

        void SpawnNewEnmey(Enemy prefab)
        {
            var newEnmey = Instantiate(prefab);
            newEnmey.gameObject.SetActive(true);
            newEnmey.ApplyRadius();
            enemyes.Add(newEnmey);
            RespawnEnemy(newEnmey);
        }

        void RespawnEnemy(Enemy enemy)
        {
            // 1
            enemy.transform.position = fieldRect.RandomPointOnSide();
            enemy.pushDir = (new Vector2(-5, 0) + Random.onUnitCircle).normalized;

            // 2
            Vector2 p1 = new Vector2(fieldRect.xMax, fieldRect.yMin);
            Vector2 p2 = new Vector2(fieldRect.xMax, fieldRect.yMax);
            enemy.transform.position = new Vector2(fieldRect.xMax, Random.Range(fieldRect.yMin, fieldRect.yMax));
            enemy.pushDir = (Random.onUnitCircle).normalized;

            enemy.health = enemy.StartHealth;
            enemy.State = EnemyState.Main;
        }



        void AddEnemy1Upgrade()
        {
            playerMoney -= AddEnemy1UpgradeCost();
            addEnemy1UpgradeLevel++;

            int newEnmeyCount = 1;// Mathf.RoundToInt(enemyes.Count * 0.3f) + 2;
            for (int i = 0; i < newEnmeyCount; i++)
                SpawnNewEnmey(enemy1Prefab);

            DrawGui();
        }

        void AddEnemy2Upgrade()
        {
            playerMoney -= AddEnemy2UpgradeCost();
            addEnemy2UpgradeLevel++;

            int newEnmeyCount = 1;// Mathf.RoundToInt(enemyes.Count * 0.3f) + 2;
            for (int i = 0; i < newEnmeyCount; i++)
                SpawnNewEnmey(enemy2Prefab);

            DrawGui();
        }

        void AddDamageUpgrade()
        {
            playerMoney -= AddDamageUpgradeCost();
            addDamageUpgradeLevel++;
            DrawGui();
        }

        void AddRangeUpgrade()
        {
            playerMoney -= AddDamageUpgradeCost();
            addRangeUpgradeLevel++;
            SetRange();
            DrawGui();
        }

        void SetRange()
        {
            cursorActualRadius = startCursorRadius * (addRangeUpgradeLevel + 1);
            cursor.localScale = Vector3.one * (cursorActualRadius * 2);
        }


        void DrawGui()
        {
            playerMoneyText.text = playerMoney.ToString();

            {
                int cost = AddEnemy1UpgradeCost();
                addEnemy1Button.Draw(playerMoney >= cost, cost, addEnemy1UpgradeLevel);
            }

            {
                int cost = AddEnemy2UpgradeCost();
                addEnemy2Button.Draw(playerMoney >= cost, cost, addEnemy2UpgradeLevel);
            }

            {
                int cost = AddDamageUpgradeCost();
                addDamageButton.Draw(playerMoney >= cost, cost, addDamageUpgradeLevel);
            }

            {
                int cost = AddRangeUpgradeCost();
                addRangeButton.Draw(playerMoney >= cost, cost, addRangeUpgradeLevel);
            }


        }


        int AddEnemy1UpgradeCost() => Mathf.RoundToInt(addEnemy1UpgradeLevel * upgradeCostFactor + upgradeStartCost);
        int AddEnemy2UpgradeCost() => Mathf.RoundToInt((addEnemy2UpgradeLevel) * 10 + 100);
        int AddDamageUpgradeCost() => Mathf.RoundToInt((addDamageUpgradeLevel + 1) * 20);
        int AddRangeUpgradeCost() => Mathf.RoundToInt((addRangeUpgradeLevel + 1) * 20);
    }
}
