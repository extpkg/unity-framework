#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

#pragma warning disable IDE0090 // Use 'new(...)'

namespace EXT.Editor {

	public class EXTPrefs : ScriptableObject {

		[Serializable]
		private class Record {
			public string Key;
			public string Value;
		}

		[SerializeField]
		private List<Record> m_Records = new List<Record>();
		
		// Seen keys for copying
		private static readonly HashSet<string> Keys = new HashSet<string>();

		// Asset configuration
		private static readonly string AssetPath = "Assets/EXT/Prefs.asset";
		private static bool AssetEnabled = false;

		public static void Initialize() {
			AssetEnabled = AssetDatabase.LoadAssetAtPath<EXTPrefs>(AssetPath) != null;
		}

		public static string GetAssetPath() {
			return AssetPath;
		}

		public static bool IsAssetEnabled() {
			return AssetEnabled;
		}

		public static void SetAssetEnabled(bool enabled) {

			// Check for changes
			if (AssetEnabled == enabled) {
				return;
			}

			// Copy records to persist them
			if (enabled) {
				CopyFromPrefs();
			} else {
				CopyFromAsset();
			}

			// Update state
			AssetEnabled = enabled;

			// Create or delete asset
			if (enabled) {
				GetAsset();
			} else {
				DeleteAsset();
			}

		}

		public static bool GetBool(string key, bool defaultValue) {
			if (!TryGetRecord(key, out string value)) return defaultValue;
			if (!bool.TryParse(value, out bool result)) return defaultValue;
			return result;
		}

		public static void SetBool(string key, bool value) {
			SetRecord(key, value.ToString());
		}

		public static string GetString(string key, string defaultValue) {
			if (!TryGetRecord(key, out string value)) return defaultValue;
			return value;
		}

		public static void SetString(string key, string value) {
			SetRecord(key, value);
		}

		public static int GetInt(string key, int defaultValue) {
			if (!TryGetRecord(key, out string value)) return defaultValue;
			if (!int.TryParse(value, out int result)) return defaultValue;
			return result;
		}

		public static void SetInt(string key, int value) {
			SetRecord(key, value.ToString());
		}

		public static void DeleteKey(string key) {
			DeleteRecord(key);
		}

		private static bool TryGetRecord(string key, out string value) {
			Keys.Add(key);
			if (AssetEnabled) {
				return TryGetRecordFromAsset(null, key, out value);
			} else {
				return TryGetRecordFromPrefs(key, out value);
			}
		}

		private static void SetRecord(string key, string value) {
			Keys.Add(key);
			if (AssetEnabled) {
				SetRecordToAsset(null, key, value, true);
			} else {
				SetRecordToPrefs(key, value);
			}
		}

		private static void DeleteRecord(string key) {
			if (AssetEnabled) {
				DeleteRecordFromAsset(null, key, true);
			} else {
				DeleteRecordFromPrefs(key);
			}
		}

		private static EXTPrefs GetAsset() {

			// Get current instance
			EXTPrefs instance = AssetDatabase.LoadAssetAtPath<EXTPrefs>(AssetPath);
			if (instance != null) {
				return instance;
			}

			// Create a new instance and save it
			instance = CreateInstance<EXTPrefs>();
			Directory.CreateDirectory(Path.GetDirectoryName(AssetPath));
			AssetDatabase.CreateAsset(instance, AssetPath);
			AssetDatabase.Refresh();

			// Load saved instance
			instance = AssetDatabase.LoadAssetAtPath<EXTPrefs>(AssetPath);
			if (instance == null) throw new Exception("Failed to create EXT configuration asset.");

			// Success
			return instance;

		}

		private static void DeleteAsset() {

			// Delete asset
			if (!AssetDatabase.DeleteAsset(AssetPath)) {
				return;
			}

			// Refresh database
			AssetDatabase.Refresh();

			// Delete asset directory if empty
			string parent = Path.GetDirectoryName(AssetPath);
			string meta = parent + ".meta";
			if (Directory.Exists(parent) && !Directory.EnumerateFileSystemEntries(parent).Any() && File.Exists(meta)) {
				Directory.Delete(parent);
				File.Delete(meta);
			}

		}

		private static bool TryGetRecordFromAsset(EXTPrefs instance, string key, out string value) {
			
			// Get record
			instance =

			// Get record
			instance != null ?

			// Get record
			instance : GetAsset();
			foreach (Record record in instance.m_Records) {
				if (record.Key == key) {
					value = record.Value;
					return true;
				}
			}

			// Not found
			value = null;
			return false;

		}

		private static void SetRecordToAsset(EXTPrefs instance, string key, string value, bool update) {

			// Update existing record
			bool found = false;
			instance = instance != null ? instance : GetAsset();
			foreach (Record record in instance.m_Records) {
				if (record.Key == key) {
					record.Value = value;
					found = true;
					break;
				}
			}

			// Add a new record if not found
			if (!found) {
				instance.m_Records.Add(new Record { Key = key, Value = value });
			}
			
			// Update asset
			if (update) {
				EditorUtility.SetDirty(instance);
				AssetDatabase.SaveAssets();
			}

		}

		private static void DeleteRecordFromAsset(EXTPrefs instance, string key, bool update) {

			// Delete record
			bool found = false;
			instance = instance != null ? instance : GetAsset();
			foreach (Record record in instance.m_Records) {
				if (record.Key == key) {
					instance.m_Records.Remove(record);
					break;
				}
			}

			// Update asset
			if (found && update) {
				EditorUtility.SetDirty(instance);
				AssetDatabase.SaveAssets();
			}

		}

		private static string GetProjectKey(string key) {
			return "EXT." + Application.dataPath.GetHashCode().ToString() + "." + key;
		}

		private static bool TryGetRecordFromPrefs(string key, out string value) {
			
			// Get record
			string projectKey = GetProjectKey(key);
			if (EditorPrefs.HasKey(projectKey)) {
				value = EditorPrefs.GetString(projectKey, null);
				return value != null;
			}

			// Not found
			value = null;
			return false;
			
		}

		private static void SetRecordToPrefs(string key, string value) {
			string projectKey = GetProjectKey(key);
			EditorPrefs.SetString(projectKey, value);
		}

		private static void DeleteRecordFromPrefs(string key) {
			string projectKey = GetProjectKey(key);
			EditorPrefs.DeleteKey(projectKey);
		}

		private static void CopyFromPrefs() {

			// Get asset
			EXTPrefs instance = GetAsset();

			// Copy records from prefs to asset
			foreach (string key in Keys) {
				if (TryGetRecordFromPrefs(key, out string value)) {
					SetRecordToAsset(instance, key, value, false);
				} else {
					DeleteRecordFromAsset(instance, key, false);
				}
			}

			// Update asset
			EditorUtility.SetDirty(instance);
			AssetDatabase.SaveAssets();

		}

		private static void CopyFromAsset() {

			// Get asset
			EXTPrefs instance = GetAsset();

			// Copy records from asset to prefs
			foreach (string key in Keys) {
				if (TryGetRecordFromAsset(instance, key, out string value)) {
					SetRecordToPrefs(key, value);
				} else {
					DeleteRecordFromPrefs(key);
				}
			}

		}

	}

}

#endif
