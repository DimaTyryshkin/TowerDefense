using GamePackages.Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game.CoreGame
{
    class TowerWithAtackRangeBrush : BuildBrush
    {
        [Inject] RangeVisualizer rangeVfx;
        [Inject] SpriteRenderer towerPreview;
        Sprite previewSprite;
        float attackRange;

        internal event UnityAction<Vector2Int> ClickBuild;

        internal TowerWithAtackRangeBrush(float attackRange, Sprite previewSprite)
        {
            Assert.IsNotNull(previewSprite);
            this.attackRange = attackRange;
            this.previewSprite = previewSprite;
        }

        internal override void UpdateFrame()
        {
            towerPreview.gameObject.SetActive(!guiHit.IsGuiUnderPointer);

            if (guiHit.IsGuiUnderPointer)
            {
                rangeVfx.StopAndCelar();
                return;
            }

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPoint = gameCamera.ScreenPointToWorldPointOnPlane(mousePos, Plaine.XY);
            Vector2Int cell = grid.WorldToCell(worldPoint);
            Vector3 cellPos = grid.CellToWorld(cell);
            towerPreview.transform.position = cellPos;
            towerPreview.sprite = previewSprite;
            rangeVfx.Play(attackRange, Color.white);
            rangeVfx.Position = cellPos;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (buildingsOnBoard.Exist(cell))
                    return;

                ClickBuild.Invoke(cell);
            }
        }
    }
}