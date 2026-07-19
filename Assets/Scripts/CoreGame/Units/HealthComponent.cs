using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Game.CoreGame
{
    class HealthComponent : MonoBehaviour
    {
        [SerializeField] float maxHealth;
        float health;

        internal event UnityAction<HealthComponent> Death;
        internal event UnityAction<HealthComponent, float> HealthChanged;
        internal bool IsDeath => health <= 0;
        internal bool IsLive => health > 0;
        internal float MaxHealth => maxHealth;
        internal float Health => health;

        internal void Init()
        {
            health = maxHealth;
            HealthChanged?.Invoke(this, 0);
        }

        internal void ApplyDamage(float damage)
        {
            Assert.IsTrue(IsLive);

            float resultDamage = Mathf.Min(damage, health);
            health -= resultDamage;

            if (health <= 0)
            {
                HealthChanged?.Invoke(this, -resultDamage);
                Death?.Invoke(this);
            }
            else
            {
                HealthChanged?.Invoke(this, -resultDamage);
            }
        }

        internal void Restart()
        {
            float add = maxHealth - health;
            health = maxHealth;
            HealthChanged?.Invoke(this, add);
        }

        [Button]
        void AddHp()
        {
            health = Mathf.Min(maxHealth, health + 1);
            HealthChanged?.Invoke(this, 1);
        }

        [Button]
        void Kill()
        {
            ApplyDamage(health);
        }
    }
}
