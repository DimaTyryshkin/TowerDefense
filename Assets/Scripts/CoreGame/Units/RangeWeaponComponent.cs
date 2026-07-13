using UnityEngine;
using UnityEngine.Events;

namespace Game.CoreGame
{
    abstract class RangeWeaponComponent : WeaponComponent
    {
        [SerializeField] float attackRange;


        internal event UnityAction TargetnInRange;

        internal float AttackRange => attackRange;

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, AttackRange);
        }
#endif

        protected void RizeTargetnInRange() { TargetnInRange?.Invoke(); }
    }
}
