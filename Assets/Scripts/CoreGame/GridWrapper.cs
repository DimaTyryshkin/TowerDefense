using UnityEngine;
using UnityEngine.Assertions;

namespace Game.CoreGame
{
    class GridWrapper
    {
        internal Grid grid;

        public GridWrapper(Grid grid)
        {
            Assert.IsNotNull(grid);
            this.grid = grid;
        }

        internal Vector3 CellToWorld(Vector2Int cell) => grid.CellToWorld((Vector3Int)cell) + new Vector3(0.5f, 0.5f, 0);

        internal Vector2Int WorldToCell(Vector3 worldPoint) => (Vector2Int)grid.WorldToCell(worldPoint);
    }
}