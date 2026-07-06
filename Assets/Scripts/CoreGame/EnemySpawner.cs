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
        int enemyTotalCounter;
        int enemyInWaveCounter;
        int enemyInWaveWasKilled;
        int waveIndex;
        bool isSpawning;

        private void Update()
        {
            if (!isSpawning)
                return;

            if (Time.time < timeNextSpawn)
                return;

            WaveData wave = waves[waveIndex];
            WaveData.WaveItem waveItem = wave.items[enemyInWaveCounter];

            EnemySpawnPoint spawnPoint = spawnPoints[waveItem.spawnPointIndex];
            var enemy = InstatiateEnemy(waveItem.enemy, spawnPoint.transform.position, spawnPoint.wayPoints);
            enemy.move.FinishMove += Enemy_FinishMove;
            enemy.health.Death += EnemyHealth_Death;
            enemy.move.gameObject.name = $"{waveItem.enemy.gameObject.name} wave={waveIndex:00} inWaveIndex={enemyInWaveCounter:00}";

            //EnemySpawn?.Invoke(newEnemy);

            enemyTotalCounter++;
            enemyInWaveCounter++;

            bool nextEnemyExist = enemyInWaveCounter < wave.items.Length;
            if (nextEnemyExist)
            {
                timeNextSpawn = Time.time + wave.items[enemyInWaveCounter].delayBeforeSpawn;
            }
            else
            {
                isSpawning = false;
            }
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
            enemyMove.gameObject.SetActive(true);
            enemyMove.transform.position = pos;
            HealthComponent enemyHealth = enemyMove.GetComponent<HealthComponent>();
            EnemyAi enemyAi = enemyMove.GetComponent<EnemyAi>();
            HealthComponentView healthView = transform.InstantiateAsChild(enemyHealthView);

            enemyHealth.Init();
            enemyAi.Init(targetsForEnemyColelction, wayPoints);
            healthView.Init(enemyHealth, Vector3.up * 0.55f);

            enemyesOnBoardCollection.Add(enemyHealth);
            return (enemyMove, enemyHealth, enemyAi, healthView);
        }

        private void Enemy_FinishMove(WayMoveComponent enemy)
        {
            enemy.gameObject.SetActive(false);
            enemyesOnBoardCollection.Remove(enemy.GetComponent<HealthComponent>());
            GameOver.Invoke();
        }

        private void EnemyHealth_Death(HealthComponent enemy)
        {
            Assert.IsNotNull(enemy);

            enemyInWaveWasKilled++;
            enemyesOnBoardCollection.Remove(enemy);

            if (enemyInWaveWasKilled == waves[waveIndex].items.Length)
            {
                waveIndex++;
                WaveEnd.Invoke();
            }
        }

        internal void Init()
        {
            isSpawning = false;
            waveIndex = 0;
            enemyTotalCounter = 0;
        }


        [Button]
        internal void StartWave()
        {
            if (waveIndex < waves.Length)
            {
                isSpawning = true;
                enemyInWaveCounter = 0;
                enemyInWaveWasKilled = 0;
                timeNextSpawn = Time.time + waves[waveIndex].items[0].delayBeforeSpawn;
                Debug.Log($"<b>Началась волна номер '{waveIndex}'</b>");
            }
            else
            {
                Debug.Log("<b>Волны закончились</b>");
            }

        }

        void IValidated.Validate(ValidationContext context)
        {
            foreach (WaveData wave in waves)
            {
                for (int i = 0; i < wave.items.Length; i++)
                {
                    if (wave.items[i].spawnPointIndex >= spawnPoints.Length)
                        context.AddProblem("EnemySpawner", ValidationProblem.Type.Error, $"WaveItem[{i}] ссылается на спавнер которого нет", wave);
                }
            }
        }
    }
}
