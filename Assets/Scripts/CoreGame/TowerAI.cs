using GamePackages.Core.Validation;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using GamePackages.Core;


#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace Game.CoreGame
{
    class TowerAI : MonoBehaviour
#if UNITY_EDITOR
        , IValidated
#endif
    {
        [SerializeField] float attackRange;
        [SerializeField] float damageValue;
        [SerializeField, IsntNull] Transform lazerStartPoint1;
        [SerializeField, IsntNull] Transform lazerStartPoint2;
        [SerializeField, IsntNull] AnimationClip attackClip;
        [SerializeField, IsntNull] Animator animator;
        [SerializeField, IsntNull] LazerParticleSystem lazerParticleSystemPrefab;


        [Header("Shop")]
        [SerializeField] int costInShop;
        internal Currency CostInShop => new Currency(costInShop);

        EnemyesOnBoardCollection enemyesOnBoardCollection;
        EnemyHealth target;
        LazerParticleSystem lazerParticleSystem1;
        LazerParticleSystem lazerParticleSystem2;
        float timeNextAttack;
        float attackAnimationPeriod;
        float timeNextAttackAnimationEvent;
        float attackAnimationEventDelay;
        Vector3 lazerStartPointsDelta;

        int attackHash = Animator.StringToHash("attack");

        private void Start()
        {
            lazerParticleSystem1 = transform.InstantiateAsChild(lazerParticleSystemPrefab);
            lazerParticleSystem2 = transform.InstantiateAsChild(lazerParticleSystemPrefab);
            lazerParticleSystem1.transform.position = new Vector3(999999, 0, 0);
            lazerParticleSystem2.transform.position = new Vector3(999999, 0, 0);
            lazerParticleSystem1.gameObject.SetActive(true);
            lazerParticleSystem2.gameObject.SetActive(true);


            lazerStartPointsDelta = (lazerStartPoint2.position - lazerStartPoint1.position) * 0.5f;

            attackAnimationEventDelay = attackClip.events[0].time;
            attackAnimationPeriod = attackClip.length + 0.1f;
        }

        private void Update()
        {
            if (timeNextAttackAnimationEvent > 0 && Time.time > timeNextAttackAnimationEvent)
            {
                timeNextAttackAnimationEvent = 0;
                AttackTarget();
            }

            if (Time.time < timeNextAttack)
                return;

            target = FindTarget();
            if (target)
                StartPlayAttackAnimation();
        }


        [CanBeNull]
        EnemyHealth FindTarget()
        {
            bool canAttackTarget = CanAttackTargte(target);
            if (canAttackTarget)
                return target;

            return enemyesOnBoardCollection.FindTargetForTower(CanAttackTargte);
        }

        bool CanAttackTargte(EnemyHealth enemyHealth)
        {
            bool isDeath = !enemyHealth || enemyHealth.IsDeath;
            if (isDeath)
                return false;

            float distanceToTarget = Vector2.Distance(transform.position, enemyHealth.transform.position);
            return distanceToTarget <= attackRange;
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

        void StartPlayAttackAnimation()
        {
            animator.SetTrigger(attackHash);
            timeNextAttack = Time.time + attackAnimationPeriod;
            timeNextAttackAnimationEvent = Time.time + attackAnimationEventDelay;
        }

        void AttackTarget()
        {
            target = FindTarget();
            if (target)
                Attack(target);
        }

        void Attack(EnemyHealth enemyHealth)
        {
            lazerParticleSystem1.Play(lazerStartPoint1, enemyHealth.transform);
            lazerParticleSystem2.Play(lazerStartPoint2, enemyHealth.transform);

            enemyHealth.SetDamage(new Demage()
            {
                value = damageValue
            });
        }

#if UNITY_EDITOR
        void IValidated.Validate(ValidationContext context)
        {
            if (attackClip && attackClip.events.Length != 1)
                context.AddProblem(nameof(TowerAI), ValidationProblem.Type.Error, "Анимация атаки должна содержать один ивент");

            if (attackClip && animator.runtimeAnimatorController)
            {
                AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
                if (!AnimationExist(controller, attackClip))
                    context.AddProblem(nameof(TowerAI), ValidationProblem.Type.Error, "Анимация атаки не найдена в аниматоре");
            }
        }

        bool AnimationExist(AnimatorController animatorController, AnimationClip animationClip)
        {

            foreach (AnimatorControllerLayer layer in animatorController.layers)
            {
                foreach (ChildAnimatorState state in layer.stateMachine.states)
                {
                    AnimationClip clip = state.state.motion as AnimationClip;
                    if (clip == attackClip)
                        return true;
                }
            }

            return false;
        }

#endif
    }
}
