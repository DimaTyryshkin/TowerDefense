using System;
using UnityEngine;

namespace Game.CoreGame
{
    internal enum FindTargetMode
    {
        Basic = 0,
        Random = 1,
    }

    abstract class ShootingRangeWeaponComponent : RangeWeaponComponent
    {
        [SerializeField] float damageValue;
        [SerializeField] DamageType damageType;
        [SerializeField] float damageDuration;
        [SerializeField] FindTargetMode findTargetMode;

        internal override bool IsTargetInRange => isAttacking || FindTarget();

        DamageReceiver lastTarget;
        float timeNextAttack;
        float timeNextAttackAnimationEvent;
        bool isAttacking;

        internal sealed override void UpdateFrame()
        {
            if (timeNextAttackAnimationEvent > 0 && Time.time > timeNextAttackAnimationEvent)
            {
                timeNextAttackAnimationEvent = 0;
                if (LastTargetExistOrFindNewTaret())
                {
                    Attack(lastTarget, new Damage()
                    {
                        value = damageValue,
                        type = damageType,
                        duration = damageDuration,

                    });
                }
            }

            if (Time.time < timeNextAttack)
                return;

            if (FindTarget())
            {
                isAttacking = true;
                PlayAttackAnimation(out float nextAttackDelay, out float animationEventDelay);
                timeNextAttackAnimationEvent = Time.time + animationEventDelay;
                timeNextAttack = Time.time + Mathf.Max(nextAttackDelay, animationEventDelay + 0.01f);
                RizeTargetnInRange();
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

        bool LastTargetExistOrFindNewTaret()
        {
            bool canAttackTarget = targets.CanAttack(transform.position, AttackRange, lastTarget);
            if (canAttackTarget)
                return true;
            else
                return FindTarget();
        }

        bool FindTarget()
        {
            lastTarget = FindTarget(lastTarget);
            return (bool)lastTarget;
        }

        protected DamageReceiver FindTarget(DamageReceiver lastTarget)
        {
            if (findTargetMode == FindTargetMode.Basic)
            {
                bool canAttackTarget = targets.CanAttack(transform.position, AttackRange, lastTarget);
                if (canAttackTarget)
                    return lastTarget;

                return targets
                    .Find(target => targets.CanAttack(transform.position, AttackRange, target));
            }

            if (findTargetMode == FindTargetMode.Random)
            {
                return targets
                    .FindRandom(lastTarget, target => targets.CanAttack(transform.position, AttackRange, target));
            }

            throw new NotSupportedException("FindTarget");
        }

        protected abstract void PlayAttackAnimation(out float nextAttackDelay, out float animationEventDelay);

        protected abstract void Attack(DamageReceiver enemy, Damage damage);
    }
}
