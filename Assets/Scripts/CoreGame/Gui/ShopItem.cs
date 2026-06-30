using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame.Gui
{
    class ShopItem : MonoBehaviour
    {
        [SerializeField, IsntNull] Sprite sprite;
        [SerializeField, IsntNull] TowerAI tower;
        [SerializeField] int cost;

        internal Sprite Sprite => sprite;
        internal TowerAI Tower => tower;
        internal Currency Cost => new Currency(cost);
    }
}