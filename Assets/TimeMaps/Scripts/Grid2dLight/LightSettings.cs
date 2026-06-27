using UnityEngine;

namespace Action7.Grid2dLight
{
	[CreateAssetMenu(menuName = "Action07/LightSettings")]
	public class LightSettings : ScriptableObject
	{
		[SerializeField] Color sunnyLightShadowColor;
  
		public Color GetColor(LightType type)
		{
			switch (type)
			{
				case LightType.SunnyLightShadow: return sunnyLightShadowColor;
			}
			
			return Color.white;
		}
	}
}