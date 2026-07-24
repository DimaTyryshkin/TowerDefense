using GamePackages.Core.Validation;
using NaughtyAttributes;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using GamePackages.Core;
using TMPro;
using Game.Common;
using UnityEngine.Events;
using UnityEngine.UI;




#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Upgrades
{
    class UpgradeTree : MonoBehaviour
    {
        [SerializeField, IsntNull] TMP_Text playerBankText;
        [SerializeField, IsntNull] RectTransform linesRoot;
        [SerializeField, IsntNull] UpgradeTreeLine lineTemplate;
        [SerializeField, IsntNull] UpgradeData startUpgrade;
        [SerializeField, IsntNull] Button closeButton;
        [SerializeField, IsntNull] UpgradeTreeNode[] allNodes;

        internal event UnityAction Close;

        int UpgradePoints
        {
            get => GameFactory.Data.upgradePoints;
            set => GameFactory.Data.upgradePoints = value;
        }

        void Start()
        {
            startUpgrade.SetOne();
            lineTemplate.gameObject.SetActive(false);
            DrawBank();

            foreach (var node in allNodes)
                node.Init();

            foreach (var node in allNodes)
                node.BuildTree();

            BuildLines();

            foreach (var node in allNodes)
            {
                NodeState state = node.CalculateState(UpgradePoints);
                node.State = state;
                node.DrawState(state);

                foreach (UpgradeTreeLine line in node.NextLines)
                    line.Draw(state == NodeState.Opened);

                node.Click += Node_Click;
            }

            closeButton.onClick.AddListener(() => Close.Invoke());
        }

        internal void Show()
        {
            DrawBank();
            gameObject.SetActive(true);
        }

        internal void Hide() => gameObject.SetActive(false);

        void DrawBank() => playerBankText.text = UpgradePoints.ToString();

        void Node_Click(UpgradeTreeNode node)
        {
            if (node.State != NodeState.CanOpen)
            {
                //ShowHint(node.GetHint());
                return;
            }

            node.OpenUpgrade();
            node.State = node.CalculateState(UpgradePoints);
            UpgradePoints -= node.upgradeData.Cost;
            DrawBank();

            foreach (UpgradeTreeNode n in allNodes.Except(node.nextNodes))
            {
                if (n == node)
                    continue;

                NodeState newState = n.CalculateState(UpgradePoints);
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

            foreach (UpgradeTreeLine line in node.NextLines)
                line.DrawOpenWithAnimation();

            //await node.Lines[0].AnimationLength

            foreach (var n in node.nextNodes)
            {
                NodeState newState = n.CalculateState(UpgradePoints);
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

                    node.AddNextLine(newLine);
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

        [Button]
        void DebugAddUpgradePoints()
        {
            UpgradePoints++;
            GameFactory.Data.Save();
            Debug.Log($"UpgradePoints={UpgradePoints}");

            if (Application.isPlaying)
                Start();
        }
#endif
    }
}
