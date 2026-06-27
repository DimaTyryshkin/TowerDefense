using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame
{
    class EnemySpawnPoint : MonoBehaviour
    {
        [IsntNull] public WayPoints wayPoints;
    }
}
