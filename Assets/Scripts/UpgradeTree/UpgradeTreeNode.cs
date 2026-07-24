using GamePackages.Core;
using GamePackages.Core.Validation;
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
        [SerializeField] Sprite towerSprite;
        [IsntNull] public UpgradeData upgradeData;
        [SerializeField, IsntNull] Button button;
        [SerializeField, IsntNull] TMP_Text infoText;
        [SerializeField, IsntNull] TMP_Text costText1;
        [SerializeField, IsntNull] TMP_Text costText2;
        [SerializeField, IsntNull] GameObject previousNotOpen;
        [SerializeField, IsntNull] GameObject notEnought;
        [SerializeField, IsntNull] GameObject canOpen;
        [SerializeField, IsntNull] GameObject opened;
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

            costText1.text = upgradeData.Cost.ToString();
            costText2.text = upgradeData.Cost.ToString();
            string text = infoText.text;
            if (text.Contains("{0}"))
            {
                string parameter = upgradeData.IncrementPerLevel > 0 ? "+" : "";

                bool isInt = Mathf.Abs(upgradeData.IncrementPerLevel % 1) < 0.01f;
                if (isInt)
                    parameter += Mathf.RoundToInt(upgradeData.IncrementPerLevel).ToString();
                else
                    parameter += upgradeData.IncrementPerLevel.ToString("0.0");

                text = string.Format(text, parameter);
            }

            infoText.text = text;
        }

#if UNITY_EDITOR
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

            Handles.color = Color.white;
            Handles.Label(transform.position + Vector3.left * 2 + Vector3.up * 1.5f, $"{upgradeData?.name} {level}");
            //Handles.Label(transform.position + Vector3.up * 1.4f, $"{upgradeData?.name}");
            Handles.Label(transform.position + Vector3.left * 2 + Vector3.up * 1.2f, $"+{upgradeData.IncrementPerLevel} cost={upgradeData.Cost}");
        }

        private void OnValidate()
        {
            foreach (var towerImage in towerImages)
                towerImage.sprite = towerSprite;
        }

        [Button]
        void DebugLog()
        {
            Debug.Log($"IsOpened={IsOpened}");
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

        internal void DrawState(NodeState state)
        {
            previousNotOpen.SetActive(false);
            notEnought.SetActive(false);
            canOpen.SetActive(false);
            opened.SetActive(false);

            previousNotOpen.SetActive(state == NodeState.PreviousNotOpened);
            notEnought.SetActive(state == NodeState.NotEnought);
            canOpen.SetActive(state == NodeState.CanOpen);
            opened.SetActive(state == NodeState.Opened);
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
    }
}
