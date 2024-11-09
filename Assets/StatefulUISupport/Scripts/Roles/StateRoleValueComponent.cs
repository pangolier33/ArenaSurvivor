using JetBrains.Annotations;
using StatefulUI.Runtime.Core;
using UnityEngine;

namespace StatefulUISupport.Scripts.Roles
{
	/// implementation to use with <see cref="UnityEngine.UI.Button"/>
	public class StateRoleValueComponent : MonoBehaviour
	{
		[SerializeField] private StateRole _stateRole;

		[UsedImplicitly]
		public void SetState(StatefulComponent component)
		{
			if (component)
			{
				component.SetState(_stateRole);
			}
			else
			{
				Debug.LogError("component is null!");
			}
		}
	}
}