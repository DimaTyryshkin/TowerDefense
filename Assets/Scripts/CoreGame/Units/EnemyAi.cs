using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.CoreGame
{
    class EnemyAi : MonoBehaviour
    {
        [SerializeField] float damage = 1;
        [SerializeField] float attackRange = 1;
        [SerializeField] float attackPeriod = 2;
        [SerializeField, IsntNull] Animator animator;
        [SerializeField, IsntNull] HealthComponent health;
        [SerializeField, IsntNull] WayMoveComponent moveComponent;

        HealthComponentOnBoardCollection targets;
        float timeNextAttack;

        private void Start()
        {
            health.Death += EnemyHealth_Death;
        }

        void Update()
        {
            if (targets is null || health.IsDeath)
                return;

            HealthComponent target = targets.Find(target => targets.CanAttack(transform.position, attackRange, target));
            if (target)
            {
                if (Time.time >= timeNextAttack)
                {
                    timeNextAttack = Time.time + attackPeriod;
                    target.SetDamage(new Demage() { value = damage });
                    //animator.SetTrigger()
                }
            }
            else
            {
                moveComponent.UpdateFrame();
            }
        }

        internal void Init(HealthComponentOnBoardCollection targets)
        {
            Assert.IsNotNull(targets);
            this.targets = targets;
        }

        void EnemyHealth_Death(HealthComponent arg0)
        {
            //animator.SetTrigger()
        }
    }
}
