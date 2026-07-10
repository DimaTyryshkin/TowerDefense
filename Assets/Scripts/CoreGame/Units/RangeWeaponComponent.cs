using UnityEngine;

namespace Game.CoreGame
{
    abstract class RangeWeaponComponent : WeaponComponent
    {
        [SerializeField] float attackRange;

        internal float AttackRange => attackRange;

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, AttackRange);
        }
#endif
    }
}
