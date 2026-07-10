using Game.CoreGame;
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
        [SerializeField, IsntNull] WayMoveComponent enemy;

        [Inject] EnemySpawner enemySpawner;


        internal event UnityAction ClickAddMoney;

        void Start()
        {
            AddButton("AddMoney", () => ClickAddMoney.Invoke());
            AddButton("Enemy01", () => enemySpawner.DebugSpawnEnmey(enemy));

            timeScaleSlider.onValueChanged.AddListener(TimeSlider_OnValueChanged);
        }

        private void TimeSlider_OnValueChanged(float arg0)
        {
            Time.timeScale = arg0;
        }

        void AddButton(string label, UnityAction onClick)
        {
            Button newButton = root.InstantiateAsChild(button);
            newButton.gameObject.SetActive(true);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = label;
            newButton.onClick.AddListener(onClick);
        }
    }
}
