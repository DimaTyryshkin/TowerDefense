using JetBrains.Annotations;
using System;
using System.Collections.Generic;
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
        internal EnemyHealth FindTargetForTower(Predicate<EnemyHealth> predicate)
        {
            foreach (EnemyHealth enemy in enemyList)
            {
                //float distance = Vector2.Distance(towerPosition, enemy.transform.position);
                //if (distance <= towerAttackRange)
                if (predicate(enemy))
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
