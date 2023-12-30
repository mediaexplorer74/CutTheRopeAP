using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
//using System.Windows.Forms;
using GameManager.ctr_commons;
using GameManager.iframework;
using GameManager.iframework.core;
using GameManager.iframework.media;
using GameManager.windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Windows.Devices.Input;
using Windows.UI.Xaml.Controls;

namespace GameManager
{
    public class Game1 : Game
    {
        private Branding branding;

        //private Process parentProcess;

        private int mouseState_X;

        private int mouseState_Y;

        private int mouseState_ScrollWheelValue;

        private Microsoft.Xna.Framework.Input.ButtonState mouseState_LeftButton;

        private Microsoft.Xna.Framework.Input.ButtonState mouseState_MiddleButton;

        private Microsoft.Xna.Framework.Input.ButtonState mouseState_RightButton;

        private Microsoft.Xna.Framework.Input.ButtonState mouseState_XButton1;

        private Microsoft.Xna.Framework.Input.ButtonState mouseState_XButton2;

        //private Cursor _cursorLast;

        private Dictionary<Microsoft.Xna.Framework.Input.Keys, bool> keyState 
            = new Dictionary<Microsoft.Xna.Framework.Input.Keys, bool>();

        private KeyboardState keyboardStateXna;

        private bool _DrawMovie;

        private int _ignoreMouseClick;

        private int frameRate;

        private int frameCounter;

        private TimeSpan elapsedTime = TimeSpan.Zero;

        private bool bFirstFrame = true;

        private bool IsMinimized
        {
            get
            {
                //Form form = WindowAsForm();
                return false;//form.WindowState == FormWindowState.Minimized;
            }
        }

        public Game1()
        {
            Global.XnaGame = this;
            Content.RootDirectory = "Content";
            Global.GraphicsDeviceManager = new GraphicsDeviceManager(this);
            try
            {
                Global.GraphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
                Global.GraphicsDeviceManager.ApplyChanges();
            }
            catch (Exception)
            {
                Global.GraphicsDeviceManager.GraphicsProfile = GraphicsProfile.Reach;
                Global.GraphicsDeviceManager.ApplyChanges();
            }
            Global.GraphicsDeviceManager.PreparingDeviceSettings 
                += GraphicsDeviceManager_PreparingDeviceSettings;

            TargetElapsedTime = TimeSpan.FromMilliseconds(11.11106666666667 / gameSpeedMult);
            IsFixedTimeStep = true;
            InactiveSleepTime = TimeSpan.FromTicks(500000L);
            IsMouseVisible = true;
            Activated += Game1_Activated;
            Deactivated += Game1_Deactivated;
            Exiting += Game1_Exiting;
            //parentProcess = ParentProcessUtilities.GetParentProcess();
            //Form form = WindowAsForm();
            //form.MouseMove += form_MouseMove;
            //form.MouseUp += form_MouseUp;
            //form.MouseDown += form_MouseDown;
        }

        private void form_MouseDown(object sender, MEventArgs e)
        {
            mouseState_X = e.X;
            mouseState_Y = e.Y;
            switch (e.Button)
            {
                /*
                
                case MouseButtons.Left:
                    mouseState_LeftButton = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                    break;
                case MouseButtons.Middle:
                    mouseState_MiddleButton = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                    break;
                case MouseButtons.Right:
                    mouseState_RightButton = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                    break;
                case MouseButtons.XButton1:
                    mouseState_XButton1 = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                    break;
                case MouseButtons.XButton2:
                    mouseState_XButton2 = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                    break;
                */
            }

            if (_DrawMovie && e.Button == MouseButtons.Left)
            {
                iframework.core.Application.sharedMovieMgr().stop();
            }
            CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativeTouchProcess(
                Global.MouseCursor.GetTouchLocation());
        }

