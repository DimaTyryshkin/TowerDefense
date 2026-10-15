using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;

namespace IncreKindom
{
    public enum EnemyState
    {
        None = 0,
        Main,
        Push,
        PushDeath,
        Hide
    }

    public class Enemy : MonoBehaviour
    {
        [SerializeField] int type;
        [SerializeField] float startHealth;
        [SerializeField] float radius;
        [SerializeField] int revard;
        [SerializeField] Color mainColor;
        [SerializeField] Color pushColor;
        [SerializeField] Color deathColor;
        [SerializeField, IsntNull] GameObject view;
        [SerializeField, IsntNull] GameObject whiteView;

        public SpriteRenderer spriteRenderer;

        public int Type => type;
        public float StartHealth => startHealth;
        public float Radius => radius;
        public int Revard => revard;

        public float health { get; set; }
        public float pushEndTime { get; set; }
        public Vector2 pushDir { get; set; }

        public EnemyState State
        {
            get => state; set
            {
                if (state == value)
                    return;

                state = value;

                if (State == EnemyState.Main)
                {
                    view.SetActive(true);
                    whiteView.SetActive(false);
                    //spriteRenderer.color = mainColor;
                }

                if (State == EnemyState.Push)
                {
                    view.SetActive(false);
                    whiteView.SetActive(true);
                    //spriteRenderer.color = pushColor;
                }

                if (State == EnemyState.PushDeath)
                {
                    //spriteRenderer.color = deathColor;
                }
            }
        }


        EnemyState state;

        [Button]
        public void ApplyRadius()
        {
            transform.localScale = Vector3.one * (Radius * 2);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
#endif 
    }
}
