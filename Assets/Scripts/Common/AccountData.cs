using System;
using GamePackages.JsonPlayerData;

namespace Game.Common
{
    [Serializable]
    public class AccountData : BaseAccountData
    {
        public int SoulsAmount;
        public int LevelNumber;
    }
}