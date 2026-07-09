using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame
{
    class BombWeaponComponent : ShootingRangeWeaponComponent
    {
        [SerializeField] float attackPeriod;
        [SerializeField, IsntNull] Bomb bombPrefab;
        [SerializeField, IsntNull] Transform startPoint;

        protected override void Attack(HealthComponent enemyHealth, Damage damage)
        {
            Bomb rocket = Instantiate(bombPrefab);
            rocket.transform.position = startPoint.position;
            rocket.gameObject.SetActive(true);
            rocket.Init(damage, targets, enemyHealth.transform.position);
        }

        protected override void PlayAttackAnimation(ref float timeNextAttack, ref float timeNextAttackAnimationEvent)
        {
            timeNextAttack = Time.time + attackPeriod;
            timeNextAttackAnimationEvent = Time.time + attackPeriod * 0.2f;
        }
    }
}
