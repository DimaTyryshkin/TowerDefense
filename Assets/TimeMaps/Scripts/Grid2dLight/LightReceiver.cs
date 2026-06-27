using GamePackages.Core.Validation;
using UnityEngine;

namespace Action7.Grid2dLight
{
	public class LightReceiver : MonoBehaviour
	{
		[SerializeField,IsntNull] LightSystem lightSystem;
		[SerializeField, IsntNull] SpriteRenderer thisRenderer;
		[SerializeField] float lerpFactor = 5;

		void LateUpdate()
		{
			Color color = lightSystem.GetColor(transform.position);
			thisRenderer.color = Color.Lerp(thisRenderer.color, color, Time.deltaTime * lerpFactor);
		}
	}

	public enum LightType
	{
		NoShadow = 0,
		SunnyLightShadow = 1
	}
}