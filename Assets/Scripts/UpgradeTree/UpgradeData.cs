using Game.Common;
using GamePackages.Core;
using UnityEngine;

namespace Game.Upgrades
{
    [CreateAssetMenu]
    class UpgradeData : ScriptableObject
    {
        [SerializeField] float startValue;
        [SerializeField] float incrementPerLevel;
        public float IncrementPerLevel => incrementPerLevel;
        public int Level => GameFactory.Data.upgrades.GetOrDefault(name, 0);
        public float Value => startValue + Level * incrementPerLevel;
        public int IntValue => Mathf.FloorToInt(Value);

        internal void AddLevel()
        {
            GameFactory.Data.upgrades[name] = Level + 1;
        }
    }
}
