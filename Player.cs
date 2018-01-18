using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShitheadServer
{
    public abstract class Player
    {
        protected List<Card> handCards = new List<Card>();
        protected List<Card> tableCards = new List<Card>();
        protected List<Card> tableFlippedCards = new List<Card>();
        public String Name { get; private set; }

        public Player(String name)
        {
            Name = name;
        }

        public void AddHandCards(params Card[] cards)
        {
            foreach (Card c in cards)
            {
                handCards.Add(c);
            }
            SortHandCardsAscending();
        }

        public void AddTableCards(params Card[] cards)
        {
            foreach (Card c in cards)
            {
                tableCards.Add(c);
            }
        }

        public void AddTableFlippedCards(params Card[] cards)
        {
            foreach (Card c in cards)
            {
                tableFlippedCards.Add(c);
            }
        }

        public void RemoveCard(Card removeCard)
        {
            foreach (Card card in handCards)
            {
                if (card.Equals(removeCard))
                {
                    handCards.Remove(removeCard);
                    SortHandCardsAscending();
                    return;
                }
            }
            foreach (Card card in tableCards)
            {
                if (card.Equals(removeCard))
                {
                    tableCards.Remove(removeCard);
                    return;
                }
            }
            foreach (Card card in tableFlippedCards)
            {
                if (card.Equals(removeCard))
                {
                    tableFlippedCards.Remove(removeCard);
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

        public abstract PlayerAction HandleInput(GameState state);

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
