using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestProject.Cards;

namespace TestProject.Game
{
    public class GameLog : GameListener
    {
        private List<GameMessage> log = new List<GameMessage>();
        public const int MAX_MESSAGES = 10;

        public GameLog()
        {
        }

        public void Update(GameTime time)
        {
            // TODO: Add timing
            foreach (GameMessage message in log)
            {
                message.UpdateTransparency();
            }
        }

        public void NewTurn(Player player)
        {
            AddLogMessage("Your turn " + player.Name + "!", Color.Red);
        }

        public void PlayerJoined(Player player)
        {
            AddLogMessage(player.Name + " has joined.");
        }

        public void SwapState()
        {
            AddLogMessage("You have 15 seconds to swap table cards!", Color.Red);
        }

        public void TriedFromDeck(Player player)
        {
            AddLogMessage(player.Name + " tried a card from the deck.");
        }

        public void PlayedCards(Player player, Card[] cards)
        {
            if (cards.Count() == 1)
            {
                if (cards[0].Number == CardNumber.Two || cards[0].Number == CardNumber.Ten)
                {
                    AddLogMessage(player.Name + " played " + cards[0].ToString() + ".", Color.Purple);
                }
                else
                {
                    AddLogMessage(player.Name + " played " + cards[0].ToString() + ".");
                }
            }
            else
            {
                if (cards[0].Number == CardNumber.Two || cards[0].Number == CardNumber.Ten)
                {
                    AddLogMessage(player.Name + " played " + cards.Count() + " " + cards[0].ToString() + ".", Color.Purple);
                }
                else
                {
                    AddLogMessage(player.Name + " played " + cards.Count() + " " + cards[0].ToString() + ".", Color.Blue);
                }
            }
        }

        public void PickedUpCards(Player player, Card[] cards)
        {
            AddLogMessage(player.Name + " picked up the pile.", Color.Black);
        }

        public void FlippedPile(Player player)
        {
            AddLogMessage(player.Name + " flipped the pile.", Color.Blue);
        }

        public void PlayerWinner(Player player)
        {
            AddLogMessage(player.Name + " has won!", Color.Gold);
        }

        public void Draw(GameTime gameTime, SpriteBatch batch)
        {
            int x = 0;
            int y = batch.GraphicsDevice.Viewport.Bounds.Height - 25;
            for (int i = log.Count - 1; i >= 0; i--)
            {
                batch.DrawString(Configuration.GameFont, log.ElementAt(i).Message, new Vector2(x, y), log.ElementAt(i).Color * log.ElementAt(i).Transparency);
                y -= 25;
            }
        }

        private void AddLogMessage(String message)
        {
            CheckMaxRange();
            log.Add(new GameMessage(message));
        }

        private void AddLogMessage(String message, Color color)
        {
            CheckMaxRange();
            log.Add(new GameMessage(message, color));
        }

        private void CheckMaxRange()
        {
            if (log.Count == MAX_MESSAGES)
            {
                log.RemoveAt(0);
            }
        }
    }

    public class GameMessage
    {
        public String Message { get; private set; }
        public Color Color { get; private set; }
        public float Transparency { get; private set; }

        public GameMessage(String message)
        {
            Message = message;
            Color = Color.White;
            Transparency = 0f;
        }

        public GameMessage(String message, Color color)
        {
            Message = message;
            Color = color;
            Transparency = 0f;
        }

        public void UpdateTransparency()
        {
            if (Transparency >= 1f)
            {
                Transparency = 1f;
            }
            else
            {
                Transparency += 0.025f;
            }
        }
    }
}
