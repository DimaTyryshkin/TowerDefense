using GamePackages.Core;
using GamePackages.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Upgrades
{
    class UpgradeTreeNode : MonoBehaviour, IValidated
    {
        [SerializeField] int cost;
        [IsntNull] public UpgradeData upgradeData;
        [SerializeField, IsntNull] Button button;
        //        [SerializeField, IsntNull] TMP_Text costText;
        [SerializeField, IsntNull] GameObject previousNotOpen;
        [SerializeField, IsntNull] GameObject notEnought;
        [SerializeField, IsntNull] GameObject canOpen;
        [SerializeField, IsntNull] GameObject opened;
        [IsntNull] public UpgradeTreeNode[] nextNodes;
        [IsntNull] public UpgradeTreeLine[] nextLines;

        [NonSerialized] List<UpgradeTreeNode> previousNodes;

        internal bool IsOpened { get; set; }
        internal bool IsPreviusOpened => previousNodes.All(n => n.IsOpened);
        internal NodeState State { get; set; }
        public int Cost => cost;

        internal event UnityAction<UpgradeTreeNode> Click;

        private void Start()
        {
            button.onClick.AddListener(OnClick);
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
        }
#endif
        internal void Init()
        {
            previousNodes = new();
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
            if (cost > playerBank)
                return NodeState.NotEnought;
            return NodeState.CanOpen;
        }

        void AddPreviousNode(UpgradeTreeNode node)
        {
            previousNodes.Add(node);
        }

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
            if (nextLines.Length != nextNodes.Length)
                context.AddProblem(nameof(UpgradeTreeNode), ValidationProblem.Type.Error, "Не совпадает количество узлов и линий к ним");
#endif
        }

        internal void OpenUpgrade()
        {
            IsOpened = true;
        }
    }
}
