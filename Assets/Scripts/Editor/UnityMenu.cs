using Game.Common;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class UnityMenu
    {
        [MenuItem("Game/PlayerPrefs/Save", false, 1)]
        public static void SavePlayerPrefs()
        {
            EditorUtility.OpenWithDefaultApp(GameFactory.PlayerDataStorageFileName);
            Debug.Log(GameFactory.PlayerDataStorageFileName);
        }

        [MenuItem("Game/PlayerPrefs/Open", false, 2)]
        public static void OpenPlayerPrefs()
        {
            EditorUtility.OpenWithDefaultApp(GameFactory.PlayerDataStorageFileName);
        }

        [MenuItem("Game/PlayerPrefs/Reload", false, 3)]
        public static void ReloadPlayerPrefs()
        {
            GameFactory.Storage.ReloadFromDisc();
        }

        [MenuItem("Game/PlayerPrefs/Delete", false, 4)]
        public static void DeletePlayerPrefs()
        {
            GameFactory.Storage.Clear();
            System.IO.File.Delete(GameFactory.PlayerDataStorageFileName);
        }

        [MenuItem("Game/PlayerPrefs/Reload", true)]
        [MenuItem("Game/PlayerPrefs/Open", true)]
        [MenuItem("Game/PlayerPrefs/Delete", true)]
        public static bool PlayerPrefs_Validate()
        {
            return System.IO.File.Exists(GameFactory.PlayerDataStorageFileName);
        }
    }
}