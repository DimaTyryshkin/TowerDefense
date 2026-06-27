using GamePackages.Core.Validation;
using System;
using UnityEngine;

namespace Game.CoreGame
{
    [CreateAssetMenu]
    class EnemyWaveData : ScriptableObject
    {
        [SerializeField, IsntNull] internal WaveItem[] items;


        [Serializable]
        internal struct WaveItem
        {
            [IsntNull] public EnemyMove enemy;
            public int spawnPointIndex;
            public float delayBeforeSpawn;
        }
    }
}
