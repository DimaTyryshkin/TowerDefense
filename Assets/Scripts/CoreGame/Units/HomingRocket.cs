using GamePackages.Core.Validation;
using NUnit.Framework;
using UnityEngine;

namespace Game.CoreGame
{
    class HomingRocket : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField, IsntNull] ParticleSystem vfx;
        HealthComponent target;
        Damage damage;

        internal void Init(HealthComponent target, Damage damage)
        {
            Assert.IsNotNull(target);
            this.target = target;
            this.damage = damage;
        }

        void Update()
        {
            if (!target || target.IsDeath)
            {
                StopFly();
                return;
            }

            if (Vector2.Distance(transform.position, target.transform.position) < 0.01f)
            {
                target.SetDamage(damage);
                StopFly();
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, target.transform.position, Time.deltaTime * speed);
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
