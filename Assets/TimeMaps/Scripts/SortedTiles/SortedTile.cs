using GamePackages.Core.Validation;
using NaughtyAttributes;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.SortedTiles
{
    public class SortedTile : MonoBehaviour
    {
        struct ParticleRendered
        {
            public ParticleSystemRenderer renderer;
            public int originSortingOrder;

            public ParticleRendered(ParticleSystemRenderer renderer, int originLayerOrder)
            {
                Assert.IsNotNull(renderer);
                this.renderer = renderer;
                this.originSortingOrder = originLayerOrder;
            }
        }

        [SerializeField] string groupName;


        [InfoBox("Сколько пикселей от низа кортинки до низа объекта", EInfoBoxType.Normal)]
        [SerializeField] float yOffsetInPixelsFromBotBase;
        [InfoBox("Высота как бы по оси Z. Например, верхушка кактуса", EInfoBoxType.Normal)]
        [SerializeField] int height;
        [SerializeField] int orderOffset;
        [SerializeField] bool isDynamic;
        [SerializeField] bool applyToParticleSystems;
        [SerializeField, IsntNull] SpriteRenderer spriteRenderer;

        int order;
        SortedTilesSystem system;
        ParticleRendered[] particles;
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

                    if (applyToParticleSystems)
                    {
                        for (int i = 0; i < particles.Length; i++)
                            particles[i].renderer.sortingOrder = value + particles[i].originSortingOrder;
                    }
                }
            }
        }

        public void Start()
        {
            if (!system)
                Init();

            if (!isDynamic && Application.isPlaying)
            {
                Order = system.GetOrder(this);
                enabled = false;
            }
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

        internal void Init()
        {
            if (system)
                return;

            system = SortedTilesSystem.inst;
            Assert.IsNotNull(system);

            order = spriteRenderer.sortingOrder;

            if (applyToParticleSystems)
                particles = GetComponentsInChildren<ParticleSystemRenderer>()
                .Select(r => new ParticleRendered(r, r.sortingOrder))
                .ToArray();
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