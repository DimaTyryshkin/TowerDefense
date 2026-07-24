using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame
{
    [CreateAssetMenu]
    class DigitSprites : ScriptableObject, IValidated
    {
        [IsntNull] public Sprite[] digits;

        public void Validate(ValidationContext context)
        {
            if (digits.Length != 10)
                context.AddProblem(nameof(DigitSprites), msg: "Цифорок должно быть 10 шт");
        }
    }
}
