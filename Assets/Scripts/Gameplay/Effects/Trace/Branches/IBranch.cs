using System;

namespace Bones.Gameplay.Effects.Provider.Branches
{
    public interface IBranch : IDisposable
    {
        void Connect(IDisposable subscription);
    }
}