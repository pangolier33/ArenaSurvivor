using System.Collections.Generic;
using Zenject;

namespace Bones.Gameplay.Factories.Pools.Base
{
    public interface IPool<in TParam, TProduct> : IFactory<TParam, TProduct>
    {
        ICollection<TProduct> Entities { get; }
        void Despawn(TProduct entity);
    }

    public interface IPool<T> : IFactory<T>
    {
        ICollection<T> Entities { get; }
        void Despawn(T entity);
    }
}