using GamePackages.Core.Validation;
using System;
using UnityEngine;

namespace Game.CoreGame
{
    [CreateAssetMenu]
    class WaveData : ScriptableObject
    {
        [SerializeField, IsntNull] internal WaveItem[] items;


        [Serializable]
        internal struct WaveItem
        {
            [IsntNull] public WayMoveComponent enemy;
            public int spawnPointIndex;
            public float delayBeforeSpawn;
        }
    }
}
