using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;

namespace Game.CoreGame.Gui
{
    class ShopItem : MonoBehaviour
    {
        [ShowAssetPreview]
        [SerializeField, IsntNull] Sprite sprite;
        [SerializeField] int cost;

        internal Sprite Sprite => sprite;
        internal Currency Cost => new Currency(cost);

        private void OnValidate()
        {
            if (!sprite)
                return;

            GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }
}