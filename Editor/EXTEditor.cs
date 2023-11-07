#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build.Reporting;
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
		private Toggle BuildAssetEnable;
		private TextField BuildAssetName;
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

			// Initialize preferences
			EXTPrefs.Initialize();

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
			WindowStyle?.RegisterCallback<ChangeEvent<Enum>>(e => OnChangeWindowStyle(e));
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

			BuildAssetEnable = root.Query<Toggle>("BuildAssetEnable");
			BuildAssetEnable?.RegisterCallback<ChangeEvent<bool>>(e => OnChangeBuildAssetEnable(e));
			if (BuildAssetEnable != null) BuildAssetEnable.value = GetBuildAssetEnable();
			if (BuildAssetEnable == null) Debug.LogError("BuildAssetEnable not found");

			BuildAssetName = root.Query<TextField>("BuildAssetName");
			if (BuildAssetName != null) BuildAssetName.value = GetBuildAssetName();
			if (BuildAssetName == null) Debug.LogError("BuildAssetName not found");

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

		private bool GetTabEnabled() => EXTPrefs.GetBool("TabEnabled", true);
		private void OnChangeTabEnabled(ChangeEvent<bool> e) => EXTPrefs.SetBool("TabEnabled", e.newValue);
		
		private bool GetTabMutable() => EXTPrefs.GetBool("TabMutable", true);
		private void OnChangeTabMutable(ChangeEvent<bool> e) => EXTPrefs.SetBool("TabMutable", e.newValue);
		
		private bool GetTabMuted() => EXTPrefs.GetBool("TabMuted", false);
		private void OnChangeTabMuted(ChangeEvent<bool> e) => EXTPrefs.SetBool("TabMuted", e.newValue);

		private string GetTabTitle() => EXTPrefs.GetString("TabTitle", Application.productName);
		private void OnChangeTabTitle(ChangeEvent<string> e) {
			EXTPrefs.SetString("TabTitle", e.newValue);
			TabTitle.value = GetTabTitle();
		}
		
		private const string TabIconDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-128.png";
		private string GetTabIcon() => EXTPrefs.GetString("TabIcon", TabIconDefault);
		private void OnChangeTabIcon(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EXTPrefs.SetString("TabIcon", "");
			if (e.newValue != null) EXTPrefs.SetString("TabIcon", AssetDatabase.GetAssetPath(e.newValue));
			TabIcon.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetTabIcon());
		}

		private const string TabIconDarkDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-128-dark.png";
		private string GetTabIconDark() => EXTPrefs.GetString("TabIconDark", TabIconDarkDefault);
		private void OnChangeTabIconDark(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EXTPrefs.SetString("TabIconDark", "");
			if (e.newValue != null) EXTPrefs.SetString("TabIconDark", AssetDatabase.GetAssetPath(e.newValue));
			TabIconDark.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetTabIconDark());
		}

		private string GetWindowTitle() => EXTPrefs.GetString("WindowTitle", Application.productName);
		private void OnChangeWindowTitle(ChangeEvent<string> e) {
			EXTPrefs.SetString("WindowTitle", e.newValue);
			WindowTitle.value = GetWindowTitle();
		}

		private const string WindowIconDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-128.png";
		private string GetWindowIcon() => EXTPrefs.GetString("WindowIcon", WindowIconDefault);
		private void OnChangeWindowIcon(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EXTPrefs.SetString("WindowIcon", "");
			if (e.newValue != null) EXTPrefs.SetString("WindowIcon", AssetDatabase.GetAssetPath(e.newValue));
			WindowIcon.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetWindowIcon());
		}

		private const EXTWindowStyle WindowStyleDefault = EXTWindowStyle.Normal;
		private EXTWindowStyle GetWindowStyle() => (EXTWindowStyle)Enum.Parse(typeof(EXTWindowStyle), EXTPrefs.GetString("WindowStyle", WindowStyleDefault.ToString()));
		private void OnChangeWindowStyle(ChangeEvent<Enum> e) => EXTPrefs.SetString("WindowStyle", e.newValue.ToString());

		private Vector2Int GetWindowSize() => new Vector2Int(EXTPrefs.GetInt("WindowSizeX", 800), EXTPrefs.GetInt("WindowSizeY", 600));
		private void OnChangeWindowSize(ChangeEvent<Vector2Int> e) {
			EXTPrefs.SetInt("WindowSizeX", e.newValue.x);
			EXTPrefs.SetInt("WindowSizeY", e.newValue.y);
		}

		private Vector2Int GetWindowMinSize() => new Vector2Int(EXTPrefs.GetInt("WindowMinSizeX", 0), EXTPrefs.GetInt("WindowMinSizeY", 0));
		private void OnChangeWindowMinSize(ChangeEvent<Vector2Int> e) {
			EXTPrefs.SetInt("WindowMinSizeX", e.newValue.x);
			EXTPrefs.SetInt("WindowMinSizeY", e.newValue.y);
		}

		private Vector2Int GetWindowMaxSize() => new Vector2Int(EXTPrefs.GetInt("WindowMaxSizeX", 0), EXTPrefs.GetInt("WindowMaxSizeY", 0));
		private void OnChangeWindowMaxSize(ChangeEvent<Vector2Int> e) {
			EXTPrefs.SetInt("WindowMaxSizeX", e.newValue.x);
			EXTPrefs.SetInt("WindowMaxSizeY", e.newValue.y);
		}

		private bool GetWindowCenter() => EXTPrefs.GetBool("WindowCenter", false);
		private void OnChangeWindowCenter(ChangeEvent<bool> e) => EXTPrefs.SetBool("WindowCenter", e.newValue);

		private bool GetWindowResizable() => EXTPrefs.GetBool("WindowResizable", true);
		private void OnChangeWindowResizable(ChangeEvent<bool> e) => EXTPrefs.SetBool("WindowResizable", e.newValue);

		private bool GetWindowMovable() => EXTPrefs.GetBool("WindowMovable", true);
		private void OnChangeWindowMovable(ChangeEvent<bool> e) => EXTPrefs.SetBool("WindowMovable", e.newValue);

		private bool GetWindowMinimizable() => EXTPrefs.GetBool("WindowMinimizable", true);
		private void OnChangeWindowMinimizable(ChangeEvent<bool> e) => EXTPrefs.SetBool("WindowMinimizable", e.newValue);

		private bool GetWindowMaximizable() => EXTPrefs.GetBool("WindowMaximizable", true);
		private void OnChangeWindowMaximizable(ChangeEvent<bool> e) => EXTPrefs.SetBool("WindowMaximizable", e.newValue);

		private bool GetWindowClosable() => EXTPrefs.GetBool("WindowClosable", true);
		private void OnChangeWindowClosable(ChangeEvent<bool> e) => EXTPrefs.SetBool("WindowClosable", e.newValue);

		private bool GetWindowAlwaysOnTop() => EXTPrefs.GetBool("WindowAlwaysOnTop", false);
		private void OnChangeWindowAlwaysOnTop(ChangeEvent<bool> e) => EXTPrefs.SetBool("WindowAlwaysOnTop", e.newValue);

		private bool GetWindowFullscreen() => EXTPrefs.GetBool("WindowFullscreen", false);
		private void OnChangeWindowFullscreen(ChangeEvent<bool> e) => EXTPrefs.SetBool("WindowFullscreen", e.newValue);

		private bool GetWindowFullscreenable() => EXTPrefs.GetBool("WindowFullscreenable", true);
		private void OnChangeWindowFullscreenable(ChangeEvent<bool> e) => EXTPrefs.SetBool("WindowFullscreenable", e.newValue);

		private bool GetWindowSkipTaskbar() => EXTPrefs.GetBool("WindowSkipTaskbar", false);
		private void OnChangeWindowSkipTaskbar(ChangeEvent<bool> e) => EXTPrefs.SetBool("WindowSkipTaskbar", e.newValue);

		private bool GetWindowLockAspect() => EXTPrefs.GetBool("WindowLockAspect", false);
		private void OnChangeWindowLockAspect(ChangeEvent<bool> e) => EXTPrefs.SetBool("WindowLockAspect", e.newValue);

		private bool GetWebviewLoadEnabled() => EXTPrefs.GetBool("WebviewLoadEnabled", true);
		private void OnChangWebviewLoadEnabled(ChangeEvent<bool> e) => EXTPrefs.SetBool("WebviewLoadEnabled", e.newValue);

		private bool IsWebviewLoadDark() => PlayerSettings.SplashScreen.unityLogoStyle == PlayerSettings.SplashScreen.UnityLogoStyle.LightOnDark;
		private string GetWebviewLoadIconDefault() => "Packages/store.ext/WebGLTemplates/EXT/ext-" + (IsWebviewLoadDark() ? "dark" : "light") + ".png";
		private string GetWebviewLoadIcon() => EXTPrefs.GetString("WebviewLoadIcon", GetWebviewLoadIconDefault());
		private void OnChangeWebviewLoadIcon(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EXTPrefs.SetString("WebviewLoadIcon", "");
			if (e.newValue != null) EXTPrefs.SetString("WebviewLoadIcon", AssetDatabase.GetAssetPath(e.newValue));
			WebviewLoadIcon.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetWebviewLoadIcon());
		}

		private bool GetWebviewPersist() => EXTPrefs.GetBool("WebviewPersist", true);
		private void OnChangWebviewPersist(ChangeEvent<bool> e) => EXTPrefs.SetBool("WebviewPersist", e.newValue);

		private bool GetWebviewDevTools() => EXTPrefs.GetBool("WebviewDevTools", false);
		private void OnChangWebviewDevTools(ChangeEvent<bool> e) => EXTPrefs.SetBool("WebviewDevTools", e.newValue);

		private string GetManifestName() => EXTPrefs.GetString("ManifestName", Application.productName);
		private void OnChangeManifestName(ChangeEvent<string> e) {
			EXTPrefs.SetString("ManifestName", e.newValue);
			ManifestName.value = GetManifestName();
		}

		private string GetManifestVersion() => EXTPrefs.GetString("ManifestVersion", Application.version);
		private void OnChangeManifestVersion(ChangeEvent<string> e) {
			EXTPrefs.SetString("ManifestVersion", e.newValue);
			ManifestVersion.value = GetManifestVersion();
		}

		private string GetManifestDescription() => EXTPrefs.GetString("ManifestDescription", "");
		private void OnChangeManifestDescription(ChangeEvent<string> e) => EXTPrefs.SetString("ManifestDescription", e.newValue);

		private string GetManifestAuthor() => EXTPrefs.GetString("ManifestAuthor", Application.companyName);
		private void OnChangeManifestAuthor(ChangeEvent<string> e) => EXTPrefs.SetString("ManifestAuthor", e.newValue);

		private string GetManifestHomepage() => EXTPrefs.GetString("ManifestHomepage", "");
		private void OnChangeManifestHomepage(ChangeEvent<string> e) => EXTPrefs.SetString("ManifestHomepage", e.newValue);

		private const string ManifestIconDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-128.png";
		private string GetManifestIcon() => EXTPrefs.GetString("ManifestIcon", ManifestIconDefault);
		private void OnChangeManifestIcon(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EXTPrefs.SetString("ManifestIcon", "");
			if (e.newValue != null) EXTPrefs.SetString("ManifestIcon", AssetDatabase.GetAssetPath(e.newValue));
			ManifestIcon.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestIcon());
		}

		private const string ManifestBackgroundDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-background.png";
		private string GetManifestBackground() => EXTPrefs.GetString("ManifestBackground", ManifestBackgroundDefault);
		private void OnChangeManifestBackground(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EXTPrefs.SetString("ManifestBackground", "");
			if (e.newValue != null) EXTPrefs.SetString("ManifestBackground", AssetDatabase.GetAssetPath(e.newValue));
			ManifestBackground.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestBackground());
		}

		private const string ManifestBannerDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-banner.png";
		private string GetManifestBanner() => EXTPrefs.GetString("ManifestBanner", ManifestBannerDefault);
		private void OnChangeManifestBanner(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EXTPrefs.SetString("ManifestBanner", "");
			if (e.newValue != null) EXTPrefs.SetString("ManifestBanner", AssetDatabase.GetAssetPath(e.newValue));
			ManifestBanner.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestBanner());
		}

		private const string ManifestIconDarkDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-128-dark.png";
		private string GetManifestIconDark() => EXTPrefs.GetString("ManifestIconDark", ManifestIconDarkDefault);
		private void OnChangeManifestIconDark(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EXTPrefs.SetString("ManifestIconDark", "");
			if (e.newValue != null) EXTPrefs.SetString("ManifestIconDark", AssetDatabase.GetAssetPath(e.newValue));
			ManifestIconDark.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestIconDark());
		}

		private const string ManifestBackgroundDarkDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-background-dark.png";
		private string GetManifestBackgroundDark() => EXTPrefs.GetString("ManifestBackgroundDark", ManifestBackgroundDarkDefault);
		private void OnChangeManifestBackgroundDark(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EXTPrefs.SetString("ManifestBackgroundDark", "");
			if (e.newValue != null) EXTPrefs.SetString("ManifestBackgroundDark", AssetDatabase.GetAssetPath(e.newValue));
			ManifestBackgroundDark.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestBackgroundDark());
		}

		private const string ManifestBannerDarkDefault = "Packages/store.ext/WebGLTemplates/EXT/icons/icon-banner-dark.png";
		private string GetManifestBannerDark() => EXTPrefs.GetString("ManifestBannerDark", ManifestBannerDarkDefault);
		private void OnChangeManifestBannerDark(ChangeEvent<UnityEngine.Object> e) {
			if (e.newValue == null) EXTPrefs.SetString("ManifestBannerDark", "");
			if (e.newValue != null) EXTPrefs.SetString("ManifestBannerDark", AssetDatabase.GetAssetPath(e.newValue));
			ManifestBannerDark.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetManifestBannerDark());
		}

		private bool GetBuildAssetEnable() {
			return EXTPrefs.IsAssetEnabled();
		}

		private void OnChangeBuildAssetEnable(ChangeEvent<bool> e) {
			if (EXTPrefs.IsAssetEnabled() != e.newValue) {
				EXTPrefs.SetAssetEnabled(e.newValue);
				CreateGUI();
			}
		}

		private string GetBuildAssetName() {
			return EXTPrefs.GetAssetPath();
		}

		private void OnClickDefaults() {
			EXTPrefs.DeleteKey("TabEnabled");
			EXTPrefs.DeleteKey("TabMutable");
			EXTPrefs.DeleteKey("TabMuted");
			EXTPrefs.DeleteKey("TabTitle");
			EXTPrefs.DeleteKey("TabIcon");
			EXTPrefs.DeleteKey("TabIconDark");
			EXTPrefs.DeleteKey("WindowTitle");
			EXTPrefs.DeleteKey("WindowIcon");
			EXTPrefs.DeleteKey("WindowStyle");
			EXTPrefs.DeleteKey("WindowSizeX");
			EXTPrefs.DeleteKey("WindowSizeY");
			EXTPrefs.DeleteKey("WindowMinSizeX");
			EXTPrefs.DeleteKey("WindowMinSizeY");
			EXTPrefs.DeleteKey("WindowMaxSizeX");
			EXTPrefs.DeleteKey("WindowMaxSizeY");
			EXTPrefs.DeleteKey("WindowCenter");
			EXTPrefs.DeleteKey("WindowResizable");
			EXTPrefs.DeleteKey("WindowMovable");
			EXTPrefs.DeleteKey("WindowMinimizable");
			EXTPrefs.DeleteKey("WindowMaximizable");
			EXTPrefs.DeleteKey("WindowClosable");
			EXTPrefs.DeleteKey("WindowAlwaysOnTop");
			EXTPrefs.DeleteKey("WindowFullscreen");
			EXTPrefs.DeleteKey("WindowFullscreenable");
			EXTPrefs.DeleteKey("WindowSkipTaskbar");
			EXTPrefs.DeleteKey("WindowLockAspect");
			EXTPrefs.DeleteKey("WebviewLoadEnabled");
			EXTPrefs.DeleteKey("WebviewLoadIcon");
			EXTPrefs.DeleteKey("WebviewPersist");
			EXTPrefs.DeleteKey("WebviewDevTools");
			EXTPrefs.DeleteKey("ManifestName");
			EXTPrefs.DeleteKey("ManifestVersion");
			EXTPrefs.DeleteKey("ManifestDescription");
			EXTPrefs.DeleteKey("ManifestAuthor");
			EXTPrefs.DeleteKey("ManifestHomepage");
			EXTPrefs.DeleteKey("ManifestIcon");
			EXTPrefs.DeleteKey("ManifestBackground");
			EXTPrefs.DeleteKey("ManifestBanner");
			EXTPrefs.DeleteKey("ManifestIconDark");
			EXTPrefs.DeleteKey("ManifestBackgroundDark");
			EXTPrefs.DeleteKey("ManifestBannerDark");
			EXTPrefs.DeleteKey("BuildName");
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
				string path = null;
				if (zip) {
					path = EditorUtility.SaveFilePanel("Location for EXT ZIP build", "", GetManifestName() + ".zip", "zip");
				} else {
					path = EditorUtility.SaveFolderPanel("Location for EXT build", "", "");
				}
				if (path == "") return;

				// Check if build path exists
				if (!zip && !Directory.Exists(path)) {
					if (!EditorUtility.DisplayDialog(
						"Build location not found",
						"Selected build location does not exist. Create it?",
						"Yes", "Cancel")
					) return;
					Directory.CreateDirectory(path);
				}

				// Check if build path has files
				if (!zip && Directory.GetFiles(path).Length > 0) {
					if (!EditorUtility.DisplayDialog(
						"Build location not empty",
						"Selected build location already contains files. Overwrite?",
						"Yes", "Cancel"
					)) return;
					if (FileUtil.DeleteFileOrDirectory(path)) {
						Directory.CreateDirectory(path);
					} else {
						throw new Exception("Failed to delete existing files.");
					}
				}

				// Check if zip file already exists
				if (zip && File.Exists(path)) {
					if (!EditorUtility.DisplayDialog(
						"Zip file already exists",
						"Selected build location already exists. Overwrite?",
						"Yes", "Cancel")
					) return;
					if (!FileUtil.DeleteFileOrDirectory(path)) {
						throw new Exception("Failed to delete existing zip file.");
					}
				}

				// Find WebGL template path
				string templatePath = null;
				if (File.Exists("Assets/WebGLTemplates/EXT/index.html")) {
					templatePath = "EXT";
				} else {
					foreach (string package in Directory.GetDirectories("Library/PackageCache")) {
						string name = Path.GetFileName(package);
						string index = "Library/PackageCache/" + name + "/WebGLTemplates/EXT/index.html";
						if (File.Exists(index)) {
							templatePath = "../../Library/PackageCache/" + name + "/WebGLTemplates/EXT";
							break;
						}
					}
					templatePath ??= "../../Packages/store.ext/WebGLTemplates/EXT";
				}

				// Use temporary build path if building a zip
				string buildPath = zip ? Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()) : path;

				// Log
				string template = "PROJECT:" + templatePath;
				Debug.Log("Building Player for EXT at: " + buildPath + "\nWebGL Template: " + template);

				// Build using EXT WebGL template
				BuildReport report = null;
				string webglTemplate = PlayerSettings.WebGL.template;
				try {
					PlayerSettings.WebGL.template = template;
					report = BuildPipeline.BuildPlayer(new BuildPlayerOptions() {
						scenes = scenes.ToArray(),
						locationPathName = buildPath,
						assetBundleManifestPath = null,
						targetGroup = BuildTargetGroup.WebGL,
						target = BuildTarget.WebGL,
						options = BuildOptions.None,
						extraScriptingDefines = null,
					});
				} finally {
					PlayerSettings.WebGL.template = webglTemplate;
				}

				// Check for build errors
				if (report.summary.result != BuildResult.Succeeded) {
					throw new Exception("EXT Build failed: " + report.summary.result);
				}

				// Copy WebGL Template files if needed
				string templateRootPath = Path.Combine("Assets/WebGLTemplates", templatePath);
				CopyTemplateFiles(Path.Combine(buildPath, "Build"), buildPath, templateRootPath);

				// Replace default files
				ReplaceIndex(buildPath);
				ReplaceManifest(buildPath);
				ReplaceScript(buildPath);

				// Create zip file and delete temporary build directory
				if (zip) {
					ZipFile.CreateFromDirectory(buildPath, path);
					if (!FileUtil.DeleteFileOrDirectory(buildPath)) throw new Exception("Failed to delete build files.");
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

		private void CopyTemplateFiles(string buildPath, string dstPath, string srcTemplatePath) {
			
			// Copy any missing files
			string[] srcFiles = Directory.GetFiles(srcTemplatePath);
			foreach (string srcFile in srcFiles) {
				if (Path.GetExtension(srcFile) == ".meta") continue;
				if (Path.GetFileName(srcFile) == "thumbnail.png") continue;
				string dstFile = Path.Combine(dstPath, Path.GetFileName(srcFile));
				if (!File.Exists(dstFile)) {
					Debug.LogWarning("WebGL Template failed to copy file: " + dstFile);
					CopyTemplateFile(buildPath, dstFile, srcFile);
				}
			}

			// Copy any missing directories
			string[] srcDirs = Directory.GetDirectories(srcTemplatePath);
			foreach (string srcDir in srcDirs) {
				string dstDir = Path.Combine(dstPath, Path.GetFileName(srcDir));
				if (!Directory.Exists(dstDir)) {
					Debug.LogWarning("WebGL Template failed to copy directory: " + dstDir);
					Directory.CreateDirectory(dstDir);
					CopyTemplateFiles(buildPath, dstDir, srcDir);
				}
			}

		}

		private void CopyTemplateFile(string buildPath, string dstPathFile, string srcPathFile) {
			
			// Copy binary files
			string extension = Path.GetExtension(srcPathFile);
			if (extension != ".html" && extension != ".js" && extension != ".css" && extension == ".json") {
				File.Copy(srcPathFile, dstPathFile);
				return;
			}

			// Get file contents
			string contents = File.ReadAllText(srcPathFile);

			// Replace filename variables
			string LOADER_FILENAME = FindFileNameWithExtension(buildPath, ".loader.js", "");
			string DATA_FILENAME = FindFileNameWithExtension(buildPath, ".data.gz", "");
			string CODE_FILENAME = FindFileNameWithExtension(buildPath, ".wasm.gz", "");
			string FRAMEWORK_FILENAME = FindFileNameWithExtension(buildPath, ".framework.js.gz", "");
			string BACKGROUND_FILENAME = FindFileNameWithExtension(buildPath, ".jpg", "");
			if (DATA_FILENAME == "") DATA_FILENAME = FindFileNameWithExtension(buildPath, ".data", "");
			if (CODE_FILENAME == "") CODE_FILENAME = FindFileNameWithExtension(buildPath, ".wasm", "");
			if (FRAMEWORK_FILENAME == "") FRAMEWORK_FILENAME = FindFileNameWithExtension(buildPath, ".framework.js", "");
			if (DATA_FILENAME == "") throw new Exception("build.data not found in build directory");
			if (CODE_FILENAME == "") throw new Exception("build.wasm not found in build directory");
			if (FRAMEWORK_FILENAME == "") throw new Exception("build.framework.js not found in build directory");
			contents = contents.Replace("{{{ LOADER_FILENAME }}}", LOADER_FILENAME);
			contents = contents.Replace("{{{ DATA_FILENAME }}}", DATA_FILENAME);
			contents = contents.Replace("{{{ CODE_FILENAME }}}", CODE_FILENAME);
			contents = contents.Replace("{{{ FRAMEWORK_FILENAME }}}", FRAMEWORK_FILENAME);
			contents = contents.Replace("{{{ BACKGROUND_FILENAME }}}", BACKGROUND_FILENAME);
			contents = contents.Replace("{{{ MEMORY_FILENAME }}}", "");
			contents = contents.Replace("{{{ SYMBOLS_FILENAME }}}", "");

			// Replace application variables
			bool dark = PlayerSettings.SplashScreen.unityLogoStyle == PlayerSettings.SplashScreen.UnityLogoStyle.LightOnDark;
			contents = contents.Replace("{{{ SPLASH_SCREEN_STYLE }}}", dark ? "Dark" : "Light");
			contents = contents.Replace("{{{ COMPANY_NAME }}}", Application.companyName);
			contents = contents.Replace("{{{ PRODUCT_NAME }}}", Application.productName);
			contents = contents.Replace("{{{ PRODUCT_VERSION }}}", Application.version);
			contents = contents.Replace("{{{ BACKGROUND_COLOR }}}", ColorToHex(PlayerSettings.SplashScreen.backgroundColor));

			// Write file
			File.WriteAllText(dstPathFile, contents);

		}

		private static string ColorToHex(Color color) {
			int r = (int)(color.r * 255);
			int g = (int)(color.g * 255);
			int b = (int)(color.b * 255);
			return string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
		}

		private static string FindFileNameWithExtension(string path, string extension, string notFound) {
			string[] files = Directory.GetFiles(path);
			foreach (string filePath in files) {
				if (filePath.EndsWith(extension)) {
					return Path.GetFileName(filePath);
				}
			}
			return notFound;
		}

		private void ReplaceIndex(string path) {

			// Copy loading icon to build directory
			string iconDst = "";
			string iconSrc = GetWebviewLoadIcon();
			if (!string.IsNullOrEmpty(iconSrc)) {
				iconDst = "load" + Path.GetExtension(iconSrc);
				FileUtil.CopyFileOrDirectory(iconSrc, Path.Combine(path, iconDst));
			}

			// Read index file
			string filepath = Path.Combine(path, "index.html");
			string contents = File.ReadAllText(filepath);

			// Replace values
			contents = ReplaceIndexValue(contents, "configLoadEnabled", GetWebviewLoadEnabled() ? "true" : "false");
			if (!string.IsNullOrEmpty(iconDst)) contents = ReplaceIndexValue(contents, "configLoadIcon", "\"" + EscapeJS(iconDst) + "\"");
			contents = ReplaceIndexValue(contents, "configDragEnabled", GetWindowStyle() == EXTWindowStyle.Normal ? "false" : "true");
			contents = ReplaceIndexValue(contents, "configDragInset", GetWindowStyle() == EXTWindowStyle.FramelessHidden ? "false" : "true");

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

		private string ReplaceIndexValue(string contents, string key, string value) {
			Regex regex = new Regex("[\\t ]*const[\\t ]+" + key + "[\\t ]*=[\\t ]*[^\\n]+;[\\t ]*");
			return regex.Replace(contents, "    const " + key + " = " + value + ";");
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
			if (!string.IsNullOrEmpty(iconSrc)) {
				string iconDst = Path.Combine("icons", "icon-128" + Path.GetExtension(iconSrc));
				FileUtil.CopyFileOrDirectory(iconSrc, Path.Combine(path, iconDst));
			}

			// Copy manifest background
			string backgroundSrc = GetManifestBackground();
			if (!string.IsNullOrEmpty(backgroundSrc)) {
				string backgroundDst = Path.Combine("icons", "icon-background" + Path.GetExtension(backgroundSrc));
				FileUtil.CopyFileOrDirectory(backgroundSrc, Path.Combine(path, backgroundDst));
			}

			// Copy manifest banner
			string bannerSrc = GetManifestBanner();
			if (!string.IsNullOrEmpty(bannerSrc)) {
				string bannerDst = Path.Combine("icons", "icon-banner" + Path.GetExtension(bannerSrc));
				FileUtil.CopyFileOrDirectory(bannerSrc, Path.Combine(path, bannerDst));
			}

			// Copy manifest dark icon
			string iconDarkSrc = GetManifestIconDark();
			if (!string.IsNullOrEmpty(iconDarkSrc)) {
				string iconDarkDst = Path.Combine("icons", "icon-128-dark" + Path.GetExtension(iconDarkSrc));
				FileUtil.CopyFileOrDirectory(iconDarkSrc, Path.Combine(path, iconDarkDst));
			}

			// Copy manifest dark background
			string backgroundDarkSrc = GetManifestBackgroundDark();
			if (!string.IsNullOrEmpty(backgroundDarkSrc)) {
				string backgroundDarkDst = Path.Combine("icons", "icon-background-dark" + Path.GetExtension(backgroundDarkSrc));
				FileUtil.CopyFileOrDirectory(backgroundDarkSrc, Path.Combine(path, backgroundDarkDst));
			}

			// Copy manifest dark banner
			string bannerDarkSrc = GetManifestBannerDark();
			if (!string.IsNullOrEmpty(bannerDarkSrc)) {
				string bannerDarkDst = Path.Combine("icons", "icon-banner-dark" + Path.GetExtension(bannerDarkSrc));
				FileUtil.CopyFileOrDirectory(bannerDarkSrc, Path.Combine(path, bannerDarkDst));
			}

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
			string tabIconDst = "";
			string tabIconSrc = GetTabIcon();
			if (!string.IsNullOrEmpty(tabIconSrc)) {
				tabIconDst = Path.Combine("icons", "tab-icon" + Path.GetExtension(tabIconSrc));
				FileUtil.CopyFileOrDirectory(tabIconSrc, Path.Combine(path, tabIconDst));
			}

			// Copy tab dark icon
			string tabIconDarkDst = "";
			string tabIconDarkSrc = GetTabIconDark();
			if (string.IsNullOrEmpty(tabIconDarkSrc)) {
				tabIconDarkDst = Path.Combine("icons", "tab-icon-dark" + Path.GetExtension(tabIconDarkSrc));
				FileUtil.CopyFileOrDirectory(tabIconDarkSrc, Path.Combine(path, tabIconDarkDst));
			}

			// Copy window icon
			string windowIconDst = "";
			string windowIconSrc = GetWindowIcon();
			if (!string.IsNullOrEmpty(windowIconSrc)) {
				windowIconDst = Path.Combine("icons", "window-icon" + Path.GetExtension(windowIconSrc));
				FileUtil.CopyFileOrDirectory(windowIconSrc, Path.Combine(path, windowIconDst));
			}

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
