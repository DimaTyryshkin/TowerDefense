using UnityEngine;

namespace Game.CoreGame
{
    abstract class RangeWeaponComponent : WeaponComponent
    {
        [SerializeField] float attackRange;
        [SerializeField] float damageValue;

        internal float AttackRange => attackRange;

        internal override bool IsTargetInRange => isAttacking || IsTargetExist();

        HealthComponent lastTarget;
        float timeNextAttack;
        float timeNextAttackAnimationEvent;
        bool isAttacking;

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        internal sealed override void UpdateFrame()
        {
            if (timeNextAttackAnimationEvent > 0 && Time.time > timeNextAttackAnimationEvent)
            {
                timeNextAttackAnimationEvent = 0;
                if (IsTargetExist())
                {
                    Attack(lastTarget, new Damage()
                    {
                        value = damageValue
                    });
                }
            }

            if (Time.time < timeNextAttack)
                return;

            if (IsTargetExist())
            {
                isAttacking = true;
                PlayAttackAnimation(ref timeNextAttack, ref timeNextAttackAnimationEvent);
            }
            else
            {
                isAttacking = false;
            }
        }

        protected override void OnDiactivateState()
        {
            isAttacking = false;
            timeNextAttackAnimationEvent = 0;
        }

        bool IsTargetExist()
        {
            lastTarget = FindTarget(lastTarget);
            return (bool)lastTarget;
        }

        protected HealthComponent FindTarget(HealthComponent lastTarget)
        {
            bool canAttackTarget = targets.CanAttack(transform.position, attackRange, lastTarget);
            if (canAttackTarget)
                return lastTarget;

            return targets
                .Find(target => targets.CanAttack(transform.position, attackRange, target));
        }

        protected abstract void PlayAttackAnimation(ref float timeNextAttack, ref float timeNextAttackAnimationEvent);

        protected abstract void Attack(HealthComponent enemyHealth, Damage damage);
    }
}
