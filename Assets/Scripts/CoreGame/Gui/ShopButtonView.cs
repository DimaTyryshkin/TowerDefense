using GamePackages.Core.Validation;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.CoreGame.Gui
{
    class ShopButtonView : MonoBehaviour
    {
        [SerializeField, IsntNull] CanvasGroup canvasGroup;
        [SerializeField, IsntNull] Image image;
        [SerializeField, IsntNull] Button button;
        [SerializeField, IsntNull] TMP_Text text;
        [SerializeField, IsntNull] Color notEnoughtTextColor;
        [SerializeField, IsntNull] Color notEnoughtImageColor;

        internal ShopButtonState State => state;
        internal event UnityAction<ShopButtonView> Click;
        ShopButtonState state;

        private void Start()
        {
            button.onClick.AddListener(() => Click.Invoke(this));
        }

        internal void Draw(ShopButtonState state)
        {
            Assert.IsNotNull(state);
            Assert.IsNotNull(state.Sprite);
            this.state = state;

            DrawCost();
            image.sprite = state.Sprite;
            button.interactable = true;
        }

        internal void DarwInvisible()
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;
        }

        internal void DrawNotEnoughtBank()
        {
            DrawCost();
            button.interactable = false;
            text.color = notEnoughtTextColor;
            image.color = notEnoughtImageColor;
        }

        void DrawCost()
        {
            if (state.Cost.money > 0)
            {
                text.text = state.Cost.money.ToString();
            }
            else
            {
                text.gameObject.SetActive(false);
            }
        }
    }
}