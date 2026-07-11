using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame
{
    class SlowingBuffComponent : BuffComponent
    {
        [SerializeField, IsntNull] GameObject vfx;
        [SerializeField, IsntNull] WayMoveComponent move;

        private void Start()
        {
            vfx.SetActive(false);
        }

        protected override void OnActivete()
        {
            move.buffSpeedFactor = 1 - Value;
            vfx.SetActive(true);
        }

        protected override void OnDeactivete()
        {
            move.buffSpeedFactor = 1;
            vfx.SetActive(false);
        }

        protected override void OnUpdate()
        {

        }
    }
}
