#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

#pragma warning disable IDE0090 // Use 'new(...)'

namespace EXT.Editor {

	public enum EXTWindowStyle {
		Normal,
		FramelessHidden,
		FramelessInset,
	}

	public class EXTEditor : EditorWindow {
		
		[MenuItem("Window/EXT")]
		public static void Build() {
			EXTEditor window = GetWindow<EXTEditor>();
			Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Packages/store.ext/Editor/EXTWindowIcon.png");
			if (icon == null) {
				Debug.LogWarning("Failed to find EXT Window icon.");
				window.titleContent = new GUIContent("EXT Framework");
			} else {
				window.titleContent = new GUIContent(" EXT Framework", icon);
			}
		}

		// Tab elements
		private Foldout TabFoldout;
		private Toggle TabEnabled;
		private Toggle TabMutable;
		private Toggle TabMuted;
		private TextField TabTitle;
		private ObjectField TabIcon;
		private ObjectField TabIconDark;

		// Window elements
		private TextField WindowTitle;
		private ObjectField WindowIcon;
		private EnumField WindowStyle;
		private Vector2IntField WindowSize;
		private Vector2IntField WindowMinSize;
		private Vector2IntField WindowMaxSize;
		private Toggle WindowCenter;
		private Toggle WindowResizable;
		private Toggle WindowMovable;
		private Toggle WindowMinimizable;
		private Toggle WindowMaximizable;
		private Toggle WindowClosable;
		private Toggle WindowAlwaysOnTop;
		private Toggle WindowFullscreen;
		private Toggle WindowFullscreenable;
		private Toggle WindowSkipTaskbar;
		private Toggle WindowLockAspect;

		// Webview elements
		private Toggle WebviewLoadEnabled;
		private ObjectField WebviewLoadIcon;
		private Toggle WebviewPersist;
		private Toggle WebviewDevTools;

		// Manifest elements
		private TextField ManifestName;
		private TextField ManifestVersion;
		private TextField ManifestDescription;
		private TextField ManifestAuthor;
		private TextField ManifestHomepage;
		private ObjectField ManifestIcon;
		private ObjectField ManifestBackground;
		private ObjectField ManifestBanner;
		private ObjectField ManifestIconDark;
		private ObjectField ManifestBackgroundDark;
		private ObjectField ManifestBannerDark;

		// Build elements
		private TextField BuildName;
		private Button BuildDefaults;
		private Button BuildDir;
		private Button BuildZip;

		private void CreateGUI() {

			// Get root element
			VisualElement root = rootVisualElement;
			root.Clear();

			// Import UXML
			VisualTreeAsset treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/store.ext/Editor/EXTEditor.uxml");
			if (treeAsset == null) {
				root.Add(new Label("Failed to load UXML"));
				return;
			} else {
				TemplateContainer tree = treeAsset.Instantiate();
				root.Add(tree);
			}

			// Tab Configuration

			TabFoldout = root.Query<Foldout>("TabFoldout");
			if (TabFoldout == null) Debug.LogError("TabFoldout not found");
			
			TabEnabled = root.Query<Toggle>("TabEnabled");
			TabEnabled?.RegisterCallback<ChangeEvent<bool>>(e => OnChangeTabEnabled(e));
			if (TabEnabled != null) TabEnabled.value = GetTabEnabled();
			if (TabEnabled == null) Debug.LogError("TabEnabled not found");
			
			TabMutable = root.Query<Toggle>("TabMutable");
			TabMutable?.RegisterCallback<ChangeEvent<bool>>(e => OnChangeTabMutable(e));
			if (TabMutable != null) TabMutable.value = GetTabMutable();
			if (TabMutable == null) Debug.LogError("TabMutable not found");
			
			TabMuted = root.Query<Toggle>("TabMuted");
			TabMuted?.RegisterCallback<ChangeEvent<bool>>(e => OnChangeTabMuted(e));
			if (TabMuted != null) TabMuted.value = GetTabMuted();
			if (TabMuted == null) Debug.LogError("TabMuted not found");
			
			TabTitle = root.Query<TextField>("TabTitle");
			TabTitle?.RegisterCallback<ChangeEvent<string>>(e => OnChangeTabTitle(e));
			if (TabTitle != null) TabTitle.value = GetTabTitle();
			if (TabTitle == null) Debug.LogError("TabTitle not found");
			
			TabIcon = root.Query<ObjectField>("TabIcon");
			TabIcon?.RegisterCallback<ChangeEvent<UnityEngine.Object>>(e => OnChangeTabIcon(e));
			if (TabIcon != null) TabIcon.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetTabIcon());
			if (TabIcon == null) Debug.LogError("TabIcon not found");

			TabIconDark = root.Query<ObjectField>("TabIconDark");
			TabIconDark?.RegisterCallback<ChangeEvent<UnityEngine.Object>>(e => OnChangeTabIconDark(e));
			if (TabIconDark != null) TabIconDark.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetTabIconDark());
			if (TabIconDark == null) Debug.LogError("TabIconDark not found");

			// Window Configuration

			WindowTitle = root.Query<TextField>("WindowTitle");
			WindowTitle?.RegisterCallback<ChangeEvent<string>>(e => OnChangeWindowTitle(e));
			if (WindowTitle != null) WindowTitle.value = GetWindowTitle();
			if (WindowTitle == null) Debug.LogError("WindowTitle not found");

