using GamePackages.Core.Validation;

using UnityEngine;
using UnityEngine.Assertions;

namespace Game.CoreGame
{
    class EnemyHealthView : MonoBehaviour
    {
        [SerializeField, IsntNull] ProgressBarView redHealthProressBar;
        [SerializeField, IsntNull] ProgressBarView greenHealthProressBar;
        [SerializeField] float redToGreenLerpfactor;

        EnemyHealth enemyHealth;
        Vector3 offset;


        private void LateUpdate()
        {
            transform.position = enemyHealth.transform.position + offset;
            redHealthProressBar.NormilizedValue = Mathf.Lerp(
                redHealthProressBar.NormilizedValue,
                greenHealthProressBar.NormilizedValue,
                Time.deltaTime * redToGreenLerpfactor);
        }

        internal void Init(EnemyHealth enemyHealth, Vector3 offset)
        {
            Assert.IsNotNull(enemyHealth);
            this.enemyHealth = enemyHealth;
            this.offset = offset;
            enemyHealth.HealthChanged += (enemy, _) => Draw(enemy);
            enemyHealth.Death += _ => gameObject.SetActive(false);

            redHealthProressBar.NormilizedValue = 1;
            Draw(enemyHealth);
        }

        void Draw(EnemyHealth enemyHealth)
        {
            //redHealthProressBar.NormilizedValue = greenHealthProressBar.NormilizedValue;
            greenHealthProressBar.NormilizedValue = enemyHealth.Health / enemyHealth.MaxHealth;
        }
    }
}
