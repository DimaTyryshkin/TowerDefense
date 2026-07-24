using GamePackages.Core;
using GamePackages.Core.Validation;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    public class DebugPanel : MonoBehaviour
    {
        [SerializeField, IsntNull] RectTransform root;
        [SerializeField, IsntNull] Button button;
        [SerializeField, IsntNull] Slider timeScaleSlider;

        void Start()
        {
            timeScaleSlider.onValueChanged.AddListener(TimeSlider_OnValueChanged);
        }

        private void TimeSlider_OnValueChanged(float arg0)
        {
            Time.timeScale = arg0;
        }

        internal void AddButton(string label, UnityAction onClick)
        {
            Button newButton = root.InstantiateAsChild(button);
            newButton.gameObject.SetActive(true);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = label;
            newButton.onClick.AddListener(onClick);
        }
    }
}
