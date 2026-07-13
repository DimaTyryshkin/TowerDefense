using GamePackages.Core;
using GamePackages.Core.Validation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.CoreGame
{
    [CreateAssetMenu]
    class WaveData : ScriptableObject
    {
        [SerializeField] float delayBetweenSpawn;
        [SerializeField, IsntNull] WaveItemInfo[] enemyInfo;

        internal int TotalAmount => totalAmount;
        internal int CurrentCount => currentCount;

        int totalAmount;
        int currentCount;
        float delayBetweenSpawnMin;
        float delayBetweenSpawnMax;
        List<WaveItemInfo> enemyInfoCopy;


        [System.Serializable]
        internal class WaveItemInfo
        {
            public int count;
            public AnimationCurve chanceOverTime;
            [IsntNull] public WayMoveComponent enemy;
            [HideInInspector] public int currentCount;
        }

        internal struct WaveItem
        {
            [IsntNull] public WayMoveComponent enemy;
            public int spawnPointIndex;
            public float delayAfterSpawn;
        }


        internal void Init()
        {
            enemyInfoCopy = enemyInfo.ToList();

            foreach (WaveItemInfo item in enemyInfoCopy)
                item.currentCount = 0;

            totalAmount = enemyInfoCopy.Sum(x => x.count);
            currentCount = 0;
            delayBetweenSpawnMin = delayBetweenSpawn - delayBetweenSpawn * 0.5f;
            delayBetweenSpawnMax = delayBetweenSpawn + delayBetweenSpawn * 0.5f;
        }

        internal WaveItem GetNextEnemy(int spawnPointsAmount)
        {
            float t = currentCount / (float)totalAmount;
            int index = enemyInfoCopy.RandomWithWeight(UnityEngine.Random.value, x => x.chanceOverTime.Evaluate(t));

            var enemyInfo = enemyInfoCopy[index];

            WaveItem item = new WaveItem()
            {
                enemy = enemyInfo.enemy,
                spawnPointIndex = Random.Range(0, spawnPointsAmount),
                delayAfterSpawn = Random.Range(delayBetweenSpawnMin, delayBetweenSpawnMax)
            };

            enemyInfo.currentCount++;
            currentCount++;
            if (enemyInfo.currentCount == enemyInfo.count)
                enemyInfoCopy.Remove(enemyInfo);

            return item;
        }
    }
}
