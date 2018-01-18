using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TestProject.Cards;
using TestProject.Game;
using TestProject.Screens;
using TestProject.Multiplayer;

namespace TestProject.Menus
{
    public class GameBoard : Screen
    {
        // Game logic implementation (server or client)
        private GameState state;
        private GameLog log;

        public GameBoard(GraphicsDevice device) : base(device, "gameboard")
        {
        }

        public override bool Init(params object[] param)
        {
            base.Init(param);
            log = new GameLog();
            // Local or multiplayer?
            if (param[0].Equals("local"))
            {
                String name = (String) param[1];
                int bots = (int)param[2];
                state = new LocalGameState(device, name, bots);
            }
            else
            {
                String name = (String)param[1];
                String ip = (String)param[2];
                int port = (int)param[3];
                state = new MultiplayerGameState(device, name, ip, port);
            }
            state.AddListener(log);
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            state.Update(gameTime);
            log.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch)
        {
            base.Draw(gameTime, batch);
            device.Clear(Color.Green);
            state.Draw(gameTime, batch);
            log.Draw(gameTime, batch);
        }
    }
}
