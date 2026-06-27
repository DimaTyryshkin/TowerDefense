using System.IO;
using GamePackages.JsonPlayerData;
using UnityEngine;

namespace Game.Common
{
    public static class GameFactory
    {
        static PlayerDataStorage<AccountData> storage;
        public static string PlayerDataStorageFileName => Path.Combine(Application.persistentDataPath, "PlayerData.json");

        public static PlayerDataStorage<AccountData> Storage
        {
            get
            {
                if (storage == null)
                {
                    IStringStorage stringStorage =
#if UNITY_EDITOR
                        new FileStringStorage(new FileInfo(PlayerDataStorageFileName));
#else
                        new PlayerPrefsStringStorage("PlayerDataJsonString", new PlayerPrefsWrapper());
#endif
                    storage = new PlayerDataStorage<AccountData>(stringStorage);
                }

                return storage;
            }
        }

        public static AccountData Data => Storage.GetDataSingleton();
    }
}