			WindowIcon = root.Query<ObjectField>("WindowIcon");
			WindowIcon?.RegisterCallback<ChangeEvent<UnityEngine.Object>>(e => OnChangeWindowIcon(e));
			if (WindowIcon != null) WindowIcon.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetWindowIcon());
			if (WindowIcon == null) Debug.LogError("WindowIcon not found");

			WindowStyle = root.Query<EnumField>("WindowStyle");
			WindowStyle?.Init(WindowStyleDefault);
			WindowStyle?.RegisterCallback<ChangeEvent<EXTWindowStyle>>(e => OnChangeWindowStyle(e));
			if (WindowStyle != null) WindowStyle.value = GetWindowStyle();
			if (WindowStyle == null) Debug.LogError("WindowStyle not found");

			WindowSize = root.Query<Vector2IntField>("WindowSize");
			WindowSize?.RegisterCallback<ChangeEvent<Vector2Int>>(e => OnChangeWindowSize(e));
			if (WindowSize != null) WindowSize.value = GetWindowSize();
			if (WindowSize == null) Debug.LogError("WindowSize not found");

			WindowMinSize = root.Query<Vector2IntField>("WindowMinSize");
			WindowMinSize?.RegisterCallback<ChangeEvent<Vector2Int>>(e => OnChangeWindowMinSize(e));
			if (WindowMinSize != null) WindowMinSize.value = GetWindowMinSize();
			if (WindowMinSize == null) Debug.LogError("WindowMinSize not found");

			WindowMaxSize = root.Query<Vector2IntField>("WindowMaxSize");
			WindowMaxSize?.RegisterCallback<ChangeEvent<Vector2Int>>(e => OnChangeWindowMaxSize(e));
			if (WindowMaxSize != null) WindowMaxSize.value = GetWindowMaxSize();
			if (WindowMaxSize == null) Debug.LogError("WindowMaxSize not found");

			WindowCenter = root.Query<Toggle>("WindowCenter");
			WindowCenter?.RegisterCallback<ChangeEvent<bool>>(e => OnChangeWindowCenter(e));
			if (WindowCenter != null) WindowCenter.value = GetWindowCenter();
			if (WindowCenter == null) Debug.LogError("WindowCenter not found");

			WindowResizable = root.Query<Toggle>("WindowResizable");
			WindowResizable?.RegisterCallback<ChangeEvent<bool>>(e => OnChangeWindowResizable(e));
			if (WindowResizable != null) WindowResizable.value = GetWindowResizable();
			if (WindowResizable == null) Debug.LogError("WindowResizable not found");

			WindowMovable = root.Query<Toggle>("WindowMovable");
			WindowMovable?.RegisterCallback<ChangeEvent<bool>>(e => OnChangeWindowMovable(e));
			if (WindowMovable != null) WindowMovable.value = GetWindowMovable();
			if (WindowMovable == null) Debug.LogError("WindowMovable not found");

			WindowMinimizable = root.Query<Toggle>("WindowMinimizable");
			WindowMinimizable?.RegisterCallback<ChangeEvent<bool>>(e => OnChangeWindowMinimizable(e));
			if (WindowMinimizable != null) WindowMinimizable.value = GetWindowMinimizable();
			if (WindowMinimizable == null) Debug.LogError("WindowMinimizable not found");

			WindowMaximizable = root.Query<Toggle>("WindowMaximizable");
			WindowMaximizable?.RegisterCallback<ChangeEvent<bool>>(e => OnChangeWindowMaximizable(e));
			if (WindowMaximizable != null) WindowMaximizable.value = GetWindowMaximizable();
			if (WindowMaximizable == null) Debug.LogError("WindowMaximizable not found");

			WindowClosable = root.Query<Toggle>("WindowClosable");
			WindowClosable?.RegisterCallback<ChangeEvent<bool>>(e => OnChangeWindowClosable(e));
			if (WindowClosable != null) WindowClosable.value = GetWindowClosable();
			if (WindowClosable == null) Debug.LogError("WindowClosable not found");

			WindowAlwaysOnTop = root.Query<Toggle>("WindowAlwaysOnTop");
			WindowAlwaysOnTop?.RegisterCallback<ChangeEvent<bool>>(e => OnChangeWindowAlwaysOnTop(e));
			if (WindowAlwaysOnTop != null) WindowAlwaysOnTop.value = GetWindowAlwaysOnTop();
			if (WindowAlwaysOnTop == null) Debug.LogError("WindowAlwaysOnTop not found");

			WindowFullscreen = root.Query<Toggle>("WindowFullscreen");
			WindowFullscreen?.RegisterCallback<ChangeEvent<bool>>(e => OnChangeWindowFullscreen(e));
			if (WindowFullscreen != null) WindowFullscreen.value = GetWindowFullscreen();
			if (WindowFullscreen == null) Debug.LogError("WindowFullscreen not found");

			WindowFullscreenable = root.Query<Toggle>("WindowFullscreenable");
			WindowFullscreenable?.RegisterCallback<ChangeEvent<bool>>(e => OnChangeWindowFullscreenable(e));
			if (WindowFullscreenable != null) WindowFullscreenable.value = GetWindowFullscreenable();
			if (WindowFullscreenable == null) Debug.LogError("WindowFullscreenable not found");

			WindowSkipTaskbar = root.Query<Toggle>("WindowFullscreen");
			WindowSkipTaskbar?.RegisterCallback<ChangeEvent<bool>>(e => OnChangeWindowSkipTaskbar(e));
			if (WindowSkipTaskbar != null) WindowSkipTaskbar.value = GetWindowSkipTaskbar();
			if (WindowSkipTaskbar == null) Debug.LogError("WindowSkipTaskbar not found");

			WindowLockAspect = root.Query<Toggle>("WindowLockAspect");
			WindowLockAspect?.RegisterCallback<ChangeEvent<bool>>(e => OnChangeWindowLockAspect(e));
			if (WindowLockAspect != null) WindowLockAspect.value = GetWindowLockAspect();
			if (WindowLockAspect == null) Debug.LogError("WindowLockAspect not found");

			// Webview Configuration

			WebviewLoadEnabled = root.Query<Toggle>("WebviewLoadEnabled");
			WebviewLoadEnabled?.RegisterCallback<ChangeEvent<bool>>(e => OnChangWebviewLoadEnabled(e));
			if (WebviewLoadEnabled != null) WebviewLoadEnabled.value = GetWebviewLoadEnabled();
			if (WebviewLoadEnabled == null) Debug.LogError("WebviewLoadEnabled not found");

			WebviewLoadIcon = root.Query<ObjectField>("WebviewLoadIcon");
			WebviewLoadIcon?.RegisterCallback<ChangeEvent<UnityEngine.Object>>(e => OnChangeWebviewLoadIcon(e));
			if (WebviewLoadIcon != null) WebviewLoadIcon.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetWebviewLoadIcon());
			if (WebviewLoadIcon == null) Debug.LogError("WebviewLoadIcon not found");

			WebviewPersist = root.Query<Toggle>("WebviewPersist");
			WebviewPersist?.RegisterCallback<ChangeEvent<bool>>(e => OnChangWebviewPersist(e));
			if (WebviewPersist != null) WebviewPersist.value = GetWebviewPersist();
			if (WebviewPersist == null) Debug.LogError("WebviewPersist not found");

			WebviewDevTools = root.Query<Toggle>("WebviewDevTools");
			WebviewDevTools?.RegisterCallback<ChangeEvent<bool>>(e => OnChangWebviewDevTools(e));
			if (WebviewDevTools != null) WebviewDevTools.value = GetWebviewDevTools();
			if (WebviewDevTools == null) Debug.LogError("WebviewDevTools not found");

			// Manifest Configuration

			ManifestName = root.Query<TextField>("ManifestName");
			ManifestName?.RegisterCallback<ChangeEvent<string>>(e => OnChangeManifestName(e));
			if (ManifestName != null) ManifestName.value = GetManifestName();
			if (ManifestName == null) Debug.LogError("ManifestName not found");

			ManifestVersion = root.Query<TextField>("ManifestVersion");
			ManifestVersion?.RegisterCallback<ChangeEvent<string>>(e => OnChangeManifestVersion(e));
			if (ManifestVersion != null) ManifestVersion.value = GetManifestVersion();
			if (ManifestVersion == null) Debug.LogError("ManifestVersion not found");

			ManifestDescription = root.Query<TextField>("ManifestDescription");
			ManifestDescription?.RegisterCallback<ChangeEvent<string>>(e => OnChangeManifestDescription(e));
			if (ManifestDescription != null) ManifestDescription.value = GetManifestDescription();
			if (ManifestDescription == null) Debug.LogError("ManifestDescription not found");

			ManifestAuthor = root.Query<TextField>("ManifestAuthor");
			ManifestAuthor?.RegisterCallback<ChangeEvent<string>>(e => OnChangeManifestAuthor(e));
			if (ManifestAuthor != null) ManifestAuthor.value = GetManifestAuthor();
			if (ManifestAuthor == null) Debug.LogError("ManifestAuthor not found");

			ManifestHomepage = root.Query<TextField>("ManifestHomepage");
			ManifestHomepage?.RegisterCallback<ChangeEvent<string>>(e => OnChangeManifestHomepage(e));
			if (ManifestHomepage != null) ManifestHomepage.value = GetManifestHomepage();
			if (ManifestHomepage == null) Debug.LogError("ManifestHomepage not found");

			ManifestIcon = root.Query<ObjectField>("ManifestIcon");
			ManifestIcon?.RegisterCallback<ChangeEvent<UnityEngine.Object>>(e => OnChangeManifestIcon(e));
			if (ManifestIcon != null) ManifestIcon.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestIcon());
			if (ManifestIcon == null) Debug.LogError("ManifestIcon not found");

			ManifestBackground = root.Query<ObjectField>("ManifestBackground");
			ManifestBackground?.RegisterCallback<ChangeEvent<UnityEngine.Object>>(e => OnChangeManifestBackground(e));
			if (ManifestBackground != null) ManifestBackground.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestBackground());
			if (ManifestBackground == null) Debug.LogError("ManifestBackground not found");

			ManifestBanner = root.Query<ObjectField>("ManifestBanner");
			ManifestBanner?.RegisterCallback<ChangeEvent<UnityEngine.Object>>(e => OnChangeManifestBanner(e));
			if (ManifestBanner != null) ManifestBanner.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestBanner());
			if (ManifestBanner == null) Debug.LogError("ManifestBanner not found");

			ManifestIconDark = root.Query<ObjectField>("ManifestIconDark");
			ManifestIconDark?.RegisterCallback<ChangeEvent<UnityEngine.Object>>(e => OnChangeManifestIconDark(e));
			if (ManifestIconDark != null) ManifestIconDark.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestIconDark());
			if (ManifestIconDark == null) Debug.LogError("ManifestIconDark not found");

			ManifestBackgroundDark = root.Query<ObjectField>("ManifestBackgroundDark");
			ManifestBackgroundDark?.RegisterCallback<ChangeEvent<UnityEngine.Object>>(e => OnChangeManifestBackgroundDark(e));
			if (ManifestBackgroundDark != null) ManifestBackgroundDark.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestBackgroundDark());
			if (ManifestBackgroundDark == null) Debug.LogError("ManifestBackgroundDark not found");

			ManifestBannerDark = root.Query<ObjectField>("ManifestBannerDark");
			ManifestBannerDark?.RegisterCallback<ChangeEvent<UnityEngine.Object>>(e => OnChangeManifestBannerDark(e));
			if (ManifestBannerDark != null) ManifestBannerDark.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestBannerDark());
			if (ManifestBannerDark == null) Debug.LogError("ManifestBannerDark not found");

			// Build

			BuildName = root.Query<TextField>("BuildName");
			BuildName?.RegisterCallback<ChangeEvent<string>>(e => OnChangeBuildName(e));
			if (BuildName != null) BuildName.value = GetBuildName();
			if (BuildName == null) Debug.LogError("BuildName not found");

			BuildDefaults = root.Query<Button>("BuildDefaults");
			if (BuildDefaults != null) BuildDefaults.clicked += OnClickDefaults;
			if (BuildDefaults == null) Debug.LogError("BuildDefaults not found");

			BuildDir = root.Query<Button>("BuildDir");
			if (BuildDir != null) BuildDir.clicked += OnClickBuildDir;
			if (BuildDir == null) Debug.LogError("BuildDir not found");

			BuildZip = root.Query<Button>("BuildZip");
			if (BuildZip != null) BuildZip.clicked += OnClickBuildZip;
			if (BuildZip == null) Debug.LogError("BuildZip not found");

		}

		private bool EditorGetBool(string key, bool defaultValue) {
			string hash = Application.dataPath.GetHashCode().ToString();
			return EditorPrefs.GetBool("EXT." + hash + "." + key, defaultValue);
		}

		private void EditorSetBool(string key, bool value) {
			string hash = Application.dataPath.GetHashCode().ToString();
			EditorPrefs.SetBool("EXT." + hash + "." + key, value);
		}

		private string EditorGetString(string key, string defaultValue) {
			string hash = Application.dataPath.GetHashCode().ToString();
			return EditorPrefs.GetString("EXT." + hash + "." + key, defaultValue);
		}

		private void EditorSetString(string key, string value) {
			string hash = Application.dataPath.GetHashCode().ToString();
			EditorPrefs.SetString("EXT." + hash + "." + key, value);
		}

		private int EditorGetInt(string key, int defaultValue) {
			string hash = Application.dataPath.GetHashCode().ToString();
			return EditorPrefs.GetInt("EXT." + hash + "." + key, defaultValue);
		}

		private void EditorSetInt(string key, int value) {
			string hash = Application.dataPath.GetHashCode().ToString();
			EditorPrefs.SetInt("EXT." + hash + "." + key, value);
		}

		private void EditorDeleteKey(string key) {
			string hash = Application.dataPath.GetHashCode().ToString();
			EditorPrefs.DeleteKey("EXT." + hash + "." + key);
		}

		private bool GetTabEnabled() => EditorGetBool("TabEnabled", true);
		private void OnChangeTabEnabled(ChangeEvent<bool> e) => EditorSetBool("TabEnabled", e.newValue);
		
		private bool GetTabMutable() => EditorGetBool("TabMutable", true);
		private void OnChangeTabMutable(ChangeEvent<bool> e) => EditorSetBool("TabMutable", e.newValue);
		
		private bool GetTabMuted() => EditorGetBool("TabMuted", false);
		private void OnChangeTabMuted(ChangeEvent<bool> e) => EditorSetBool("TabMuted", e.newValue);

		private string GetTabTitle() => EditorGetString("TabTitle", Application.productName);
		private void OnChangeTabTitle(ChangeEvent<string> e) {
			EditorSetString("TabTitle", e.newValue);
			TabTitle.value = GetTabTitle();
		}
		
		private const string TabIconDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-128.png";
		private string GetTabIcon() => EditorGetString("TabIcon", TabIconDefault);
		private void OnChangeTabIcon(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EditorDeleteKey("TabIcon");
			if (e.newValue != null) EditorSetString("TabIcon", AssetDatabase.GetAssetPath(e.newValue));
			TabIcon.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetTabIcon());
		}

		private const string TabIconDarkDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-128-dark.png";
		private string GetTabIconDark() => EditorGetString("TabIconDark", TabIconDarkDefault);
		private void OnChangeTabIconDark(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EditorDeleteKey("TabIconDark");
			if (e.newValue != null) EditorSetString("TabIconDark", AssetDatabase.GetAssetPath(e.newValue));
			TabIconDark.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetTabIconDark());
		}

		private string GetWindowTitle() => EditorGetString("WindowTitle", Application.productName);
		private void OnChangeWindowTitle(ChangeEvent<string> e) {
			EditorSetString("WindowTitle", e.newValue);
			WindowTitle.value = GetWindowTitle();
		}

		private const string WindowIconDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-128.png";
		private string GetWindowIcon() => EditorGetString("WindowIcon", WindowIconDefault);
		private void OnChangeWindowIcon(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EditorDeleteKey("WindowIcon");
			if (e.newValue != null) EditorSetString("WindowIcon", AssetDatabase.GetAssetPath(e.newValue));
			WindowIcon.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetWindowIcon());
		}

		private const EXTWindowStyle WindowStyleDefault = EXTWindowStyle.Normal;
		private EXTWindowStyle GetWindowStyle() => Enum.Parse<EXTWindowStyle>(EditorGetString("WindowStyle", WindowStyleDefault.ToString()));
		private void OnChangeWindowStyle(ChangeEvent<EXTWindowStyle> e) => EditorSetString("WindowStyle", e.newValue.ToString());

		private Vector2Int GetWindowSize() => new Vector2Int(EditorGetInt("WindowSizeX", 800), EditorGetInt("WindowSizeY", 600));
		private void OnChangeWindowSize(ChangeEvent<Vector2Int> e) {
			EditorSetInt("WindowSizeX", e.newValue.x);
			EditorSetInt("WindowSizeY", e.newValue.y);
		}

		private Vector2Int GetWindowMinSize() => new Vector2Int(EditorGetInt("WindowMinSizeX", 0), EditorGetInt("WindowMinSizeY", 0));
		private void OnChangeWindowMinSize(ChangeEvent<Vector2Int> e) {
			EditorSetInt("WindowMinSizeX", e.newValue.x);
			EditorSetInt("WindowMinSizeY", e.newValue.y);
		}

		private Vector2Int GetWindowMaxSize() => new Vector2Int(EditorGetInt("WindowMaxSizeX", 0), EditorGetInt("WindowMaxSizeY", 0));
		private void OnChangeWindowMaxSize(ChangeEvent<Vector2Int> e) {
			EditorSetInt("WindowMaxSizeX", e.newValue.x);
			EditorSetInt("WindowMaxSizeY", e.newValue.y);
		}

		private bool GetWindowCenter() => EditorGetBool("WindowCenter", false);
		private void OnChangeWindowCenter(ChangeEvent<bool> e) => EditorSetBool("WindowCenter", e.newValue);

		private bool GetWindowResizable() => EditorGetBool("WindowResizable", true);
		private void OnChangeWindowResizable(ChangeEvent<bool> e) => EditorSetBool("WindowResizable", e.newValue);

		private bool GetWindowMovable() => EditorGetBool("WindowMovable", true);
		private void OnChangeWindowMovable(ChangeEvent<bool> e) => EditorSetBool("WindowMovable", e.newValue);

		private bool GetWindowMinimizable() => EditorGetBool("WindowMinimizable", true);
		private void OnChangeWindowMinimizable(ChangeEvent<bool> e) => EditorSetBool("WindowMinimizable", e.newValue);

		private bool GetWindowMaximizable() => EditorGetBool("WindowMaximizable", true);
		private void OnChangeWindowMaximizable(ChangeEvent<bool> e) => EditorSetBool("WindowMaximizable", e.newValue);

		private bool GetWindowClosable() => EditorGetBool("WindowClosable", true);
		private void OnChangeWindowClosable(ChangeEvent<bool> e) => EditorSetBool("WindowClosable", e.newValue);

		private bool GetWindowAlwaysOnTop() => EditorGetBool("WindowAlwaysOnTop", false);
		private void OnChangeWindowAlwaysOnTop(ChangeEvent<bool> e) => EditorSetBool("WindowAlwaysOnTop", e.newValue);

		private bool GetWindowFullscreen() => EditorGetBool("WindowFullscreen", false);
		private void OnChangeWindowFullscreen(ChangeEvent<bool> e) => EditorSetBool("WindowFullscreen", e.newValue);

		private bool GetWindowFullscreenable() => EditorGetBool("WindowFullscreenable", true);
		private void OnChangeWindowFullscreenable(ChangeEvent<bool> e) => EditorSetBool("WindowFullscreenable", e.newValue);

		private bool GetWindowSkipTaskbar() => EditorGetBool("WindowSkipTaskbar", false);
		private void OnChangeWindowSkipTaskbar(ChangeEvent<bool> e) => EditorSetBool("WindowSkipTaskbar", e.newValue);

		private bool GetWindowLockAspect() => EditorGetBool("WindowLockAspect", false);
		private void OnChangeWindowLockAspect(ChangeEvent<bool> e) => EditorSetBool("WindowLockAspect", e.newValue);

		private bool GetWebviewLoadEnabled() => EditorGetBool("WebviewLoadEnabled", true);
		private void OnChangWebviewLoadEnabled(ChangeEvent<bool> e) => EditorSetBool("WebviewLoadEnabled", e.newValue);

		private bool IsWebviewLoadDark() => PlayerSettings.SplashScreen.unityLogoStyle == PlayerSettings.SplashScreen.UnityLogoStyle.LightOnDark;
		private string GetWebviewLoadIconDefault() => "Packages/store.ext/WebGLTemplates/EXT/ext-" + (IsWebviewLoadDark() ? "dark" : "light") + ".png";
		private string GetWebviewLoadIcon() => EditorGetString("WebviewLoadIcon", GetWebviewLoadIconDefault());
		private void OnChangeWebviewLoadIcon(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EditorDeleteKey("WebviewLoadIcon");
			if (e.newValue != null) EditorSetString("WebviewLoadIcon", AssetDatabase.GetAssetPath(e.newValue));
			WebviewLoadIcon.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetWebviewLoadIcon());
		}

		private bool GetWebviewPersist() => EditorGetBool("WebviewPersist", true);
		private void OnChangWebviewPersist(ChangeEvent<bool> e) => EditorSetBool("WebviewPersist", e.newValue);

		private bool GetWebviewDevTools() => EditorGetBool("WebviewDevTools", false);
		private void OnChangWebviewDevTools(ChangeEvent<bool> e) => EditorSetBool("WebviewDevTools", e.newValue);

		private string GetManifestName() => EditorGetString("ManifestName", Application.productName);
		private void OnChangeManifestName(ChangeEvent<string> e) {
			EditorSetString("ManifestName", e.newValue);
			ManifestName.value = GetManifestName();
		}

		private string GetManifestVersion() => EditorGetString("ManifestVersion", Application.version);
		private void OnChangeManifestVersion(ChangeEvent<string> e) {
			EditorSetString("ManifestVersion", e.newValue);
			ManifestVersion.value = GetManifestVersion();
		}

		private string GetManifestDescription() => EditorGetString("ManifestDescription", "");
		private void OnChangeManifestDescription(ChangeEvent<string> e) => EditorSetString("ManifestDescription", e.newValue);

		private string GetManifestAuthor() => EditorGetString("ManifestAuthor", Application.companyName);
		private void OnChangeManifestAuthor(ChangeEvent<string> e) => EditorSetString("ManifestAuthor", e.newValue);

		private string GetManifestHomepage() => EditorGetString("ManifestHomepage", "");
		private void OnChangeManifestHomepage(ChangeEvent<string> e) => EditorSetString("ManifestHomepage", e.newValue);

		private const string ManifestIconDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-128.png";
		private string GetManifestIcon() => EditorGetString("ManifestIcon", ManifestIconDefault);
		private void OnChangeManifestIcon(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EditorDeleteKey("ManifestIcon");
			if (e.newValue != null) EditorSetString("ManifestIcon", AssetDatabase.GetAssetPath(e.newValue));
			ManifestIcon.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestIcon());
		}

		private const string ManifestBackgroundDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-background.png";
		private string GetManifestBackground() => EditorGetString("ManifestBackground", ManifestBackgroundDefault);
		private void OnChangeManifestBackground(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EditorDeleteKey("ManifestBackground");
			if (e.newValue != null) EditorSetString("ManifestBackground", AssetDatabase.GetAssetPath(e.newValue));
			ManifestBackground.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestBackground());
		}

		private const string ManifestBannerDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-banner.png";
		private string GetManifestBanner() => EditorGetString("ManifestBanner", ManifestBannerDefault);
		private void OnChangeManifestBanner(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EditorDeleteKey("ManifestBanner");
			if (e.newValue != null) EditorSetString("ManifestBanner", AssetDatabase.GetAssetPath(e.newValue));
			ManifestBanner.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestBanner());
		}

		private const string ManifestIconDarkDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-128-dark.png";
		private string GetManifestIconDark() => EditorGetString("ManifestIconDark", ManifestIconDarkDefault);
		private void OnChangeManifestIconDark(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EditorDeleteKey("ManifestIconDark");
			if (e.newValue != null) EditorSetString("ManifestIconDark", AssetDatabase.GetAssetPath(e.newValue));
			ManifestIconDark.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestIconDark());
		}

		private const string ManifestBackgroundDarkDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-background-dark.png";
		private string GetManifestBackgroundDark() => EditorGetString("ManifestBackgroundDark", ManifestBackgroundDarkDefault);
		private void OnChangeManifestBackgroundDark(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EditorDeleteKey("ManifestBackgroundDark");
			if (e.newValue != null) EditorSetString("ManifestBackgroundDark", AssetDatabase.GetAssetPath(e.newValue));
			ManifestBackgroundDark.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestBackgroundDark());
		}

		private const string ManifestBannerDarkDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-banner-dark.png";
		private string GetManifestBannerDark() => EditorGetString("ManifestBannerDark", ManifestBannerDarkDefault);
		private void OnChangeManifestBannerDark(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EditorDeleteKey("ManifestBannerDark");
			if (e.newValue != null) EditorSetString("ManifestBannerDark", AssetDatabase.GetAssetPath(e.newValue));
			ManifestBannerDark.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestBannerDark());
		}

		private string GetBuildName() => EditorGetString("BuildName", Application.productName + ".zip");
		private void OnChangeBuildName(ChangeEvent<string> e) => EditorSetString("BuildName", e.newValue);

		private void OnClickDefaults() {
			EditorDeleteKey("TabEnabled");
			EditorDeleteKey("TabMutable");
			EditorDeleteKey("TabMuted");
			EditorDeleteKey("TabTitle");
			EditorDeleteKey("TabIcon");
			EditorDeleteKey("TabIconDark");
			EditorDeleteKey("WindowTitle");
			EditorDeleteKey("WindowIcon");
			EditorDeleteKey("WindowStyle");
			EditorDeleteKey("WindowSizeX");
			EditorDeleteKey("WindowSizeY");
			EditorDeleteKey("WindowMinSizeX");
			EditorDeleteKey("WindowMinSizeY");
			EditorDeleteKey("WindowMaxSizeX");
			EditorDeleteKey("WindowMaxSizeY");
			EditorDeleteKey("WindowCenter");
			EditorDeleteKey("WindowResizable");
			EditorDeleteKey("WindowMovable");
			EditorDeleteKey("WindowMinimizable");
			EditorDeleteKey("WindowMaximizable");
			EditorDeleteKey("WindowClosable");
			EditorDeleteKey("WindowAlwaysOnTop");
			EditorDeleteKey("WindowFullscreen");
			EditorDeleteKey("WindowFullscreenable");
			EditorDeleteKey("WindowSkipTaskbar");
			EditorDeleteKey("WindowLockAspect");
			EditorDeleteKey("WebviewLoadEnabled");
			EditorDeleteKey("WebviewLoadIcon");
			EditorDeleteKey("WebviewPersist");
			EditorDeleteKey("WebviewDevTools");
			EditorDeleteKey("ManifestName");
			EditorDeleteKey("ManifestVersion");
			EditorDeleteKey("ManifestDescription");
			EditorDeleteKey("ManifestAuthor");
			EditorDeleteKey("ManifestHomepage");
			EditorDeleteKey("ManifestIcon");
			EditorDeleteKey("ManifestBackground");
			EditorDeleteKey("ManifestBanner");
			EditorDeleteKey("ManifestIconDark");
			EditorDeleteKey("ManifestBackgroundDark");
			EditorDeleteKey("ManifestBannerDark");
			EditorDeleteKey("BuildName");
			CreateGUI();
		}

		private void OnClickBuildZip() {
			OnClickBuild(true);
		}

		private void OnClickBuildDir() {
			OnClickBuild(false);
		}

		private void OnClickBuild(bool zip) {
			try {

				// Get list of scenes
				List<string> scenes = new List<string>();
				foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
					if (scene.enabled && !string.IsNullOrEmpty(scene.path)) {
						scenes.Add(scene.path);
					}
				}

				// Ask for build path
				string path = EditorUtility.SaveFolderPanel("Location for EXT build", "", "");
				if (path == "") return;

				// Check if path is not empty
				if (!Directory.Exists(path)) {
					bool create = EditorUtility.DisplayDialog("Build location not found", "Selected build location does not exist. Create it?", "Yes", "Cancel");
					if (!create) return;
					Directory.CreateDirectory(path);
				} else if (Directory.GetFiles(path).Length > 0) {
					bool create = EditorUtility.DisplayDialog("Build location not empty", "Selected build location already contains files. Overwrite?", "Yes", "Cancel");
					if (!create) return;
					if (!FileUtil.DeleteFileOrDirectory(path)) throw new Exception("Failed to delete existing files.");
					Directory.CreateDirectory(path);
				}

				// Log
				Debug.Log("Building Player for EXT at: " + path);

				// Find WebGL template
				string template = null;
				if (File.Exists("Assets/WebGLTemplates/EXT/index.html")) {
					template = "PROJECT:EXT";
				} else {
					foreach (string package in Directory.GetDirectories("Library/PackageCache")) {
						string name = Path.GetFileName(package);
						string index = "Library/PackageCache/" + name + "/WebGLTemplates/EXT/index.html";
						if (File.Exists(index)) {
							template = "PROJECT:../../Library/PackageCache/" + name + "/WebGLTemplates/EXT";
							break;
						}
					}
					template ??= "PROJECT:../../Packages/store.ext";
				}

				// Build using EXT WebGL template
				string webglTemplate = PlayerSettings.WebGL.template;
				try {
					PlayerSettings.WebGL.template = template;
					BuildPipeline.BuildPlayer(scenes.ToArray(), path, BuildTarget.WebGL, BuildOptions.None);
				} finally {
					PlayerSettings.WebGL.template = webglTemplate;
				}

				// Replace default files
				ReplaceIndex(path);
				ReplaceManifest(path);
				ReplaceScript(path);

				// Create zip file
				if (zip) {

					// Create zip file
					string tmp = Path.GetTempPath();
					string zipname = GetBuildName();
					ZipFile.CreateFromDirectory(path, Path.Combine(tmp, zipname));

					// Delete build directory
					if (!FileUtil.DeleteFileOrDirectory(path)) throw new Exception("Failed to delete build files.");
					Directory.CreateDirectory(path);

					// Move zip file to build directory
					File.Move(Path.Combine(tmp, zipname), Path.Combine(path, zipname));

				}

			} catch (Exception exception) {

				// Log error
				Debug.LogError(exception);

				// Show retry dialog
				if (EditorUtility.DisplayDialog("EXT Build Error", exception.Message, "Retry", "Cancel")) {
					OnClickBuildDir();
				}

			}
		}

		private void ReplaceIndex(string path) {

			// Copy loading icon to build directory
			string iconSrc = GetWebviewLoadIcon();
			string iconDst = "load" + Path.GetExtension(iconSrc);
			FileUtil.CopyFileOrDirectory(iconSrc, Path.Combine(path, iconDst));

			// Read index file
			string filepath = Path.Combine(path, "index.html");
			string contents = File.ReadAllText(filepath);

			// Replace enabled state for loading screen
			Regex regexEnabled = new Regex("[\\t ]*const[\\t ]+configLoadEnabled[\\t ]*=[\\t ]*[^\\n]+;[\\t ]*");
			contents = regexEnabled.Replace(contents, "    const configLoadEnabled = " + (GetWebviewLoadEnabled() ? "true" : "false") + ";");

			// Replace loading icon src
			Regex regexIcon = new Regex("[\\t ]*const[\\t ]+configLoadIcon[\\t ]*=[\\t ]*[^\\n]+;[\\t ]*");
			contents = regexIcon.Replace(contents, "    const configLoadIcon = \"" + EscapeJS(iconDst) + "\";");

			// Save index file
			using (StreamWriter writer = new StreamWriter(filepath)) {
				writer.Write(contents);
			}

			// Delete default loading icons
			try {
				File.Delete(Path.Combine(path, "ext-dark.png"));
				File.Delete(Path.Combine(path, "ext-light.png"));
			} catch (Exception exception) {
				Debug.LogError(exception);
			}

		}

		private void ReplaceManifest(string path) {

			// Delete default manifest icons
			File.Delete(Path.Combine(path, "icons", "icon-128.png"));
			File.Delete(Path.Combine(path, "icons", "icon-background.png"));
			File.Delete(Path.Combine(path, "icons", "icon-banner.png"));
			File.Delete(Path.Combine(path, "icons", "icon-128-dark.png"));
			File.Delete(Path.Combine(path, "icons", "icon-background-dark.png"));
			File.Delete(Path.Combine(path, "icons", "icon-banner-dark.png"));

			// Copy manifest icon
			string iconSrc = GetManifestIcon();
			string iconDst = Path.Combine("icons", "icon-128" + Path.GetExtension(iconSrc));
			FileUtil.CopyFileOrDirectory(iconSrc, Path.Combine(path, iconDst));

			// Copy manifest background
			string backgroundSrc = GetManifestBackground();
			string backgroundDst = Path.Combine("icons", "icon-background" + Path.GetExtension(backgroundSrc));
			FileUtil.CopyFileOrDirectory(backgroundSrc, Path.Combine(path, backgroundDst));

			// Copy manifest banner
			string bannerSrc = GetManifestBanner();
			string bannerDst = Path.Combine("icons", "icon-banner" + Path.GetExtension(bannerSrc));
			FileUtil.CopyFileOrDirectory(bannerSrc, Path.Combine(path, bannerDst));

			// Copy manifest dark icon
			string iconDarkSrc = GetManifestIconDark();
			string iconDarkDst = Path.Combine("icons", "icon-128-dark" + Path.GetExtension(iconDarkSrc));
			FileUtil.CopyFileOrDirectory(iconDarkSrc, Path.Combine(path, iconDarkDst));

			// Copy manifest dark background
			string backgroundDarkSrc = GetManifestBackgroundDark();
			string backgroundDarkDst = Path.Combine("icons", "icon-background-dark" + Path.GetExtension(backgroundDarkSrc));
			FileUtil.CopyFileOrDirectory(backgroundDarkSrc, Path.Combine(path, backgroundDarkDst));

			// Copy manifest dark banner
			string bannerDarkSrc = GetManifestBannerDark();
			string bannerDarkDst = Path.Combine("icons", "icon-banner-dark" + Path.GetExtension(bannerDarkSrc));
			FileUtil.CopyFileOrDirectory(bannerDarkSrc, Path.Combine(path, bannerDarkDst));

			// Read manifest file
			string filepath = Path.Combine(path, "manifest.json");
			string contents = File.ReadAllText(filepath);

			// Replace name
			Regex regexName = new Regex("[\\t ]*\"name\":[\\t ]*\"[^\\n]*\",[\\t ]*");
			contents = regexName.Replace(contents, "\t\"name\": \"" + EscapeJSON(GetManifestName()) + "\",");

			// Replace version
			Regex regexVersion = new Regex("[\\t ]*\"version\":[\\t ]*\"[^\\n]*\",[\\t ]*");
			contents = regexVersion.Replace(contents, "\t\"version\": \"" + EscapeJSON(GetManifestVersion()) + "\",");

			// Replace description
			Regex regexDescription = new Regex("[\\t ]*\"description\":[\\t ]*\"[^\\n]*\",[\\t ]*");
			contents = regexDescription.Replace(contents, "\t\"description\": \"" + EscapeJSON(GetManifestDescription()) + "\",");

			// Replace author
			Regex regexAuthor = new Regex("[\\t ]*\"author\":[\\t ]*\"[^\\n]*\",[\\t ]*");
			contents = regexAuthor.Replace(contents, "\t\"author\": \"" + EscapeJSON(GetManifestAuthor()) + "\",");

			// Replace homepage
			Regex regexHomepage = new Regex("[\\t ]*\"homepage\":[\\t ]*\"[^\\n]*\",[\\t ]*");
			contents = regexHomepage.Replace(contents, "\t\"homepage\": \"" + EscapeJSON(GetManifestHomepage()) + "\",");

			// Save manifest file
			using StreamWriter writer = new StreamWriter(filepath);
			writer.Write(contents);

		}

		private void ReplaceScript(string path) {

			// Read script file
			string filepath = Path.Combine(path, "main.js");
			string contents = File.ReadAllText(filepath);

			// Copy tab icon
			string tabIconSrc = GetTabIcon();
			string tabIconDst = Path.Combine("icons", "tab-icon" + Path.GetExtension(tabIconSrc));
			FileUtil.CopyFileOrDirectory(tabIconSrc, Path.Combine(path, tabIconDst));

			// Copy tab dark icon
			string tabIconDarkSrc = GetTabIconDark();
			string tabIconDarkDst = Path.Combine("icons", "tab-icon-dark" + Path.GetExtension(tabIconDarkSrc));
			FileUtil.CopyFileOrDirectory(tabIconDarkSrc, Path.Combine(path, tabIconDarkDst));

			// Copy window icon
			string windowIconSrc = GetWindowIcon();
			string windowIconDst = Path.Combine("icons", "window-icon" + Path.GetExtension(windowIconSrc));
			FileUtil.CopyFileOrDirectory(windowIconSrc, Path.Combine(path, windowIconDst));

			// Replace tab values
			contents = ReplaceScriptValue(contents, "ConfigTabEnabled", GetTabEnabled() ? "true" : "false");
			contents = ReplaceScriptValue(contents, "ConfigTabMutable", GetTabMutable() ? "true" : "false");
			contents = ReplaceScriptValue(contents, "ConfigTabMuted", GetTabMuted() ? "true" : "false");
			contents = ReplaceScriptValue(contents, "ConfigTabTitle", "\"" + EscapeJS(GetTabTitle()) + "\"");
			contents = ReplaceScriptValue(contents, "ConfigTabIcon", "\"" + EscapeJS(tabIconDst) + "\"");
			contents = ReplaceScriptValue(contents, "ConfigTabIconDark", "\"" + EscapeJS(tabIconDarkDst) + "\"");

			// Replace window values
			contents = ReplaceScriptValue(contents, "ConfigWindowTitle", "\"" + EscapeJS(GetWindowTitle()) + "\"");
			contents = ReplaceScriptValue(contents, "ConfigWindowIcon", "\"" + EscapeJS(windowIconDst) + "\"");
			contents = ReplaceScriptValue(contents, "ConfigWindowFrame", GetWindowStyle() == EXTWindowStyle.Normal ? "true" : "false");
			contents = ReplaceScriptValue(contents, "ConfigWindowTitleBarStyle", GetWindowStyle() == EXTWindowStyle.FramelessHidden ? "\"hidden\"" : "\"inset\"");
			contents = ReplaceScriptValue(contents, "ConfigWindowSizeX", GetWindowSize().x > 0 ? GetWindowSize().x.ToString() : "undefined");
			contents = ReplaceScriptValue(contents, "ConfigWindowSizeY", GetWindowSize().y > 0 ? GetWindowSize().y.ToString() : "undefined");
			contents = ReplaceScriptValue(contents, "ConfigWindowMinSizeX", GetWindowMinSize().x > 0 ? GetWindowMinSize().x.ToString() : "undefined");
			contents = ReplaceScriptValue(contents, "ConfigWindowMinSizeY", GetWindowMinSize().y > 0 ? GetWindowMinSize().y.ToString() : "undefined");
			contents = ReplaceScriptValue(contents, "ConfigWindowMaxSizeX", GetWindowMaxSize().x > 0 ? GetWindowMaxSize().x.ToString() : "undefined");
			contents = ReplaceScriptValue(contents, "ConfigWindowMaxSizeY", GetWindowMaxSize().y > 0 ? GetWindowMaxSize().y.ToString() : "undefined");
			contents = ReplaceScriptValue(contents, "ConfigWindowCenter", GetWindowCenter() ? "true" : "false");
			contents = ReplaceScriptValue(contents, "ConfigWindowResizable", GetWindowResizable() ? "true" : "false");
			contents = ReplaceScriptValue(contents, "ConfigWindowMovable", GetWindowMovable() ? "true" : "false");
			contents = ReplaceScriptValue(contents, "ConfigWindowMinimizable", GetWindowMinimizable() ? "true" : "false");
			contents = ReplaceScriptValue(contents, "ConfigWindowMaximizable", GetWindowMaximizable() ? "true" : "false");
			contents = ReplaceScriptValue(contents, "ConfigWindowClosable", GetWindowClosable() ? "true" : "false");
			contents = ReplaceScriptValue(contents, "ConfigWindowAlwaysOnTop", GetWindowAlwaysOnTop() ? "true" : "false");
			contents = ReplaceScriptValue(contents, "ConfigWindowFullscreen", GetWindowFullscreen() ? "true" : "false");
			contents = ReplaceScriptValue(contents, "ConfigWindowFullscreenable", GetWindowFullscreenable() ? "true" : "false");
			contents = ReplaceScriptValue(contents, "ConfigWindowSkipTaskbar", GetWindowSkipTaskbar() ? "true" : "false");
			if (GetWindowLockAspect() && GetWindowSize().x > 0 && GetWindowSize().y > 0) {
				contents = ReplaceScriptValue(contents, "ConfigWindowAspectRatio", GetWindowSize().x.ToString() + "/" + GetWindowSize().y.ToString());
			}

			// Replace webview values
			contents = ReplaceScriptValue(contents, "ConfigWebviewPersist", GetWebviewPersist() ? "true" : "false");
			contents = ReplaceScriptValue(contents, "ConfigWebviewDevTools", GetWebviewDevTools() ? "true" : "false");

			// Save script file
			using StreamWriter writer = new StreamWriter(filepath);
			writer.Write(contents);

		}

		private string ReplaceScriptValue(string contents, string key, string value) {
			Regex regex = new Regex("[\\t ]*const[\\t ]+" + key + "[\\t ]+=[\\t ]+[^\\n]+");
			return regex.Replace(contents, "const " + key + " = " + value);
		}

		private static string EscapeJS(string text) {
			return EscapeJSON(text);
		}

		private static string EscapeJSON(string text) {
			text = text.Replace("\\", "\\\\");
			text = text.Replace("\"", "\\\"");
			text = text.Replace("\b", "\\\\b");
			text = text.Replace("\f", "\\\\f");
			text = text.Replace("\n", "\\\\n");
			text = text.Replace("\r", "\\\\r");
			text = text.Replace("\t", "\\\\t");
			text = text.Replace("/", "\\/");
			return text;
		}

	}

}

#endif
