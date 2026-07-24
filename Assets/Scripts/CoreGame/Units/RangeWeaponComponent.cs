using Game.Upgrades;
using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Events;

namespace Game.CoreGame
{
    abstract class RangeWeaponComponent : WeaponComponent
    {
        [SerializeField, IsntNull] UpgradeData attackRange;

        internal event UnityAction TargetnInRange;
        internal float AttackRange => attackRange.Value;


#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (!attackRange)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, AttackRange);
        }
#endif

        protected void RizeTargetnInRange() { TargetnInRange?.Invoke(); }
    }
}
