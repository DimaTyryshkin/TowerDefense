using GamePackages.Core;
using GamePackages.InputSystem;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game.CoreGame
{
    class BuildingPlayerInput : MonoBehaviour
    {
        [Inject] GridWrapper grid;
        [Inject] GuiHit guihit;
        [Inject] Camera gameCamera;
        [Inject] RangeVisualizer rangeVfx;
        [Inject] SpriteRenderer towerPreview;
        [Inject] BuildingsOnBoardColelction buildingsOnBoard;

        BuildBrush activeBrush;
        DebugMarker marker;

        internal event UnityAction CancelBuilding;

        private void Update()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            if (guihit.IsGuiUnderPointer)
            {
                rangeVfx.StopAndCelar();
                return;
            }

            if (activeBrush != null)
            {
                if (Mouse.current.rightButton.wasPressedThisFrame)
                {
                    Debug.Log("rightButton");
                    StopBuilding();
                    CancelBuilding.Invoke();
                    return;
                }

                activeBrush.UpdateFrame();
            }
            else
            {
                Vector3 worldPoint = gameCamera.ScreenPointToWorldPointOnPlane(mousePos, Plaine.XY);
                Vector2Int cell = (Vector2Int)grid.WorldToCell(worldPoint);
                TowerAI towerAI = buildingsOnBoard.Get<TowerAI>(cell);
                if (towerAI)
                {
                    //marker.Text($"{cell} {towerAI.name}");
                    rangeVfx.Play(towerAI.AttackRange, Color.white);
                    rangeVfx.Position = towerAI.transform.position;
                }
                else
                {

                    //marker.Text($"{cell} null");
                    rangeVfx.StopAndCelar();
                }
            }
        }

        internal void StopBuilding()
        {
            activeBrush = null;
            towerPreview.gameObject.SetActive(false);
        }

        internal void Init()
        {
            //marker = GizmosDrawer.Inst.GetMarker(Vector3.zero);
        }

        internal void SetBrush(BuildBrush buildBrush)
        {
            Assert.IsNotNull(buildBrush);
            activeBrush = buildBrush;
        }
    }
}