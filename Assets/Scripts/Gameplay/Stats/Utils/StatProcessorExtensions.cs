using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Players;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Numbers;
using Bones.Gameplay.Stats.Processors;
using Bones.Gameplay.Stats.Units;
using Bones.Gameplay.Stats.Units.Operations;
using UniRx;

namespace Bones.Gameplay.Stats.Utils
{
	public static class StatProcessorExtensions
	{
		public static T GetValue<T>(this IStatMap map, StatName name) => ((IGetStat<T>)map.Get(name)).Get();

		public static IObservable<T> ObserveGetAndSet<T>(this IStat stat)
		{
			if (stat is not IGetStat<T> getStat)
				throw new InvalidOperationException($"Stat {stat} is not serving getting");
			if (stat is not ISetStat<T> setStat)
				throw new InvalidOperationException($"Stat {stat} is not serving setting");
			if (stat is not IObservable<T> observableStat)
				throw new InvalidOperationException($"Stat {stat} is not observable");
			
			var property = new ReactiveProperty<T>(getStat.Get());
			
			observableStat.Subscribe(OnValueUpdated);
			Get<ChainStatProcessor<T>, T>(ref getStat.Processor).NodeAdded.Subscribe(OnProcessorUpdated);
			Get<ChainStatProcessor<T>, T>(ref setStat.Processor).NodeAdded.Subscribe(OnProcessorUpdated);
			
			return property;

			void OnValueUpdated(T value) => property.Value = value;
			void OnProcessorUpdated(IStatProcessor<T> _) => property.Value = getStat.Get();
		}
		
		public static IDisposable SubscribeOnAdd(this IStat stat, Action callback)
		{
			return stat switch
			{
				IAddStat<Value> valueStat => ObserveOnAdd(valueStat).Subscribe(_ => callback()),
				IAddStat<Points> pointsStat => ObserveOnAdd(pointsStat).Subscribe(_ => callback()),
				IAddStat<SpeedValue> speedStat => ObserveOnAdd(speedStat).Subscribe(_ => callback()),
				IAddStat<TimedValue> timedStat => ObserveOnAdd(timedStat).Subscribe(_ => callback()),
				_ => throw new InvalidOperationException($"Stat {stat} is not serving subtraction")
			};
		}
		public static IObservable<T> ObserveOnAdd<T>(this IStat stat)
		{
			return ((IAddStat<T>)stat).ObserveOnAdd();
		}
		public static IObservable<T> ObserveOnAdd<T>(this IAddStat<T> stat)
		{
			return Get<RxProcessor<T>, T>(ref stat.Processor);
		}
		
		public static IDisposable SubscribeOnSubtract(this IStat stat, Action callback)
		{
			return stat switch
			{
				ISubtractStat<Value> valueStat => ObserveOnSubtract(valueStat).Subscribe(_ => callback()),
				ISubtractStat<Points> pointsStat => ObserveOnSubtract(pointsStat).Subscribe(_ => callback()),
				ISubtractStat<SpeedValue> speedStat => ObserveOnSubtract(speedStat).Subscribe(_ => callback()),
				ISubtractStat<TimedValue> timedStat => ObserveOnSubtract(timedStat).Subscribe(_ => callback()),
				_ => throw new InvalidOperationException($"Stat {stat} is not serving subtraction")
			};
		}
		public static IObservable<T> ObserveOnSubtract<T>(this IStat stat)
		{
			return ((ISubtractStat<T>)stat).ObserveOnSubtract();
		}
		public static IObservable<T> ObserveOnSubtract<T>(this ISubtractStat<T> stat)
		{
			return Get<RxProcessor<T>, T>(ref stat.Processor);
		}

