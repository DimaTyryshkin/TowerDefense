using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame
{
    class RangeVisualizer : MonoBehaviour
    {
        [SerializeField, IsntNull] ParticleSystem rangeVfx;

        bool isStoped;

        internal Vector3 Position
        {
            set => transform.position = value;
        }

        internal void StopAndCelar()
        {
            if (isStoped)
                return;

            rangeVfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            isStoped = true;
        }

        internal void Play(float range, Color color)
        {
            if (!isStoped)
                return;

            var share = rangeVfx.shape;
            var main = rangeVfx.main;

            share.radius = range;
            main.startColor = color;

            rangeVfx.Play(true);

            isStoped = false;
        }
    }
}