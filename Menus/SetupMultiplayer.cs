using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestProject.Cards;
using TestProject.ScreenManager;
using TestProject.Screens;

namespace TestProject.Menus
{
    public class SetupMultiplayer : Screen, ButtonListener
    {
        private TextField nameField;
        private TextField ipField;
        private TextField portField;
        private List<Card> droppingCards = new List<Card>();
        private const int VELOCITY = 1;

        public SetupMultiplayer(GraphicsDevice device) : base(device, "setupmultiplayer")
        {
            nameField = new TextField("Name", device.Viewport.Width / 2 - 100, device.Viewport.Height / 3, 150, 50);
            ipField = new TextField("127.0.0.1", device.Viewport.Width / 2 - 100, (device.Viewport.Height / 3) + 100, 150, 50);
            ipField.MaxLength = 20;
            portField = new TextField("7772", device.Viewport.Width / 2 - 100, (device.Viewport.Height / 3) + 200, 150, 50);
            Button startButton = new Button("Start", device.Viewport.Width / 2 - 100, (device.Viewport.Height / 3) + 300, 150, 50);
            startButton.AddButtonListener(this);
            AddTextField(nameField);
            AddTextField(ipField);
            AddTextField(portField);
            AddButton(startButton);
        }

        public override bool Init(params object[] param)
        {
            base.Init(param);
            if (param.Count() > 0)
            {
                droppingCards = (List<Card>)param[0];
            }
            return true;
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch)
        {
            device.Clear(Color.Green);
            foreach (Card card in droppingCards)
            {
                card.Draw(gameTime, batch, 0f);
            }
            base.Draw(gameTime, batch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            KeyboardState kbState = Keyboard.GetState();
            Keys[] pressedKeys = kbState.GetPressedKeys();

            foreach (Keys key in pressedKeys)
            {
                if (key == Keys.Escape)
                {
                    Screens.ScreenManager.GotoScreen("menu", droppingCards);
                    return;
                }
            }
            Random random = new Random();
            if (random.Next(100) < 4)
            {
                int number = random.Next(2, 14);
                int suit = random.Next(3);
                Card card = new Card((CardNumber)number, (SuitType)suit);
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
            if (button.Text.Equals("Start"))
            {
                Screens.ScreenManager.GotoScreen("gameboard", "multiplayer", nameField.Text, ipField.Text, Int32.Parse(portField.Text));
            }
        }
    }
}
