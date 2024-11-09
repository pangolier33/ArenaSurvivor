using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Bones.UI
{
	public class Screen : MonoBehaviour, IInitializable
	{
		[SerializeField] private UnityEvent _onInit;
		public virtual void Initialize() { }
	}
}