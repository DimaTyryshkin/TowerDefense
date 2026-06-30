using GamePackages.Core.Validation;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.CoreGame.Gui
{
    class TowerButtonView : MonoBehaviour
    {
        [SerializeField, IsntNull] CanvasGroup canvasGroup;
        [SerializeField, IsntNull] Image towerImage;
        [SerializeField, IsntNull] Button button;
        [SerializeField, IsntNull] TMP_Text text;
        [SerializeField, IsntNull] Color notEnoughtTextColor;
        [SerializeField, IsntNull] Color notEnoughtImageColor;

        internal ShopItemState ShopItemState => shopItemState;
        internal event UnityAction<TowerButtonView> Click;
        ShopItemState shopItemState;

        private void Start()
        {
            button.onClick.AddListener(OnClick);
        }

        internal void Draw(ShopItemState shopItemState)
        {
            Assert.IsNotNull(shopItemState);
            Assert.IsNotNull(shopItemState.shopItem);

            ShopItem shopItem = shopItemState.shopItem;
            Assert.IsNotNull(shopItem);
            Assert.IsNotNull(shopItem.Tower);
            Assert.IsNotNull(shopItem.Sprite);
            this.shopItemState = shopItemState;

            DrawCost();
            towerImage.sprite = shopItem.Sprite;
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
            towerImage.color = notEnoughtImageColor;
        }

        void DrawCost()
        {
            if (shopItemState.shopItem.Cost.money > 0)
            {
                text.text = shopItemState.shopItem.Cost.money.ToString();
            }
            else
            {
                text.gameObject.SetActive(false);
            }
        }

        void OnClick()
        {
            Click.Invoke(this);
        }
    }
}