using GamePackages.Core.Validation;

using UnityEngine;
using UnityEngine.Assertions;

namespace Game.CoreGame
{
    class HealthComponentView : MonoBehaviour
    {
        [SerializeField, IsntNull] ProgressBarView redHealthProressBar;
        [SerializeField, IsntNull] ProgressBarView greenHealthProressBar;
        [SerializeField] float redToGreenLerpfactor;
        [SerializeField] bool doNotMove;

        HealthComponent enemyHealth;
        Vector3 offset;

        private void LateUpdate()
        {
            if (!doNotMove)
                transform.position = enemyHealth.transform.position + offset;

            redHealthProressBar.NormilizedValue = Mathf.Lerp(
                redHealthProressBar.NormilizedValue,
                greenHealthProressBar.NormilizedValue,
                Time.deltaTime * redToGreenLerpfactor);
        }

        internal void Init(HealthComponent enemyHealth, Vector3 offset)
        {
            Assert.IsNotNull(enemyHealth);
            this.enemyHealth = enemyHealth;
            this.offset = offset;
            enemyHealth.HealthChanged += (enemy, _) => Draw(enemy);
            enemyHealth.Death += _ => gameObject.SetActive(false);

            redHealthProressBar.NormilizedValue = 1;
            Draw(enemyHealth);
        }

        void Draw(HealthComponent enemyHealth)
        {
            //redHealthProressBar.NormilizedValue = greenHealthProressBar.NormilizedValue;
            greenHealthProressBar.NormilizedValue = enemyHealth.Health / enemyHealth.MaxHealth;
        }
    }
}
