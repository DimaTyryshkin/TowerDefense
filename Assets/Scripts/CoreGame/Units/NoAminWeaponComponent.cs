using UnityEngine;

namespace Game.CoreGame
{
    class NoAninWeaponComponent : ShootingRangeWeaponComponent
    {
        [SerializeField] float attackPeriod;

        protected override void Attack(HealthComponent enemyHealth, Damage damage)
        {
            enemyHealth.SetDamage(damage);
        }

        protected override void PlayAttackAnimation(ref float timeNextAttack, ref float timeNextAttackAnimationEvent)
        {
            timeNextAttack = Time.time + attackPeriod;
            timeNextAttackAnimationEvent = Time.time + attackPeriod * 0.5f;
        }
    }
}