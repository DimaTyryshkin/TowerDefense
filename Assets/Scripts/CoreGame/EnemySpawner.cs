using GamePackages.Core;
using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Game.CoreGame
{
    class EnemySpawner : MonoBehaviour, IValidated
    {
        [SerializeField, IsntNull] HealthComponentView enemyHealthView;
        [SerializeField, IsntNull] EnemySpawnPoint[] spawnPoints;
        [SerializeField, IsntNull] WaveData[] waves;
        [Inject] HealthComponentOnBoardCollection enemyesOnBoardCollection;
        [Inject] HealthComponentOnBoardCollection targetsForEnemyColelction;

        internal event UnityAction WaveEnd;
        internal event UnityAction GameOver;

        float timeNextSpawn;
        //int enemyTotal; 
        int enmeyNeedKillToWin;
        int enemyWasKilledInWave;
        int waveIndex;
        bool isSpawning;

        private void Update()
        {
            if (!isSpawning)
                return;

            if (Time.time < timeNextSpawn)
                return;

            WaveData wave = waves[waveIndex];
            WaveData.WaveItem waveItem = wave.GetNextEnemy(spawnPoints.Length);

            EnemySpawnPoint spawnPoint = spawnPoints[waveItem.spawnPointIndex];
            var enemy = InstatiateEnemy(waveItem.enemy, spawnPoint.wayPoints.GetPoint(0).position, spawnPoint.wayPoints);
            enemy.move.gameObject.name = $"{waveItem.enemy.gameObject.name} wave={waveIndex:00} inWaveIndex={(wave.CurrentCount - 1):00}";

            //EnemySpawn?.Invoke(newEnemy);

            bool nextEnemyExist = wave.CurrentCount < wave.TotalAmount;
            if (nextEnemyExist)
            {
                timeNextSpawn = Time.time + waveItem.delayAfterSpawn;
            }
            else
            {
                isSpawning = false;
            }
        }

        internal void Init()
        {
            isSpawning = false;
            waveIndex = 0;
            //enemyTotal = 0;
        }


        [Button]
        internal void StartWave()
        {
            if (waveIndex < waves.Length)
            {
                isSpawning = true;
                enemyWasKilledInWave = 0;
                waves[waveIndex].Init();
                enmeyNeedKillToWin = waves[waveIndex].TotalAmount;
                timeNextSpawn = Time.time;
                Debug.Log($"<b>Началась волна номер '{waveIndex + 1}'</b>");
            }
            else
            {
                Debug.Log("<b>Волны закончились</b>");
            }

        }

        internal void DebugSpawnEnmey(WayMoveComponent prefab)
        {
            EnemySpawnPoint spawnPoint = spawnPoints[0];
            InstatiateEnemy(prefab, spawnPoint.transform.position, spawnPoint.wayPoints);
            enmeyNeedKillToWin++;
        }


        (
            WayMoveComponent move,
            HealthComponent health,
            EnemyAi ai,
            HealthComponentView healthView
        )
            InstatiateEnemy(WayMoveComponent prefab, Vector2 pos, WayPoints wayPoints)
        {
            WayMoveComponent enemyMove = transform.InstantiateAsChild(prefab);
            DamageReceiver enemy = enemyMove.GetComponent<DamageReceiver>();
            HealthComponent enemyHealth = enemyMove.GetComponent<HealthComponent>();
            EnemyAi enemyAi = enemyMove.GetComponent<EnemyAi>();
            HealthComponentView healthView = transform.InstantiateAsChild(enemyHealthView);
            enemyMove.transform.position = pos;
            enemyMove.gameObject.SetActive(true);
            enemyMove.FinishMove += Enemy_FinishMove;
            enemyHealth.Death += EnemyHealth_Death;
            enemyHealth.Init();
            enemyAi.Init(targetsForEnemyColelction, wayPoints);
            healthView.Init(enemyHealth, Vector3.up * 0.55f);

            enemyesOnBoardCollection.Add(enemy);
            return (enemyMove, enemyHealth, enemyAi, healthView);
        }

        private void Enemy_FinishMove(WayMoveComponent enemy)
        {
            //TODO тут мы можем убивать уже удаленых, как-то не хорошо.
            Destroy(enemy.gameObject);
            //enemyesOnBoardCollection.Remove(enemy.GetComponent<HealthComponent>());
            GameOver.Invoke();
        }

        private void EnemyHealth_Death(HealthComponent enemy)
        {
            Assert.IsNotNull(enemy);
            DamageReceiver damageReceiver = enemy.GetComponent<DamageReceiver>();
            Assert.IsNotNull(damageReceiver);

            enemyWasKilledInWave++;
            enemyesOnBoardCollection.Remove(damageReceiver);

            if (enemyWasKilledInWave == enmeyNeedKillToWin)
            {
                waveIndex++;
                WaveEnd.Invoke();
            }
        }

        void IValidated.Validate(ValidationContext context)
        {
            //foreach (WaveData wave in waves)
            //{
            //    for (int i = 0; i < wave.items.Length; i++)
            //    {
            //        if (wave.items[i].spawnPointIndex >= spawnPoints.Length)
            //            context.AddProblem("EnemySpawner", ValidationProblem.Type.Error, $"WaveItem[{i}] ссылается на спавнер которого нет", wave);
            //    }
            //}
        }
    }
}
