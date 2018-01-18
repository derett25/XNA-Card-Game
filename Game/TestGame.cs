using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TestProject.Game;
using TestProject.Cards;
using TestProject.ScreenManager;
using TestProject.Menus;

namespace TestProject
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TestGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager deviceManager;
        SpriteBatch spriteBatch;

        public static float FPS = 144.0f;

        public TestGame()
        {
            deviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Window initilization
            Window.AllowUserResizing = false;
            Window.Title = "Shithead";

            // Device manager initilization
            deviceManager.IsFullScreen = false;
            deviceManager.PreferredBackBufferHeight = 768;
            deviceManager.PreferredBackBufferWidth = 1024;
            deviceManager.ApplyChanges();

            // Game initilization
            Configuration.Init(this.Content);
            GameCursor.Init();
            GameCursor.CustomMouse = true;
            Deck.Init();
            Button.Init(GraphicsDevice);
            TextField.Init(GraphicsDevice);
            PlayerPositions.Init(GraphicsDevice);

            // Screen initilization
            Screens.ScreenManager.AddScreen(new Menu(GraphicsDevice));
            Screens.ScreenManager.AddScreen(new GameBoard(GraphicsDevice));
            Screens.ScreenManager.AddScreen(new SetupLocal(GraphicsDevice));
            Screens.ScreenManager.AddScreen(new SetupMultiplayer(GraphicsDevice));
            Screens.ScreenManager.GotoScreen("menu");

            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / FPS);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Screens.ScreenManager.Init();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Screens.ScreenManager.Shutdown();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            GameCursor.Update(gameTime);
            Screens.ScreenManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin();
            Screens.ScreenManager.Draw(gameTime, spriteBatch);
            GameCursor.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
    }
}
