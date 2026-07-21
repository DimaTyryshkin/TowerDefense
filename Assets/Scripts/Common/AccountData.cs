using GamePackages.JsonPlayerData;
using System.Collections.Generic;

namespace Game.Common
{
    public class AccountData : BaseAccountData
    {
        public int upgradePoints;

#pragma warning disable UAC1009 // Unsupported collection type for serialization
#pragma warning disable UAC1001 // Public field skipped by serialization due to missing [Serializable]

        public Dictionary<string, int> upgrades = new();

#pragma warning restore UAC1001 // Public field skipped by serialization due to missing [Serializable]
#pragma warning restore UAC1009 // Unsupported collection type for serialization
    }
}