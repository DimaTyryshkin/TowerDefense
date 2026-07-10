using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Game.CoreGame
{
    class WayMoveComponent : UnitStateComponent
    {
        [SerializeField] float speed;
        [SerializeField, IsntNull] Animator animator;

        internal event UnityAction<WayMoveComponent> FinishMove;

        WayPoints wayPoints;
        int targetPointIndex;
        Vector2 nextPoint;

        int sideHash = Animator.StringToHash("side");
        int walkHash = Animator.StringToHash("walk");

        internal override void UpdateFrame()
        {
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
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPoint, GetSpeed() * Time.deltaTime);
                animator.SetBool(walkHash, true);

                Vector2 toTaret = nextPoint - pos;
                int side = Side.FromDir(toTaret).side;
                // GizmosDrawer.Inst.AddText(transform.position, side).SetDuration(-1);

                animator.SetInteger(sideHash, side);
            }

        }

        internal void Init(WayPoints wayPoints)
        {
            Assert.IsNotNull(wayPoints);
            this.wayPoints = wayPoints;
            nextPoint = wayPoints.GetPoint(0).position;
        }

        internal Vector2 PredictPosition(float inFutureTimeOffset)
        {
            Vector2 pos = transform.position;
            float distance = GetSpeed() * inFutureTimeOffset;

            int pointIndex = targetPointIndex;

            while (distance > 0)
            {
                Vector2 pointPos = wayPoints.GetPoint(pointIndex).transform.position;
                float distanceToPoint = Vector2.Distance(pos, pointPos);
                if (distanceToPoint >= distance)
                {
                    pos = pos + (pointPos - pos).normalized * distance;
                    distance = 0;
                }
                else
                {
                    pos = pointPos;
                    distance -= distanceToPoint;
                    pointIndex++;

                    if (pointIndex >= wayPoints.Count)
                        return pos;
                }
            }

            return pos;
        }

        float GetSpeed()
        {
            return speed;
        }
    }
}
