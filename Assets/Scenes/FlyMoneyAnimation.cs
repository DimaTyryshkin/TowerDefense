using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Events;

namespace IncreKindom
{
    class FlyMoneyAnimation : MonoBehaviour
    {
        [SerializeField, IsntNull] float flyTime;
        [SerializeField, IsntNull] AnimationCurve flyMoneyAnimationCurve;

        public event UnityAction<FlyMoneyAnimation> FlyFinish;
        public int Money => money;

        float t;
        int money;
        Vector2 target;
        Vector2 startPos;

        public void Init(int money, Vector2 startPos, Vector2 target)
        {
            this.t = 0;
            transform.position = startPos;
            this.startPos = startPos;
            this.target = target;
            this.money = money;
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
                transform.position = Vector2.Lerp(startPos, target, flyMoneyAnimationCurve.Evaluate(t));
            }
        }
    }
}
