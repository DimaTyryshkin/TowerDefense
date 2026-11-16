using GamePackages.Core;
using GamePackages.Core.Validation;
using IncreGame;
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
    public class UpgradeTreeNode : MonoBehaviour, IValidated
    {
        [SerializeField] int level;
        [SerializeField] bool allLevels;

        [IsntNull] public UpgradeData2 upgradeData;

        [Header("Gui")]
        [SerializeField, IsntNull] Button button;
        [SerializeField, IsntNull] TMP_Text nameText;
        [SerializeField, IsntNull] TMP_Text costText;
        [SerializeField, IsntNull] TMP_Text levelText;
        [SerializeField, IsntNull] Image mainImage;
        [SerializeField, IsntNull] Image border;
        [SerializeField, IsntNull] Image bask;
        [SerializeField, IsntNull] GameObject costPanel;
        [SerializeField, IsntNull] GameObject particle;
        [SerializeField, IsntNull] GameObject levelPanel;

        [Space]
        [IsntNull] public UpgradeTreeNode[] nextNodes;
        [SerializeField, IsntNull] Image[] towerImages;

        [NonSerialized] List<UpgradeTreeLine> nextLines;
        [NonSerialized] List<UpgradeTreeNode> previousNodes;


        internal IReadOnlyList<UpgradeTreeLine> NextLines => nextLines;
        internal bool IsOpened
        {
            get
            {
                if (allLevels)
                    return upgradeData.IsMaxLevel;
                else
                    return upgradeData.Level.level > level;
            }
        }
        public bool IsPreviusOpened => previousNodes.All(n => n.IsOpened);
        public bool AllLevels => allLevels;
        public int Level => level;
        public NodeState State { get; set; }

        public event UnityAction<UpgradeTreeNode> Click;

        void Start()
        {
            button.onClick.AddListener(OnClick);
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
            string lebel = allLevels ? "ALL" : $"{level}";
            Handles.Label(transform.position + new Vector3(-0.2f, 0.7f), lebel);
            //Handles.Label(transform.position + Vector3.left * 2 + Vector3.up * 1.2f, $"+{upgradeData.IncrementPerLevel} cost={upgradeData.Cost}");
        }

        private void OnValidate()
        {
            //foreach (var towerImage in towerImages)
            //    towerImage.sprite = towerSprite;
        }

        [Button()]
        void DebugLog()
        {

            Debug.Log($"IsOpened={IsOpened}");
            Debug.Log($"IsPreviusOpened={IsPreviusOpened}");
        }

        [Button]
        void SwichGizmos()
        {
            EditorPrefs.SetBool(drawGizmosKey, !EditorPrefs.GetBool(drawGizmosKey));
        }
#endif
        public void Init()
        {
            previousNodes = new();
            nextLines = new();
        }

        /*
        public void Darw()
        { 
            string text = nameText.text;
            if (text.Contains("{0}"))
            {
                string parameter = upgradeData.IncrementPerLevel > 0 ? "+" : "";

                bool isInt = Mathf.Abs(upgradeData.IncrementPerLevel % 1) < 0.01f;
                if (isInt)
                    parameter += Mathf.RoundToInt(upgradeData.IncrementPerLevel).ToString();
                else
                    parameter += upgradeData.IncrementPerLevel.ToString("0.0");

                text = string.Format(text, parameter);
                text = string.Format(text, "99");
            }

            nameText.text = text; 
        }
        */

        public void BuildTree()
        {
            foreach (var node in nextNodes)
                node.AddPreviousNode(this);
        }

        public NodeState CalculateState(int playerBank)
        {
            if (IsOpened)
                return NodeState.Opened;

            if (!IsPreviusOpened)
                return NodeState.PreviousNotOpened;

            if (upgradeData.Cost > playerBank)
                return NodeState.NotEnought;

            return NodeState.CanOpen;
        }

        public void AddNextLine(UpgradeTreeLine line) => nextLines.Add(line);

        void AddPreviousNode(UpgradeTreeNode node) => previousNodes.Add(node);


        void OnClick()
        {
            Click.Invoke(this);
        }

        public void OpenUpgrade()
        {
            if (!allLevels)
                Assert.IsTrue(level == upgradeData.Level.level);

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
            mainImage.sprite = upgradeData.Sprite;
            nameText.text = upgradeData.GuiName;


            Style style = new();

            if (state == NodeState.PreviousNotOpened)
                style = previousNotOpenedStyle;

            if (state == NodeState.NotEnought)
                style = notEnoughtStyle;

            if (state == NodeState.CanOpen)
                style = canOpenStyle;

            if (state == NodeState.Opened)
                style = openedStyle;

            mainImage.color = style.mainImage;
            costText.color = style.textColor;
            border.color = style.borderColor;
            bask.color = style.backgroundColor;
            costPanel.SetActive(style.cost);
            levelPanel.SetActive(style.level);
            particle.SetActive(style.particle);
            costText.text = GameUtils.AddCoinIcon(upgradeData.IntCost.ToString(), style.coinColor);

            levelPanel.SetActive(style.level && allLevels && upgradeData.IsOpen);
            if (allLevels)
            {
                if (upgradeData.IsMaxLevel)
                    levelText.text = "max";
                else
                    levelText.text = "× " + upgradeData.Level.level.ToString();
            }
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
            public bool level;
            public bool particle;
        }
    }
}
