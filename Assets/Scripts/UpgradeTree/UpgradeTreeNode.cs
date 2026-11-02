using GamePackages.Core;
using GamePackages.Core.Validation;
using IncreKindom;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Upgrades
{
    class UpgradeTreeNode : MonoBehaviour, IValidated
    {
        [SerializeField] int level;
        [IsntNull] public UpgradeData2 upgradeData;

        [Header("Gui")]
        [SerializeField, IsntNull] Button button;
        [SerializeField, IsntNull] TMP_Text nameText;
        [SerializeField, IsntNull] TMP_Text costText;
        [SerializeField, IsntNull] Image mainImage;
        [SerializeField, IsntNull] Image border;
        [SerializeField, IsntNull] Image bask;
        [SerializeField, IsntNull] GameObject costPanel;
        [SerializeField, IsntNull] GameObject particle;

        [Space]
        [IsntNull] public UpgradeTreeNode[] nextNodes;
        [SerializeField, IsntNull] Image[] towerImages;

        [NonSerialized] List<UpgradeTreeLine> nextLines;
        [NonSerialized] List<UpgradeTreeNode> previousNodes;




        internal IReadOnlyList<UpgradeTreeLine> NextLines => nextLines;
        internal bool IsOpened => upgradeData.Level.level >= level;
        internal bool IsPreviusOpened => previousNodes.All(n => n.IsOpened);
        internal NodeState State { get; set; }

        internal event UnityAction<UpgradeTreeNode> Click;

        private void Start()
        {
            button.onClick.AddListener(OnClick);

            costText.text = GameUtils.AddCoinIcon(upgradeData.Cost.ToString());
            string text = nameText.text;
            if (text.Contains("{0}"))
            {
                /*
                string parameter = upgradeData.IncrementPerLevel > 0 ? "+" : "";

                bool isInt = Mathf.Abs(upgradeData.IncrementPerLevel % 1) < 0.01f;
                if (isInt)
                    parameter += Mathf.RoundToInt(upgradeData.IncrementPerLevel).ToString();
                else
                    parameter += upgradeData.IncrementPerLevel.ToString("0.0");

                text = string.Format(text, parameter);
                */
                text = string.Format(text, "99");
            }

            nameText.text = text;
        }

#if UNITY_EDITOR
        static string drawGizmosKey => nameof(UpgradeTreeNode) + "drawGizmos";
        private void OnDrawGizmos()
        {
            if (nextNodes == null)
                return;

            Gizmos.color = Color.red;
            foreach (var item in nextNodes)
            {
                if (!item)
                    continue;

                GizmosExtension.DrawArrowXY(transform.position, item.transform.position);
            }

            if (!EditorPrefs.GetBool(drawGizmosKey))
                return;

            Handles.color = Color.white;
            Handles.Label(transform.position + Vector3.left * 2 + Vector3.up * 1.5f, $"{upgradeData?.name} {level}");
            //Handles.Label(transform.position + Vector3.left * 2 + Vector3.up * 1.2f, $"+{upgradeData.IncrementPerLevel} cost={upgradeData.Cost}");
        }

        private void OnValidate()
        {
            //foreach (var towerImage in towerImages)
            //    towerImage.sprite = towerSprite;
        }

        [Button]
        void DebugLog()
        {
            Debug.Log($"IsOpened={IsOpened}");
        }

        [Button]
        void SwichGizmos()
        {
            EditorPrefs.SetBool(drawGizmosKey, !EditorPrefs.GetBool(drawGizmosKey));
        }
#endif
        internal void Init()
        {
            previousNodes = new();
            nextLines = new();
        }

        internal void BuildTree()
        {
            foreach (var node in nextNodes)
                node.AddPreviousNode(this);
        }

        internal NodeState CalculateState(int playerBank)
        {
            if (IsOpened)
                return NodeState.Opened;

            if (!IsPreviusOpened)
                return NodeState.PreviousNotOpened;

            if (upgradeData.Cost > playerBank)
                return NodeState.NotEnought;

            return NodeState.CanOpen;
        }
        internal void AddNextLine(UpgradeTreeLine line) => nextLines.Add(line);

        void AddPreviousNode(UpgradeTreeNode node) => previousNodes.Add(node);


        void OnClick()
        {
            Click.Invoke(this);
        }


        internal void OpenUpgrade()
        {
            Assert.IsTrue(level == upgradeData.Level.level + 1);
            upgradeData.AddLevel();
        }

        internal void DrawNotEnoughtWithAnimation()
        {
            Debug.Log("DrawNotEnoughtWithAnimation " + gameObject.name);
            DrawState(NodeState.NotEnought);
        }

        internal void DrawOpenWithAnimation()
        {
            Debug.Log("DrawOpenWithAnimation " + gameObject.name);
            DrawState(NodeState.Opened);
        }

        internal void DrawCanOpenWithAnimation()
        {
            Debug.Log("DrawCanOpenWithAnimation " + gameObject.name);
            DrawState(NodeState.CanOpen);
        }

        public void Validate(ValidationContext context)
        {
#if UNITY_EDITOR
            //if (nextLines.Count != nextNodes.Length)
            //    context.AddProblem(nameof(UpgradeTreeNode), ValidationProblem.Type.Error, "Не совпадает количество узлов и линий к ним");
#endif
        }
        [Header("Styles")]
        [SerializeField] Style previousNotOpenedStyle;
        [SerializeField] Style notEnoughtStyle;
        [SerializeField] Style canOpenStyle;
        [SerializeField] Style openedStyle;

        internal void DrawState(NodeState state)
        {
            if (state == NodeState.PreviousNotOpened)
                previousNotOpenedStyle.Apply(this, 100);

            if (state == NodeState.NotEnought)
                notEnoughtStyle.Apply(this, 100);

            if (state == NodeState.CanOpen)
                canOpenStyle.Apply(this, 100);

            if (state == NodeState.Opened)
                openedStyle.Apply(this, 100);
        }

        [Button] void DrawPreviousNotOpened() => DrawState(NodeState.PreviousNotOpened);
        [Button] void DrawNotEnought() => DrawState(NodeState.NotEnought);
        [Button] void DrawCanOpen() => DrawState(NodeState.CanOpen);
        [Button] void DrawOpened() => DrawState(NodeState.Opened);

        [Serializable]
        public struct Style
        {
            public Color coinColor;
            public Color textColor;
            public Color backgroundColor;
            public Color borderColor;
            public Color mainImage;
            public bool cost;
            public bool particle;

            [Button]
            public void Apply(UpgradeTreeNode node, int costValue)
            {
                node.mainImage.color = mainImage;
                node.costText.color = textColor;
                node.border.color = borderColor;
                node.bask.color = backgroundColor;
                node.costPanel.SetActive(cost);
                node.particle.SetActive(particle);
                node.costText.text = GameUtils.AddCoinIcon(costValue.ToString(), coinColor);
            }
        }
    }
}
