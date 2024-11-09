using System.Collections.Generic;
using UniRx;

namespace Bones.UI.Presenters.New
{
	public static class ListViewModelExtensions
	{
		public static IReactiveCollection<T> Bind<T>(this ListViewModel<T> viewModel, IEnumerable<T> collection)
		{
			var rxCollection = new ReactiveCollection<T>(collection);
			viewModel.Bind(rxCollection);
			return rxCollection;
		}
	}
}