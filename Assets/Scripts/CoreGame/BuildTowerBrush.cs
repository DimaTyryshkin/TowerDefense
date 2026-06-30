using Game.SortedTiles;
using GamePackages.Core;
using GamePackages.Core.Validation;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.CoreGame
{

    class BuildTowerBrush : MonoBehaviour
    {
        [SerializeField, IsntNull] Grid grid;
        [SerializeField, IsntNull] SpriteRenderer towerPreview;

        [Inject] Camera gameCamera;
        [Inject] SortedTilesSystem sortedTilesSystem;
        [Inject] EnemyesOnBoardCollection enemyesOnBoardCollection;

        Dictionary<Vector2Int, TowerAI> cellToTower;
        TowerAI towerPrefab;

        internal void Init()
        {
            Assert.IsNotNull(enemyesOnBoardCollection);
            cellToTower = new();
            sortedTilesSystem.LinkTilesFromTileMaps();
        }

        private void Start()
        {
            SetPreviewEnable(false);
        }

        internal void UpdateFrame(Vector3 mousePos, bool clickInput, Action<TowerAI> onBuildCallback)
        {
            Vector3 worldPoint = gameCamera.ScreenPointToWorldPointOnPlane(mousePos, Plaine.XY);
            Vector2Int cell = (Vector2Int)grid.WorldToCell(worldPoint);
            towerPreview.transform.position = CellToWorld(cell);

            if (clickInput)
            {
                if (cellToTower.ContainsKey(cell))
                    return;

                TowerAI newTower = grid.transform.InstantiateAsChild(towerPrefab);
                newTower.transform.position = CellToWorld(cell);
                newTower.gameObject.SetActive(true);
                sortedTilesSystem.LinkGameObject(newTower.gameObject);
                cellToTower[cell] = newTower;
                newTower.Init(enemyesOnBoardCollection);

                onBuildCallback.Invoke(newTower);
                //tilemap.SetTile(tilemap.WorldToCell(worldPoint), tile);
            }

            //TileBase tile = tilemap.GetTile(tilemap.WorldToCell(worldPoint));
            //if (tile)
            //    Debug.Log(tile.name);

        }

        internal void SetTowerPrefab(TowerAI towerPrefab)
        {
            Assert.IsNotNull(towerPrefab);
            this.towerPrefab = towerPrefab;
        }

        internal void SetPreviewEnable(bool enable)
        {
            towerPreview.gameObject.SetActive(enable);
        }

        Vector3 CellToWorld(Vector2Int cell) => grid.CellToWorld((Vector3Int)cell) + new Vector3(0.5f, 0.5f, 0);
    }
}