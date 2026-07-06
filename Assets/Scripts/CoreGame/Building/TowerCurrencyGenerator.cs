using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;


namespace Game.CoreGame
{
    class TowerCurrencyGenerator : MonoBehaviour
    {
        [SerializeField] int moneyAddPedWave;
        [SerializeField, IsntNull] ParticleSystem effectVfx;

        internal Currency MoneyAddPedWaveAmount => new Currency(moneyAddPedWave);

        [Button]
        internal void ShowEffect()
        {
            ParticleSystem vfx = Instantiate(effectVfx, transform.position, Quaternion.identity);
            //ParticleSystem vfx = transform.InstantiateAsChild(effectVfx);
            //vfx.transform.position = transform.position;

            //vfx.Play(withChildren: true);
            Destroy(vfx.gameObject, vfx.totalTime + 5);
        }
    }
}
