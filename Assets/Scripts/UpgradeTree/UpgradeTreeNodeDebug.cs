using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Upgrades
{
    class UpgradeTreeNodeDebug : MonoBehaviour
    {
        UpgradeTreeNode Node => GetComponent<UpgradeTreeNode>();

        [Button(text: "==DarwTree==")]
        void DrawTree()
        {
            GetComponentInParent<UpgradeTree>().DebugDraw();
        }

        [Button()]
        void Open()
        {
            Node.upgradeData.SetOne();
            DrawTree();
        }
    }
}
