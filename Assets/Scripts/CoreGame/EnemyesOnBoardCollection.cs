using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.CoreGame
{
    class EnemyesOnBoardCollection
    {
        List<EnemyHealth> enemyList = new();

        internal void AddEnemy(EnemyHealth enemyHealth)
        {
            Assert.IsNotNull(enemyHealth);
            enemyList.Add(enemyHealth);
        }

        [CanBeNull]
        internal EnemyHealth FindTargetForTower(Vector3 towerPosition, float towerAttackRange)
        {
            foreach (EnemyHealth enemy in enemyList)
            {
                float distance = Vector2.Distance(towerPosition, enemy.transform.position);
                if (distance <= towerAttackRange)
                    return enemy;
            }

            return null;
        }

        internal void RemoveEnemy(EnemyHealth enemyHealth)
        {
            Assert.IsNotNull(enemyHealth);
            enemyList.Remove(enemyHealth);
        }
    }
}
