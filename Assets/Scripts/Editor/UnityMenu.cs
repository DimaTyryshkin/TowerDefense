using Game.Common;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class UnityMenu
    {
        [MenuItem("Game/Save", false, 1)]
        public static void SavePlayerPrefs()
        {
            EditorUtility.OpenWithDefaultApp(GameFactory.PlayerDataStorageFileName);
            Debug.Log(GameFactory.PlayerDataStorageFileName);
        }

        [MenuItem("Game/Open", false, 2)]
        public static void OpenPlayerPrefs()
        {
            EditorUtility.OpenWithDefaultApp(GameFactory.PlayerDataStorageFileName);
        }

        [MenuItem("Game/Reload", false, 3)]
        public static void ReloadPlayerPrefs()
        {
            GameFactory.Storage.ReloadFromDisc();
        }

        [MenuItem("Game/Delete", false, 4)]
        public static void DeletePlayerPrefs()
        {
            GameFactory.Storage.Clear();
            System.IO.File.Delete(GameFactory.PlayerDataStorageFileName);
        }

        [MenuItem("Game/Reload", true)]
        [MenuItem("Game/Open", true)]
        [MenuItem("Game/Delete", true)]
        public static bool PlayerPrefs_Validate()
        {
            return System.IO.File.Exists(GameFactory.PlayerDataStorageFileName);
        }
    }
}