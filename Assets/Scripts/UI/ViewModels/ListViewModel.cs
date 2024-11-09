using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Bones.UI.Presenters.New
{
	public class ListViewModel<T> : BaseViewModel<IReadOnlyReactiveCollection<T>>
	{
		[SerializeField] private BaseViewModel<T> _prefab;
		[SerializeField] private Transform _container;
        
		private readonly Dictionary<T, BaseViewModel<T>> _viewsByModels = new();
		private IDisposable _subscription;
        
		protected sealed override IDisposable SetupBindings(IReadOnlyReactiveCollection<T> collection)
		{
			if (_prefab == null)
				throw new NullReferenceException($"Prefab for {gameObject.name} is not set");
			
			var subscriptions = new CompositeDisposable();

			foreach (var (model, index) in collection.Select((x, i) => (x, i)))
				CreateInstance(model, index);
			
			collection.ObserveReset().Subscribe(OnReset).AddTo(subscriptions);
			collection.ObserveMove().Subscribe(OnItemMoved);
			collection.ObserveReplace().Subscribe(OnItemReplaced).AddTo(subscriptions);
			collection.ObserveRemove().Subscribe(OnItemRemoved).AddTo(subscriptions);
			collection.ObserveAdd().Subscribe(OnItemAdded).AddTo(subscriptions);
			
			Disposable.Create(() => OnReset(Unit.Default)).AddTo(subscriptions);
			return subscriptions;
		}
        
		private void OnReset(Unit _)
		{
			foreach (var (_, viewModel) in _viewsByModels)
				Destroy(viewModel);
			_viewsByModels.Clear();
		}

		private void OnItemMoved(CollectionMoveEvent<T> args)
		{
			ThrowIfModelNotBound(args.Value);
			_viewsByModels[args.Value].transform.SetSiblingIndex(args.NewIndex);
		}

		private void OnItemReplaced(CollectionReplaceEvent<T> args)
		{
			DestroyInstance(args.OldValue);
			CreateInstance(args.NewValue, args.Index);
		}
		private void OnItemRemoved(CollectionRemoveEvent<T> args) => DestroyInstance(args.Value);
		private void OnItemAdded(CollectionAddEvent<T> args) => CreateInstance(args.Value, args.Index);

		private void DestroyInstance(T model)
		{
			ThrowIfModelNotBound(model);
            
			_viewsByModels.Remove(model, out var viewModel);
			Destroy(viewModel);
		}
        
		private void CreateInstance(T model, int index)
		{
			ThrowIfModelBound(model);
            
			var instance = Instantiate(_prefab, _container);
			instance.Bind(model);
			instance.transform.SetSiblingIndex(index);
			_viewsByModels.Add(model, instance);
		}
        
		private void ThrowIfModelBound(T model)
		{
			if (_viewsByModels.ContainsKey(model))
				throw new InvalidOperationException($"Can't add view-model for already bound model {model}");
		}

		private void ThrowIfModelNotBound(T model)
		{
			if (!_viewsByModels.ContainsKey(model))
				throw new InvalidOperationException($"Can't remove view-model of unbound model {model}");
		}
	}
}