using GamePackages.Core;
using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.CoreGame
{

    class WayPoints : MonoBehaviour
    {
        [SerializeField] bool drawGizmos;

        [SerializeField, IsntNull]
        Transform[] wayPoints;

        internal int Count => wayPoints.Length;

        internal Transform GetPoint(int pointIndex)
        {
            if (pointIndex >= wayPoints.Length)
                throw new System.IndexOutOfRangeException($"{nameof(pointIndex)}={pointIndex} go={gameObject.FullName()}");

            return wayPoints[pointIndex];
        }


#if UNITY_EDITOR
        internal void OnDrawGizmos()
        {
            if (!drawGizmos)
                return;

            Gizmos.color = Color.yellow;
            GizmosExtension.DrawLines(wayPoints);
        }

        [Button]
        void LoadPoints()
        {
            wayPoints = transform.GetComponentsInFirstChild<Transform>().ToArray();
            Undo.RecordObject(this, "LoadPoints");
        }
#endif
    }
}
