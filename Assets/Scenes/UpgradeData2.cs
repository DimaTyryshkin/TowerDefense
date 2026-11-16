using Game.Common;
using Game.Upgrades;
using GamePackages.Core;
using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;

namespace IncreGame
{
    [CreateAssetMenu]
    public class UpgradeData2 : ScriptableObject
    {
        [SerializeField] string guiName;
        [SerializeField, ShowAssetPreview, IsntNull] Sprite sprite;
        [SerializeField] int maxLevel;

        [Header("Cost")]
        [SerializeField] int startCost;
        [SerializeField] int lastCost;

        [Header("Value")]
        [SerializeField] float startValue;
        [SerializeField] float lastValue;


        public string GuiName => guiName;
        public Sprite Sprite => sprite;
        public float Value => GetValue(level);
        public float Cost => GetCost(level);
        public int IntValue => Mathf.FloorToInt(GetValue(level));
        public bool IsOpen => level > 0;
        public bool IsMaxLevel => level == maxLevel;
        public int IntCost => Mathf.FloorToInt(GetCost(level));
        public UpgradeLevel Level => new UpgradeLevel(level);
        //public UpgradeLevel Level => new UpgradeLevel(GameFactory.Data.upgrades.GetOrDefault(name, 0));
        public UpgradeLevel MaxLevel => new UpgradeLevel(maxLevel);
        public bool NotMaxLevel => level < maxLevel || maxLevel == 0;


        int level
        {
            get => GameFactory.Data.upgrades.GetOrDefault(name, 0);
            set => GameFactory.Data.upgrades[name] = value;
        }

        public void AddLevel()
        {
            level++;
            //GameFactory.Data.upgrades[name] = Level.level + 1;

        }

        public void SetLevel(int level) => this.level = level;

        [Button]
        public void ResetLevet()
        {
            //GameFactory.Data.upgrades[name] = 1;
            level = 0;
        }

        [Button]
        public void SetOne()
        {
            level = 1;
        }

        public float GetValue(int level)
        {
            //return ((lastValue - startValue) / (maxLevel - 1)) * (level - 1) + startValue; // Линейный рост от Lvl1
            return ((lastValue - startValue) / maxLevel) * level + startValue; // Линейный рост от Lvl0
        }

        public int GetCostSum(int level)
        {
            float sum = 0;
            for (int i = 0; i < level; i++)
                sum += GetCost(i);

            return Mathf.FloorToInt(sum);
        }


        public int GetCost(int level)
        {
            if (maxLevel == 0)
                return startCost;

            // return ((lastCost - startCost) / (maxLevel - 1)) * (level - 1) + startCost; // Линейный рост от Lvl1
            float cost = ((lastCost - startCost) / maxLevel) * level + startCost; // Линейный рост от Lvl0
            return Mathf.FloorToInt(cost); ;
        }

        [ShowNativeProperty] float Cost0 => GetCost(0);
        [ShowNativeProperty] float Cost1 => GetCost(1);
        [ShowNativeProperty] float CostLast => GetCost(maxLevel);
        [ShowNativeProperty] float Value0 => GetValue(0);
        [ShowNativeProperty] float Value1 => GetValue(1);
        [ShowNativeProperty] float ValueLast => GetValue(maxLevel);

    }
}
