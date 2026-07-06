using UnityEngine.Assertions;

namespace Game.CoreGame
{
    class UnitStateMashine
    {
        UnitStateComponent oldState;

        internal void UpdateFrame(UnitStateComponent state)
        {
            Assert.IsNotNull(state);

            if (oldState && oldState != state)
            {
                oldState.IsActive = false;
                state.IsActive = true;
                oldState = state;
            }

            state.UpdateFrame();
        }
    }
}
