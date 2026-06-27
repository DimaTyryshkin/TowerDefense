using System.Collections.Generic;
using GamePackages.Core.Validation;
using UnityEngine;

namespace Action7.Grid2dLight
{
	public class LightSystem : MonoBehaviour
	{
		[SerializeField, IsntNull] LightSettings settings;
		[SerializeField, IsntNull] GameObject disableObject;
		[SerializeField, IsntNull] Grid grid;

		Dictionary<Vector2Int, LightCell> cellToLight;

		Color defaultColor;

		void Start()
		{
			defaultColor = Color.white;
			
			cellToLight = new Dictionary<Vector2Int, LightCell>();
			var allLights = GetComponentsInChildren<LightCell>(includeInactive: true);
			foreach (var light in allLights)
			{
				light.Init(this, settings);
				Vector3Int cell = grid.WorldToCell(light.transform.position);
				cellToLight[new Vector2Int(cell.x, cell.y)] = light;
			}
			
			disableObject.SetActive(false);
		}

		public Color GetColor(Vector3 position)
		{
			Vector3Int cell = grid.WorldToCell(position);
			if (cellToLight.TryGetValue(new Vector2Int(cell.x, cell.y), out LightCell lightCell))
				return lightCell.Color;
			
			return defaultColor;
		}
	}
}