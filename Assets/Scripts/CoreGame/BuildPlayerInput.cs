using GamePackages.Core;
using GamePackages.InputSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game.CoreGame
{
    class BuildPlayerInput : MonoBehaviour
    {
        [Inject] GuiHit guihit;
        [Inject] BuildTowerBrush buildTowerBrush;

        BuildTowerBrush actualBrush;

        internal event UnityAction CancelBuild;
        internal event UnityAction Build;

        private void Update()
        {
            if (!actualBrush)
                return;

            bool isGuiUnderMouse = guihit.IsGuiUnderMouse(Mouse.current.position.ReadValue());
            bool leftClick = !isGuiUnderMouse && Mouse.current.leftButton.wasPressedThisFrame;
            bool rightClick = !isGuiUnderMouse && Mouse.current.rightButton.wasPressedThisFrame;

            if (rightClick)
            {
                CancelBuilding();
                return;
            }

            actualBrush.UpdateFrame(Mouse.current.position.ReadValue(), leftClick, OnBuild);
        }

        internal void Init()
        {

        }

        internal void SetTower(TowerAI tower)
        {
            Debug.Log("TowerShopView_SelectTower");
            actualBrush = buildTowerBrush;
            actualBrush.SetTowerPrefab(tower);
            actualBrush.SetPreviewEnable(true);
        }

        internal void CancelBuilding()
        {
            DisableActualBrush();
            CancelBuild.Invoke();
        }

        void OnBuild(TowerAI newTower)
        {
            DisableActualBrush();
            Build.Invoke();
        }

        void DisableActualBrush()
        {
            actualBrush?.SetPreviewEnable(false);
            actualBrush = null;
        }
    }
}