using GamePackages.Core.Validation;
using NUnit.Framework;
using UnityEngine;

namespace Game.CoreGame
{
    class Bomb : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] float height;
        [SerializeField] float explosionRange;
        [SerializeField] float minDuration;
        [SerializeField, IsntNull] ParticleSystem vfx;
        [SerializeField, IsntNull] ParticleSystem vfxExplosionPrefab;

        HealthComponentOnBoardCollection healthCollection;
        Damage damage;

        Vector2 startPos;
        Vector2 endPos;
        float time;
        float totalDuration;

        internal void Init(Damage damage, HealthComponentOnBoardCollection healthCollection, Vector2 targetPos)
        {
            Assert.IsNotNull(healthCollection);
            this.healthCollection = healthCollection;
            this.damage = damage;
            startPos = transform.position;
            endPos = targetPos;
            time = 0;
            totalDuration = Mathf.Max(minDuration, Vector2.Distance(startPos, endPos));
        }

        void Update()
        {
            float t = Mathf.Clamp01(time / totalDuration);
            Vector2 p1 = startPos + new Vector2(0, height * (1 - t));
            Vector2 p2 = endPos + new Vector2(0, height * t);
            Vector2 pos = Vector2.Lerp(p1, p2, t);

            time += Time.deltaTime;

            if (time >= totalDuration)
                StopFly();
        }

        void StopFly()
        {
            enabled = false;
            vfx.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);

            Destroy(Instantiate(vfxExplosionPrefab, endPos, Quaternion.identity), 4);

            HealthComponent[] tarets = healthCollection.FindAll(x => healthCollection.CanAttack(endPos, explosionRange, x));
            foreach (HealthComponent t in tarets)
                t.SetDamage(damage);

            Destroy(gameObject, 4);
        }
    }
}
