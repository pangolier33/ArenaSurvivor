using Bones.Gameplay.Meta.Equipment;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bones.UI.Views
{
	public class EquipedView : MonoBehaviour
	{
		[SerializeField] private EquipmentType _type;
		[SerializeField] private Image _icon;
		[SerializeField] private ExtraRareView _extraRareView;
		[SerializeField] private TMP_Text _level;
		[SerializeField] private ProgressView _liveTime;
		[SerializeField] private ProgressView _durability;

		public EquipmentType Type => _type;		
		public IEquipment Equipment { get; private set; }

		public void Initialize(IEquipment equipment)
		{
			Equipment = equipment;
			_icon.sprite = equipment.Data.Icon;
			_extraRareView.Initialize(equipment.RareLevel);
			_level.text = equipment.Level.ToString();
			_liveTime.Initialize(equipment.Livetime, 100);
			_durability.Initialize(equipment.Durability, 100);
		}
	}
}