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
        [SerializeField] UpgradeName upgradeName;
        [SerializeField] int ungradeNumber;

        [Space]
        [SerializeField] bool allLevels;
        [SerializeField] int level;

        [Header("Gui")]
        [SerializeField, IsntNull] UpgradeCollection upgrades;
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

        [NonSerialized] List<UpgradeTreeLine> nextLines;
        [NonSerialized] List<UpgradeTreeNode> previousNodes;


        internal IReadOnlyList<UpgradeTreeLine> NextLines => nextLines;
        public bool IsPreviusOpened => previousNodes.All(n => n.IsOpened);
        public bool AllLevels => allLevels;
        public int Level => level;
        public NodeState State { get; set; }
        public UpgradeData2 UpgradeData => upgrades.Get(upgradeName, ungradeNumber);

        public event UnityAction<UpgradeTreeNode> Click;


        int Cost
        {
            get
            {
                if (allLevels)
                    return UpgradeData.IntCost;
                else
                    return UpgradeData.GetCost(level);
            }
        }

        internal bool IsOpened
        {
            get
            {
                if (allLevels)
                    return UpgradeData.IsMaxLevel;
                else
                    return UpgradeData.Level.level > level;
            }
        }

        void Start()
        {
            button.onClick.AddListener(OnClick);
        }

#if UNITY_EDITOR
        static string drawGizmosKey => nameof(UpgradeTreeNode) + "drawGizmos";
        private void OnDrawGizmos()
        {
            if (Selection.activeObject == UpgradeData)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(transform.position, new Vector3(1.3f, 1.3f, 0));
            }

            if (nextNodes == null)
                return;

            Gizmos.color = Color.red;
            foreach (var item in nextNodes)
            {
                if (!item)
                    continue;

                GizmosExtension.DrawArrowXY(transform.position, item.transform.position, size: 0.1f);
            }


            if (!EditorPrefs.GetBool(drawGizmosKey))
                return;

            Handles.color = Color.white;
            //string lebel = allLevels ? "ALL" : $"{level}";
            string lebel = UpgradeData.GetValue(1).ToString();
            Handles.Label(transform.position + new Vector3(-0.2f, 0.8f), lebel);
            //Handles.Label(transform.position + Vector3.left * 2 + Vector3.up * 1.2f, $"+{UpgradeData.IncrementPerLevel} cost={UpgradeData.Cost}");
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
                string parameter = UpgradeData.IncrementPerLevel > 0 ? "+" : "";

                bool isInt = Mathf.Abs(UpgradeData.IncrementPerLevel % 1) < 0.01f;
                if (isInt)
                    parameter += Mathf.RoundToInt(UpgradeData.IncrementPerLevel).ToString();
                else
                    parameter += UpgradeData.IncrementPerLevel.ToString("0.0");

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
                return NodeState.Open;

            if (!IsPreviusOpened)
                return NodeState.PreviousNotOpened;

            if (Cost > playerBank)
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
                Assert.IsTrue(level == UpgradeData.Level.level);

            UpgradeData.AddLevel();
        }

        internal void DrawStateWithAnimation(NodeState state)
        {
            DrawState(state);
        }

        [Button]
        public void Rename()
        {
#if UNITY_EDITOR
            Undo.RecordObject(gameObject, "Rename");
            gameObject.name = $"{UpgradeData.ObjectNameFroUpgrade()}{(allLevels ? string.Empty : "_" + level)}";
#endif
        }

        public void Validate(ValidationContext context)
        {
#if UNITY_EDITOR
            if (!upgrades.TryGet(upgradeName, ungradeNumber))
                context.AddProblem(nameof(UpgradeTreeNode), ValidationProblem.Type.Error, $"{upgradeName}_{ungradeNumber} не найден");

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
            mainImage.sprite = UpgradeData.Sprite;
            nameText.text = UpgradeData.GuiName;


            Style style = new();

            if (state == NodeState.PreviousNotOpened)
                style = previousNotOpenedStyle;

            if (state == NodeState.NotEnought)
                style = notEnoughtStyle;

            if (state == NodeState.CanOpen)
                style = canOpenStyle;

            if (state == NodeState.Open)
                style = openedStyle;

            mainImage.color = style.mainImage;
            costText.color = style.textColor;
            border.color = style.borderColor;
            bask.color = style.backgroundColor;
            costPanel.SetActive(style.cost);
            levelPanel.SetActive(style.level);
            particle.SetActive(style.particle);
            costText.text = GameUtils.AddCoinIcon(Cost.ToString(), style.coinColor);

            levelPanel.SetActive(style.level && allLevels && UpgradeData.IsOpen);
            if (allLevels)
            {
                if (UpgradeData.IsMaxLevel)
                    levelText.text = "max";
                else
                    levelText.text = "× " + UpgradeData.Level.level.ToString();
            }
        }

        [Button] void DrawPreviousNotOpened() => DrawState(NodeState.PreviousNotOpened);
        [Button] void DrawNotEnought() => DrawState(NodeState.NotEnought);
        [Button] void DrawCanOpen() => DrawState(NodeState.CanOpen);
        [Button] void DrawOpened() => DrawState(NodeState.Open);

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
