using UnityEngine;

namespace Game.CoreGame
{
    class NoAnimWeaponComponent : ShootingRangeWeaponComponent
    {
        [SerializeField] float attackPeriod;

        protected override void Attack(DamageReceiver enemyHealth, Damage damage)
        {
            enemyHealth.ApplyDamage(damage);
        }

        protected override void PlayAttackAnimation(out float nextAttackDelay, out float animationEventDelay)
        {
            nextAttackDelay = attackPeriod;
            animationEventDelay = 0;
        }
    }
}