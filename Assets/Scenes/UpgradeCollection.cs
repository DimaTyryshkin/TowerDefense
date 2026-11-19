using GamePackages.Core;
using NaughtyAttributes;
using System.Linq;
using UnityEngine;

namespace IncreGame
{
    [CreateAssetMenu(menuName = "Game/UpgradeColelction")]
    public class UpgradeCollection : ObjectCollection<UpgradeData2>
    {
        public UpgradeData2 Get(UpgradeName upgradeName, int number)
        {
            for (int i = 0; i < collection.Length; i++)
            {
                UpgradeData2 item = collection[i];
                if (item.UpgradeName == upgradeName && item.UpgradeNumber == number)
                    return item;
            }

            throw new System.Exception($"{nameof(UpgradeCollection)} item not found {upgradeName}_{number}");
        }

        public UpgradeData2[] GetAll(UpgradeName upgradeName)
        {
            UpgradeData2[] array = collection.Where(x => x.UpgradeName == upgradeName).OrderBy(x => x.UpgradeNumber).ToArray();
            if (!array.IsSorted(x => x.UpgradeNumber))
                throw new System.Exception($"Не отсортирован Upgrade '{upgradeName}'");

            return array;
        }

#if UNITY_EDITOR
        [Button()]
        void Rename()
        {
            this.Load();

            foreach (var item in collection)
                item.Rename();
        }
#endif
    }
}
