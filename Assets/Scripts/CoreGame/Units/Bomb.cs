using GamePackages.Core.Validation;
using NUnit.Framework;
using UnityEngine;

namespace Game.CoreGame
{
    class Bomb : MonoBehaviour
    {
        [SerializeField] float duration;
        [SerializeField] float height;
        [SerializeField] float explosionRange;
        [SerializeField, IsntNull] ParticleSystem vfx;
        [SerializeField, IsntNull] ParticleSystem vfxExplosionPrefab;


        internal float Duration => duration;
        HealthComponentOnBoardCollection healthCollection;
        Damage damage;

        Vector2 startPos;
        Vector2 endPos;
        float time;

        internal void Init(Damage damage, HealthComponentOnBoardCollection healthCollection, Vector2 targetPos)
        {
            Assert.IsNotNull(healthCollection);
            this.healthCollection = healthCollection;
            this.damage = damage;
            startPos = transform.position;
            endPos = targetPos;
            time = 0;

            //float totalTrajectoryLenght = Vector2.Distance(startPos, endPos) + height;
            //duration = totalTrajectoryLenght / speed;
        }

        void Update()
        {
            float t = Mathf.Clamp01(time / duration);
            Vector2 p1 = startPos + new Vector2(0, height * t);
            Vector2 p2 = endPos + new Vector2(0, height * (1 - t));
            transform.position = Vector2.Lerp(p1, p2, t);
            if (time >= duration)
                StopFly();

            time += Time.deltaTime;
        }

        void StopFly()
        {
            enabled = false;
            vfx.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);

            ShowEplosionVfx();

            HealthComponent[] tarets = healthCollection.FindAll(x => healthCollection.CanAttack(endPos, explosionRange, x));
            foreach (HealthComponent t in tarets)
                t.SetDamage(damage);

            Destroy(gameObject, 4);
        }

        void ShowEplosionVfx()
        {
            ParticleSystem vfx = Instantiate(vfxExplosionPrefab, endPos, Quaternion.identity);
            ParticleSystem.ShapeModule shape = vfx.shape;
            shape.radius = explosionRange;
            Destroy(vfx.gameObject, 4);
        }


#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, explosionRange);
        }
#endif
    }
}
