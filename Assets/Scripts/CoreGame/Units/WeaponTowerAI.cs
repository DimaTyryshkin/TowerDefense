using GamePackages.Core.Validation;
using UnityEngine;


#if UNITY_EDITOR
#endif

namespace Game.CoreGame
{
    class WeaponTowerAI : MonoBehaviour
    {
        [SerializeField, IsntNull] WeaponComponent weaponComponent;
        UnitStateMashine stateMashine;

        void Start()
        {
            stateMashine = new UnitStateMashine();
        }

        void Update()
        {
            stateMashine.UpdateFrame(weaponComponent);
        }

        internal void Init(HealthComponentOnBoardCollection enemyOnBoard)
        {
            weaponComponent.Init(enemyOnBoard);
        }
    }
}
