using UnityEngine;
using UnityEngine.Assertions;

namespace Action7.Grid2dLight
{
	public class LightCell : MonoBehaviour
	{
		[SerializeField] LightType type;
		
		//LightSystem system;
		LightSettings settings;

		public Color Color =>settings.GetColor(type);

		public void Init(LightSystem system, LightSettings settings)
		{
			Assert.IsNotNull(system);
			Assert.IsNotNull(settings);

			//this.system = system;
			this.settings = settings;
		}
	}
}