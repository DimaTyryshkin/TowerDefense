using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.CoreGame
{
    class LazerParticleSystem : MonoBehaviour
    {
        [SerializeField] float timeLife;
        [SerializeField, IsntNull] ParticleSystem[] particleSystems;

        Transform p1;
        Transform p2;

        internal void Play(Transform p1, Transform p2)
        {
            Assert.IsNotNull(p1);
            Assert.IsNotNull(p2);
            this.p1 = p1;
            this.p2 = p2;

            foreach (var particleSystem in particleSystems)
            {
                particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particleSystem.Clear();
                particleSystem.Simulate(0f, withChildren: true, restart: true);
                particleSystem.Play(true);
            }
        }

        private void Update()
        {
            if (!p1 || !p2)
                return;

            Vector2 toTarget = p2.position - p1.position;
            Vector3 center = (p2.position + p1.position) * 0.5f;
            float angle = Vector2.SignedAngle(Vector2.up, toTarget) + 90;

            transform.position = center;
            foreach (var particleSystem in particleSystems)
            {
                ParticleSystem.ShapeModule shape = particleSystem.shape;
                shape.rotation = new Vector3(0, 0, angle);
                shape.radius = toTarget.magnitude * 0.5f;
            }
        }
    }
}
