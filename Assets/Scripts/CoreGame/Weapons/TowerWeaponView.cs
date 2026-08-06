using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;

namespace Game.CoreGame
{
    class TowerWeaponView : MonoBehaviour
    {
        [SerializeField, IsntNull] ShootingRangeWeaponComponent weapon;
        [SerializeField, IsntNull] SpriteRenderer towerHead;
        [SerializeField, IsntNull] Transform startPoint;
        [SerializeField, IsntNull] Transform[] sartPoints;
        [SerializeField, IsntNull] Sprite[] sprites;

        int lastSile;

        private void Start()
        {
            lastSile = -1;
        }

        private void LateUpdate()
        {
            DamageReceiver lastTarget = weapon.LastTarget;
            if (!lastTarget || lastTarget.Health.IsDeath)
                return;

            int side = Side.FromDir(lastTarget.transform.position - towerHead.transform.position).side;

            if (side == lastSile)
                return;

            lastSile = side;
            towerHead.sprite = sprites[side];
            startPoint.position = sartPoints[side].position;
        }

#if UNITY_EDITOR
        [Button] void Up() => towerHead.sprite = sprites[0];
        [Button] void Left() => towerHead.sprite = sprites[1];
        [Button] void Down() => towerHead.sprite = sprites[2];
        [Button] void Right() => towerHead.sprite = sprites[3];
#endif

    }
}