        private void form_MouseUp(object sender, MEventArgs e)
        {
            mouseState_X = e.X;
            mouseState_Y = e.Y;
            switch (e.Button)
            {
                /*
                case MouseButtons.Left:
                    mouseState_LeftButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
                    break;
                case MouseButtons.Middle:
                    mouseState_MiddleButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
                    break;
                case MouseButtons.Right:
                    mouseState_RightButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
                    break;
                case MouseButtons.XButton1:
                    mouseState_XButton1 = Microsoft.Xna.Framework.Input.ButtonState.Released;
                    break;
                case MouseButtons.XButton2:
                    mouseState_XButton2 = Microsoft.Xna.Framework.Input.ButtonState.Released;
                    break;
                */
            }
            CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativeTouchProcess(
                Global.MouseCursor.GetTouchLocation());
        }

        private void form_MouseMove(object sender, MEventArgs e)
        {
            mouseState_X = e.X;
            mouseState_Y = e.Y;
            CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativeTouchProcess
                (Global.MouseCursor.GetTouchLocation());
        }

        public MouseState GetMouseState()
        {
            return new MouseState(mouseState_X, mouseState_Y, 
                mouseState_ScrollWheelValue, mouseState_LeftButton, mouseState_MiddleButton, mouseState_RightButton, mouseState_XButton1, mouseState_XButton2);
        }

        private void GraphicsDeviceManager_PreparingDeviceSettings(
            object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.DepthStencilFormat
                = DepthFormat.None;
        }

        private void form_Resize(object sender, EventArgs e)
        {
            if (Global.ScreenSizeManager.SkipSizeChanges)
            {
                return;
            }
            Form form = WindowAsForm();
            if (form.WindowState == FormWindowState.Maximized)
            {
                form.WindowState = FormWindowState.Normal;
                //if (!Global.ScreenSizeManager.IsFullScreen)
                //{
                    //Global.ScreenSizeManager.ToggleFullScreen();
                //}
            }
        }

        public void SetCursor(Cursor cursor, MouseState mouseState)
        {
            if (Window.ClientBounds.Contains(Window.ClientBounds.X + mouseState.X,
                Window.ClientBounds.Y + mouseState.Y) && _cursorLast != cursor)
            {
                WindowAsForm().Cursor = cursor;
                _cursorLast = cursor;
            }
        }

        private Form WindowAsForm()
        {
            return default;//(Form)Control.FromHandle(Window.Handle);
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            Window.ClientSizeChanged -= Window_ClientSizeChanged;
            Global.ScreenSizeManager.FixWindowSize(Window.ClientBounds);
            Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        private void Game1_Exiting(object sender, EventArgs e)
        {
            Preferences._savePreferences();
            Preferences.Update();
        }

        private void Game1_Deactivated(object sender, EventArgs e)
        {
            _ignoreMouseClick = 60;
            CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativePause();
        }

        private void Game1_Activated(object sender, EventArgs e)
        {
            CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativeResume();
        }

        protected override void LoadContent()
        {
            Global.GraphicsDevice = GraphicsDevice;
            Global.SpriteBatch = new SpriteBatch(GraphicsDevice);
            
            SoundMgr.SetContentManager(Content);
            
            OpenGL.Init();
            
            Global.MouseCursor.Load(Content);

            Form form = WindowAsForm();
            Window.AllowUserResizing = true;
            if (form != null)
            {
                form.MaximizeBox = false;
            }
            Preferences._loadPreferences();
            int num = Preferences._getIntForKey("PREFS_WINDOW_WIDTH");

            //RnD
            bool isFullScreen = false;//num <= 0 || Preferences._getBooleanForKey("PREFS_WINDOW_FULLSCREEN");
            
            Global.ScreenSizeManager.Init(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode, num, isFullScreen);
            
            Window.ClientSizeChanged += Window_ClientSizeChanged;
            if (form != null)
            {
                Global.ScreenSizeManager.SetWindowMinimumSize(form);
                form.BackColor = default;//System.Drawing.Color.Black;
                form.Resize += form_Resize;
            }
            CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativeInit(GetSystemLanguage());
            CtrRenderer.onSurfaceCreated();
            CtrRenderer.onSurfaceChanged(Global.ScreenSizeManager.WindowWidth, Global.ScreenSizeManager.WindowHeight);
            branding = new Branding();
            branding.LoadSplashScreens();
        }

        protected override void UnloadContent()
        {
            //
        }

        private Language GetSystemLanguage()
        {
            Language result = Language.LANG_EN;
            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ru")
            {
                result = Language.LANG_RU;
            }
            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "de")
            {
                result = Language.LANG_DE;
            }
            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
            {
                result = Language.LANG_FR;
            }
            return result;
        }

