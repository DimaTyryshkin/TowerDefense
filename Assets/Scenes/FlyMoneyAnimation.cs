using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Events;

namespace IncreKindom
{
    class FlyMoneyAnimation : MonoBehaviour
    {
        [SerializeField, IsntNull] float flySpeed;
        [SerializeField, IsntNull] AnimationCurve flyMoneyAnimationCurve;

        public event UnityAction<FlyMoneyAnimation> FlyFinish;
        public int Money => money;

        float t;
        int money;
        float flyTime;
        Vector2 endPos;
        Vector2 startPos;

        public void Init(int money, Vector2 startPos, Vector2 endPos)
        {
            this.t = 0;
            transform.position = startPos;
            this.startPos = startPos;
            this.endPos = endPos;
            this.money = money;
            flyTime = Vector2.Distance(startPos, endPos) / flySpeed;
        }

        private void Update()
        {
            t += Time.deltaTime / flyTime;
            if (t > 1)
            {
                ActionWrapper.ClearAndInvoke(ref FlyFinish, this);
            }
            else
            {
                transform.position = Vector2.Lerp(startPos, endPos, flyMoneyAnimationCurve.Evaluate(t));
            }
        }
    }
}
