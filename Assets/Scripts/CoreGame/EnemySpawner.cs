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
        [SerializeField, IsntNull] EnemyHealthView enemyHealthView;
        [SerializeField, IsntNull] EnemySpawnPoint[] spawnPoints;
        [SerializeField, IsntNull] EnemyWaveData[] waves;
        [Inject] EnemyesOnBoardCollection enemyesOnBoardCollection;

        internal event UnityAction WaveEnd;

        float timeNextSpawn;
        int enemyTotalCounter;
        int enemyInWaveCounter;
        int enemyInWaveWasKilledOrRemoved;
        int waveIndex;
        bool isSpawning;

        private void Update()
        {
            if (!isSpawning)
                return;

            if (Time.time < timeNextSpawn)
                return;

            EnemyWaveData wave = waves[waveIndex];
            EnemyWaveData.WaveItem waveItem = wave.items[enemyInWaveCounter];

            EnemyMove newEnemy = transform.InstantiateAsChild(waveItem.enemy);
            EnemySpawnPoint spawnPoint = spawnPoints[waveItem.spawnPointIndex];
            newEnemy.transform.position = spawnPoint.transform.position;
            newEnemy.name = $"{waveItem.enemy.gameObject.name} wave={waveIndex:00} inWaveIndex={enemyInWaveCounter:00}";
            newEnemy.SetWayPoints(spawnPoint.wayPoints);
            newEnemy.FinishMove += NewEnemy_FinishMove;
            newEnemy.gameObject.SetActive(true);

            EnemyHealth enemyHealth = newEnemy.GetComponent<EnemyHealth>();
            enemyHealth.Init();
            enemyesOnBoardCollection.AddEnemy(enemyHealth);

            EnemyHealthView healthView = transform.InstantiateAsChild(enemyHealthView);
            healthView.Init(enemyHealth, Vector3.up * 0.55f);
            healthView.gameObject.SetActive(true);

            //EnemySpawn?.Invoke(newEnemy);

            enemyHealth.Death += EnemyHealth_Death;


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

        private void NewEnemy_FinishMove(EnemyMove enemy)
        {
            EnemyHealth_Death(enemy.GetComponent<EnemyHealth>());
        }

        private void EnemyHealth_Death(EnemyHealth enemy)
        {
            Assert.IsNotNull(enemy);

            enemyInWaveWasKilledOrRemoved++;
            enemyesOnBoardCollection.RemoveEnemy(enemy);

            if (enemyInWaveWasKilledOrRemoved == waves[waveIndex].items.Length)
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
                enemyInWaveWasKilledOrRemoved = 0;
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
            foreach (EnemyWaveData wave in waves)
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
