using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.CoreGame
{
    class EnemyAi : MonoBehaviour
    {
        [SerializeField, IsntNull] HealthComponent health;
        [SerializeField, IsntNull] WayMoveComponent moveComponent;
        [SerializeField, IsntNull] WeaponComponent weapon;
        [SerializeField, IsntNull] GameObject grave;

        UnitStateMashine stateMashine;
        Transform gravesRoot;

        void Update()
        {
            if (health.IsDeath)
                return;

            if (weapon.IsTargetInRange)
            {
                stateMashine.UpdateFrame(weapon);
            }
            else
            {
                stateMashine.UpdateFrame(moveComponent);
            }
        }

        internal void Init(HealthComponentOnBoardCollection targets, WayPoints wayPoints, Transform gravesRoot)
        {
            Assert.IsNotNull(targets);
            Assert.IsNotNull(wayPoints);
            Assert.IsNotNull(gravesRoot);
            this.gravesRoot = gravesRoot;
            stateMashine = new();
            weapon.Init(targets);
            moveComponent.Init(wayPoints);
            health.Death += Health_Death;
        }

        private void Health_Death(HealthComponent arg0)
        {
            Vector3 p = (Vector2)transform.position + Random.insideUnitCircle * 0.4f;
            var go = gravesRoot.InstantiateAsChild(grave, p);
            go.gameObject.transform.position = p;
            Destroy(gameObject);
        }

        internal Vector2 PredictPosition(float inFutureTimeOffset)
        {
            if (stateMashine.LastState == moveComponent)
                return moveComponent.PredictPosition(inFutureTimeOffset);
            else
                return transform.position;
        }
    }
}