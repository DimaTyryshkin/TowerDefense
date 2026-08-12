using GamePackages.Core.Validation;

using UnityEngine;
using UnityEngine.Assertions;

namespace Game.CoreGame
{
    class HealthComponentView : MonoBehaviour
    {
        [SerializeField, IsntNull] ProgressBarView redHealthProressBar;
        [SerializeField, IsntNull] ProgressBarView greenHealthProressBar;
        [SerializeField] float redToGreenSpeed;
        [SerializeField] bool doNotMove;

        DamageReceiver damageReceiver;
        Vector3 offset;

        private void LateUpdate()
        {
            if (!damageReceiver)
            {
                Destroy(gameObject);
                return;
            }

            if (!doNotMove)
                transform.position = damageReceiver.ViewCenter.position + offset;

            redHealthProressBar.NormilizedValue = Mathf.MoveTowards(
                redHealthProressBar.NormilizedValue,
                greenHealthProressBar.NormilizedValue,
                Time.deltaTime * redToGreenSpeed);
        }

        internal void Init(DamageReceiver damageReceiver, Vector3 offset)
        {
            Assert.IsNotNull(damageReceiver);
            this.damageReceiver = damageReceiver;
            this.offset = offset;
            damageReceiver.Health.HealthChanged += (enemy, _) => Draw(enemy);
            damageReceiver.Health.Death += _ => Destroy(gameObject);

            redHealthProressBar.NormilizedValue = 1;
            Draw(damageReceiver.Health);
        }

        void Draw(HealthComponent enemyHealth)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            //redHealthProressBar.NormilizedValue = greenHealthProressBar.NormilizedValue;
            greenHealthProressBar.NormilizedValue = enemyHealth.Health / enemyHealth.MaxHealth;
        }
    }
}
