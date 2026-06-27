using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.AI;

namespace Action7
{
    internal class NavigateDebug : MonoBehaviour
    {
        [SerializeField, IsntNull] NavMeshAgent agent;
        [SerializeField] bool showPath = true;
        [SerializeField] bool showAhead;

        public static void DebugDrawPath(Vector3[] corners)
        {
            if (corners.Length < 2)
            {
                return;
            }

            int i = 0;
            for (; i < corners.Length - 1; i++)
            {
                Debug.DrawLine(corners[i], corners[i + 1], Color.blue);
            }

            Debug.DrawLine(corners[0], corners[1], Color.red);
        }

        private void OnDrawGizmos()
        {
            DrawGizmos(agent, showPath, showAhead);
        }

        public static void DrawGizmos(NavMeshAgent agent, bool showPath, bool showAhead)
        {
            if (Application.isPlaying && agent != null)
            {
                if (showPath && agent.hasPath)
                {
                    var corners = agent.path.corners;
                    if (corners.Length < 2)
                    {
                        return;
                    }

                    int i = 0;
                    for (; i < corners.Length - 1; i++)
                    {
                        Debug.DrawLine(corners[i], corners[i + 1], Color.blue);
                        Gizmos.color = Color.blue;
                        Gizmos.DrawSphere(agent.path.corners[i + 1], 0.03f);
                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(agent.path.corners[i], agent.path.corners[i + 1]);
                    }

                    Debug.DrawLine(corners[0], corners[1], Color.blue);
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(agent.path.corners[1], 0.03f);
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(agent.path.corners[0], agent.path.corners[1]);
                }

                if (showAhead)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawRay(agent.transform.position, agent.transform.up * 0.5f);
                }
            }
        }
    }
}
