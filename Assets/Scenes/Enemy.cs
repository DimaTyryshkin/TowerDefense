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
        [SerializeField] float startHealth;
        [SerializeField] float radius;
        [SerializeField] int revard;
        [SerializeField] Color mainColor;
        [SerializeField] Color pushColor;
        [SerializeField] Color deathColor;

        public SpriteRenderer spriteRenderer;

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
                    spriteRenderer.color = mainColor;
                }

                if (State == EnemyState.Push)
                {
                    spriteRenderer.color = pushColor;
                }

                if (State == EnemyState.PushDeath)
                {
                    spriteRenderer.color = deathColor;
                }
            }
        }


        EnemyState state;

        [Button]
        public void ApplyRadius()
        {
            spriteRenderer.transform.localScale = Vector3.one * (Radius * 2);
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
