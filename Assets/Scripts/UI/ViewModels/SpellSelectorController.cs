using System;
using System.Collections.Generic;
using Bones.Gameplay.Spells.Classes;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.UI.Presenters.New
{
	public class SpellSelectorController : MonoBehaviour, IInitializable, IDisposable, IValidatable
	{
		[SerializeField] private SpellSelectorMenu _menu;
		[SerializeField] private Transform _parent;
		
		private readonly Queue<SpellSelectorRequestedArgs> _pendingRequests = new();
		
		private DiContainer _container;
		private IMessageReceiver _receiver;
		private IDisposable _receiverSubscription;
		private bool _isOpened;

		public void Initialize() => _receiverSubscription = _receiver.Receive<SpellSelectorRequestedArgs>().Subscribe(OnRequested);
		public void Dispose() => _receiverSubscription.Dispose();
		public void Validate() => _container.Inject(_menu);

		private void OnRequested(SpellSelectorRequestedArgs args)
		{
			if (_isOpened)
				_pendingRequests.Enqueue(args);
			else
			{
				CreateMenu(args);
				_isOpened = true;
			}
		}

		private void OnExited(object sender, SpellSelectorMenu instance)
		{
			Destroy(instance.gameObject);
			if (_pendingRequests.Count > 0)
			{
				CreateMenu(_pendingRequests.Dequeue());				
			}
			else
				_isOpened = false;
		}

		private void CreateMenu(SpellSelectorRequestedArgs args)
		{
			var instance = Instantiate(_menu, _parent);
			_container.Inject(instance);
			instance.Show(args);
			instance.Exited += OnExited;
		}
		
		[Inject]
		private void Inject(DiContainer container, IMessageReceiver receiver)
		{
			_container = container;
			_receiver = receiver;
		}
	}
}