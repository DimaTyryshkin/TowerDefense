using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.CoreGame
{
    class BuildingsOnBoardColelction
    {
        Dictionary<Vector2Int, GameObject> cellToBuilding = new();

        internal GameObject this[Vector2Int cell]
        {
            get => cellToBuilding.GetValueOrDefault(cell);
            set => Set(cell, value);
        }

        internal bool Exist(Vector2Int cell) => cellToBuilding.ContainsKey(cell);

        internal GameObject Get(Vector2Int cell) => cellToBuilding.GetValueOrDefault(cell);

        internal T Get<T>(Vector2Int cell) where T : MonoBehaviour
        {
            GameObject go = cellToBuilding.GetValueOrDefault(cell);
            if (go)
                return go.GetComponent<T>();

            return null;
        }

        internal IEnumerable<T> GetAllByType<T>() where T : MonoBehaviour
        {
            foreach (var pair in cellToBuilding)
            {
                T component = pair.Value.GetComponent<T>();
                if (component)
                    yield return component;
            }
        }

        internal void Set(Vector2Int cell, GameObject building)
        {
            Assert.IsNotNull(building);
            Assert.IsFalse(cellToBuilding.ContainsKey(cell));
            cellToBuilding[cell] = building;
        }

        internal void RemveValue(GameObject go)
        {
            Vector2Int? cell = null;
            foreach (var pair in cellToBuilding)
            {
                if (pair.Value == go)
                    cell = pair.Key;
            }

            if (cell.HasValue)
                cellToBuilding.Remove(cell.Value);
        }
    }
}