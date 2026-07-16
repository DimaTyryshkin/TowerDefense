using GamePackages.Core;
using GamePackages.Core.Validation;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.SortedTiles
{
    // TODO Сценариии
    // - Спавним посреди игры статический объект, например блок или душу в хоре. Нужно однйо командой высстаивть слой.
    // - Сорруем объекты соспавненные сразу на уровне
    // - Как-то должны бегать динамичесские объекты
    // - на сцене ммогут быть одновременно две системы сортировки. Не должны конфликтовать.
    // - можем соспавнть посреди игры объект, требующий груповой сортировки
    // - описать справку тут в терминах жизненного цикла тайла или системы
    public class SortedTilesSystem : MonoBehaviour
    {
        [SerializeField, IsntNull] Grid grid;

        public readonly int PixelsPerUnit = 16;
        readonly int heightToLayerFactor = 16;
        Dictionary<Vector3Int, SortedTile> cellToTile;
        float maxY;
        float cellHeight;
        internal static SortedTilesSystem inst;

        void OnDrawGizmos()
        {
            //Gizmos.color = Color.red;
            //GizmosExtension.DrawBounds(tilemap.get);
        }

        public void LinkTilesFromTileMaps()
        {
            inst = this;
            cellHeight = grid.cellSize.y;
            maxY = transform.position.y + 20;
            cellToTile = new Dictionary<Vector3Int, SortedTile>();

            SortedTile[] allTiles = grid.GetComponentsInChildren<SortedTile>(true);

            foreach (SortedTile tile in allTiles)
                LinkTile(tile, groupsActualySorted: false);

            SortedTile[] tilesOnGrid = cellToTile.Values.ToArray();
            foreach (var tile in tilesOnGrid)
                UpdateOrder(tile);
        }

        public void LinkGameObject(GameObject go)
        {
            foreach (SortedTile t in go.GetComponentsInChildren<SortedTile>(true))
                LinkTile(t);
        }

        public void LinkTile(SortedTile tile) => LinkTile(tile, true);

        int GetOrderFromY(float yPosition, SortedTile tile)
        {
            if (yPosition > maxY)
                Debug.LogError($"{tile.gameObject.FullName()} yPosition={yPosition}");

            float fromTopToPosition = maxY - yPosition;
            return (int)(fromTopToPosition * heightToLayerFactor);
        }

        SortedTile TileByCell(Vector3 world)
        {
            var cell = grid.WorldToCell(world);
            return cellToTile.GetValueOrDefault(cell);
        }

        internal void LinkTile(SortedTile tile, bool groupsActualySorted)
        {
            Assert.IsFalse(tile.IsDynamic && tile.IsGroupingTile);

            tile.Init();
            if (tile.IsGroupingTile)
            {
                Vector3Int cell = grid.WorldToCell(tile.transform.position);
                if (cellToTile.ContainsKey(cell))
                    throw new Exception($"{tile.gameObject.FullName()} <===> {cellToTile[cell].gameObject.FullName()}");

                cellToTile[cell] = tile;

                if (groupsActualySorted)
                    UpdateOrder(tile);
            }
            else
            {
                UpdateOrder(tile);
            }
        }

        void UpdateOrder(SortedTile tile)
        {
            if (tile.IsGroupingTile)
            {
                Vector3 pos = tile.transform.position;
                Vector3Int cell = grid.WorldToCell(pos);
                cell.y--;

                SortedTile tileDown = TileByCell(cell);
                if (tileDown && tileDown.GroupName == tile.GroupName)
                {
                    UpdateOrder(tileDown);
                    tile.Order = tileDown.Order;//+ 1 * system.heightToLayerFactor;

#if UNITY_EDITOR
                    GizmosDrawer.Inst.AddArrow(tile.transform.position, tileDown.transform.position).Duration = 10;
                    GizmosDrawer.Inst.AddText(tile.transform.position, tile.Order.ToString()).Duration = 10;
#endif
                    return;
                }
            }

            tile.Order = GetOrder(tile);
        }

        internal int GetOrder(SortedTile tile)
        {
            float y = tile.SpriteRenderer.bounds.min.y;
            float additionalOffset = tile.YOffsetInPixelsFromBotBase / (float)PixelsPerUnit;
            return GetOrderFromY(y + additionalOffset - tile.Height * cellHeight, tile) + tile.OrderOffset;
        }

#if UNITY_EDITOR
        [Button()]
        internal void UpdateOrder()
        {
            LinkTilesFromTileMaps();
        }
#endif
    }
}