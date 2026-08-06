using GamePackages.Core.Validation;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace Game.CoreGame
{
    class World : MonoBehaviour
    {
        [IsntNull] public Grid grid;
        [IsntNull] public EnemySpawnPoint[] enemySpawnPoints;
    }
}