		public static IDisposable DivideOnGet(this IStat subject, IStat source) =>
			subject switch
			{
				IGetStat<Value> valueSubject when source is IGetStat<Value> valueSource =>
					Amplify(ref valueSubject.Processor, () => new Value(1 / valueSource.Get())),
				_ => throw new InvalidOperationException($"Can't add {source} to {subject}")
			};
		public static IDisposable AddOnGet(this IStat subject, IStat source) =>
			subject switch
			{
				IGetStat<Points> pointsSubject when source is IGetStat<Amount> valueSource =>
					Offset(ref pointsSubject.Processor, valueSource.Get),
				IGetStat<Value> valueSubject when source is IGetStat<Value> valueSource =>
					Offset(ref valueSubject.Processor, valueSource.Get),
				IGetStat<SpeedValue> speedValueSubject when source is IGetStat<Value> valueSource =>
					Offset(ref speedValueSubject.Processor, valueSource.Get),
				IGetStat<TimedValue> timedValueSubject when source is IGetStat<Value> valueSource =>
					Offset(ref timedValueSubject.Processor, valueSource.Get),
				_ => throw new InvalidOperationException($"Can't add {source} to {subject}")
			};

		public static IDisposable ModifyOnGet<T>(this IGetStat<T> stat, Func<T, T> action) =>
			Modify(ref stat.Processor, Create(action));
		public static IDisposable ModifyOnSet<T>(this ISetStat<T> stat, Func<T, T> action) =>
			Modify(ref stat.Processor, Create(action));
		public static IDisposable ModifyOnAdd<T>(this IAddStat<T> stat, Func<T, T> action) =>
			Modify(ref stat.Processor, Create(action));
		public static IDisposable ModifyOnSubtract<T>(this ISubtractStat<T> stat, Func<T, T> action) =>
			Modify(ref stat.Processor, Create(action));

		public static IStatProcessor<T> Create<T>(Func<T, T> action) =>
			new DelegateStatProcessor<T>(action);

		public static IDisposable Modify<T>(ref IStatProcessor<T> existing, IStatProcessor<T> addition)
		{
			switch (existing)
			{
				case DummyStatProcessor<T>:
					var dummyChain = new ChainStatProcessor<T>();
					existing = dummyChain;
					return dummyChain.Add(addition).CreateSubscription();
				case ChainStatProcessor<T> existingChain:
					return existingChain.Add(addition).CreateSubscription();
				default:
					throw new ArgumentException($"Cannot modify {existing}", nameof(existing));
			}
		}

		private static IDisposable Amplify<TStat, TAmplifier>(ref IStatProcessor<TStat> existing, Func<TAmplifier> getter)
			where TAmplifier : struct, IAddSupportable<TAmplifier, TAmplifier>
			where TStat : IMultiplySupportable<TAmplifier, TStat> =>
			Get<AmplifierStatProcessor<TStat, TAmplifier>, TStat>(ref existing).Add(getter);

		private static IDisposable Offset<TStat, TAmplifier>(ref IStatProcessor<TStat> existing, Func<TAmplifier> getter)
			where TAmplifier : struct, IAddSupportable<TAmplifier, TAmplifier>
			where TStat : IAddSupportable<TAmplifier, TStat> =>
			Get<OffsetStatProcessor<TStat, TAmplifier>, TStat>(ref existing).Add(getter);

		private static TProcessor Get<TProcessor, TValue>(ref IStatProcessor<TValue> sourceProcessor)
			where TProcessor : IStatProcessor<TValue>, new()
		{
			switch (sourceProcessor)
			{
				case TProcessor resultProcessor:
					return resultProcessor;
				case ChainStatProcessor<TValue> chainProcessor:
					foreach (var processor in chainProcessor.Processors)
					{
						if (processor is not TProcessor resultProcessor)
							continue;
						return resultProcessor;
					}
					var newProcessor = new TProcessor();
					chainProcessor.Add(newProcessor);
					return newProcessor;
				case null:
				case DummyStatProcessor<TValue>:
					var unitProcessor = new TProcessor();
					sourceProcessor = unitProcessor;
					return unitProcessor;
				default:
					var newChainProcessor = new ChainStatProcessor<TValue>();
					newChainProcessor.Add(sourceProcessor);
					
					newProcessor = new TProcessor();
					newChainProcessor.Add(newProcessor);
					
					sourceProcessor = newChainProcessor;
					return newProcessor;
			}
		}
	}
}