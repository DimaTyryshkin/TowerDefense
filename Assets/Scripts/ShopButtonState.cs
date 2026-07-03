using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    class ShopButtonState
    {
        internal Currency Cost { get; private set; }
        internal Sprite Sprite { get; private set; }
        internal Action OnClick { get; private set; }

        internal bool wasBuilded;

        internal ShopButtonState(Currency cost, Sprite sprite, Action onClick)
        {
            Assert.IsNotNull(sprite);
            Assert.IsNotNull(onClick);
            this.Cost = cost;
            this.Sprite = sprite;
            this.OnClick = onClick;
        }
    }
}
