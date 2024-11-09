using System.Collections;
using System.Collections.Generic;

namespace Bones.Gameplay.Effects.Provider.Debug
{
	public class DebugNode : IMutableTrace
    {
        private static DebugNode s_root;

        private readonly DebugNode _previous;
        private readonly List<DebugNode> _children = new();

        private DebugNode()
        {
            _previous = null;
        }

        private DebugNode(DebugNode parent, object value)
        {
            parent._children.Add(this);
            _previous = parent;
            Value = value;
        }

        public static DebugNode Root => s_root ??= new DebugNode()
        {
	        Foldout = true,
        };
        public IReadOnlyCollection<DebugNode> Children => _children;

        public object Value { get; }

        public bool Foldout { get; set; } = false;
        
        public static void ClearCurrent()
        {
            Root._children.Clear();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<object> GetEnumerator()
        {
            var node = this;
            while (node != null)
            {
                yield return node.Value;
                node = node._previous;
            }
        }
        
        public IMutableTrace Add<T>(T arg)
        {
            return new DebugNode(this, arg);
        }

        public override string ToString()
        {
            if (Value == null)
                return "root";
            return $"{Value}\n{_previous}";
        }
    }
}