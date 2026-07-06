using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame
{
    class EnemyAi : MonoBehaviour
    {
        [SerializeField, IsntNull] HealthComponent health;
        [SerializeField, IsntNull] WayMoveComponent moveComponent;
        [SerializeField, IsntNull] WeaponComponent weapon;

        UnitStateMashine stateMashine;

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

        internal void Init(HealthComponentOnBoardCollection targets, WayPoints wayPoints)
        {
            stateMashine = new();
            weapon.Init(targets);
            moveComponent.Init(wayPoints);
        }
    }
}