        public bool IsKeyPressed(Microsoft.Xna.Framework.Input.Keys key)
        {
            bool value = false;
            keyState.TryGetValue(key, out value);
            bool flag = keyboardStateXna.IsKeyDown(key);
            keyState[key] = flag;
            if (flag)
            {
                return value != flag;
            }
            return false;
        }

        public bool IsKeyDown(Microsoft.Xna.Framework.Input.Keys key)
        {
            return keyboardStateXna.IsKeyDown(key);
        }

        private float gameSpeedMult = 1f;
        private Process parentProcess;
        private Cursor _cursorLast;

        protected override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime > TimeSpan.FromSeconds(1.0))
            {
                elapsedTime -= TimeSpan.FromSeconds(1.0);
                frameRate = frameCounter;
                frameCounter = 0;
                Preferences.Update();
            }
            if (IsMinimized)
            {
                return;
            }
            keyboardStateXna = Keyboard.GetState();
            if (IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F11)
                || ((IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt)
                || IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightAlt))
                && IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter)))
            {
                Global.ScreenSizeManager.ToggleFullScreen();
                //Thread.Sleep(500);
                return;
            }
            if (branding != null)
            {
                if (IsActive && branding.IsLoaded)
                {
                    if (branding.IsFinished)
                    {
                        branding = null;
                    }
                    else
                    {
                        branding.Update(gameTime);
                    }

                }
                return;
            }
            if ( IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape)
                 || 
                 GamePad.GetState(PlayerIndex.One).Buttons.Back
                   == Microsoft.Xna.Framework.Input.ButtonState.Pressed )
            {
                iframework.core.Application.sharedMovieMgr().stop();
                CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativeBackPressed();
            }
            MouseState mouseState = windows.MouseCursor.GetMouseState();

            //RnD
            //iframework.core.Application.sharedRootController().mouseMoved(
            //    CtrRenderer.transformX(mouseState.X), CtrRenderer.transformY(mouseState.Y));
            
            CtrRenderer.update((float)gameTime.ElapsedGameTime.TotalSeconds * 1.5f * gameSpeedMult);
            
            base.Update(gameTime);
        }

        public void DrawMovie()
        {
            _DrawMovie = true;
            GraphicsDevice.Clear(Color.Black);
            Texture2D texture = iframework.core.Application.sharedMovieMgr().getTexture();
            if (texture == null)
            {
                return;
            }
            if (_ignoreMouseClick > 0)
            {
                _ignoreMouseClick--;
            }
            else
            {
                MouseState mouseState = Global.XnaGame.GetMouseState();
                if (mouseState.LeftButton
                    == Microsoft.Xna.Framework.Input.ButtonState.Pressed 
                    && Global.ScreenSizeManager.CurrentSize.Contains(mouseState.X, mouseState.Y))
                {
                    iframework.core.Application.sharedMovieMgr().stop();
                }
            }
            Global.GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            Global.ScreenSizeManager.FullScreenCropWidth = false;
            Global.ScreenSizeManager.ApplyViewportToDevice();
            Microsoft.Xna.Framework.Rectangle destinationRectangle 
                = new Microsoft.Xna.Framework.Rectangle(
                    0, 0, GraphicsDevice.Viewport.Width,
                      GraphicsDevice.Viewport.Height);

            Global.SpriteBatch.Begin();
            Global.SpriteBatch.Draw(texture, destinationRectangle, Color.White);
            Global.SpriteBatch.End();
        }

        protected override void Draw(GameTime gameTime)
        {
            frameCounter++;
            GraphicsDevice.Clear(Color.Black);
            if (branding != null)
            {
                if (branding.IsLoaded)
                {
                    branding.Draw(gameTime);
                    Global.GraphicsDevice.SetRenderTarget(null);
                }
                return;
            }
            Global.ScreenSizeManager.FullScreenCropWidth = true;
            Global.ScreenSizeManager.ApplyViewportToDevice();
            _DrawMovie = false;
            CtrRenderer.onDrawFrame();
            Global.MouseCursor.Draw();
            Global.GraphicsDevice.SetRenderTarget(null);
            if (bFirstFrame)
            {
                GraphicsDevice.Clear(Color.Black);
            }
            else if (!_DrawMovie)
            {
                OpenGL.CopyFromRenderTargetToScreen();
            }
            base.Draw(gameTime);
            bFirstFrame = false;
        }
    }
}

