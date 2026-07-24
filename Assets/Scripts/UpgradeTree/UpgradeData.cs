using Game.Common;
using GamePackages.Core;
using UnityEngine;

namespace Game.Upgrades
{
    public struct UpgradeLevel
    {
        public int level;

        public UpgradeLevel(int level)
        {
            this.level = level;
        }
    }

    [CreateAssetMenu]
    class UpgradeData : ScriptableObject
    {
        [SerializeField] float startValue;
        [SerializeField] float incrementPerLevel;
        [SerializeField] int cost;

        public float IncrementPerLevel => incrementPerLevel;
        public int Cost => cost;
        public UpgradeLevel Level => new UpgradeLevel(GameFactory.Data.upgrades.GetOrDefault(name, 0));
        public float Value => startValue + Level.level * incrementPerLevel;
        public int IntValue => Mathf.FloorToInt(Value);

        internal void AddLevel()
        {
            GameFactory.Data.upgrades[name] = Level.level + 1;
        }
        internal void SetOne()
        {
            GameFactory.Data.upgrades[name] = 1;
        }
    }
}
