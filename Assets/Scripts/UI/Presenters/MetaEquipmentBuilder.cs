using Bones.Gameplay.Meta.Equipment;
using Bones.UI.Views;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Bones.UI.Presenters
{
	public class MetaEquipmentBuilder : MonoBehaviour
	{
		[SerializeField] private BackpackEquipmentView _prefab;
		[SerializeField] private Transform _backpackContainer;
		[SerializeField] private EquipedView[] _equipedViews;

		private List<BackpackEquipmentView> _backpackEquipmentViews = new();
		private IEquipmentStorage _equipmentStorage;

		[Inject]
		private void Initialize(IEquipmentStorage equipmentStorage)
		{
			_equipmentStorage = equipmentStorage;
			_equipmentStorage.BackpackChanged += OnBackpackChanged;
			_equipmentStorage.EquipedChanged += OnEquipedChanged;
		}

		private void OnEquipedChanged(IEnumerable<IEquipment> equipments)
		{
			UpdateEquiped();
		}

		private void OnBackpackChanged(IEnumerable<IEquipment> equipments)
		{
			BuildBackpack();
			UpdateEquiped();
		}

		private void BuildBackpack()
		{
			foreach (var equipmentView in _backpackEquipmentViews)
				Destroy(equipmentView.gameObject);
			_backpackEquipmentViews.Clear();
			foreach (var equipment in _equipmentStorage.Backpack.OrderBy(x => x.Id))
			{
				var equipmentView = Instantiate(_prefab, _backpackContainer);
				equipmentView.Initialize(equipment);
				_backpackEquipmentViews.Add(equipmentView);
				equipmentView.Clicked += OnBackpackEquipmentClicked;
				equipmentView.Choosed += OnEquipmentChoosed;
			}
		}

		private void OnEquipmentChoosed(BackpackEquipmentView view)
		{
			_equipmentStorage.Equip(view.Equipment);
		}

		private void OnBackpackEquipmentClicked(BackpackEquipmentView clickedView)
		{
			foreach (var backpackEquipmentView in _backpackEquipmentViews)
				if (backpackEquipmentView != clickedView)
					backpackEquipmentView.HideExpand();
			if (!clickedView.Expanded)
				clickedView.ShowExpand();
			else
				clickedView.HideExpand();
		}

		private void UpdateEquiped()
		{
			foreach(var equip in _equipmentStorage.Equiped)
			{
				var view = _equipedViews.First(x => x.Type == equip.Type);
				view.Initialize(equip);
			}
		}
	}
}