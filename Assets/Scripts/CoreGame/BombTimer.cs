using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;

namespace Game.CoreGame
{
    class BombTimer : MonoBehaviour
    {
        [SerializeField, IsntNull] DigitSprites digitSprites;
        [SerializeField, IsntNull] SpriteRenderer tensRenreder;
        [SerializeField, IsntNull] SpriteRenderer onesRenderer;


        internal void SetCount(int count)
        {
            count = Mathf.Clamp(count, 0, 99);

            int tens = count / 10;
            int ones = count % 10;

            tensRenreder.sprite = digitSprites.digits[tens];
            onesRenderer.sprite = digitSprites.digits[ones];
        }

        [SerializeField] int testValue;

        [Button]
        void BegugTest()
        {
            SetCount(testValue);
        }
    }
}
