using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestProject.Cards
{
    public class Deck
    {
        private static Random random = new Random();

        public Stack<Card> Cards { get; private set; }

        public Rectangle Position { get; private set; }

        private static Texture2D deckTexture;
        public static int DECK_WIDTH = 55;
        public static int DECK_HEIGHT = 80;

        private Deck(int x, int y)
        {
            Cards = new Stack<Card>();
            Position = new Rectangle(x, y, DECK_WIDTH, DECK_HEIGHT);
        }

        public static void Init()
        {
            deckTexture = Game.Configuration.ContentManager.Load<Texture2D>(Game.Configuration.RESOURCE_PATH + "Deck_back");
        }

        public static Deck CreateFullDeck(int x, int y)
        {
            Deck deck = new Deck(x, y);
            for (int suitIndex = 0; suitIndex < 4; suitIndex++)
            {
                for (int cardNumberIndex = 2; cardNumberIndex <= 14; cardNumberIndex++)
                {
                    Card card = new Card((CardNumber)cardNumberIndex, (SuitType)suitIndex);
                    card.SetOriginPosition(x, y, false);
                    deck.Cards.Push(card);
                }
            }
            return deck;
        }

        public void SortAscending()
        {
            List<Card> cardList = Cards.ToList();
            Cards.Clear();
            cardList.Sort();
            foreach (Card c in cardList) {
                Cards.Push(c);
            }
        }

        public void Shuffle()
        {
            List<Card> cardsToShuffle = new List<Card>(Cards);
            Cards.Clear();
            while (cardsToShuffle.Count > 0)
            {
                var cardIndex = random.Next(cardsToShuffle.Count);

                var cardToShuffle = cardsToShuffle[cardIndex];
                cardsToShuffle.RemoveAt(cardIndex);

                Cards.Push(cardToShuffle);
            }
        }

        public Card DrawCard()
        {
            if (Cards.Count > 0)
            {
                return Cards.Pop();
            } else
            {
                return null;
            }
        }

        public bool CanDrawCard()
        {
            return Cards.Count > 0;
        }

        public void Draw(GameTime time, SpriteBatch batch)
        {
            if (Cards.Count > 0)
            {
                batch.Draw(deckTexture, Position, Color.White);
            }
        }

        public bool Contains(Point point)
        {
            return Position.Contains(point);
        }
    }
}
