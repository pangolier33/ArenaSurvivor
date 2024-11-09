using System;
using System.Collections.Generic;
using System.Linq;

namespace Railcar.Time.Subscriptions
{
	public class Stopwatch : IDisposable, IStopwatch
	{
		private readonly LinkedList<Token> _tokens = new();
		List<Token> _temp = new();

		public void Update(float delta)
		{
			_temp.Clear();

			foreach (var token in _tokens.TakeWhile(token => !token.IsDisposed))
				_temp.Add(token);

			foreach (var token in _temp)
				token.Callback.Invoke(delta);
		}

		public IDisposable Observe(Action<float> callback)
		{
			if (_tokens.Count == 0)
				return new Token(_tokens, callback);

			var token = _tokens.First.Value;
			if (!token.IsDisposed)
				return new Token(_tokens, callback);
			
			token.Refresh(callback);
			return token;
		}

		public void Dispose()
		{
			_tokens.Clear();
		}
		
		private class Token : IDisposable
		{
			private readonly LinkedListNode<Token> _node;

			public Token(LinkedList<Token> list, Action<float> callback)
			{
				Callback = callback;
				_node = list.AddFirst(this);
			}
		
			public Action<float> Callback { get; private set; }
			public bool IsDisposed => Callback == null;
		
			public void Refresh(Action<float> callback)
			{
				Callback = callback;

				var list = _node.List;
				list.Remove(_node);
				list.AddFirst(this);
			}
		
			public void Dispose()
			{
				var list = _node.List; 
				list.Remove(_node);
				list.AddLast(_node);

				Callback = null;
			}
		}
	}
}