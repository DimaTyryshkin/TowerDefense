using GamePackages.Core.Validation;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.CoreGame.Gui
{

    class ContextMenuGui : MonoBehaviour
    {
        [SerializeField, IsntNull] RectTransform root;
        [SerializeField, IsntNull] Button buttonSold;
        [SerializeField, IsntNull] TMP_Text damageText;
        [SerializeField, IsntNull] TMP_Text totalDamageText;

        internal event UnityAction<ShopItem> ClickSoldTower;

        ShopItem shopItem;

        private void Start()
        {
            buttonSold.onClick.AddListener(() => ClickSoldTower.Invoke(shopItem));
        }

        internal RectTransform RectTransform => root;

        public void Show(ShopItem shopItem)
        {
            gameObject.SetActive(true);
            Assert.IsNotNull(shopItem);
            this.shopItem = shopItem;

            WeaponStatistics weaponStatistics = shopItem.GetComponent<WeaponStatistics>();
            totalDamageText.text = weaponStatistics ?
                GuiFormat.FormatToInt(weaponStatistics.totalDamage) :
                "-";


            ShootingRangeWeaponComponent weapon = shopItem.GetComponent<ShootingRangeWeaponComponent>();
            damageText.text = weapon ?
               GuiFormat.FormatToInt(weapon.DamageValue) :
               "-";
        }

        internal void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}