using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame
{
    class TargetForEnemy : MonoBehaviour
    {
        [SerializeField, IsntNull] HealthComponentView healthComponentView;
        [SerializeField, IsntNull] DamageReceiver damageReceiver;

        internal DamageReceiver DamageReceiver => damageReceiver;

        internal void Start()
        {
            healthComponentView.Init(damageReceiver.Health, Vector3.zero);
            damageReceiver.Health.Death += HealthComponent_Death;
        }

        private void HealthComponent_Death(HealthComponent arg0)
        {
            gameObject.SetActive(false);
        }
    }
}
