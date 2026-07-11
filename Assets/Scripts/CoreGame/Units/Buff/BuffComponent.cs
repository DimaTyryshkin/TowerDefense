using UnityEngine;

namespace Game.CoreGame
{
    abstract class BuffComponent : MonoBehaviour
    {
        float value;
        float endTime;

        internal float Value => value;

        private void Update()
        {
            if (Time.time > endTime)
                Deactivate();
            else
                OnUpdate();
        }

        internal void Activate(float value, float duration)
        {
            if (value >= this.value)
            {
                this.value = value;
                endTime = Time.time + duration;

                if (!enabled)
                {
                    enabled = true;
                    OnActivete();
                }
            }
        }

        internal void Deactivate()
        {
            if (enabled)
            {
                value = 0;
                enabled = false;
                OnDeactivete();
            }
        }

        protected abstract void OnActivete();
        protected abstract void OnUpdate();
        protected abstract void OnDeactivete();
    }
}
