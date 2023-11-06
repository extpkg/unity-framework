// EXT - {{{ PRODUCT_NAME }}}

// Global resources
let tab = null
let window = null
let webview = null
let websession = null
let created = false

// Tab Configuration
const ConfigTabEnabled = true
const ConfigTabMutable = true
const ConfigTabMuted = false
const ConfigTabTitle = '{{{ PRODUCT_NAME }}}'
const ConfigTabIcon = 'icons/icon-128.png'
const ConfigTabIconDark = 'icons/icon-128-dark.png'

// Window Configuration
const ConfigWindowTitle = "{{{ PRODUCT_NAME }}}"
const ConfigWindowIcon = 'icons/icon-128.png'
const ConfigWindowFrame = false
const ConfigWindowTitleBarStyle = 'inset'
const ConfigWindowSizeX = undefined
const ConfigWindowSizeY = undefined
const ConfigWindowMinSizeX = undefined
const ConfigWindowMinSizeY = undefined
const ConfigWindowMaxSizeX = undefined
const ConfigWindowMaxSizeY = undefined
const ConfigWindowCenter = false
const ConfigWindowResizable = true
const ConfigWindowMovable = true
const ConfigWindowMinimizable = true
const ConfigWindowMaximizable = true
const ConfigWindowClosable = true
const ConfigWindowAlwaysOnTop = false
const ConfigWindowFullscreen = false
const ConfigWindowFullscreenable = true
const ConfigWindowSkipTaskbar = false
const ConfigWindowAspectRatio = undefined

// Webview Configuration
const ConfigWebviewPersist = true
const ConfigWebviewDevTools = false

// Extension clicked
ext.runtime.onExtensionClick.addListener(async () => {
  try {

    // Check if window already exists
    if (created && window !== null) {
      if (ConfigWindowMaximizable) await ext.windows.restore(window.id)
      await ext.windows.focus(window.id)
      return
    }

    // Create tab
    if (ConfigTabEnabled) {
      tab = await ext.tabs.create({
        icon: ConfigTabIcon,
        icon_dark: ConfigTabIconDark,
        text: ConfigTabTitle,
        muted: ConfigTabMuted,
        mutable: ConfigTabMutable,
        closable: ConfigWindowClosable,
      })
    }

    // Create window
    window = await ext.windows.create({
      title: ConfigWindowTitle,
      icon: ConfigWindowIcon,
      frame: ConfigWindowFrame,
      titleBarStyle: ConfigWindowTitleBarStyle,
      width: ConfigWindowSizeX,
      height: ConfigWindowSizeY,
      minWidth: ConfigWindowMinSizeX,
      minHeight: ConfigWindowMinSizeY,
      maxWidth: ConfigWindowMaxSizeX,
      maxHeight: ConfigWindowMaxSizeY,
      center: ConfigWindowCenter,
      resizable: ConfigWindowResizable,
      movable: ConfigWindowMovable,
      minimizable: ConfigWindowMinimizable,
      maximizable: ConfigWindowMaximizable,
      closable: ConfigWindowClosable,
      alwaysOnTop: ConfigWindowAlwaysOnTop,
      fullscreen: ConfigWindowFullscreen,
      fullscreenable: ConfigWindowFullscreenable,
      skipTaskbar: ConfigWindowSkipTaskbar,
      aspectRatio: ConfigWindowAspectRatio,
      vibrancy: false,
    })

    // Create websession
    if (ConfigWebviewPersist) {
      websession = await ext.websessions.create({
        persistent: true,
        cache: true,
      })
    }

    // Create webview
    webview = await ext.webviews.create({ websession: websession ?? undefined })
    const size = await ext.windows.getContentSize(window.id)
    await ext.webviews.loadFile(webview.id, 'index.html')
    await ext.webviews.attach(webview.id, window.id)
    await ext.webviews.setBounds(webview.id, { x: 0, y: 0, width: size.width, height: size.height })
    await ext.webviews.setAutoResize(webview.id, { width: true, height: true })

    // Mute tab initially
    if (ConfigTabMuted) {
      await ext.webviews.setAudioMuted(webview.id, true)
    }
    
    // Open DevTools
    if (ConfigWebviewDevTools) {
      await ext.webviews.openDevTools(webview.id, { mode: 'detach' })
    }

    // Mark window as created
    created = true

  } catch (error) {

    // Print error
    console.error('ext.runtime.onExtensionClick', JSON.stringify(error))

  }
})

// Tab was removed
ext.tabs.onRemoved.addListener(async (event) => {
  try {

    // Remove objects
    if (event.id === tab?.id) {
      if (window !== null) await ext.windows.remove(window.id)
      if (webview !== null) await ext.webviews.remove(webview.id)
      if (websession !== null) await ext.websessions.remove(websession.id)
      tab = window = websession = webview = null
      created = false
    }

  } catch (error) {

    // Print error
    console.error('ext.tabs.onRemoved', JSON.stringify(error))

  }
})

// Tab was clicked
ext.tabs.onClicked.addListener(async (event) => {
  try {

    // Restore & Focus window
    if (event.id === tab?.id && window !== null) {
      if (ConfigWindowMaximizable) await ext.windows.restore(window.id)
      await ext.windows.focus(window.id)
    }

  } catch (error) {

    // Print error
    console.error('ext.tabs.onClicked', JSON.stringify(error))

  }
})

// Tab was closed
ext.tabs.onClickedClose.addListener(async (event) => {
  try {

    // Remove objects
    if (event.id === tab?.id) {
      if (tab !== null) await ext.tabs.remove(tab.id)
      if (window !== null) await ext.windows.remove(window.id)
      if (webview !== null) await ext.webviews.remove(webview.id)
      if (websession !== null) await ext.websessions.remove(websession.id)
      tab = window = websession = webview = null
      created = false
    }

  } catch (error) {

    // Print error
    console.error('ext.tabs.onClickedClose', JSON.stringify(error))

  }
})

// Tab was muted
ext.tabs.onClickedMute.addListener(async (event) => {
  try {

    // Update audio
    if (event.id === tab?.id && tab !== null && webview !== null) {
      const muted = await ext.webviews.isAudioMuted(webview.id)
      await ext.webviews.setAudioMuted(webview.id, !muted)
      await ext.tabs?.update(tab.id, { muted: !muted })
    }

  } catch (error) {

    // Print error
    console.error('ext.tabs.onClickedMute', JSON.stringify(error))

  }
})

// Window was removed
ext.windows.onRemoved.addListener(async (event) => {
  try {

    // Remove objects
    if (event.id === window?.id) {
      if (tab !== null) await ext.tabs.remove(tab.id)
      if (webview !== null) await ext.webviews.remove(webview.id)
      if (websession !== null) await ext.websessions.remove(websession.id)
      tab = window = websession = webview = null
      created = false
    }

  } catch (error) {

    // Print error
    console.error('ext.windows.onRemoved', JSON.stringify(error))

  }
})

// Window was closed
ext.windows.onClosed.addListener(async (event) => {
  try {

   // Remove objects
   if (event.id === window?.id) {
    if (tab !== null) await ext.tabs.remove(tab.id)
    if (webview !== null) await ext.webviews.remove(webview.id)
    if (websession !== null) await ext.websessions.remove(websession.id)
    tab = window = websession = webview = null
    created = false
  }

  } catch (error) {

    // Print error
    console.error('ext.windows.onClosed', JSON.stringify(error))

  }
})
