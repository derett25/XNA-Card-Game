using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TestProject.Cards;
using TestProject.Game;

namespace TestProject
{
    public abstract class Player
    {
        protected List<Card> handCards = new List<Card>();
        protected List<Card> tableCards = new List<Card>();
        protected List<Card> tableFlippedCards = new List<Card>();
        public PlayerPosition Position { get; set; }
        public String Name { get; private set; }

        public Player(PlayerPosition position, String name)
        {
            Position = position;
            Name = name;
        }

        public void AddHandCards(params Card[] cards)
        {
            foreach (Card c in cards) {
                handCards.Add(c);
            }
            UpdateHandCardPositions();
        }

        public void AddTableCards(params Card[] cards)
        {
            foreach (Card c in cards)
            {
                tableCards.Add(c);
            }
            UpdateCardPositions();
        }

        public void AddTableFlippedCards(params Card[] cards)
        {
            foreach (Card c in cards)
            {
                tableFlippedCards.Add(c);
            }
            UpdateCardPositions();
        }

        public Card GetCard(Card compareCard)
        {
            foreach (Card card in handCards)
            {
                if (card.Equals(compareCard))
                {
                    return card;
                }
            }
            foreach (Card card in tableCards)
            {
                if (card.Equals(compareCard))
                {
                    return card;
                }
            }
            foreach (Card card in tableFlippedCards)
            {
                if (card.Equals(compareCard))
                {
                    return card;
                }
            }
            return null;
        }

        public void RemoveCard(Card removeCard)
        {
            foreach (Card card in handCards)
            {
                if (card.Equals(removeCard))
                {
                    handCards.Remove(removeCard);
                    UpdateHandCardPositions();
                    return;
                }
            }
            foreach (Card card in tableCards)
            {
                if (card.Equals(removeCard))
                {
                    tableCards.Remove(removeCard);
                    UpdateHandCardPositions();
                    return;
                }
            }
            foreach (Card card in tableFlippedCards)
            {
                if (card.Equals(removeCard))
                {
                    tableFlippedCards.Remove(removeCard);
                    UpdateHandCardPositions();
                    return;
                }
            }
        }

        public int HandCardCount()
        {
            return handCards.Count;
        }

        public int TableCardCount()
        {
            return tableCards.Count;
        }

        public int TableFlippedCardCount()
        {
            return tableFlippedCards.Count;
        }

        public bool HasNoCardsLeft()
        {
            return handCards.Count == 0 && tableCards.Count == 0 && tableFlippedCards.Count == 0;
        }

        public void ResetCards()
        {
            handCards.Clear();
            tableCards.Clear();
            tableFlippedCards.Clear();
        }

        public abstract PlayerAction HandleInput(GameTime gameTime, GameState state);

        public void Update(GameTime gameTime)
        {
            foreach (Card card in handCards)
            {
                card.Update(gameTime);
            }
            foreach (Card card in tableCards)
            {
                card.Update(gameTime);
            }
            foreach (Card card in tableFlippedCards)
            {
                card.Update(gameTime);
            }
        }

        public virtual void Draw(GameTime time, SpriteBatch batch)
        {
            if (Position.Name.Equals("south"))
            {
                batch.DrawString(Configuration.GameFont, Name, new Vector2(Position.X + (80 - (Name.Count() * 3)), Position.Y - 35), Color.White);
            }
            else if (Position.Name.Equals("north"))
            {
                batch.DrawString(Configuration.GameFont, Name, new Vector2(Position.X + (60 - (Name.Count() * 3)), Position.Y + 50), Color.White);
            }
            else if (Position.Name.Equals("west"))
            {
                batch.DrawString(Configuration.GameFont, Name, new Vector2(Position.X + (60 - (Name.Count() * 3)), Position.Y + 160), Color.White);
            }
            else if (Position.Name.Equals("east"))
            {
                batch.DrawString(Configuration.GameFont, Name, new Vector2(Position.X + (60 - (Name.Count() * 3)), Position.Y + 160), Color.White);
            }
        }

        /// <summary>
        /// Updates the cards positions to line up with the player position
        /// </summary>
        private void UpdateCardPositions()
        {
            UpdateHandCardPositions();
            if (Position.Name.Equals("south") || Position.Name.Equals("north"))
            {
                int x = Position.X;
                foreach (Card card in tableFlippedCards)
                {
                    card.SetOriginPosition(x, Position.Y);
                    x += 60;
                }
                x = Position.X;
                foreach (Card card in tableCards)
                {
                    card.SetOriginPosition(x, Position.Y);
                    x += 60;
                }
            }
            else if (Position.Name.Equals("west"))
            {
                int y = Position.Y;
                foreach (Card card in tableFlippedCards)
                {
                    card.SetOriginPosition(Position.X + 100, y);
                    y += 60;
                }
                y = Position.Y;
                foreach (Card card in tableCards)
                {
                    card.SetOriginPosition(Position.X + 100, y);
                    y += 60;
                }
            }
            else if (Position.Name.Equals("east"))
            {
                int y = Position.Y;
                foreach (Card card in tableFlippedCards)
                {
                    card.SetOriginPosition(Position.X + 50, y);
                    y += 60;
                }
                y = Position.Y;
                foreach (Card card in tableCards)
                {
                    card.SetOriginPosition(Position.X + 50, y);
                    y += 60;
                }
            }
        }

        private void UpdateHandCardPositions()
        {
            SortHandCardsAscending();
            if (Position.Name.Equals("south"))
            {
                int x = Position.X + (70 - (handCards.Count * 10));
                foreach (Card card in handCards)
                {
                    card.SetOriginPosition(x, Position.Y + 100);
                    x += 20;
                }
            }
            else if (Position.Name.Equals("north"))
            {
                int x = Position.X + (70 - (handCards.Count * 10));
                foreach (Card card in handCards)
                {
                    card.SetOriginPosition(x, Position.Y - 100);
                    x += 20;
                }
            }
            else if (Position.Name.Equals("west"))
            {
                int y = Position.Y + (70 - (handCards.Count * 10));
                foreach (Card card in handCards)
                {
                    card.SetOriginPosition(Position.X, y);
                    y += 20;
                }
            }
            else if (Position.Name.Equals("east"))
            {
                int y = Position.Y + (70 - (handCards.Count * 10));
                foreach (Card card in handCards)
                {
                    card.SetOriginPosition(Position.X + 150, y);
                    y += 20;
                }
            }
        }

        public void SortHandCardsAscending()
        {
            List<Card> cardList = handCards.ToList();
            handCards.Clear();
            cardList.Sort();
            foreach (Card c in cardList)
            {
                handCards.Add(c);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Player)
            {
                Player otherPlayer = (Player)obj;
                return this.Name.Equals(otherPlayer.Name);
            }
            else
            {
                return base.Equals(obj);
            }
        }
    }
}
