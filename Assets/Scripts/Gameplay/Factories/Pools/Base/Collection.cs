using System;
using System.Collections;
using System.Collections.Generic;

namespace Bones.Gameplay.Factories.Pools.Base
{
	public class Collection<TValue> : ICollection<TValue>
    {
        private readonly HashSet<TValue> _items = new();
        private readonly Queue<TValue> _addingQueue = new();
        private readonly Queue<TValue> _removingQueue = new();
        private IEnumerator<TValue> _enumerator;
            
        bool ICollection<TValue>.IsReadOnly => false;
        public int Count => _items.Count;

        private bool IsEnumerating => _enumerator != null;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            if (IsEnumerating)
                throw new InvalidOperationException($"Pool is already iterating by {_enumerator}. Dispose it before calling {nameof(GetEnumerator)} again");
            return new Enumerator(this);
        }

        public void Add(TValue item)
        {
            if (IsEnumerating)
                _addingQueue.Enqueue(item);
            else
                _items.Add(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public bool Remove(TValue item)
        {
            if (!Contains(item))
                return false;
            
            if (IsEnumerating)
                _removingQueue.Enqueue(item);
            else
                _items.Remove(item);
            return true;
        }

        public void Clear()
        {
            if (IsEnumerating)
            {
                _addingQueue.Clear();
                foreach (var item in _items)
                    _removingQueue.Enqueue(item);
            }
            else
            {
                _items.Clear();
            }
        }

        public bool Contains(TValue item)
        {
            return _items.Contains(item);
        }
        
        private void ReleaseQueues()
        {
            while (_removingQueue.Count > 0)
                _items.Remove(_removingQueue.Dequeue());
            while (_addingQueue.Count > 0)
                _items.Add(_addingQueue.Dequeue());
        }

        private class Enumerator : IEnumerator<TValue>
        {
            private readonly Collection<TValue> _origin;

            public Enumerator(Collection<TValue> pool)
            {
                _origin = pool;
                _origin._enumerator = _origin._items.GetEnumerator();
            }
            
            object IEnumerator.Current => Current;
            public TValue Current => _origin._enumerator.Current;

            public bool MoveNext() => _origin._enumerator.MoveNext();
            public void Reset() => _origin._enumerator.Reset();
            public void Dispose()
            {
                _origin._enumerator.Dispose();
                _origin._enumerator = null;
                _origin.ReleaseQueues();
            }
        }
    }
}