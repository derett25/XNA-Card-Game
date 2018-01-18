using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TestProject.Game;
using Microsoft.Xna.Framework.Input;
using TestProject.Cards;
using TestProject.ScreenManager;
using TestProject.Screens;

namespace TestProject.Menus
{
    public class Menu : Screen, ButtonListener
    {
        private List<Card> droppingCards = new List<Card>();
        private const int VELOCITY = 1;
        private Label errorText;

        public Menu(GraphicsDevice device) : base(device, "menu")
        {
            Button local = new Button("Local", device.Viewport.Width / 2 - 100, device.Viewport.Height / 2, 150, 50);
            local.AddButtonListener(this);
            AddButton(local);
            Button network = new Button("Network", device.Viewport.Width / 2 - 100, (device.Viewport.Height / 2) + 100, 150, 50);
            network.AddButtonListener(this);
            AddButton(network);
            errorText = new Label("", 0, device.Viewport.Height - 25, Color.Red, Configuration.ButtonFont);
            AddLabel(errorText);
        }

        ~Menu()
        {

        }

        public override bool Init(params Object[] param)
        {
            if (param.Count() > 0)
            {
                if (param[0] is String)
                {
                    errorText.Text = (String)param[0];
                    errorText.Animate = true;
                    errorText.Transparency = 1f;
                }
                else
                {
                    droppingCards = (List<Card>)param[0];
                }
            }
            return base.Init();
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch)
        {
            device.Clear(Color.Green);
            foreach (Card card in droppingCards)
            {
                card.Draw(gameTime, batch, 0f);
            }
            batch.DrawString(Configuration.TitleFont, "Shithead", new Vector2((batch.GraphicsDevice.Viewport.Width / 2) - 140, 50), Color.White);
            base.Draw(gameTime, batch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Random random = new Random();
            if (random.Next(100) < 4)
            {
                int number = random.Next(2, 14);
                int suit = random.Next(3);
                Card card = new Card((CardNumber) number, (SuitType) suit);
                card.SetOriginPosition(random.Next(device.Viewport.Width), -100, false);
                droppingCards.Add(card);
            }
            List<Card> removeCards = new List<Card>();
            foreach (Card card in droppingCards)
            {
                card.SetPosition(card.Position.X, card.Position.Y + VELOCITY);
                if (card.Position.Y > (device.Viewport.Height + 100))
                {
                    removeCards.Add(card);
                }
            }
            foreach (Card card in removeCards)
            {
                droppingCards.Remove(card);
            }
            removeCards.Clear();
        }

        public void Clicked(Button button)
        {
            if (button.Text.Equals("Local"))
            {
                Screens.ScreenManager.GotoScreen("setuplocal", droppingCards);
                //Screens.ScreenManager.GotoScreen("gameboard", "local", 3);
            }
            else if (button.Text.Equals("Network"))
            {
                Screens.ScreenManager.GotoScreen("setupmultiplayer", droppingCards);
            }
        }
    }
}
