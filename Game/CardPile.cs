using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestProject.Cards;

namespace TestProject.Game
{
    public class CardPile
    {
        public Stack<Card> Pile { get; private set; }
        public Rectangle Position { get; private set; }

        private Texture2D placeholder;
        private float transparency = 1f;
        private Card lastTopCard = null;

        public CardPile(int x, int y)
        {
            Pile = new Stack<Card>();
            Position = new Rectangle(x, y, Card.CARD_WIDTH, Card.CARD_HEIGHT);
            placeholder = Configuration.ContentManager.Load<Texture2D>(Configuration.RESOURCE_PATH + "Card_placeholder");
        }

        public void AddCard(Card card)
        {
            Pile.Push(card);
            Pile.Peek().SetOriginPosition(Position.X, Position.Y);
        }

        public void FlipPile()
        {
            lastTopCard = Pile.Peek();
            Pile.Clear();
        }

        public bool Contains(Point point)
        {
            return Position.Contains(point);
        }

        public void Update(GameTime gameTime)
        {
            // TODO: Add timing
            foreach (Card card in Pile)
            {
                card.Update(gameTime);
            }
            if (transparency > 0f && lastTopCard != null)
            {
                transparency -= 0.025f;
            }
            else
            {
                lastTopCard = null;
                transparency = 1f;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch batch)
        {
            if (Pile.Count > 0)
            {
                // We draw the cards in the pile
                if (Pile.Peek().AccelerateTowardsOrigin)
                {
                    // If our top card is animating towards the pile
                    if (Pile.Count > 1)
                    {
                        // Draw the second top card
                        Pile.ElementAt(1).Draw(gameTime, batch, 0f);
                    }
                    else
                    {
                        // Draw the placeholder
                        batch.Draw(placeholder, Position, Color.White);
                    }
                    // Draw the animated card on top
                    Pile.Peek().Draw(gameTime, batch, 0f);
                }
                else
                {
                    Pile.Peek().Draw(gameTime, batch, 0f);
                }
            }
            else if (lastTopCard != null)
            {
                // Draw the animated flipped card
                batch.Draw(placeholder, Position, Color.White);
                lastTopCard.Draw(gameTime, batch, 0f, false, false, transparency);
            }
            else
            {
                // Draw placeholder
                batch.Draw(placeholder, Position, Color.White);
            }
        }
    }
}
