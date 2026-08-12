using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.CoreGame.Gui
{

    class ContextMenuSystem : MonoBehaviour
    {
        [SerializeField, IsntNull] Canvas canvas;
        [SerializeField, IsntNull] RectTransform canvasRect;
        [Inject] Camera gameCamera;

        internal void ShowAtWorldPoint(Vector3 worldPoint, RectTransform view)
        {
            Vector3 screenPoint = gameCamera.WorldToScreenPoint(worldPoint, Camera.MonoOrStereoscopicEye.Mono);
            ShowAtScreenPoint(screenPoint, view);
        }

        internal void ShowAtScreenPoint(Vector3 mouseScreenPos, RectTransform view)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
               canvasRect,
               mouseScreenPos,
               canvas.worldCamera,
               out Vector2 localPoint
            );

            view.pivot = new Vector2(
                localPoint.x > 0 ? 1 : 0,
                localPoint.y > 0 ? 1 : 0);

            view.anchoredPosition = localPoint;
        }
    }
}