using JetBrains.Annotations;

namespace Bones.Gameplay.Stats.Processors
{
	internal class DummyStatProcessor<T> : IStatProcessor<T>
	{
		[CanBeNull] private static IStatProcessor<T> s_instance;
		
		private DummyStatProcessor() { }
		public static IStatProcessor<T> Instance => s_instance ??= new DummyStatProcessor<T>();

		public T Process(T from)
		{
			return from;
		}
	}
}