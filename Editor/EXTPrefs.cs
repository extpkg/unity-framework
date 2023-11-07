#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EXT.Editor {

	public class EXTPrefs : ScriptableObject {

		[Serializable]
		private class Record<T> {
			public string Key;
			public T Value;
		}

		[SerializeField]
		private List<Record<bool>> BoolRecords = new List<Record<bool>>();
		
		[SerializeField]
		private List<Record<string>> StringRecords = new List<Record<string>>();
		
		[SerializeField]
		private List<Record<int>> IntRecords = new List<Record<int>>();

		private static readonly string AssetPath = "Assets/EXT/Prefs.asset";
		private static bool AssetEnabled = false;

		public static void Initialize() {
			EXTPrefs instance = AssetDatabase.LoadAssetAtPath<EXTPrefs>(AssetPath);
			if (instance != null) {
				AssetEnabled = true;
				GetAsset();
			} else {
				AssetEnabled = false;
				DeleteAsset();
			}
		}

		public static string GetAssetPath() {
			return AssetPath;
		}

		public static bool IsAssetEnabled() {
			return AssetEnabled;
		}

		public static void SetAssetEnabled(bool enabled) {
			AssetEnabled = enabled;
			if (enabled) {
				GetAsset();
			} else {
				DeleteAsset();
			}
		}

		private static void DeleteAsset() {
			if (AssetDatabase.DeleteAsset(AssetPath)) {
				AssetDatabase.Refresh();
				string parent = Path.GetDirectoryName(AssetPath);
				string meta = parent + ".meta";
				if (Directory.Exists(parent) && !Directory.EnumerateFileSystemEntries(parent).Any() && File.Exists(meta)) {
					Directory.Delete(parent);
					File.Delete(meta);
				}
			}
		}

		private static EXTPrefs GetAsset() {
			EXTPrefs instance = AssetDatabase.LoadAssetAtPath<EXTPrefs>(AssetPath);
			if (instance == null) {
				instance = CreateInstance<EXTPrefs>();
				Directory.CreateDirectory(Path.GetDirectoryName(AssetPath));
				AssetDatabase.CreateAsset(instance, AssetPath);
				AssetDatabase.Refresh();
			}
			return instance;
		}

		private static string GetProjectHash() {
			return Application.dataPath.GetHashCode().ToString();
		}

		public static bool GetBool(string key, bool defaultValue) {
			if (AssetEnabled) {
				List<Record<bool>> records = GetAsset().BoolRecords;
				foreach (Record<bool> record in records) {
					if (record.Key == key) {
						return record.Value;
					}
				}
				return defaultValue;
			} else {
				return EditorPrefs.GetBool("EXT." + GetProjectHash() + "." + key, defaultValue);
			}
		}

		public static void SetBool(string key, bool value) {
			if (AssetEnabled) {
				List<Record<bool>> records = GetAsset().BoolRecords;
				foreach (Record<bool> record in records) {
					if (record.Key == key) {
						record.Value = value;
						return;
					}
				}
				records.Add(new Record<bool> { Key = key, Value = value });
			} else {
				EditorPrefs.SetBool("EXT." + GetProjectHash() + "." + key, value);
			}
		}

		public static string GetString(string key, string defaultValue) {
			if (AssetEnabled) {
				List<Record<string>> records = GetAsset().StringRecords;
				foreach (Record<string> record in records) {
					if (record.Key == key) {
						return record.Value;
					}
				}
				return defaultValue;
			} else {
				return EditorPrefs.GetString("EXT." + GetProjectHash() + "." + key, defaultValue);
			}
		}

		public static void SetString(string key, string value) {
			if (AssetEnabled) {
				List<Record<string>> records = GetAsset().StringRecords;
				foreach (Record<string> record in records) {
					if (record.Key == key) {
						record.Value = value;
						return;
					}
				}
				records.Add(new Record<string> { Key = key, Value = value });
			} else {
				EditorPrefs.SetString("EXT." + GetProjectHash() + "." + key, value);
			}
		}

		public static int GetInt(string key, int defaultValue) {
			if (AssetEnabled) {
				List<Record<int>> records = GetAsset().IntRecords;
				foreach (Record<int> record in records) {
					if (record.Key == key) {
						return record.Value;
					}
				}
				return defaultValue;
			} else {
				return EditorPrefs.GetInt("EXT." + GetProjectHash() + "." + key, defaultValue);
			}
		}

		public static void SetInt(string key, int value) {
			if (AssetEnabled) {
				List<Record<int>> records = GetAsset().IntRecords;
				foreach (Record<int> record in records) {
					if (record.Key == key) {
						record.Value = value;
						return;
					}
				}
				records.Add(new Record<int> { Key = key, Value = value });
			} else {
				EditorPrefs.SetInt("EXT." + GetProjectHash() + "." + key, value);
			}
		}

		public static void DeleteKey(string key) {
			if (AssetEnabled) {
				DeleteKeyBool(key);
				DeleteKeyString(key);
				DeleteKeyInt(key);
			} else {
				EditorPrefs.DeleteKey("EXT." + GetProjectHash() + "." + key);
			}
		}

		private static void DeleteKeyBool(string key) {
			List<Record<bool>> records = GetAsset().BoolRecords;
			foreach (Record<bool> record in records) {
				if (record.Key == key) {
					records.Remove(record);
					return;
				}
			}
		}

		private static void DeleteKeyString(string key) {
			List<Record<string>> records = GetAsset().StringRecords;
			foreach (Record<string> record in records) {
				if (record.Key == key) {
					records.Remove(record);
					return;
				}
			}
		}

		private static void DeleteKeyInt(string key) {
			List<Record<int>> records = GetAsset().IntRecords;
			foreach (Record<int> record in records) {
				if (record.Key == key) {
					records.Remove(record);
					return;
				}
			}
		}

	}

}

#endif
