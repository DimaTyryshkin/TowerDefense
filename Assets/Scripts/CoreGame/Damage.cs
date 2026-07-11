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
        internal float value;
        internal DamageType type;
        internal float duration;
    }
}
