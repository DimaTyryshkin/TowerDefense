using GamePackages.Core;
using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.CoreGame
{
    //class WayPoint : MonoBehaviour
    //{
    //    void OnDrawGizmos() => GetComponentInParent<WayPoints>().OnDrawGizmos();
    //}

    class WayPoints : MonoBehaviour
    {
        [SerializeField, IsntNull]
        Transform[] wayPoints;

        internal int Count => wayPoints.Length;

        internal Transform GetPoint(int pointIndex)
        {
            if (pointIndex >= wayPoints.Length)
                throw new System.IndexOutOfRangeException($"{nameof(pointIndex)}={pointIndex} go={gameObject.FullName()}");

            return wayPoints[pointIndex];
        }


        internal void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            GizmosExtension.DrawLines(wayPoints);
        }

#if UNITY_EDITOR
        [Button]
        void LoadPoints()
        {
            wayPoints = transform.GetComponentsInFirstChild<Transform>().ToArray();
            Undo.RecordObject(this, "LoadPoints");
        }
#endif
    }
}
