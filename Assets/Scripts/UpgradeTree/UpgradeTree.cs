using GamePackages.Core.Validation;
using NaughtyAttributes;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Upgrades
{
    class UpgradeTree : MonoBehaviour
    {
        [SerializeField] int playerBank;
        [SerializeField, IsntNull] UpgradeTreeNode[] allNodes;

        void Start()
        {
            foreach (var node in allNodes)
                node.Init();

            foreach (var node in allNodes)
                node.BuildTree();

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

        void Node_Click(UpgradeTreeNode node)
        {
            Debug.Log("1");
            if (node.State != NodeState.CanOpen)
            {
                //ShowHint(node.GetHint());
                return;
            }

            node.OpenUpgrade();
            node.State = node.CalculateState(playerBank);
            playerBank -= node.Cost;


            Debug.Log("2");
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


            Debug.Log("3");
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
