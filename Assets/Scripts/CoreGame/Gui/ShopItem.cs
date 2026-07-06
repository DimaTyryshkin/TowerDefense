using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame.Gui
{
    class ShopItem : MonoBehaviour
    {
        [SerializeField, IsntNull] Sprite sprite;
        [SerializeField] int cost;

        internal Sprite Sprite => sprite;
        internal Currency Cost => new Currency(cost);
    }
}