using System;

namespace Bones.Gameplay.Stats.Processors
{
	public class DelegateStatProcessor<T> : IStatProcessor<T>
	{
		private readonly Func<T, T> _transformer;

		public DelegateStatProcessor(Func<T, T> transformer)
		{
			_transformer = transformer;
		}

		public static implicit operator DelegateStatProcessor<T>(Func<T, T> f) => new(f);

		public T Process(T from) => _transformer(from);
	}
}