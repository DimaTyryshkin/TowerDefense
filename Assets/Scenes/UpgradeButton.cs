using GamePackages.Core.Validation;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace IncreKindom
{
    public class UpgradeButton : MonoBehaviour
    {
        [SerializeField] string text;
        [SerializeField, IsntNull] Button button;
        [SerializeField, IsntNull] GameObject panel1;
        [SerializeField, IsntNull] GameObject panel2;
        [SerializeField, IsntNull] TMP_Text labelText1;
        [SerializeField, IsntNull] TMP_Text labelText2;
        [SerializeField, IsntNull] TMP_Text costText1;
        [SerializeField, IsntNull] TMP_Text costText2;
        [SerializeField, IsntNull] TMP_Text levelText1;
        [SerializeField, IsntNull] TMP_Text levelText2;

        public event UnityAction Click;

        private void Start()
        {
            labelText1.text = text;
            labelText2.text = text;

            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            Click?.Invoke();
        }

        public void Draw(bool available, int cost, int level)
        {
            panel1.SetActive(available);
            panel2.SetActive(!available);

            if (available)
            {
                levelText1.text = level.ToString();
                costText1.text = cost.ToString();
            }
            else
            {
                levelText2.text = level.ToString();
                costText2.text = cost.ToString();
            }
        }

        [Button]
        void Draw1()
        {
            Draw(true, 5, 2);
        }

        [Button]
        void Draw2()
        {
            Draw(false, 5, 2);
        }
    }
}
