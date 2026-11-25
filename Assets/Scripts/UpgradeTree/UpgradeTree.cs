using GamePackages.Core.Validation;
using NaughtyAttributes;
using System.Linq;
using UnityEngine;
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
    public class UpgradeTree : MonoBehaviour
    {
        [SerializeField, IsntNull] TMP_Text playerBankText;
        [SerializeField, IsntNull] RectTransform linesRoot;
        [SerializeField, IsntNull] UpgradeTreeLine lineTemplate;
        [SerializeField, IsntNull] Button closeButton;
        [SerializeField, IsntNull] Button cheatButton;

        [Header("Debug")]
        [SerializeField] bool showCost;

        [Space]
        [IsntNull] public UpgradeTreeNode[] allNodes;
        [IsntNull] public UpgradeTreeNode[] debugNodes;

        internal event UnityAction Close;

        public int UpgradePoints;

        public bool IsBuilded { get; set; }

        void Start()
        {
            lineTemplate.gameObject.SetActive(false);

            //Draw(UpgradePoints);
            closeButton.onClick.AddListener(() => Close.Invoke());

            cheatButton.gameObject.SetActive(false);
            cheatButton.gameObject.SetActive(Application.isEditor);
            cheatButton.onClick.AddListener(DebugAddPoints);
        }

        private void DebugAddPoints()
        {
            UpgradePoints += 2000;
            Draw(UpgradePoints);
        }

        void BuildTree()
        {
            foreach (var node in allNodes)
            {
                //Injector.Inject2(node, upgrades);
                node.Init();
            }

            foreach (var node in allNodes)
            {
                node.BuildTree();
                node.Click += Node_Click;
            }

            BuildLines();
        }

        public void Draw(int upgradePoint, bool debugShowCost = false)
        {
            if (!IsBuilded)
            {
                IsBuilded = true;
                BuildTree();
            }

            DrawBank(upgradePoint);

            foreach (var node in allNodes)
            {
                NodeState state = node.CalculateState(upgradePoint);

                if (debugShowCost)
                    state = NodeState.CanOpen;

                node.State = state;
                node.DrawState(state);

                foreach (UpgradeTreeLine line in node.NextLines)
                    line.Draw(state == NodeState.Open);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            Draw(UpgradePoints);
        }

        internal void Hide() => gameObject.SetActive(false);

        void DrawBank(int upgradePoint) => playerBankText.text = upgradePoint.ToString();

        void Node_Click(UpgradeTreeNode node)
        {
            if (node.State != NodeState.CanOpen)
            {
                //ShowHint(node.GetHint());
                return;
            }

            UpgradePoints -= node.UpgradeData.IntCost;
            node.OpenUpgrade();
            node.State = node.CalculateState(UpgradePoints);
            node.DrawStateWithAnimation(node.State);
            DrawBank(UpgradePoints);

            foreach (UpgradeTreeNode n in allNodes.Except(node.nextNodes))
            {
                if (n == node)
                    continue;

                NodeState newState = n.CalculateState(UpgradePoints);
                if (newState != n.State)
                {
                    //Assert.IsTrue(newState == NodeState.NotEnought, $"state={n.State} newState={newState} name={n.gameObject.name}");
                    n.State = newState;
                    n.DrawState(newState);

                    //if (newState == NodeState.NotEnought)
                    //    n.DrawNotEnoughtWithAnimation();
                    //if (newState == NodeState.CanOpen)
                    //    n.DrawCanOpenWithAnimation();
                }
            }

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
                    n.DrawStateWithAnimation(newState);
                }
            }
        }

        void BuildLines()
        {
            linesRoot.DestroyChildren();
            foreach (UpgradeTreeNode node in allNodes)
            {
                if (!node.gameObject.activeSelf)
                    continue;

                foreach (var nextNode in node.nextNodes)
                {
                    if (!nextNode.gameObject.activeSelf)
                        continue;

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
            allNodes = transform.GetComponentsInChildren<UpgradeTreeNode>(includeInactive: true);
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

        [SerializeField] int debugUpgradePoints;

        [Button]
        public void DebugDraw()
        {
            IsBuilded = false;
            LoadNodes();
            //GameFactory.Data.Save(); 
            Draw(debugUpgradePoints, showCost);

            foreach (var item in allNodes)
                item.Rename();
        }
#endif
    }
}
