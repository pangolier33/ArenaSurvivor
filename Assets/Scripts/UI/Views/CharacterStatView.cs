using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Views
{
	public class CharacterStatView : MonoBehaviour
	{
		[SerializeField] private StatName _name;
		[SerializeField] private TMP_Text _value;

		public StatName Name => _name;

		public void Initialize(IStat stat)
		{
			//stat
		}
	}
}