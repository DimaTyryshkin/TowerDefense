using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Events;

namespace IncreKindom
{
    class FlySeedlingAnimator : MonoBehaviour
    {
        [SerializeField, IsntNull] float flySpeed;
        [SerializeField, IsntNull] float height;
        [SerializeField, IsntNull] AnimationCurve yCurve;

        public event UnityAction<FlySeedlingAnimator> FlyFinish;
        public Enemy EnemyPrefab => enemyPrefab;
        public Vector2 EndPos => endPos;

        float t;
        float flyTime;
        Enemy enemyPrefab;
        Vector2 endPos;
        Vector2 startPos;

        public void Init(Enemy enemyPrefab, Vector2 startPos, Vector2 target)
        {
            t = 0;
            transform.position = startPos;
            this.startPos = startPos;
            this.endPos = target;
            this.enemyPrefab = enemyPrefab;

            flyTime = (Vector2.Distance(startPos, target) + height) / flySpeed;
        }

        private void Update()
        {
            t += Time.deltaTime / flyTime;
            if (t > 1)
            {
                transform.position = endPos;
                ActionWrapper.ClearAndInvoke(ref FlyFinish, this);
            }
            else
            {
                Vector2 pos = Vector2.Lerp(startPos, endPos, t);
                pos.y += yCurve.Evaluate(t);
                transform.position = pos;
            }
        }
    }
}
