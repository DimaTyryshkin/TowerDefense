using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Game.CoreGame
{
    class WayMoveComponent : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField, IsntNull] Animator animator;

        internal event UnityAction<WayMoveComponent> FinishMove;

        WayPoints wayPoints;
        int targetPointIndex;
        Vector2 nextPoint;

        int sideHash = Animator.StringToHash("side");
        int walkHash = Animator.StringToHash("walk");

        void Start()
        {
            //debugText = GizmosDrawer.Inst.AddText(transform.position, "");
            animator.SetBool(walkHash, true);
        }

        internal void UpdateFrame()
        {
            if (wayPoints == null)
                return;

            Vector2 pos = transform.position;

            float distanceToPoint = Vector2.Distance(pos, nextPoint);
            bool closeToPoint = distanceToPoint < 0.001f;
            if (closeToPoint)
            {
                bool nextPointExist = targetPointIndex < wayPoints.Count - 1;
                if (nextPointExist)
                {
                    targetPointIndex++;
                    Transform wayPoint = wayPoints.GetPoint(targetPointIndex);
                    nextPoint = wayPoint.transform.position;
                }
                else
                {
                    FinishMove.Invoke(this);
                    gameObject.SetActive(false);
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPoint, GetSpeed() * Time.deltaTime);

                Vector2 toTaret = nextPoint - pos;
                int side = Side.FromDir(toTaret).side;
                // GizmosDrawer.Inst.AddText(transform.position, side).SetDuration(-1);

                animator.SetInteger(sideHash, side);
            }
        }

        internal void SetWayPoints(WayPoints wayPoints)
        {
            Assert.IsNotNull(wayPoints);
            this.wayPoints = wayPoints;
            nextPoint = wayPoints.GetPoint(0).position;
        }

        float GetSpeed()
        {
            return speed;
        }
    }
}
