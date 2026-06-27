using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame
{
    class ProgressBarView : MonoBehaviour
    {
        [SerializeField, IsntNull] Transform progress;

        float normilizedValue;

        internal float NormilizedValue
        {
            get => normilizedValue;
            set
            {
                normilizedValue = value;
                progress.localScale = new Vector3(value, 1, 1);
            }
        }

    }
}
