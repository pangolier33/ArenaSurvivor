using System;
using Bones.UI.Bindings.Transitions.Base;

namespace Bones.UI.Bindings.Transitions
{
    [Serializable]
    public sealed class IntToStringTransBinding : TransBinding<int, string>
    {
        protected override string Convert(int from)
        {
            return from.ToString();
        }
    }
}