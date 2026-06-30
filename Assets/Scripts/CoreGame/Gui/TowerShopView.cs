using GamePackages.Core;
using GamePackages.Core.Validation;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.CoreGame.Gui
{
    class TowerShopView : MonoBehaviour
    {
        [SerializeField] int buttonMaxAmount;
        [SerializeField, IsntNull] TMP_Text bankText;
        [SerializeField, IsntNull] Button startWaveButton;
        [SerializeField, IsntNull] TowerButtonView buttonTemplate;
        [SerializeField, IsntNull] RectTransform buttonsRoot;

        internal event UnityAction<TowerButtonView> SelectTower;
        internal event UnityAction ClickStartWave;
        internal int MaxButtonAmount => buttonMaxAmount;

        private void Start()
        {
            buttonTemplate.gameObject.SetActive(false);
            startWaveButton.onClick.AddListener(() => ClickStartWave.Invoke());
        }

        internal void Darw(Currency playerBank, ShopItemState[] shopStates)
        {
            buttonsRoot.DestroyChildren();

            bankText.text = playerBank.money.ToString();
            Assert.IsTrue(shopStates.Length <= MaxButtonAmount);
            for (var i = 0; i < shopStates.Length; i++)
            {
                ShopItemState state = shopStates[i];

                TowerButtonView button = buttonsRoot.InstantiateAsChild(buttonTemplate);
                button.gameObject.SetActive(true);

                if (state == null || state.wasBuilded)
                {
                    button.DarwInvisible();
                    button.gameObject.name = state == null ? "null" : "wasBuilded";
                    continue;
                }

                Assert.IsNotNull(state.shopItem);
                button.Draw(state);
                button.Click += TowerButton_Click;

                if (playerBank < state.shopItem.Cost)
                    button.DrawNotEnoughtBank();
            }
        }

        internal void Show() => gameObject.SetActive(true);

        internal void Hide() => gameObject.SetActive(false);

        private void TowerButton_Click(TowerButtonView button)
        {
            SelectTower.Invoke(button);
        }


    }
}