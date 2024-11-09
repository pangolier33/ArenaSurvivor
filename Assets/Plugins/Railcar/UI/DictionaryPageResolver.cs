using System.Collections.Generic;

namespace Railcar.UI
{
	public sealed class DictionaryPageResolver<T> : IPageResolver<T>
	{
		private readonly IDictionary<T, IMenu> _library;

		public DictionaryPageResolver(IDictionary<T, IMenu> library)
		{
			_library = library;
		}

		public IMenu Resolve(T key) => _library[key];
	}
}