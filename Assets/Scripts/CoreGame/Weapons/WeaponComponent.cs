using UnityEngine.Assertions;

namespace Game.CoreGame
{
    abstract class WeaponComponent : UnitStateComponent
    {
        internal abstract bool IsTargetInRange { get; }

        protected HealthComponentOnBoardCollection targets;


        internal void Init(HealthComponentOnBoardCollection targets)
        {
            Assert.IsNotNull(targets);
            this.targets = targets;
        }
    }
}