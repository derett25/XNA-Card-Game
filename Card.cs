using System;

namespace ShitheadServer
{
    public class Card : IComparable<Card>
    {
        public CardNumber Number { get; private set; }
        public SuitType Suit { get; private set; }

        public Card(CardNumber number, SuitType suit)
        {
            Number = number;
            Suit = suit;
        }

        ~Card()
        {
        }

        public int CompareTo(Card other)
        {
            // Card number is most important for our sorting
            if (Number != other.Number)
            {
                return Number.CompareTo(other.Number);
            }

            // The card numbers are the same
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is Card)
            {
                Card otherCard = (Card)obj;
                return this.Number == otherCard.Number && this.Suit == otherCard.Suit;
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override string ToString()
        {
            return Number.ToString() + " of " + Suit.ToString();
        }
    }

    public enum CardNumber
    {
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public enum SuitType
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }
}
