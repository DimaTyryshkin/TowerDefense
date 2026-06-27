using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.CoreGame
{
    class EnemyMove : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField, IsntNull] Animator animator;
        [SerializeField, IsntNull] EnemyHealth enemyHealth;

        WayPoints wayPoints;
        int targetPointIndex;
        Vector2 target;

        int sideHash = Animator.StringToHash("side");
        int walkHash = Animator.StringToHash("walk");

        void Start()
        {
            //debugText = GizmosDrawer.Inst.AddText(transform.position, "");
            animator.SetBool(walkHash, true);
            enemyHealth.Death += EnemyHealth_Death;
        }

        void Update()
        {
            if (wayPoints == null)
                return;

            Vector2 pos = transform.position;

            float distanceToTarget = Vector2.Distance(pos, target);
            bool closeToTarget = distanceToTarget < 0.001f;
            if (closeToTarget)
            {
                bool nextPointExist = targetPointIndex < wayPoints.Count - 1;
                if (nextPointExist)
                {
                    targetPointIndex++;
                    Transform wayPoint = wayPoints.GetPoint(targetPointIndex);
                    target = wayPoint.transform.position;
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, target, GetSpeed() * Time.deltaTime);

                Vector2 toTaret = target - pos;
                int side = Side.FromDir(toTaret).side;
                // GizmosDrawer.Inst.AddText(transform.position, side).SetDuration(-1);

                animator.SetInteger(sideHash, side);
            }
        }

        void EnemyHealth_Death(EnemyHealth arg0)
        {
            wayPoints = null;
        }



        internal void SetWayPoints(WayPoints wayPoints)
        {
            Assert.IsNotNull(wayPoints);
            this.wayPoints = wayPoints;
            target = wayPoints.GetPoint(0).position;
        }

        float GetSpeed()
        {
            return speed;
        }
    }
}
