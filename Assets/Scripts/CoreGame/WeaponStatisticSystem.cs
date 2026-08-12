using Game.CoreGame;
using UnityEngine;

namespace Game.Assets.Scripts.CoreGame
{
    class WeaponStatisticSystem : MonoBehaviour
    {
        internal void OnSpawnEnmey(HealthComponent healthComponent)
        {
            healthComponent.TakeDemage += OnEnemy_TakeDemage;
        }

        private void OnEnemy_TakeDemage(HealthComponent target, float damage, WeaponComponent damageOwner)
        {
            WeaponStatistics statistics = damageOwner?.GetComponent<WeaponStatistics>();
            if (!statistics)
                return;

            statistics.totalDamage += damage;
        }
    }
}
