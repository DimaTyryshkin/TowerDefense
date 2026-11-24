using Game.Upgrades;
using GamePackages.Core;
using IncreGame;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;

namespace Game
{
    [Overlay(typeof(SceneView), nameof(UpgradeTree))]
    public class UpgradeTreeOverlay : BaseSceneOverlay<UpgradeTree>
    {
        Label labelSum;

        protected override void CreatePanelContent(VisualElement root)
        {
            var resetBtn = new Button(() =>
            {
                if (target != null)
                {
                    // Регистрируем действие для поддержки Ctrl+Z (Undo)
                    //Undo.RecordObject(target.transform, "Reset Position");
                    //target.ResetObject();
                    target.DebugDraw();
                    Draw();
                }
            })
            {
                text = "DebugDraw"
            };


            labelSum = new UnityEngine.UIElements.Label();

            root.Add(resetBtn);
            root.Add(labelSum);
            Draw();
        }

        void Draw()
        {
            labelSum.text = "Сумм = " + GameUtils.IntToSting(GetUpgradePointSun(target));
        }

        int GetUpgradePointSun(UpgradeTree upgradeTree)
        {
            int sum = 0;
            foreach (var node in upgradeTree.allNodes)
            {
                int maxLevel = node.UpgradeData.MaxLevel.level;
                if (maxLevel > 0)
                {
                    if (node.AllLevels)
                    {
                        sum += node.UpgradeData.GetCostSum(maxLevel);
                    }
                    else
                    {
                        sum += node.UpgradeData.GetCost(node.Level);
                    }
                }
            }

            return sum;
        }

    }

}