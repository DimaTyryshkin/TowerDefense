using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame
{
    class WayMoveComponentDebug : MonoBehaviour
    {
        [SerializeField, Range(0, 10)] float time;
        [SerializeField, IsntNull] WayMoveComponent wayMoveComponent;


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(wayMoveComponent.PredictPosition(time), 0.3f);
        }
    }
}
