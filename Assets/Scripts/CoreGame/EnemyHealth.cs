using UnityEngine;
using UnityEngine.Events;

namespace Game.CoreGame
{
    class EnemyHealth : MonoBehaviour
    {
        [SerializeField] float maxHealth;
        float health;

        internal event UnityAction<EnemyHealth> Death;
        internal event UnityAction<EnemyHealth, float> HealthChanged;

        internal bool IsDeath => health <= 0;
        internal float MaxHealth => maxHealth;
        internal float Health => health;


        internal void Init()
        {
            health = maxHealth;
        }

        internal void SetDamage(Demage demage)
        {
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
