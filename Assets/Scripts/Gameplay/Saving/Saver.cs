using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Saving
{
	[CreateAssetMenu]
	public class Saver : ScriptableObject
	{
		[SerializeField] bool _isEnabled = true;
		[SerializeReference, InlineProperty, HideLabel, HideReferenceObjectPicker] SaveMethod SaveMethod = new SaveMethodJSON();
		[ShowInInspector, NonSerialized] GuidToSaveDataDictionary Saved = new();
		[ShowInInspector] private Dictionary<SerializedGuid, ISaveable> savable;

		[Button] public void Save()
		{
			if (!_isEnabled) return;

			var saves = Resources.FindObjectsOfTypeAll<Object>().OfType<ISaveable>().Where(s => Application.IsPlaying(s.gameObject));
			foreach (ISaveable save in saves)
			{
				ISaveData saved = save.Save();
				Saved[saved.Guid] = saved;
			}
			SaveMethod.Save(Saved);

			SetDirty();
		}

		[Button] public void Load()
		{
			if (!_isEnabled) return;
			SaveMethod.Load(ref Saved);
			savable = Resources.FindObjectsOfTypeAll<Object>()
				//.Select(Application.IsPlaying)
				.OfType<ISaveable>().Where(s => s.gameObject.scene.IsValid())
				.ToDictionary(value =>
				{
					//Debug.Log($"{value.gameObject.GetInstanceID()} | {value.Data.Guid}", value.gameObject);
					return value.Data.Guid;
				});

			//Debug.Log(saves.Count);

			foreach (var kvp in Saved)
			{
				if (savable.TryGetValue(kvp.Key, out ISaveable saved))
				{
					saved.Load(kvp.Value);
				}
			}
		}

		[Button] public void Clear()
		{
			SaveMethod.Clear();
			Saved.Clear();
		}

		new void SetDirty()
		{
			#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
			#endif
		}
	}

	[Serializable]
	public abstract class SaveMethod
	{
		public abstract void Save(GuidToSaveDataDictionary data);
		public abstract void Load(ref GuidToSaveDataDictionary data);
		public abstract void Clear();
	}

	[Serializable]
	public abstract class SaveMethodToFile : SaveMethod
	{
		protected string FilePath => Path.GetFullPath(FileName, Application.persistentDataPath);
		[SerializeField] protected string FileName = "Save.save";
		public override void Clear() => File.Delete(FilePath);
		[Button] public void Open() => System.Diagnostics.Process.Start(FilePath);
		[Button] public void OpenFolder() => System.Diagnostics.Process.Start(Path.GetDirectoryName(FilePath));
	}

	[Serializable]
	public class SaveMethodJSON : SaveMethodToFile
	{
		[SerializeField] bool _isPrettyPrint = true;

		public override void Save(GuidToSaveDataDictionary data)
		{
			string contents = JsonUtility.ToJson(data, _isPrettyPrint);
			File.WriteAllText(FilePath, contents);
		}

		public override void Load(ref GuidToSaveDataDictionary data)
		{
			if (File.Exists(FilePath))
			{
				string contents = File.ReadAllText(FilePath);
				JsonUtility.FromJsonOverwrite(contents, data);
			}
		}
	}

	// [Serializable]
	// public class SaveMethodBinary : SaveMethodToFile
	// {
	// 	[SerializeField] private bool _isPrettyPrint = true;
	// 	public override void Save(GuidToSaveDataDictionary data)
	// 	{
	// 		BinaryFormatter formater = new BinaryFormatter();
	// 		formater.AssemblyFormat = FormatterAssemblyStyle.Full;
	// 		MemoryStream mstream = new MemoryStream();
	// 		formater.Serialize(mstream, data);
	// 		File.WriteAllBytes(FilePath, mstream.ToArray());
	// 	}
	// 	public override void Load(ref GuidToSaveDataDictionary data)
	// 	{
	// 		var mstream = new FileStream(FilePath, FileMode.Open);
	// 		BinaryFormatter formater = new BinaryFormatter();
	// 		formater.AssemblyFormat = FormatterAssemblyStyle.Full;
	// 		data = formater.Deserialize(mstream) as GuidToSaveDataDictionary;
	// 	}
	// }

	[Serializable, InlineProperty]
	public struct SerializedGuid : IEquatable<SerializedGuid>, ISerializationCallbackReceiver
	{
		[SerializeField, HideInInspector] string _value;
		[ShowInInspector, HideLabel] public Guid Guid { get; set; }

		public bool Equals(SerializedGuid other) => Guid.Equals(other.Guid);

		public static SerializedGuid NewGuid() => new() { Guid = Guid.NewGuid() };

		public override string ToString() => Guid.ToString();
		public override int GetHashCode() => Guid.GetHashCode();
		public void OnBeforeSerialize() => _value = Guid.ToString();

		public void OnAfterDeserialize()
		{
			if (Guid.TryParse(_value, out Guid temp))
			{
				Guid = temp;
			}
			else
			{
				Debug.LogError($"can't deserialize Guid {_value}");
			}
		}
	}

	public abstract class SaveData : ISaveData
	{
		[field: SerializeField] public SerializedGuid Guid { get; set; } = SerializedGuid.NewGuid();
	}

	public interface ISaveData
	{
		SerializedGuid Guid { get; set; }
	}

	[Serializable, HideReferenceObjectPicker]
	public struct TransformSaveData : ISaveData
	{
		public static TransformSaveData Default = new() { Guid = SerializedGuid.NewGuid(), localScale = Vector3.one };
		[field: SerializeField] public SerializedGuid Guid { get; set; }
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 localScale;

		public TransformSaveData(Transform transform)
		{
			Guid = SerializedGuid.NewGuid();
			position = transform.position;
			rotation = transform.rotation;
			localScale = transform.localScale;
		}

		public TransformSaveData(Transform transform, SerializedGuid guid)
		{
			Guid = guid;
			position = transform.position;
			rotation = transform.rotation;
			localScale = transform.localScale;
		}

		public void Load(Transform transform)
		{
			transform.position = position;
			transform.rotation = rotation;
			transform.localScale = localScale;
		}
	}

	public interface IChildSaveable
	{
		ISaveData Data { get; }
		ISaveData Save();
		void Load(ISaveData data);
	}

	public interface ISaveable
	{
		GameObject gameObject { get; }
		ISaveData Data { get; }
		ISaveData Save();
		void Load(ISaveData data);
	}

	[Serializable, DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, IsReadOnly = true)]
	public class GuidToSaveDataDictionary : SerializedDictionary<SerializedGuid, ISaveData, string, ISaveData>, ISaveData
	{
		[field: SerializeField, PropertyOrder(-1)] public SerializedGuid Guid { get; set; } = SerializedGuid.NewGuid();

		public override string SerializeKey(SerializedGuid key) => key.Guid.ToString();
		public override ISaveData SerializeValue(ISaveData value) => value;
		public override ISaveData DeserializeValue(ISaveData serializedValue) => serializedValue;

		public override SerializedGuid DeserializeKey(string serializedKey)
		{
			if (System.Guid.TryParse(serializedKey, out Guid temp))
			{
				return new SerializedGuid { Guid = temp };
			}

			Debug.LogError($"can't deserialize Guid {serializedKey}");
			return default;
		}
	}

	/// <summary>
	/// Unity can't serialize Dictionary so here's a custom wrapper that does. Note that you have to
	/// extend it before it can be serialized as Unity won't serialized generic-based types either.
	/// </summary>
	/// <typeparam name="K">The key type</typeparam>
	/// <typeparam name="V">The value</typeparam>
	/// <example>
	/// public sealed class MyDictionary : SerializedDictionary&lt;KeyType, ValueType&gt; {}
	/// </example>
	[Serializable]
	public class SerializedDictionary<K, V> : SerializedDictionary<K, V, K, V>
	{
		/// <summary>
		/// Conversion to serialize a key
		/// </summary>
		/// <param name="key">The key to serialize</param>
		/// <returns>The Key that has been serialized</returns>
		public override K SerializeKey(K key) => key;

		/// <summary>
		/// Conversion to serialize a value
		/// </summary>
		/// <param name="val">The value</param>
		/// <returns>The value</returns>
		public override V SerializeValue(V val) => val;

		/// <summary>
		/// Conversion to serialize a key
		/// </summary>
		/// <param name="key">The key to serialize</param>
		/// <returns>The Key that has been serialized</returns>
		public override K DeserializeKey(K key) => key;

		/// <summary>
		/// Conversion to serialize a value
		/// </summary>
		/// <param name="val">The value</param>
		/// <returns>The value</returns>
		public override V DeserializeValue(V val) => val;
	}

	/// <summary>
	/// Dictionary that can serialize keys and values as other types
	/// </summary>
	/// <typeparam name="K">The key type</typeparam>
	/// <typeparam name="V">The value type</typeparam>
	/// <typeparam name="SK">The type which the key will be serialized for</typeparam>
	/// <typeparam name="SV">The type which the value will be serialized for</typeparam>
	[Serializable]
	public abstract class SerializedDictionary<K, V, SK, SV> : Dictionary<K, V>, ISerializationCallbackReceiver
	{
		[SerializeField] protected List<SK> m_Keys = new List<SK>();

		[SerializeReference] protected List<SV> m_Values = new List<SV>();

		/// <summary>
		/// From <see cref="K"/> to <see cref="SK"/>
		/// </summary>
		/// <param name="key">They key in <see cref="K"/></param>
		/// <returns>The key in <see cref="SK"/></returns>
		public abstract SK SerializeKey(K key);

		/// <summary>
		/// From <see cref="V"/> to <see cref="SV"/>
		/// </summary>
		/// <param name="value">The value in <see cref="V"/></param>
		/// <returns>The value in <see cref="SV"/></returns>
		public abstract SV SerializeValue(V value);

		/// <summary>
		/// From <see cref="SK"/> to <see cref="K"/>
		/// </summary>
		/// <param name="serializedKey">They key in <see cref="SK"/></param>
		/// <returns>The key in <see cref="K"/></returns>
		public abstract K DeserializeKey(SK serializedKey);

		/// <summary>
		/// From <see cref="SV"/> to <see cref="V"/>
		/// </summary>
		/// <param name="serializedValue">The value in <see cref="SV"/></param>
		/// <returns>The value in <see cref="V"/></returns>
		public abstract V DeserializeValue(SV serializedValue);

		/// <summary>
		/// OnBeforeSerialize implementation.
		/// </summary>
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			m_Keys.Clear();
			m_Values.Clear();

			foreach (var kvp in this)
			{
				m_Keys.Add(SerializeKey(kvp.Key));
				m_Values.Add(SerializeValue(kvp.Value));
			}
		}

		/// <summary>
		/// OnAfterDeserialize implementation.
		/// </summary>
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			Clear();
			for (int i = 0; i < m_Keys.Count; i++)
			{
				var key = DeserializeKey(m_Keys[i]);
				if (!ContainsKey(key))
				{
					Add(key, DeserializeValue(m_Values[i]));
				}
				else
				{
					Debug.LogError($"key duplicated {m_Keys[i]} {key}");
				}
			}
		}
	}
}