using GamePackages.Core.Validation;
using System;
using UnityEngine;

namespace IncreKindom
{
    public class FlashSprite : MonoBehaviour
    {
        [SerializeField] private Color flashColor = Color.white; // Цвет вспышки
        [SerializeField, Range(0f, 1f)] private float flashAmount = 0f;         // Сила вспышки (0-1)

        [SerializeField, IsntNull] SpriteRenderer spriteRenderer;
        private MaterialPropertyBlock _propertyBlock;

        private static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");
        private static readonly int FlashColorID = Shader.PropertyToID("_FlashColor");


        public SpriteRenderer SpriteRenderer => spriteRenderer;

        private void OnValidate()
        {
            if (!spriteRenderer)
                return;

            ApplyProperties();
        }

        private void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void ApplyProperties()
        {
            if (_propertyBlock == null)
                _propertyBlock = new MaterialPropertyBlock();

            spriteRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(FlashColorID, flashColor);
            _propertyBlock.SetFloat(FlashAmountID, flashAmount);
            spriteRenderer.SetPropertyBlock(_propertyBlock);
        }

        public void SetAmount(int value)
        {
            flashAmount = value;
            ApplyProperties();
        }
    }
}
