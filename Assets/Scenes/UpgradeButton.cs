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
        [SerializeField, IsntNull] Button button;
        [SerializeField, IsntNull] GameObject panel1;
        [SerializeField, IsntNull] GameObject panel2;
        [SerializeField, IsntNull] GameObject panelMax;
        [SerializeField, IsntNull] TMP_Text labelText1;
        [SerializeField, IsntNull] TMP_Text labelText2;
        [SerializeField, IsntNull] TMP_Text labelText3;
        [SerializeField, IsntNull] TMP_Text costText1;
        [SerializeField, IsntNull] TMP_Text costText2;
        [SerializeField, IsntNull] TMP_Text levelText1;
        [SerializeField, IsntNull] TMP_Text levelText2;

        public event UnityAction Click;

        private void Start()
        {
            button.onClick.AddListener(OnClick);
        }

        void OnClick()
        {
            Click?.Invoke();
        }

        public void DrawLabel(string label)
        {
            labelText1.text = label;
            labelText2.text = label;
            labelText3.text = label;
        }

        public void Draw(bool available, int cost, int level, int maxLevel)
        {
            bool isMaxLevel = level >= maxLevel;
            bool a = !isMaxLevel && available;
            bool b = !isMaxLevel && !available;

            panel1.SetActive(a);
            panel2.SetActive(b);
            panelMax.SetActive(isMaxLevel);

            if (a)
            {
                levelText1.text = level.ToString();
                costText1.text = cost.ToString();
            }

            if (b)
            {
                levelText2.text = level.ToString();
                costText2.text = cost.ToString();
            }
        }

        [Button]
        void Draw1()
        {
            Draw(true, 5, 2, 2);
        }

        [Button]
        void Draw2()
        {
            Draw(false, 5, 2, 3);
        }
    }
}
