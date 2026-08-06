using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;

namespace Game.CoreGame
{
    class BombDebug : MonoBehaviour
    {
        [SerializeField, IsntNull] Bomb bombPrefab;
        [SerializeField, IsntNull] Transform target;

        [Button]
        void Spawn()
        {
            Bomb bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
            bomb.gameObject.SetActive(true);
            bomb.Init(new Damage(), new HealthComponentOnBoardCollection(), target.position);
        }
    }
}
