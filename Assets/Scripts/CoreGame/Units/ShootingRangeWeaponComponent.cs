using Game.Upgrades;
using GamePackages.Core.Validation;
using System;
using UnityEngine;

namespace Game.CoreGame
{
    internal enum FindTargetMode
    {
        Basic = 0,
        Random = 1,
        MaxHp = 2,
        BasicAndRandom = 3,
    }

    abstract class ShootingRangeWeaponComponent : RangeWeaponComponent
    {
        [SerializeField] float damageDuration;
        [SerializeField, IsntNull] UpgradeData damageValue;
        [SerializeField] DamageType damageType;
        [SerializeField] FindTargetMode findTargetMode;

        internal override bool IsTargetInRange => isAttacking || FindTarget();

        DamageReceiver lastTarget;
        float timeNextAttack;
        float timeNextAttackAnimationEvent;
        bool isAttacking;
        int findTaretCounter;

        float DamageValue => damageValue.Value;

        private void Start()
        {
            findTaretCounter = UnityEngine.Random.Range(0, 100);
        }

        internal sealed override void UpdateFrame()
        {
            if (Time.time >= timeNextAttack)
            {
                if (FindTarget())
                {
                    isAttacking = true;
                    PlayAttackAnimation(out float nextAttackDelay, out float animationEventDelay);
                    timeNextAttackAnimationEvent = Time.time + animationEventDelay;
                    timeNextAttack = Time.time + Mathf.Max(nextAttackDelay, animationEventDelay);
                    RizeTargetnInRange();
                }
                else
                {

                    isAttacking = false;
                }
            }

            if (timeNextAttackAnimationEvent > 0 && Time.time >= timeNextAttackAnimationEvent)
            {
                timeNextAttackAnimationEvent = 0;
                if (LastTargetExistOrFindNewTaret())
                {
                    findTaretCounter++;
                    Attack(lastTarget, new Damage()
                    {
                        value = DamageValue,
                        type = damageType,
                        duration = damageDuration,

                    });
                }
            }
        }


        protected override void OnDiactivateState()
        {
            isAttacking = false;
            timeNextAttackAnimationEvent = 0;
        }

        bool LastTargetExistOrFindNewTaret()
        {
            bool canAttackTarget = targets.CanAttackWithRange(transform.position, AttackRange, lastTarget);
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

        DamageReceiver FindTarget(DamageReceiver lastTarget)
        {
            if (findTargetMode == FindTargetMode.Basic ||
               (findTargetMode == FindTargetMode.BasicAndRandom && (findTaretCounter % 2 == 0)))
            {
                bool canAttackTarget = targets.CanAttackWithRange(transform.position, AttackRange, lastTarget);
                if (canAttackTarget)
                    return lastTarget;

                return targets
                    .Find(target => targets.CanAttackWithRange(transform.position, AttackRange, target));
            }

            if (findTargetMode == FindTargetMode.Random ||
               (findTargetMode == FindTargetMode.BasicAndRandom && (findTaretCounter % 2 == 1)))
            {
                return targets
                    .FindRandom(lastTarget, target => targets.CanAttackWithRange(transform.position, AttackRange, target));
            }

            if (findTargetMode == FindTargetMode.MaxHp)
            {
                return targets
                    .FindMaxHp(target => targets.CanAttackWithRange(transform.position, AttackRange, target));
            }

            throw new NotSupportedException("FindTarget");
        }

        protected abstract void PlayAttackAnimation(out float nextAttackDelay, out float animationEventDelay);

        protected abstract void Attack(DamageReceiver enemy, Damage damage);
    }
}
