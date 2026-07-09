using UnityEngine;

namespace Game.CoreGame
{
    abstract class RangeWeaponComponent : WeaponComponent
    {
        [SerializeField] float attackRange;

        internal float AttackRange => attackRange;

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, AttackRange);
        }
    }
}
