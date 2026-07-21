using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.Upgrades
{
    class UpgradeTreeLine : MonoBehaviour
    {
        [SerializeField, IsntNull] GameObject opened;
        [SerializeField, IsntNull] GameObject particles;

        internal void DrawOpenWithAnimation()
        {
            particles.SetActive(true);
            Draw(true);
        }

        internal void Draw(bool isOpened)
        {
            opened.SetActive(isOpened);
        }
    }
}
