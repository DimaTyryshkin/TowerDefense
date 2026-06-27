using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.AI;


namespace Action7
{
    public class NavMeshAgentCharacterMotor : MonoBehaviour
    {
        [SerializeField, IsntNull] NavMeshAgent agent;
        [SerializeField] Camera gameCamera;
        [SerializeField] bool showPath;
        [SerializeField] bool showAhead;
        [SerializeField] bool updateRotation;
        [SerializeField] bool updateUpAxis;

        bool stop;
        Vector2 target;

        void Start()
        {
            agent.updateUpAxis = updateUpAxis;
            agent.updateRotation = updateRotation;
            Stop();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                stop = false;
                target = gameCamera.ScreenPointToWorldPointOnPlane(Input.mousePosition, Plaine.XY);
                agent.SetDestination(target);
                agent.isStopped = false;
            }

            if (!stop)
            {
                Vector2 pos = transform.position;

                if ((pos - target).magnitude < 1)
                    Stop();
            }
        }

        void Stop()
        {
            agent.isStopped = true;
            stop = true;
        }
    }
}