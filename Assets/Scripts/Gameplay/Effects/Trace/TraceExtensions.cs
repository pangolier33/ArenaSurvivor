using System;
using System.Collections.Generic;
using Bones.Gameplay.Effects.Provider.Branches;
using Bones.Gameplay.Effects.Provider.Variables;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units.Operations;
using JetBrains.Annotations;

namespace Bones.Gameplay.Effects.Provider
{
	public static class TraceExtensions
    {
        [Pure]
        public static T Get<T>(this ITrace trace)
        {
            foreach (var arg in trace)
            {
                if (arg is T result)
                    return result;
            }

            throw new NullReferenceException($"Variable of type {typeof(T)} is not presented in chain");
        }
        
        [Pure]
        public static T TryGet<T>(this ITrace trace)
        {
            foreach (var arg in trace)
            {
                if (arg is T result)
                    return result;
            }

            return default;
        }

        [Pure]
        public static T GetStatValue<T>(this ITrace trace, StatName name) =>
            trace.GetStat<IGetStat<T>>(name).Get();
        public static void SetStatValue<T>(this ITrace trace, StatName name, T value) =>
            trace.GetStat<ISetStat<T>>(name).Set(value);
        public static void MultiplyStatValue<TStat, TMultiplier>(this ITrace trace, StatName name, TMultiplier value)
            where TStat : IMultiplySupportable<TMultiplier, TStat>
        {
            var stat = trace.GetStat(name);
            ((ISetStat<TStat>)stat).Set(((IGetStat<TStat>)stat).Get().Multiply(value));
        }

        public static void AddStatValue<T>(this ITrace trace, StatName name, T value) =>
            trace.GetStat<IAddStat<T>>(name).Add(value);
        public static void SubtractStatValue<T>(this ITrace trace, StatName name, T value) =>
            trace.GetStat<ISubtractStat<T>>(name).Subtract(value);

        [Pure]
        public static IStat GetStat(this ITrace trace, StatName name)
        {
            foreach (var stat in trace.GetStats(name))
                return stat;
            throw new NullReferenceException($"Stat {name} is not presented in the trace");
        }

        [Pure]
        public static T GetStat<T>(this ITrace trace, StatName name)
        {
            foreach (var stat in trace.GetStats<T>(name))
                return stat;
            throw new NullReferenceException($"Stat {name} of type {typeof(T).Name} with value is not presented in the trace");
        }
        
        private static IEnumerable<T> GetStats<T>(this ITrace trace, StatName name)
        {
            foreach (var stat in trace.GetStats(name))
            {
                if (stat is not T richStat)
                    continue;
                yield return richStat;
            }     
        }
        public static IEnumerable<IStat> GetStats(this ITrace trace, StatName name)
        {
            foreach (var arg in trace)
            {
                if (arg is not IStatMap map)
                    continue;
                if (map.TryGet(name, out var stat))
                    yield return stat;
            }
        }

        [Pure]
        public static T GetVariable<T>(this ITrace trace, string id)
        {
            foreach (var arg in trace)
            {
                if (arg is not IVariable<T> variable)
                    continue;
                if (variable.ID != id)
                    continue;
                return variable.Value;
            }

            throw new NullReferenceException($"Variable with ID {id} is not presented in trace");
        }

		public static bool TryGetVariable<T>(this ITrace trace, string id, out T res)
		{
			foreach (var arg in trace)
			{
				if (arg is not IVariable<T> variable)
					continue;
				if (variable.ID != id)
					continue;

				res = variable.Value;
				return true;
			}

			res = default;
			return false;
		}

		public static void SetVariable<T>(this ITrace trace, string id, T value)
        {
            foreach (var arg in trace)
            {
                if (arg is not MutableVariable<T> variable)
                    continue;
                if (variable.ID != id)
                    continue;
                variable.Value = value;
                return;
            }
            
            throw new NullReferenceException($"MutableVariable with ID {id} is not presented in trace");
        }

        public static void ConnectToClosest(this ITrace trace, IDisposable subscription)
        {
            trace.Get<IBranch>().Connect(subscription);
        }
    }
}