using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace Game.CoreGame
{
    class TwoLazerWeaponComponent : ShootingRangeWeaponComponent
#if UNITY_EDITOR
        , IValidated
#endif
    {
        [SerializeField, IsntNull] Transform lazerStartPoint1;
        [SerializeField, IsntNull] Transform lazerStartPoint2;
        [SerializeField, IsntNull] AnimationClip attackClip;
        [SerializeField, IsntNull] Animator animator;
        [SerializeField, IsntNull] LazerParticleSystem lazerParticleSystemPrefab;

        LazerParticleSystem lazerParticleSystem1;
        LazerParticleSystem lazerParticleSystem2;

        int attackHash = Animator.StringToHash("attack");
        float attackAnimationEventDelay;
        float attackAnimationPeriod;

        private void Start()
        {
            lazerParticleSystem1 = transform.InstantiateAsChild(lazerParticleSystemPrefab);
            lazerParticleSystem2 = transform.InstantiateAsChild(lazerParticleSystemPrefab);
            lazerParticleSystem1.transform.position = new Vector3(999999, 0, 0);
            lazerParticleSystem2.transform.position = new Vector3(999999, 0, 0);
            lazerParticleSystem1.gameObject.SetActive(true);
            lazerParticleSystem2.gameObject.SetActive(true);

            attackAnimationEventDelay = attackClip.events[0].time;
            attackAnimationPeriod = attackClip.length + 0.1f;
        }
        protected override void Attack(HealthComponent enemyHealth, Damage damage)
        {
            lazerParticleSystem1.Play(lazerStartPoint1, enemyHealth.transform);
            lazerParticleSystem2.Play(lazerStartPoint2, enemyHealth.transform);
            enemyHealth.SetDamage(damage);
        }

        protected override void PlayAttackAnimation(ref float timeNextAttack, ref float timeNextAttackAnimationEvent)
        {
            animator.SetTrigger(attackHash);
            timeNextAttack = Time.time + attackAnimationPeriod;
            timeNextAttackAnimationEvent = Time.time + attackAnimationEventDelay;
        }

#if UNITY_EDITOR
        void IValidated.Validate(ValidationContext context)
        {
            if (attackClip && attackClip.events.Length != 1)
                context.AddProblem(nameof(WeaponTowerAI), ValidationProblem.Type.Error, "Анимация атаки должна содержать один ивент");

            if (attackClip && animator.runtimeAnimatorController)
            {
                AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
                if (!AnimationExist(controller, attackClip))
                    context.AddProblem(nameof(WeaponTowerAI), ValidationProblem.Type.Error, "Анимация атаки не найдена в аниматоре");
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
