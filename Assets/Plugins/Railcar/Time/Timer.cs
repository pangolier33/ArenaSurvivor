using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Railcar.Time.Subscriptions
{
	public class Timer : IDisposable, ITimer
	{
		private readonly LinkedListNode<SortableToken> _mid;
		private float _time;

		public Timer()
		{
			_mid = new LinkedList<SortableToken>().AddFirst(new SortableToken(null, float.NegativeInfinity));
		}

		public void Update(float time)
		{
			_time = time;
			while (_mid.Next != null)
			{
				if (_mid.Next.Value.Timestamp > _time)
					return;
				_mid.Next.Value.Callback.Invoke(_time);
				_mid.Next.Value.Dispose();
			}
		}

		public IDisposable Mark(float delay, Action<float> callback)
		{
			if (delay < 0)
				throw new InvalidOperationException("Marking time must be positive");

			if (delay == 0)
			{
				callback(_time);
				return Disposable.Empty;
			}
			var timestamp = _time + delay;
			var previous = _mid.Previous;
			if (previous == null)
				return new SortableToken(_mid.List, callback, timestamp);
			
			var token = previous.Value;
			token.Refresh(callback, timestamp);
			return token;
		}
		
		public void Dispose()
		{
			_mid.List.Remove(_mid);
		}

		private sealed class SortableToken : IDisposable
		{
			private readonly LinkedListNode<SortableToken> _node;

			public SortableToken(Action<float> callback, float timestamp)
			{
				Callback = callback;
				Timestamp = timestamp;
			}

			public SortableToken(LinkedList<SortableToken> list, Action<float> callback, float timestamp)
				: this(callback, timestamp)
			{
				_node = new(this);
				Insert(list, _node);
			}
			
			public Action<float> Callback { get; private set; }
			public float Timestamp { get; private set; }

			public void Refresh(Action<float> value, float timestamp)
			{
				Callback = value;
				Timestamp = timestamp;
			
				var list = _node.List;
				list.Remove(_node);
				Insert(list, _node);
			}

			public void Dispose()
			{
				Callback = null;
				Timestamp = float.NegativeInfinity;
				var list = _node.List; 
				list.Remove(_node);
				list.AddFirst(_node);
			}

			private static void Insert(LinkedList<SortableToken> list, LinkedListNode<SortableToken> subject)
			{
				var node = list.Last;
				while (node!.Value.Timestamp > subject.Value.Timestamp)
					node = node.Previous;
				list.AddAfter(node, subject);
			}
		}
	}
}