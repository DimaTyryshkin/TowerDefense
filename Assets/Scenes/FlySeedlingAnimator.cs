using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Events;

namespace IncreGame
{
    class FlySeedlingAnimator : MonoBehaviour
    {
        [SerializeField, IsntNull] float flySpeed;
        [SerializeField, IsntNull] float stayTime;
        [SerializeField, IsntNull] float height;
        [SerializeField, IsntNull] AnimationCurve yCurve;
        [SerializeField, IsntNull] AnimationCurve tCurve;

        public event UnityAction<FlySeedlingAnimator> FlyFinish;
        public Enemy EnemyPrefab => enemyPrefab;
        public Vector2 EndPos => endPos;

        float t;
        float flyTime;
        float timeStartFly;
        Enemy enemyPrefab;
        Vector2 endPos;
        Vector2 startPos;

        public void Init(Enemy enemyPrefab, Vector2 startPos, Vector2 target)
        {
            t = 0;
            timeStartFly = Time.time + stayTime;
            transform.position = startPos;
            this.startPos = startPos;
            this.endPos = target;
            this.enemyPrefab = enemyPrefab;

            flyTime = 0.2f + (Vector2.Distance(startPos, target) + height) / flySpeed;
        }

        private void Update()
        {
            t += Time.deltaTime / flyTime;
            if (Time.time <= timeStartFly)
                t = 0;

            if (t > 1)
            {
                transform.position = endPos;
                ActionWrapper.ClearAndInvoke(ref FlyFinish, this);
            }
            else
            {
                float t2 = tCurve.Evaluate(t);
                Vector2 pos = Vector2.Lerp(startPos, endPos, t2);
                pos.y += yCurve.Evaluate(t2) * height;
                transform.position = pos;
            }
        }
    }
}
