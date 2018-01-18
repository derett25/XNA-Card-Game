using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShitheadServer
{
    public class Deck
    {
        private static Random random = new Random();
        public Stack<Card> Cards { get; private set; }

        private Deck()
        {
            Cards = new Stack<Card>();
        }

        public static Deck CreateFullDeck()
        {
            Deck deck = new Deck();
            for (int suitIndex = 0; suitIndex < 4; suitIndex++)
            {
                for (int cardNumberIndex = 2; cardNumberIndex <= 14; cardNumberIndex++)
                {
                    Card card = new Card((CardNumber)cardNumberIndex, (SuitType)suitIndex);
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
            foreach (Card c in cardList)
            {
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
            }
            else
            {
                return null;
            }
        }

        public bool CanDrawCard()
        {
            return Cards.Count > 0;
        }
    }
}
