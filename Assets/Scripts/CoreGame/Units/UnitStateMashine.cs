using UnityEngine.Assertions;

namespace Game.CoreGame
{
    class UnitStateMashine
    {
        UnitStateComponent lastState;

        internal UnitStateComponent LastState => lastState;

        internal void UpdateFrame(UnitStateComponent state)
        {
            Assert.IsNotNull(state);

            if (lastState != state)
            {
                if (lastState)
                    lastState.IsActive = false;

                state.IsActive = true;
                lastState = state;
            }

            lastState.UpdateFrame();
        }
    }
}
