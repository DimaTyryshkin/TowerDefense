using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace Game.CoreGame
{
    class LazerWeaponComponent : ShootingRangeWeaponComponent
    {
        [SerializeField] float attackPeriod;
        [SerializeField, IsntNull] Transform lazerStartPoint;
        [SerializeField, IsntNull] Animator animator;
        [SerializeField, IsntNull] LazerParticleSystem lazerParticleSystemPrefab;

        LazerParticleSystem lazerParticleSystem;
        int attackHash = Animator.StringToHash("attack");


        private void Start()
        {
            lazerParticleSystem = transform.InstantiateAsChild(lazerParticleSystemPrefab);
            lazerParticleSystem.transform.position = new Vector3(999999, 0, 0);
            lazerParticleSystem.gameObject.SetActive(true);
        }
        protected override void Attack(DamageReceiver enemy, Damage damage)
        {
            lazerParticleSystem.Play(lazerStartPoint, enemy.transform);
            enemy.ApplyDamage(damage);
        }

        protected override void PlayAttackAnimation(out float nextAttackDelay, out float animationEventDelay)
        {
            animator.SetTrigger(attackHash);
            nextAttackDelay = attackPeriod;
            animationEventDelay = 0;
        }

    }
}
