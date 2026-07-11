using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame
{
    class EnemyDamageReceiver : DamageReceiver
    {
        [SerializeField, IsntNull] SlowingBuffComponent slowingBuff;
        [SerializeField, IsntNull] PeriodicDamageBuffComponent periodicDamageBuff;

        private void Start()
        {
            healthComponent.Death += HealthComponent_Death;
        }

        internal override void ApplyDamage(Damage damage)
        {
            if (damage.type == DamageType.Basic)
                healthComponent.ApplyDamage(damage.value);
            else if (damage.type == DamageType.Coold)
                slowingBuff.Activate(damage.value, damage.duration);
            else if (damage.type == DamageType.Poison)
                periodicDamageBuff.Activate(damage.value, damage.duration);
        }

        void HealthComponent_Death(HealthComponent arg0)
        {
            slowingBuff.Deactivate();
            periodicDamageBuff.Deactivate();
        }
    }
}
