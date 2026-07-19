using GamePackages.Core;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.CoreGame
{
    class HealthComponentOnBoardCollection
    {
        List<DamageReceiver> targetList = new();
        List<DamageReceiver> tempList = new();

        internal bool CanAttackWithRange(Vector2 damageSourcePos, float attackRange, DamageReceiver target)
        {
            bool isDeath = !target || target.Health.IsDeath;
            if (isDeath)
                return false;

            float distanceToTarget = Vector2.Distance(damageSourcePos, target.transform.position);
            return distanceToTarget <= attackRange;
        }

        [CanBeNull]
        internal DamageReceiver Find(Predicate<DamageReceiver> predicate)
        {
            foreach (DamageReceiver target in targetList)
            {
                //float distance = Vector2.Distance(towerPosition, enemy.transform.position);
                //if (distance <= towerAttackRange)
                if (predicate(target))
                    return target;
            }

            return null;
        }


        internal DamageReceiver[] FindAll(Predicate<DamageReceiver> predicate)
        {
            foreach (DamageReceiver target in targetList)
            {
                //float distance = Vector2.Distance(towerPosition, enemy.transform.position);
                //if (distance <= towerAttackRange)
                if (predicate(target))
                    tempList.Add(target);
            }

            DamageReceiver[] result = tempList.ToArray();
            tempList.Clear();
            return result;
        }

        internal DamageReceiver FindRandom(DamageReceiver oldTaret, Predicate<DamageReceiver> predicate)
        {
            foreach (DamageReceiver target in targetList)
            {
                if (target != oldTaret && predicate(target))
                    tempList.Add(target);
            }

            if (tempList.Count > 0)
            {
                DamageReceiver randomTarget = tempList.Random();
                tempList.Clear();
                return randomTarget;
            }

            if (predicate(oldTaret))
                return oldTaret;

            return null;
        }

        internal DamageReceiver FindMaxHp(Predicate<DamageReceiver> predicate)
        {
            foreach (DamageReceiver target in targetList)
            {
                if (predicate(target))
                    tempList.Add(target);
            }

            if (tempList.Count > 0)
            {
                DamageReceiver target = tempList.MaxItem(t => t.Health.Health);
                tempList.Clear();
                return target;
            }

            return null;
        }

        internal void Add(DamageReceiver target)
        {
            Assert.IsNotNull(target);
            Assert.IsFalse(targetList.Contains(target));

            targetList.Add(target);
        }

        internal void Remove(DamageReceiver target)
        {
            Assert.IsNotNull(target);
            Assert.IsTrue(targetList.Remove(target));
        }
    }
}
