using GamePackages.Core.Validation;
using NUnit.Framework;
using UnityEngine;

namespace Game.CoreGame
{
    class HomingRocket : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField, IsntNull] ParticleSystem vfx;
        DamageReceiver target;
        Damage damage;

        internal void Init(DamageReceiver target, Damage damage)
        {
            Assert.IsNotNull(target);
            this.target = target;
            this.damage = damage;
        }

        void Update()
        {
            if (!target || target.Health.IsDeath)
            {
                StopFly();
                return;
            }

            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * speed);
            if (Vector2.Distance(transform.position, target.transform.position) < 0.01f)
            {
                target.ApplyDamage(damage);
                StopFly();
            }
        }

        void StopFly()
        {
            vfx.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
            Destroy(gameObject, 5);
            enabled = false;
        }
    }
}
