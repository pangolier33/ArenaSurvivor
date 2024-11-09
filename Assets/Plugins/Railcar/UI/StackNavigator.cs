using System.Collections.Generic;

namespace Railcar.UI
{
	public class StackNavigator : INavigator
	{
		private readonly Stack<IMenu> _history = new();
		
		public void Open(IMenu menu)
		{
			menu.Open();
			_history.Push(menu);
		}

		public void Back()
		{
			_history.Pop().Close();
		}
	}
}