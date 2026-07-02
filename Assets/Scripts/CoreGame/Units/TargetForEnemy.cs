using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame
{
    class TargetForEnemy : MonoBehaviour
    {
        [SerializeField, IsntNull] HealthComponentView healthComponentView;
        [SerializeField, IsntNull] HealthComponent healthComponent;

        internal HealthComponent HealthComponent => healthComponent;

        internal void Start()
        {
            healthComponentView.Init(healthComponent, Vector3.zero);
            healthComponent.Death += HealthComponent_Death;
        }

        private void HealthComponent_Death(HealthComponent arg0)
        {
            gameObject.SetActive(false);
        }
    }
}
