using Bones.Gameplay.Meta;
using Bones.Gameplay.Meta.Equipment;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Bones.UI.Views
{
	public class BackpackEquipmentView : MonoBehaviour
	{
		[SerializeField] private Image _icon;
		[SerializeField] private Image _border;
		[SerializeField] private ProgressView _livetime;
		[SerializeField] private ProgressView _durability;
		[SerializeField] private ExtraRareView _extraRareView;
		[SerializeField] private Button _iconButton;
		[SerializeField] private Button _infoButton;
		[SerializeField] private Button _chooseButton;
		[SerializeField] private Transform _expand;
		
		private Canvas _canvas;
		private GraphicRaycaster _raycaster;

		public IEquipment Equipment { get; private set; }
		public bool Expanded { get; private set; }

		private void Start()
		{
			_iconButton.onClick.AddListener(() => Clicked?.Invoke(this));
			_infoButton.onClick.AddListener(() => InfoCalled?.Invoke(this));
			_chooseButton.onClick.AddListener(() => Choosed?.Invoke(this));
		}

		public event Action<BackpackEquipmentView> Clicked;
		public event Action<BackpackEquipmentView> InfoCalled;
		public event Action<BackpackEquipmentView> Choosed;

		public void Initialize(IEquipment equipment)
		{
			HideExpand();
			Equipment = equipment;
			_icon.sprite = equipment.Data.Icon;
			_border.color = RareColorDeterminator.GetColor(equipment.Rare);
			_extraRareView.Initialize(equipment.RareLevel);
			_livetime.Initialize(equipment.Livetime , 100);
			_durability.Initialize(equipment.Durability , 100);
		}

		public void ShowExpand()
		{
			Expanded = true;
			_expand.gameObject.SetActive(true);
			_canvas = gameObject.AddComponent<Canvas>();
			_canvas.overrideSorting = true;
			_canvas.sortingOrder = 2;
			_raycaster = gameObject.AddComponent<GraphicRaycaster>();
		}

		public void HideExpand()
		{
			Expanded = false;
			_expand.gameObject.SetActive(false);
			Destroy(_raycaster);
			Destroy(_canvas);
		}
	}
}