using GamePackages.Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game.CoreGame
{
    class NoAttackTowerBrush : BuildBrush
    {
        [Inject] SpriteRenderer towerPreview;
        Sprite previewSprite;

        internal event UnityAction<Vector2Int> ClickBuild;

        internal NoAttackTowerBrush(Sprite previewSprite)
        {
            Assert.IsNotNull(previewSprite);
            this.previewSprite = previewSprite;
        }

        internal override void UpdateFrame()
        {
            towerPreview.gameObject.SetActive(!guiHit.IsGuiUnderPointer);

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPoint = gameCamera.ScreenPointToWorldPointOnPlane(mousePos, Plaine.XY);
            Vector2Int cell = grid.WorldToCell(worldPoint);
            Vector3 cellPos = grid.CellToWorld(cell);
            towerPreview.transform.position = cellPos;
            towerPreview.sprite = previewSprite;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (buildingsOnBoard.Exist(cell))
                    return;

                ClickBuild.Invoke(cell);
            }
        }
    }
}