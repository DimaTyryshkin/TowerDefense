namespace Game.CoreGame
{
    internal enum DamageType
    {
        Basic = 0,
        Coold = 1,
        Poison = 2,
    }

    struct Damage
    {
        internal readonly float value;
        internal readonly DamageType type;
        internal readonly float duration;
        internal readonly WeaponComponent damageOwner;

        public Damage(float value, DamageType type, float duration, WeaponComponent damageOwner)
        {
            this.value = value;
            this.type = type;
            this.duration = duration;
            this.damageOwner = damageOwner;
        }
    }
}
