using System.Collections;
using System.Collections.Generic;

namespace Bones.Gameplay.Entities
{
	public class LazyCollection<T> : IEnumerable<T>
	{
		private readonly List<T> _pool = new();
		private IEnumerator<T> _source;
		private int _index;

		public LazyCollection(IEnumerator<T> source) => Reset(source);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<T> GetEnumerator() => new Iterator(this);

		public void Reset(IEnumerator<T> source)
		{
			_index = 0;
			_source = source;
		}
		
		private bool TryRequest(int index, out T item)
		{
			if (index < _index)
			{
				item = _pool[index];
				return true;
			}

			if (_source.MoveNext())
			{
				item = _source.Current;
				if (_index >= _pool.Count)
					_pool.Add(item);
				else
					_pool[_index] = item;

				_index++;
				return true;
			}

			item = default;
			return false;
		}
		
		private class Iterator : IEnumerator<T>
		{
			private readonly LazyCollection<T> _origin;
			private int _index;
			
			public Iterator(LazyCollection<T> origin)
			{
				_origin = origin;
			}

			public T Current { get; private set; }
			object IEnumerator.Current => Current;
			
			public bool MoveNext()
			{
				var result = _origin.TryRequest(_index++, out var item);
				Current = item;
				return result;
			}

			public void Reset()
			{
				_index = 0;
			}

			public void Dispose()
			{
				// TODO: can cause de-syncing on source reset
			}
		}
	}
}