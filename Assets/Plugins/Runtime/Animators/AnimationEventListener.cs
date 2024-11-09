using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
namespace Plugins.Runtime.Animators
{
	[Serializable]
	public class AnimationEventData
	{
		[HideLabel] public string key;
		public UnityEvent<string> Event;

	}

	public class AnimationEventListener : MonoBehaviour
	{
		[SerializeField,ListDrawerSettings(DraggableItems = false,ListElementLabelName = "@key")] AnimationEventData[] events;
		Dictionary<string, UnityEvent<string>> eventsDict = new();

		public void SampleAnimEvent(string key)
		{
			if (eventsDict.Count != events.Length)
			{
				foreach (AnimationEventData data in events)
				{
					eventsDict.Add(data.key, data.Event);
				}
			}
			if (eventsDict.ContainsKey(key))
			{
				eventsDict[key].Invoke(key);
			}
		}
	}
}