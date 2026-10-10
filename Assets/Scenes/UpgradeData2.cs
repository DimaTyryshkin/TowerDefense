using Game.Upgrades;
using NaughtyAttributes;
using UnityEngine;

namespace IncreKindom
{
    [CreateAssetMenu]
    class UpgradeData2 : ScriptableObject
    {
        [SerializeField] string guiName;
        [SerializeField] int maxLevel;

        [Header("Cost")]
        [SerializeField] float startCost;
        [SerializeField] float lastCost;

        [Header("Value")]
        [SerializeField] float startValue;
        [SerializeField] float lastValue;


        public string GuiName => guiName;
        public float Value => GetValue(level);
        public float Cost => GetCost(level);
        public int IntValue => Mathf.FloorToInt(GetValue(level));
        public int IntCost => Mathf.FloorToInt(GetCost(level));
        public UpgradeLevel Level => new UpgradeLevel(level);
        //public UpgradeLevel Level => new UpgradeLevel(GameFactory.Data.upgrades.GetOrDefault(name, 0));
        public UpgradeLevel MaxLevel => new UpgradeLevel(maxLevel);


        int level { get; set; }

        internal void AddLevel()
        {
            level++;
            //GameFactory.Data.upgrades[name] = Level.level + 1;

        }

        [Button]
        internal void ResetLevet()
        {
            //GameFactory.Data.upgrades[name] = 1;
            level = 0;
        }

        public float GetValue(int level)
        {
            //return ((lastValue - startValue) / (maxLevel - 1)) * (level - 1) + startValue; // Линейный рост от Lvl1
            return ((lastValue - startValue) / maxLevel) * level + startValue; // Линейный рост от Lvl0
        }

        public float GetCost(int level)
        {
            // return ((lastCost - startCost) / (maxLevel - 1)) * (level - 1) + startCost; // Линейный рост от Lvl1
            return ((lastCost - startCost) / maxLevel) * level + startCost; // Линейный рост от Lvl0
        }

        [ShowNativeProperty] float Cost0 => GetCost(0);
        [ShowNativeProperty] float Cost1 => GetCost(1);
        [ShowNativeProperty] float CostLast => GetCost(maxLevel);
        [ShowNativeProperty] float Value0 => GetValue(0);
        [ShowNativeProperty] float Value1 => GetValue(1);
        [ShowNativeProperty] float ValueLast => GetValue(maxLevel);
    }
}
