using NaughtyAttributes;
using UnityEngine;

namespace Game.CoreGame
{
    class LazerParticleSystemTest : MonoBehaviour
    {
        [SerializeField] Transform p1;
        [SerializeField] Transform p2;

        [Button]
        void Play()
        {
            GetComponent<LazerParticleSystem>().Play(p1, p2);
        }
    }
}
