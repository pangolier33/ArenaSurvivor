using Bones.UI.Bindings.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Bindings
{
    public class GroupBinding : BaseBinding<int>
    {
        [SerializeField] private Transform _container;
        [SerializeField] private Image _prefab;

        public override void OnNext(int value)
        {
            for (int i = 1; i < value; i++)
            {
                Object.Instantiate(_prefab, _container);
            }
        }
    }
}