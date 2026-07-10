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
            Bomb bomb = Instantiate(bombPrefab);
            bomb.transform.position = startPoint.position;
            bomb.gameObject.SetActive(true);

            EnemyAi enemyAi = enemyHealth.GetComponent<EnemyAi>();

            Vector2 targetPos = Vector2.zero;

            if (enemyAi)
                targetPos = enemyAi.PredictPosition(bombPrefab.Duration);
            else
                targetPos = enemyHealth.transform.position;

            bomb.Init(damage, targets, targetPos);
        }

        protected override void PlayAttackAnimation(ref float timeNextAttack, ref float timeNextAttackAnimationEvent)
        {
            timeNextAttack = Time.time + attackPeriod;
            timeNextAttackAnimationEvent = Time.time;
        }
    }
}
