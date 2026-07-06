using UnityEngine;

namespace Game.CoreGame
{
    abstract class UnitStateComponent : MonoBehaviour
    {
        private bool isActive;

        internal bool IsActive
        {
            get => isActive;
            set
            {
                if (isActive == value)
                    return;

                if (value)
                {
                    isActive = true;
                    OnActivateState();
                }
                else
                {
                    isActive = false;
                    OnDiactivateState();
                }
            }
        }

        internal abstract void UpdateFrame();

        protected virtual void OnActivateState() { }

        protected virtual void OnDiactivateState() { }
    }
}
