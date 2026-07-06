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

        internal void SetDamage(Damage demage)
        {
            Assert.IsTrue(IsLive);

            float resultDamage = Mathf.Min(demage.value, health);
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
    }
}