/*
 
 // Game1 (Main)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using TheWitnessPuzzles;
using System.Threading;

namespace GameManager
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        public static ScreenManager _screenManager;
        SpriteBatch _spriteBatch;
   
        Point defaultScreenSize = new Point(800, 480);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this) 
            {
                SynchronizeWithVerticalRetrace = true 
            };
            //_graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            //Create a new instance of the Screen Manager

            _screenManager = new ScreenManager();//(this);

            //RnD
            //Components.Add(Game1._screenManager);

            IsMouseVisible = true;



            IsFixedTimeStep = false;
            
            //Content.RootDirectory = "Content";
            
            InputManager.Initialize(this);
            
            SettingsManager.LoadSettings();
            
            FileStorageManager.MigrateInternalToExternal();

//#if WINDOWS
            // This method call is present twice (here and in the Initialize method later), because of cross-platform magic
            // On Windows GraphicsDevice should me initialized in constructor, otherwise window size will be default and not corresponding to the backbuffer
            // But on Android GraphicsDevice is still null after ApplyChanges() if it's called in constructor
            InitializeGraphicsDevice();
//#endif

            Window.ClientSizeChanged += ResizeScreen;

            this.Activated += (object sender, EventArgs e) =>
            {
                InputManager.IsFocused = true;
            };
            
            this.Deactivated += (object sender, EventArgs e) =>
            {
                InputManager.IsFocused = false;
            };

//#if ANDROID
//            SettingsManager.OrientationLockChanged += () =>
//            {
//                SetScreenOrientation(Window.CurrentOrientation);
//                graphics.ApplyChanges();
//            };
//
//#endif
        }

        private void ResizeScreen(object sender, EventArgs e)
        {
#if ANDROID
            // When the game launches in the landscape mode it calls ResizeScreen before GraphicsDevice gets initialized
            if (GraphicsDevice == null || graphics == null)
                return;

            // On my andoird 4.0.3 i have weird behaviour when backbuffer is automatically being resized wrong
            // So i update it manually, TitleSafeArea has the right size of the free screeen area
            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.TitleSafeArea.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.TitleSafeArea.Height;
            graphics.ApplyChanges();

            // Apply screen resize to active GameScreen
            ScreenManager.Instance.UpdateScreenSize(GraphicsDevice.DisplayMode.TitleSafeArea.Size);

            // Dirteh haxx here!
            // Touch panel behaves weirdly after screen rotation, like being stretched horizontally, 
            // so the coordinates of the touch point is not where your finger is, but slightly to the left.
            // But! If you call ApplyChanges again, then it gets back to normal.
            // But! You can't call it right away, there should be slight delay for hacky magic to happen.
            // I know, right?! (.__.)
            System.Threading.Tasks.Task.Run(() =>
            {
                Thread.Sleep(200);
                graphics.ApplyChanges();
            });
#else
            // But on PC it actually should be Window size, not the TitleSafeArea
            _screenManager.UpdateScreenSize(Window.ClientBounds.Size);
#endif

        }

        public void SetFullscreen(bool isFullscreen)
        {
#if ANDROID
            graphics.IsFullScreen = isFullscreen;
#else
            if (isFullscreen)
            {
                //Window.IsBorderless = true;
                //Window.Position = Point.Zero;
                _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                //Window.IsBorderless = false;
                _graphics.PreferredBackBufferWidth = defaultScreenSize.X;
                _graphics.PreferredBackBufferHeight = defaultScreenSize.Y;
            }
#endif
            _graphics.ApplyChanges();
            ResizeScreen(null, null);
        }
        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
#if ANDROID
            // This method call is present twice (here and in the Constructor earlier), because of cross-platform magic
            // On Windows GraphicsDevice should me initialized in constructor, otherwise window size will be default and not corresponding to the backbuffer
            // But on Android GraphicsDevice is still null after ApplyChanges() if it's called in constructor
            InitializeGraphicsDevice();
#endif
            //RnD
            InitializeGraphicsDevice();

            _screenManager.Initialize(this, GraphicsDevice, Content);

            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 768;
            

            _screenManager.ScreenSize = new Point
                (_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            _graphics.ApplyChanges();

            Glob.Content = Content;

            base.Initialize();
        }

        private void InitializeGraphicsDevice()
        {
            //RnD
            _graphics.SupportedOrientations = 
                DisplayOrientation.Portrait 
                | DisplayOrientation.LandscapeLeft 
                | DisplayOrientation.LandscapeRight;

#if ANDROID

            graphics.IsFullScreen = SettingsManager.IsFullscreen;
            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            SetScreenOrientation(SettingsManager.ScreenOrientation);
            TouchPanel.EnabledGestures = GestureType.Tap | GestureType.FreeDrag;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.ApplyChanges();

#else

            if (SettingsManager.IsFullscreen)
            {
                //Window.IsBorderless = true;
                _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                //Window.Position = Point.Zero;
            }
            else
            {
                _graphics.PreferredBackBufferWidth = defaultScreenSize.X;
                _graphics.PreferredBackBufferHeight = defaultScreenSize.Y;
            }

            Window.AllowUserResizing = true;
            IsMouseVisible = true;

            //RnD
            _graphics.IsFullScreen = SettingsManager.IsFullscreen;
            _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            //SetScreenOrientation(SettingsManager.ScreenOrientation);
            TouchPanel.EnabledGestures = GestureType.Tap | GestureType.FreeDrag;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.ApplyChanges();      
#endif
        }

#if ANDROID
        private void SetScreenOrientation(DisplayOrientation orientation)
        {
            if (SettingsManager.IsOrientationLocked)
            {
                SettingsManager.ScreenOrientation = graphics.SupportedOrientations = orientation;
                switch (orientation)
                {
                    case DisplayOrientation.LandscapeLeft:
                        Activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape; break;
                    case DisplayOrientation.LandscapeRight:
                        Activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.ReverseLandscape; break;
                    case DisplayOrientation.Portrait:
                        Activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait; break;
                }
            }
            else
            {
                graphics.SupportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
                Activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Sensor;
            }
        }
#endif

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Glob.SpriteBatch = _spriteBatch;

            _screenManager.LoadContent();
            
            SoundManager.LoadContent(Content);

            InitializeAfterContentIsLoaded();
        }

        protected virtual void InitializeAfterContentIsLoaded()
        {
            _screenManager.AddScreen<MenuGameScreen>();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Go to the previous screen when Back button is pressed. If we are at the last screen, then Exit.
            if (InputManager.IsFocused 
                && (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
                || Keyboard.GetState().IsKeyDown(Keys.Escape)))
            {
                bool? goBackResult = _screenManager.GoBack();
                if (goBackResult != null)
                {
                    SoundManager.PlayOnce(Sound.MenuEscape);
                    if (goBackResult == false)
                    {
//#if WINDOWS
                        Exit();
//#else
//                        // If you Exit() app in Android, it wont come back after restart until you manually kill the process
//                        // This is actually a bug of MonoGame 3.6
//                        //Activity.MoveTaskToBack(true);
//#endif
                    }
                }
            }

            //if (InputManager.IsKeyPressed(Keys.Enter))
            //    ScreenManager.Instance.AddScreen<PanelGameScreen>(true, true, DI.Get<PanelGenerator>().GeneratePanel());

            Glob.Update(gameTime);            
            _screenManager.Update(gameTime);            
            InputManager.Update();
            base.Update(gameTime);
        }
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _screenManager.Draw(_spriteBatch);            

            _spriteBatch.End();            

            base.Draw(gameTime);
        }
    }
}


 
 */
