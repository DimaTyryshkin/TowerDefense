using Game.SortedTiles;
using GamePackages.Core;
using GamePackages.Core.Validation;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace Game.CoreGame
{

    class TowerBuilder : MonoBehaviour
    {
        [SerializeField, IsntNull] Grid grid;
        [SerializeField, IsntNull] Camera gameCamera;
        //[SerializeField, IsntNull] Tilemap tilemap;
        //[SerializeField, IsntNull] TileBase tile;
        [SerializeField, IsntNull] SortedTilesSystem sortedTilesSystem;
        [SerializeField, IsntNull] TowerAI tower;
        [SerializeField, IsntNull] GameObject towerPreview;

        Dictionary<Vector2Int, TowerAI> cellToTower;
        EnemyesOnBoardCollection enemyesOnBoardCollection;

        internal void Init(EnemyesOnBoardCollection enemyesOnBoardCollection)
        {
            Assert.IsNotNull(enemyesOnBoardCollection);
            this.enemyesOnBoardCollection = enemyesOnBoardCollection;
            cellToTower = new();
            SetTower(tower);
            sortedTilesSystem.LinkTilesFromTileMaps();
        }

        private void Start()
        {
            towerPreview.SetActive(true);
        }

        void Update()
        {
            Vector3 worldPoint = gameCamera.ScreenPointToWorldPointOnPlane(Mouse.current.position.ReadValue(), Plaine.XY);
            Vector2Int cell = (Vector2Int)grid.WorldToCell(worldPoint);
            towerPreview.transform.position = CellToWorld(cell);

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (cellToTower.ContainsKey(cell))
                    return;

                TowerAI newTower = grid.transform.InstantiateAsChild(tower);
                newTower.transform.position = CellToWorld(cell);
                newTower.gameObject.SetActive(true);
                sortedTilesSystem.LinkGameObject(newTower.gameObject);
                cellToTower[cell] = newTower;
                newTower.Init(enemyesOnBoardCollection);

                //tilemap.SetTile(tilemap.WorldToCell(worldPoint), tile);
            }


            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                //TileBase tile = tilemap.GetTile(tilemap.WorldToCell(worldPoint));
                //if (tile)
                //    Debug.Log(tile.name);
            }

        }

        Vector3 CellToWorld(Vector2Int cell) => grid.CellToWorld((Vector3Int)cell) + new Vector3(0.5f, 0.5f, 0);


        internal void SetTower(TowerAI tower)
        {
            Assert.IsNotNull(tower);
            this.tower = tower;
        }

    }
}
