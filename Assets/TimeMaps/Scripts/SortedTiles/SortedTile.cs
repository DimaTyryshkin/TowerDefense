using GamePackages.Core;
using GamePackages.Core.Validation;
using NaughtyAttributes;
using System.Linq;
using UnityEditor;
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

        [SerializeField] bool simpleMod;

        [SerializeField, ShowIf("OldMod")] string groupName;
        [InfoBox("Сколько пикселей от низа кортинки до низа объекта", EInfoBoxType.Normal)]
        [SerializeField, ShowIf("OldMod")] float yOffsetInPixelsFromBotBase;
        [InfoBox("Высота как бы по оси Z. Например, верхушка кактуса", EInfoBoxType.Normal)]
        [SerializeField, ShowIf("OldMod")] int height;
        [SerializeField, ShowIf("OldMod")] int orderOffset;
        [SerializeField, ShowIf("OldMod")] bool applyToChild;

        [SerializeField, ShowIf("simpleMod")] float offset;

        [SerializeField, BoxGroup("")] bool isDynamic;
        [SerializeField, BoxGroup("")] bool applyToParticleSystems;
        [SerializeField, IsntNull, BoxGroup("")] SpriteRenderer spriteRenderer;

        int order;
        SortedTilesSystem system;
        ParticleRendered[] particles;
        public bool debug;

        bool OldMod => !simpleMod;// для Редактора ShowIf("OldMod")
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

                    //if (applyToChild)
                    //{
                    //    for (int i = 0; i < spriteRenderers.Length; i++)
                    //        spriteRenderers[i].sortingOrder = value + i + 1;
                    //}
                }
            }
        }

        void Start()
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

        internal void Init()
        {
            if (system)
                return;

            system = SortedTilesSystem.inst;
            UnityEngine.Assertions.Assert.IsNotNull(system);

            order = spriteRenderer.sortingOrder;

            if (applyToParticleSystems)
                particles = GetComponentsInChildren<ParticleSystemRenderer>()
                .Select(r => new ParticleRendered(r, r.sortingOrder))
                .ToArray();

            //if (applyToChild)
            //{
            //    List<SpriteRenderer> childs = new();
            //    for (int i = 0; i < transform.childCount; i++)
            //    {
            //        SpriteRenderer sr = transform.GetChild(i).GetComponent<SpriteRenderer>();
            //        if (sr)
            //            childs.Add(sr);
            //    }

            //    spriteRenderers = childs.ToArray();
            //}
        }

        public void SetLayer(int sortingLayerId)
        {
            spriteRenderer.sortingLayerID = sortingLayerId;
        }

        public void SetGroupName(string groupName)
        {
            this.groupName = groupName;
        }

        internal float GetY()
        {
            if (simpleMod)
                return transform.position.y + offset;
            else
                return GetSpriteRendererBot();
        }

        float GetSpriteRendererBot() => spriteRenderer.bounds.min.y;

#if UNITY_EDITOR
        private void Reset()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            //spriteRenderers[0] = GetComponentInChildren<SpriteRenderer>();
        }

        private void OnDrawGizmosSelected()
        {
            if (!spriteRenderer)
                return;

            Vector3 p = transform.position;
            p.y = GetY();
            Gizmos.color = Color.green;
            GizmosExtension.DrawCrossXY(p, 0.1f);
        }

        [Button()]
        void CalculateOffset()
        {
            Undo.RecordObject(this, "offset");
            offset = GetSpriteRendererBot() - transform.position.y;


        }

        [Button()]
        void OffsetAdd()
        {
            Undo.RecordObject(this, "offset");
            offset += 1f / spriteRenderer.sprite.pixelsPerUnit;
        }

        [Button()]
        void OffsetSub()
        {
            Undo.RecordObject(this, "offset");
            offset -= 1f / spriteRenderer.sprite.pixelsPerUnit;
        }

        [Button(),]
        void UpdateOrder()
        {
            GetComponentInParent<SortedTilesSystem>().UpdateOrder();
        }
#endif 
    }
}