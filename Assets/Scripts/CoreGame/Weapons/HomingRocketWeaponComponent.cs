using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame
{
    class HomingRocketWeaponComponent : ShootingRangeWeaponComponent
    {
        [SerializeField] float attackPeriod;
        [SerializeField] float animationEventDelay;

        [Space]
        [SerializeField, IsntNull] HomingRocket homingRocketPrefab;
        [SerializeField, IsntNull] Transform startPoint;

        protected override void Attack(DamageReceiver enemy, Damage damage)
        {
            HomingRocket rocket = Instantiate(homingRocketPrefab);
            rocket.transform.position = startPoint.position;
            rocket.gameObject.SetActive(true);
            rocket.Init(enemy, damage);
        }

        protected override void PlayAttackAnimation(out float nextAttackDelay, out float animationEventDelay)
        {
            nextAttackDelay = attackPeriod;
            animationEventDelay = this.animationEventDelay;
        }
    }
}
