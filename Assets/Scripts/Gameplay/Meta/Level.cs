using System;
using UniRx;
using UnityEngine;

namespace Bones.Gameplay.Meta
{
	public class Level
	{
		private ReactiveProperty<int> _current;
		private ReactiveProperty<int> _maxUnlocked;

		public IReadOnlyReactiveProperty<int> Current => _current;
		public IReadOnlyReactiveProperty<int> MaxUnlocked => _maxUnlocked;

		public event Action Completed;
		public event Action Failed;
		public event Action Started;

		public Level()
		{
			_current = new ReactiveProperty<int>();
			_maxUnlocked = new ReactiveProperty<int>();
		}

		public void ApplySave(int current, int maxUnlocked)
		{
			_current.Value += current;
			_maxUnlocked.Value += maxUnlocked;
		}

		public void CompleteLevel()
		{
			_current.Value++;
			_maxUnlocked.Value = Math.Max(_maxUnlocked.Value, _current.Value);
			Completed?.Invoke();
		}

		public void FailLevel()
		{
			Failed?.Invoke();
		}

		public void StartLevel()
		{
			Started?.Invoke();
		}

		public void SetCurrent(int index)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (index > _maxUnlocked.Value)
				throw new InvalidOperationException($"Level {index} locked.");

			_current.Value = index;
		}
	}
}
