using UnityEngine;
using UnityEngine.Assertions;




#if UNITY_EDITOR
#endif

namespace Game.CoreGame
{
    class Bullet : MonoBehaviour
    {
        [SerializeField] float speed;

        HealthComponent target;
        Damage damage;

        private void Update()
        {
            if (!target && target.IsDeath)
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

        internal void Init(HealthComponent target, Damage damage)
        {
            Assert.IsNotNull(target);
            Assert.IsTrue(damage.value > 0);

            this.target = target;
            this.damage = damage;
        }

        void StopFly()
        {
            Destroy(gameObject, 0);
            enabled = false;
        }
    }
}
