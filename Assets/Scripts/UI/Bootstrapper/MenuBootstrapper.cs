using System;
using System.Collections.Generic;
using System.Linq;
using Mitaywalle.ProceduralUI;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Bones.UI
{
	[RequireComponent(typeof(SwitchGo))]
	public class MenuBootstrapper : UIBootstrapper, IInitializable
	{
		[SerializeField] private bool _logs;
		[SerializeField] private ToggleGroup _navigation;
		private SwitchGo _screenSwitch;
		[ShowInInspector] private Dictionary<Type, Screen> _screens = new();

		public void Initialize()
		{
			Application.targetFrameRate = 120;

			Log("init menu");
			_screenSwitch = GetComponent<SwitchGo>();
			_screenSwitch.Targets.Clear();
			_screenSwitch.Targets.AddRange(GetComponentsInChildren<Screen>(true).Select(s => s.gameObject));
			_screenSwitch.ShowAll();
			_screens = GetComponentsInChildren<Screen>(true).Where(s => s.GetType() != typeof(Screen)).ToDictionary(screen => screen.GetType());
			foreach (var viewModel in GetComponentsInChildren<IInitializable>())
			{
				Container.Inject(viewModel);
			}
			GetComponentsInChildren<IInitializable>(true).ForEach(i =>
			{
				if (i != this)
				{
					i.Initialize();
				}
			});

			_navigation.allowSwitchOff = true;
			_navigation.SetAllTogglesOff();
			_navigation.GetComponentsInChildren<Toggle>(true).ForEach(t =>
			{
				Log($"init navigation | toggle {t.name} | screen id {(eScreen)t.transform.GetSiblingIndex()}");

				t.onValueChanged.RemoveListener(ShowScreen);
				t.onValueChanged.AddListener(ShowScreen);

				void ShowScreen(bool value)
				{
					if (value)
					{
						Show((eScreen)t.transform.GetSiblingIndex());
					}
				}

				t.isOn = (eScreen)t.transform.GetSiblingIndex() == eScreen.Main;
			});

			_navigation.allowSwitchOff = false;
		}

		public void Show<T>() where T : Screen
		{
			if (_screens.TryGetValue(typeof(T), out var screen))
			{
				_screenSwitch.HideAll();
				screen.gameObject.SetActive(true);
				Log($"menu show screen | type {typeof(T).Name} | object {screen.name}");
			}
			else
			{
				UnityEngine.Debug.LogError($"Screen '{typeof(T).Name}' not found!");
			}
		}

		public void Show(eScreen screen)
		{
			_screenSwitch.ShowOnly((int)screen);
			Log($"menu show screen | id {screen} | object {_screenSwitch.Targets[(int)screen].name}");
		}

		private void Log(string message)
		{
			if (_logs) UnityEngine.Debug.Log(message);
		}
	}
}