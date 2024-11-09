using System.Collections.Generic;
using Zenject;

namespace Bones.Gameplay.Factories
{
    public class CachedFactory<TParam, TProduct> : IFactory<TParam, TProduct>
    {
        private readonly Dictionary<TParam, TProduct> _existing = new();
        private readonly IFactory<TParam, TProduct> _factory;

        public CachedFactory(IFactory<TParam, TProduct> factory)
        {
            _factory = factory;
        }

        public IEnumerable<TProduct> Products => _existing.Values;

        public TProduct Create(TParam param)
        {
            if (!_existing.TryGetValue(param, out var product))
            {
                product = _factory.Create(param);
                _existing.Add(param, product);
            }

            return product;
        }
    }
}