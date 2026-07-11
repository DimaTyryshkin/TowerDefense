using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame
{
    abstract class DamageReceiver : MonoBehaviour
    {
        [SerializeField, IsntNull] protected HealthComponent healthComponent;
        internal HealthComponent Health => healthComponent;

        internal abstract void ApplyDamage(Damage damage);
    }
}
