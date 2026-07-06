using NaughtyAttributes;
using UnityEngine;

namespace Game
{
    class Test : MonoBehaviour
    {
        [Button]
        void Do()
        {
            var psr = GetComponent<ParticleSystemRenderer>();
            Debug.Log(psr.sortingLayerID + "  " + psr.sortingLayerName);
        }
    }
}
