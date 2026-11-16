using GamePackages.Core.Validation;
using TMPro;
using UnityEngine;

namespace IncreGame
{
    public class ProgressBarWithText : MonoBehaviour
    {
        [SerializeField] string format = "{0} %";
        [SerializeField, IsntNull] RectTransform slider;
        [SerializeField, IsntNull] TMP_Text text;

        int LastPercentValue { get; set; } = -1;

        public void Draw(int value, int maxValue)
        {
            float normilizedValue = Mathf.Clamp01((float)value / maxValue);
            int percentValue = Mathf.FloorToInt(normilizedValue * 100);
            if (LastPercentValue != percentValue)
            {
                LastPercentValue = percentValue;
                slider.localScale = new Vector3(normilizedValue, 1, 1);
                text.text = string.Format(format, percentValue);
            }
        }
    }
}
