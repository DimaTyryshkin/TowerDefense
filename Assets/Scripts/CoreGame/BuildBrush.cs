using GamePackages.Core;
using GamePackages.InputSystem;
using UnityEngine;

namespace Game.CoreGame
{
    abstract class BuildBrush
    {
        [Inject] protected GridWrapper grid;
        [Inject] protected Camera gameCamera;
        [Inject] protected GuiHit guiHit;
        [Inject] protected BuildingsOnBoardColelction buildingsOnBoard;

        internal abstract void UpdateFrame();
    }
}