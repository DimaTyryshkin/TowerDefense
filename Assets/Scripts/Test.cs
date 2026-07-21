using Game.Common;
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

        [Button]
        void SavePrefsTest()
        {
            GameFactory.Data.upgrades["testNode"] = 1;
            GameFactory.Data.Save();
        }

        [Button]
        void ReadPrefsTest()
        {
            GameFactory.Storage.ReloadFromDisc();
            foreach ((string key, int value) in GameFactory.Data.upgrades)
            {
                Debug.Log(key + "  " + value);
            }
        }
    }
}
