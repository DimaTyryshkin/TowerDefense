using UnityEngine;
using NaughtyAttributes;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Common
{
    [CreateAssetMenu]
    public class DeveloperOptions : ScriptableObject
    {
        static DeveloperOptions inst;

        [SerializeField] bool dontIncrementLevel;
        [SerializeField] bool showLevelItems;
        [SerializeField] int debugLevelNumber;
        [SerializeField] int debugSoulsAmount;


        public bool HideLevelBlocks => !GetDebugValue(showLevelItems);
        public bool IncrementLevel => !GetDebugValue(dontIncrementLevel);

        public static DeveloperOptions Inst
        {
            get
            {
                if (!inst)
                    inst = Resources.Load<DeveloperOptions>("DeveloperOptions");

                return inst;
            }
        }

        bool GetDebugValue(bool value) => Application.isEditor ? value : false;

        public void SetDebugLevel(int level)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "SetDebugLevel");
            debugLevelNumber = level;
            //GameFactory.Data.LevelNumber = debugLevelNumber;
            GameFactory.Data.Save();
            EditorUtility.SetDirty(this);
#endif
        }

#if UNITY_EDITOR
        [Button]
        void SetLevel()
        {
            SetDebugLevel(debugLevelNumber);
        }

        [Button]
        void SetSouls()
        {
            //GameFactory.Data.SoulsAmount = debugSoulsAmount;
            GameFactory.Storage.Save();
        }

        [Button]
        void ClearSave()
        {
            GameFactory.Storage.Clear();
        }
#endif
    }
}