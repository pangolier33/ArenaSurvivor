using System.Collections;
using System.Collections.Generic;

namespace Bones.Gameplay.Effects.Provider.Common
{
    public class Node : IMutableTrace
    {
        private static Node s_root;
        private readonly Node _previous;
        private readonly object _value;

        private Node()
        {
            _previous = null;
            _value = null;
        }
        
        private Node(Node head, object value)
        {
            _previous = head;
            _value = value;
        }

        public static IMutableTrace Root => s_root ??= new Node();

        public IEnumerator<object> GetEnumerator()
        {
            var node = this;
            while (node != null)
            {
                yield return _value; 
                node = node._previous;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public IMutableTrace Add<T>(T arg)
        {
            return new Node(this, arg);
        }

        public bool TryGet<T>(out T value)
        {
            if (_value is T result)
            {
                value = result;
                return true;
            }

            if (_previous == null)
            {
                value = default;
                return false;
            }

            return _previous.TryGet(out value);
        }
    }
}