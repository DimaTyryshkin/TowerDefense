using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.CoreGame
{
    class HealthComponentOnBoardCollection
    {
        List<HealthComponent> healthList = new();

        internal bool CanAttack(Vector2 damageSourcePos, float attackRange, HealthComponent targetHealth)
        {
            bool isDeath = !targetHealth || targetHealth.IsDeath;
            if (isDeath)
                return false;

            float distanceToTarget = Vector2.Distance(damageSourcePos, targetHealth.transform.position);
            return distanceToTarget <= attackRange;
        }

        [CanBeNull]
        internal HealthComponent Find(Predicate<HealthComponent> predicate)
        {
            foreach (HealthComponent health in healthList)
            {
                //float distance = Vector2.Distance(towerPosition, enemy.transform.position);
                //if (distance <= towerAttackRange)
                if (predicate(health))
                    return health;
            }

            return null;
        }

        internal void Add(HealthComponent health)
        {
            Assert.IsNotNull(health);
            Assert.IsFalse(healthList.Contains(health));

            healthList.Add(health);
        }

        internal void Remove(HealthComponent health)
        {
            Assert.IsNotNull(health);
            Assert.IsTrue(healthList.Remove(health));
        }
    }
}
