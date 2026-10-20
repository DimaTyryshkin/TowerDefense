using GamePackages.Core.Validation;
using NaughtyAttributes;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace IncreKindom
{
    public class UpgradeButton : MonoBehaviour
    {
        [SerializeField, IsntNull] Button button;
        [SerializeField, IsntNull] Image image;
        [SerializeField, IsntNull] GameObject greenBack;
        [SerializeField, IsntNull] GameObject grayBask;
        [SerializeField, IsntNull] GameObject maxLevelPanel;
        [SerializeField, IsntNull] GameObject mainPanel;
        [SerializeField, IsntNull] TMP_Text labelText1;
        [SerializeField, IsntNull] TMP_Text labelText2;
        [SerializeField, IsntNull] TMP_Text costText1;
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
        }

        public void DrawImage(Sprite sprite)
        {
            image.sprite = sprite;
        }

        public void Draw(bool available, int cost, int level, int maxLevel)
        {
            bool isMaxLevel = level >= maxLevel && maxLevel > 0;
            bool canClick = !isMaxLevel && available;
            bool notEnought = !isMaxLevel && !available;

            greenBack.SetActive(canClick);
            grayBask.SetActive(!canClick);
            maxLevelPanel.SetActive(isMaxLevel);
            mainPanel.SetActive(!isMaxLevel);

            if (canClick || notEnought)
            {
                levelText1.text = LevelToStr(level, maxLevel);
                costText1.text = CostToStr(cost);
            }
            else if (isMaxLevel)
            {
                levelText2.text = LevelToStr(level, maxLevel);
            }
        }

        string LevelToStr(int level, int maxLevel)
        {
            if (maxLevel == 0)
                return level.ToString("N0", CultureInfo.InvariantCulture);

            if (level == maxLevel && maxLevel > 0)
                return "MAX";

            return level.ToString("N0", CultureInfo.InvariantCulture) + "/" + maxLevel;
        }

        string CostToStr(int cost) => cost.ToString("N0", CultureInfo.InvariantCulture);

        [Button]
        void DrawMax()
        {
            Draw(true, 5, 2, 2);
        }

        [Button]
        void DrawNotEnought()
        {
            Draw(false, 7, 5, 6);
        }

        [Button]
        void DrawCanBuy()
        {
            Draw(true, 10, 10, 13);
        }
    }
}
