using UnityEngine;

namespace Railcar.UI
{
	public class ExampleBootstrapper : MonoBehaviour
	{
		[SerializeField] private ExampleMenu _menu;
		[SerializeField] private Sprite _icon;
		
		private void Start()
		{
			_menu.Inject(new Service(_icon));
			_menu.Open();
		}
	}
}