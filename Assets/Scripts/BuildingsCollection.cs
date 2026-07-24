using Game.CoreGame.Gui;
using Game.Upgrades;
using GamePackages.Core.Validation;
using System;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu]
    class BuildingsCollection : ScriptableObject
    {
        [Serializable]
        public class BuildigInfo
        {
            [IsntNull] public ShopItem shopItem;
            [IsntNull] public UpgradeData upgradeData;
        }

        [IsntNull] public BuildigInfo[] buildigs;
    }
}
