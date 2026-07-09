using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame
{
    class HomingRocketWeaponComponent : ShootingRangeWeaponComponent
    {
        [SerializeField] float attackPeriod;
        [SerializeField, IsntNull] HomingRocket homingRocketPrefab;
        [SerializeField, IsntNull] Transform startPoint;

        protected override void Attack(HealthComponent enemyHealth, Damage damage)
        {
            HomingRocket rocket = Instantiate(homingRocketPrefab);
            rocket.transform.position = startPoint.position;
            rocket.gameObject.SetActive(true);
            rocket.Init(enemyHealth, damage);
        }

        protected override void PlayAttackAnimation(ref float timeNextAttack, ref float timeNextAttackAnimationEvent)
        {
            timeNextAttack = Time.time + attackPeriod;
            timeNextAttackAnimationEvent = Time.time + attackPeriod * 0.2f;
        }
    }
}
