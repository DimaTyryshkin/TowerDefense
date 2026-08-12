using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame
{
    abstract class DamageReceiver : MonoBehaviour
    {
        [SerializeField, IsntNull] protected HealthComponent healthComponent;
        [SerializeField, IsntNull] Transform viewCenter;
        internal HealthComponent Health => healthComponent;

        internal Transform ViewCenter => viewCenter;

        internal abstract void ApplyDamage(Damage damage);
    }
}
