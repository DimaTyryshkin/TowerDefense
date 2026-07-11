using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame
{
    class PeriodicDamageBuffComponent : BuffComponent
    {
        [SerializeField, IsntNull] GameObject vfx;
        [SerializeField, IsntNull] HealthComponent health;

        private void Start()
        {
            vfx.SetActive(false);
        }

        protected override void OnActivete()
        {
            vfx.SetActive(true);
        }

        protected override void OnDeactivete()
        {
            vfx.SetActive(false);
        }

        protected override void OnUpdate()
        {
            health.ApplyDamage(Value * Time.deltaTime);
        }
    }
}
