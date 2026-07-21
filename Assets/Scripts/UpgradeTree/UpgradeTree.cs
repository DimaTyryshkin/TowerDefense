using GamePackages.Core.Validation;
using NaughtyAttributes;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using GamePackages.Core;
using TMPro;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Upgrades
{
    class UpgradeTree : MonoBehaviour
    {
        [SerializeField] int playerBank;
        [SerializeField, IsntNull] TMP_Text playerBankText;
        [SerializeField, IsntNull] RectTransform linesRoot;
        [SerializeField, IsntNull] UpgradeTreeLine lineTemplate;
        [SerializeField, IsntNull] UpgradeTreeNode[] allNodes;

        void Start()
        {
            lineTemplate.gameObject.SetActive(false);
            DrawBank();

            foreach (var node in allNodes)
                node.Init();

            foreach (var node in allNodes)
                node.BuildTree();

            BuildLines();

            foreach (var node in allNodes)
            {
                NodeState state = node.CalculateState(playerBank);
                node.State = state;
                node.DrawState(state);

                foreach (UpgradeTreeLine line in node.nextLines)
                    line.Draw(state == NodeState.Opened);

                node.Click += Node_Click;
            }


        }

        void DrawBank() => playerBankText.text = playerBank.ToString();

        void Node_Click(UpgradeTreeNode node)
        {
            if (node.State != NodeState.CanOpen)
            {
                //ShowHint(node.GetHint());
                return;
            }

            node.OpenUpgrade();
            node.State = node.CalculateState(playerBank);
            playerBank -= node.Cost;
            DrawBank();

            foreach (UpgradeTreeNode n in allNodes.Except(node.nextNodes))
            {
                if (n == node)
                    continue;

                NodeState newState = n.CalculateState(playerBank);
                if (newState != n.State)
                {
                    Assert.IsTrue(newState == NodeState.NotEnought, $"state={n.State} newState={newState} name={n.gameObject.name}");
                    n.State = newState;
                    n.DrawNotEnoughtWithAnimation();

                    //if (newState == NodeState.NotEnought)
                    //    n.DrawNotEnoughtWithAnimation();
                    //if (newState == NodeState.CanOpen)
                    //    n.DrawCanOpenWithAnimation();
                }
            }

            node.DrawOpenWithAnimation();
            if (node.nextNodes.Length == 0)
                return;

            foreach (UpgradeTreeLine line in node.nextLines)
                line.DrawOpenWithAnimation();

            //await node.Lines[0].AnimationLength

            foreach (var n in node.nextNodes)
            {
                NodeState newState = n.CalculateState(playerBank);
                if (n.State != newState)
                {
                    n.State = newState;
                    if (newState == NodeState.CanOpen)
                        n.DrawCanOpenWithAnimation();
                    if (newState == NodeState.NotEnought)
                        n.DrawNotEnoughtWithAnimation();
                }
            }

        }

        [Button]
        void BuildLines()
        {
            linesRoot.DestroyChildren();
            foreach (UpgradeTreeNode node in allNodes)
            {
                foreach (var nextNode in node.nextNodes)
                {
                    Vector2 p1 = node.transform.position;
                    Vector2 p2 = nextNode.transform.position;
                    Vector2 toTarget = p2 - p1;
                    Vector3 center = (p2 + p1) * 0.5f;
                    float angle = Vector2.SignedAngle(Vector2.up, toTarget) + 90;

                    UpgradeTreeLine newLine = linesRoot.InstantiateAsChild(lineTemplate);
                    newLine.gameObject.SetActive(true);
                    newLine.transform.position = center;
                    newLine.transform.rotation = Quaternion.Euler(0, 0, angle);

                    Vector2 anchP1 = node.GetComponent<RectTransform>().anchoredPosition;
                    Vector2 anchP2 = nextNode.GetComponent<RectTransform>().anchoredPosition;

                    float width = (anchP2 - anchP1).magnitude * 1f;
                    newLine.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

                    node.nextLines.Add(newLine);
                }
            }
        }

#if UNITY_EDITOR
        [Button]
        void LoadNodes()
        {
            Undo.RegisterCompleteObjectUndo(this, nameof(LoadNodes));
            allNodes = transform.GetComponentsInChildren<UpgradeTreeNode>();
        }
#endif
    }
}
