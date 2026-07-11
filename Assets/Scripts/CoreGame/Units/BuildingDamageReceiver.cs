namespace Game.CoreGame
{
    class BuildingDamageReceiver : DamageReceiver
    {
        internal override void ApplyDamage(Damage damage)
        {
            if (damage.type == DamageType.Basic)
                healthComponent.ApplyDamage(damage.value);
        }
    }
}
