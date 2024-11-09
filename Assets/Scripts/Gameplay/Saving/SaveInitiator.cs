using Bones.Gameplay.Meta;
using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Meta.Equipment;
using System;
using System.Collections.Generic;
using UniRx;

namespace Bones.Gameplay.Saving
{
	public class SaveInitiator : IDisposable
	{
		private ISaver _saver;
		private ISaveCreator _saveCreator;
		private ICurrencyStorage _currencyStorage;
		private IEquipmentStorage _equipmentStorage;
		private ICharacterClassStorage _characterClassStorage;
		private Level _level;
		private ILoader _loader;
		private List<IDisposable> _disposed = new();
		private List<IDisposable> _ativeClassesDisposed = new();

		public SaveInitiator(ISaver saver, 
			ISaveCreator saveCreator,
			ICurrencyStorage currencyStorage,
			IEquipmentStorage equipmentStorage,
			ICharacterClassStorage characterClassStorage,
			Level level,
			ILoader loader)
		{
			_saver = saver;
			_saveCreator = saveCreator;
			_currencyStorage = currencyStorage;
			_equipmentStorage = equipmentStorage;
			_characterClassStorage = characterClassStorage;
			_level = level;
			_loader = loader;
			Subscribe();
		}

		public void Dispose()
		{
			Unsubscribe();
		}

		private void Subscribe()
		{
			foreach (var currency in _currencyStorage)
				_disposed.Add(currency.ObservableValue.Subscribe(CurrencyChanged));
			_equipmentStorage.BackpackChanged += EquipmentChanged;
			_equipmentStorage.EquipedChanged += EquipmentChanged;
			_equipmentStorage.SlotsChanged += SlotsChanged;
			_characterClassStorage.ActiveClassChanged += CharacterClassChanged;
			_characterClassStorage.OpenedClassChanged += CharacterClassChanged;
			_characterClassStorage.ActiveClassChanged += ActiveClassChanged;
			ActiveClassChanged(_characterClassStorage.ActiveClasses);
			_disposed.Add(_level.Current.Subscribe(_ => Save()));
			_disposed.Add(_level.MaxUnlocked.Subscribe(_ => Save()));
		}

		private void SlotsChanged(IEnumerable<IEquipmentSlot> slots)
		{
			Save();
		}

		private void ActiveClassChanged(IEnumerable<ICharacterClass> activeClasses)
		{
			foreach (var acitveClassDisposed in _ativeClassesDisposed)
				acitveClassDisposed.Dispose();
			_ativeClassesDisposed.Clear();
			foreach (var activeClass in activeClasses)
				_ativeClassesDisposed.Add(activeClass.Experience.ObservableValue.Subscribe(_ => Save()));
		}

		private void Unsubscribe()
		{
			foreach (var disposed in _disposed)
				disposed.Dispose();
			_disposed.Clear();
			foreach (var disposed in _ativeClassesDisposed)
				disposed.Dispose();
			_ativeClassesDisposed.Clear();
			_equipmentStorage.BackpackChanged -= EquipmentChanged;
			_equipmentStorage.EquipedChanged -= EquipmentChanged;
			_equipmentStorage.SlotsChanged -= SlotsChanged;
			_characterClassStorage.ActiveClassChanged -= CharacterClassChanged;
			_characterClassStorage.OpenedClassChanged -= CharacterClassChanged;
		}

		private void CharacterClassChanged(IEnumerable<ICharacterClass> enumerable)
		{
			Save();
		}

		private void EquipmentChanged(IEnumerable<IEquipment> enumerable)
		{
			Save();
		}

		private void CurrencyChanged(float value)
		{
			Save();
		}

		private void Save()
		{
			if (_loader.Loaded)
			{
				var save = _saveCreator.Create();
				_saver.Save(save);
			}
		}
	}
}
