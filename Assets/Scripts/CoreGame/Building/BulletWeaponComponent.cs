using GamePackages.Core.Validation;
using UnityEngine;




#if UNITY_EDITOR
#endif

namespace Game.CoreGame
{
    class BulletWeaponComponent : RangeWeaponComponent
    {
        [SerializeField] float attackPeriod;
        [SerializeField, IsntNull] Bullet bulletPrefab;

        protected override void Attack(HealthComponent enemyHealth, Damage damage)
        {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.transform.position = transform.position;
            bullet.gameObject.SetActive(true);
            bullet.Init(enemyHealth, damage);
        }

        protected override void PlayAttackAnimation(ref float timeNextAttack, ref float timeNextAttackAnimationEvent)
        {
            timeNextAttack = Time.time + attackPeriod;
            timeNextAttackAnimationEvent = Time.time + attackPeriod * 0.2f;
        }
    }
}
