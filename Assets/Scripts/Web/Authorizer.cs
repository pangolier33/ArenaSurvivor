using Bones.Gameplay.Meta;
using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Meta.Equipment;
using Bones.Gameplay.Saving;
using Bones.Gameplay.Saving.Firebase;
using Bones.Gameplay.Saving.Local;
using Bones.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Bones.Web
{
	public class Authorizer : MonoBehaviour
	{
		[SerializeField] private TMP_Text _message;
		[SerializeField] private Transform _authorizePanel;
		[SerializeField] private Button _closeButton;
		[SerializeField] private LevelSelect _levelSelect;

		[Inject] private FirebaseClient _firebaseClient;
		[Inject] private ICurrencyStorage _currencyStorage;
		[Inject] private ICharacterClassStorage _characterClassStorage;
		[Inject] private IEquipmentStorage _equipmentStorage;
		[Inject] private Level _level;
		[Inject] private TonrareClient _client;
		[Inject] private ILoader _loader;

		private void Start()
		{
			if (!_client.Authorized)
			{
				StartCoroutine(Authorize());
				//StartCoroutine(Load());
				_closeButton.onClick.AddListener(() => _authorizePanel.gameObject.SetActive(false));
			}
			else
			{
				_levelSelect.CustomInitialize();
				_authorizePanel.gameObject.SetActive(false);
			}
		}

		private IEnumerator Load()
		{
			_firebaseClient.UserId = "1042456061";
			yield return _firebaseClient.GetSave();
			if (_firebaseClient.ResponseCode == 200)
			{
				_message.text = $"Save data loaded.";
				ApplySave(_firebaseClient.Save.fields.json.stringValue);
			}
			_closeButton.gameObject.SetActive(true);
			_client.Authorized = true;
			_levelSelect.CustomInitialize();
			_loader.Loaded = true;
		}

	private IEnumerator Authorize()
		{
			yield return _client.Authorize();
			if (_client.ResponseCode == 200)
			{
				_message.text = "Authorization sucess. Getting user.";
				StartCoroutine(GetUser());
			}
			else
			{
				_message.text = $"Authorization failed. Code: {_client.ResponseCode}. {_client.Message}";
			}
		}

		private IEnumerator GetUser()
		{
			yield return _client.GetUser();
			if (_client.ResponseCode == 200)
			{
				_message.text = $"User data received. Loading save.";
				_closeButton.gameObject.SetActive(true);
				_firebaseClient.UserId = _client.User.id;
				_client.Authorized = true;
				yield return _firebaseClient.GetSave();
				if (_firebaseClient.ResponseCode == 200)
				{
					_message.text = $"Save data loaded.";
					ApplySave(_firebaseClient.Save.fields.json.stringValue);
					_levelSelect.CustomInitialize();
					_loader.Loaded = true;
				}
				else if (_firebaseClient.ResponseCode == 404)
				{
					_message.text = "Saved data not found.";
					_loader.Loaded = true;
					_levelSelect.CustomInitialize();
				}
				else
					_message.text = $"Failed. Code: {_firebaseClient.ResponseCode}. {_firebaseClient.Message}";
			}		
			else
			{
				_message.text = $"Getting user failed. Code: {_client.ResponseCode}. {_client.Message}";
			}
		}

		private void ApplySave(string json)
		{
			var save = JsonUtility.FromJson<LocalSave>(json);
			_currencyStorage.ApplySave(save.CurrencySave);
			_characterClassStorage.ApplySave(save.CharacterClassStorageSave);
			_equipmentStorage.ApplySave(save.EquipmentStorageSave);
			_level.ApplySave(save.SettingsSave?.CurrentLevel ?? 0, save.SettingsSave?.MaxUnlockedLevel ?? 0);
			_loader.Loaded = true;
		}
	}
}