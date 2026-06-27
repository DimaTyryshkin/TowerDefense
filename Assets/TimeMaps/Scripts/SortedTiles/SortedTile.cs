using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.SortedTiles
{
    public class SortedTile : MonoBehaviour
    {
        [SerializeField] string groupName;

        [Tooltip("Сколько пикселей от низа кортинки до низа объекта")]
        [SerializeField] float yOffsetInPixelsFromBotBase;
        [SerializeField] int height;
        [SerializeField] int orderOffset;
        [SerializeField] bool isDynamic;
        [SerializeField, IsntNull] SpriteRenderer spriteRenderer;

        int order;
        SortedTilesSystem system;
        public bool debug;

        public string GroupName => groupName;
        public int Height => height;
        public int OrderOffset => orderOffset;
        public float YOffsetInPixelsFromBotBase => yOffsetInPixelsFromBotBase;
        public bool IsDynamic => isDynamic;
        public bool IsGroupingTile => !string.IsNullOrEmpty(groupName);
        public SpriteRenderer SpriteRenderer => spriteRenderer;

        public int Order
        {
            get => order;
            set
            {
                if (order != value)
                {
                    order = value;
                    spriteRenderer.sortingOrder = value;
                }
            }
        }

        public void Start()
        {
            if (!isDynamic && Application.isPlaying)
                enabled = false;
        }

        void LateUpdate()
        {
            if (enabled)
                Order = system.GetOrder(this);
        }

#if UNITY_EDITOR
        private void Reset()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
#endif

        internal void Init(SortedTilesSystem system)
        {
            Assert.IsNotNull(system);
            order = spriteRenderer.sortingOrder;
            this.system = system;
        }

        public void SetGroupName(string groupName)
        {
            this.groupName = groupName;
        }

#if UNITY_EDITOR
        [Button()]
        void UpdateOrder()
        {
            GetComponentInParent<SortedTilesSystem>().UpdateOrder();
        }
#endif
    }
}