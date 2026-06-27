using UnityEngine;
using UnityEngine.Assertions;

namespace Game.CoreGame
{
    class TowerAI : MonoBehaviour
    {
        [SerializeField] float attackRange;
        [SerializeField] float attackPeriod;
        [SerializeField] float damageValue;

        EnemyesOnBoardCollection enemyesOnBoardCollection;
        EnemyHealth target;
        float timeNextAttack;

        private void Update()
        {
            if (Time.time < timeNextAttack)
                return;

            bool canAttackTarget = target && !target.IsDeath;
            if (canAttackTarget)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
                canAttackTarget = distanceToTarget <= attackRange;
            }

            if (canAttackTarget)
            {
                AttackTaret();
            }
            else
            {
                target = enemyesOnBoardCollection.FindTargetForTower(transform.position, attackRange);
            }
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        internal void Init(EnemyesOnBoardCollection enemyesOnBoardCollection)
        {
            Assert.IsNotNull(enemyesOnBoardCollection);
            this.enemyesOnBoardCollection = enemyesOnBoardCollection;
        }

        void AttackTaret()
        {
            timeNextAttack = Time.time + attackPeriod;
            target.SetDamage(new Demage()
            {
                value = damageValue
            });
        }



    }
}
