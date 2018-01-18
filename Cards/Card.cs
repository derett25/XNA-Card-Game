using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestProject.Cards
{
    public class Card : IComparable<Card>
    {
        public CardNumber Number { get; private set; }
        public SuitType Suit { get; private set; }
        public bool AccelerateTowardsOrigin { get; private set; }
        public Rectangle Position = new Rectangle(0, 0, CARD_WIDTH, CARD_HEIGHT);
        public Rectangle OriginPosition = new Rectangle(0, 0, CARD_WIDTH, CARD_HEIGHT);

        private Texture2D cardTexture = null;
        private Texture2D cardBackTexture = null;
        private Vector2 spriteOrigin;
        
        private int velocity = START_VELOCITY;
        public static int CARD_WIDTH = 55;
        public static int CARD_HEIGHT = 80;
        public static int START_VELOCITY = 20;
        public static int VELOCITY_DECREMENT = 0;
        public static int MIN_VELOCITY = 0;

        public Card(CardNumber number, SuitType suit)
        {
            Number = number;
            Suit = suit;
            LoadCard();
            AccelerateTowardsOrigin = false;
        }

        ~Card()
        {
        }

        public void Update(GameTime gameTime)
        {
            // TODO: Add timing
            if (AccelerateTowardsOrigin)
            {
                if (OriginPosition.X > Position.X)
                {
                    Position.X += velocity;
                    if (OriginPosition.X < Position.X)
                    {
                        Position.X = OriginPosition.X;
                    }
                }
                else if (OriginPosition.X < Position.X)
                {
                    Position.X -= velocity;
                    if (OriginPosition.X > Position.X)
                    {
                        Position.X = OriginPosition.X;
                    }
                }

                if (OriginPosition.Y > Position.Y)
                {
                    Position.Y += velocity;
                    if (OriginPosition.Y < Position.Y)
                    {
                        Position.Y = OriginPosition.Y;
                    }
                }
                else if (OriginPosition.Y < Position.Y)
                {
                    Position.Y -= velocity;
                    if (OriginPosition.Y > Position.Y)
                    {
                        Position.Y = OriginPosition.Y;
                    }
                }
                if (Math.Abs(OriginPosition.Y - Position.Y) < 200 || Math.Abs(OriginPosition.X - Position.X) < 200)
                {
                    if ((velocity - VELOCITY_DECREMENT) > MIN_VELOCITY)
                    {
                        velocity -= VELOCITY_DECREMENT;
                    }
                }
                if (OriginPosition.X == Position.X && OriginPosition.Y == Position.Y)
                {
                    AccelerateTowardsOrigin = false;
                }
            }
        }

        public void LoadCard()
        {
            cardBackTexture = Game.Configuration.ContentManager.Load<Texture2D>(Game.Configuration.RESOURCE_PATH + "Card_back");
            String textureName = Suit.ToString();
            switch (Number)
            {
                case CardNumber.Ace:
                    {
                        textureName = String.Concat(textureName, "_ace");
                        break;
                    }
                case CardNumber.Two:
                    {
                        textureName = String.Concat(textureName, "_2");
                        break;
                    }
                case CardNumber.Three:
                    {
                        textureName = String.Concat(textureName, "_3");
                        break;
                    }
                case CardNumber.Four:
                    {
                        textureName = String.Concat(textureName, "_4");
                        break;
                    }
                case CardNumber.Five:
                    {
                        textureName = String.Concat(textureName, "_5");
                        break;
                    }
                case CardNumber.Six:
                    {
                        textureName = String.Concat(textureName, "_6");
                        break;
                    }
                case CardNumber.Seven:
                    {
                        textureName = String.Concat(textureName, "_7");
                        break;
                    }
                case CardNumber.Eight:
                    {
                        textureName = String.Concat(textureName, "_8");
                        break;
                    }
                case CardNumber.Nine:
                    {
                        textureName = String.Concat(textureName, "_9");
                        break;
                    }
                case CardNumber.Ten:
                    {
                        textureName = String.Concat(textureName, "_10");
                        break;
                    }
                case CardNumber.Jack:
                    {
                        textureName = String.Concat(textureName, "_jack");
                        break;
                    }
                case CardNumber.Queen:
                    {
                        textureName = String.Concat(textureName, "_queen");
                        break;
                    }
                case CardNumber.King:
                    {
                        textureName = String.Concat(textureName, "_king");
                        break;
                    }
            }

            // Load resource
            cardTexture = Game.Configuration.ContentManager.Load<Texture2D>(Game.Configuration.RESOURCE_PATH + textureName);
        }

        public void SetPosition(int x, int y)
        {
            Position.X = x;
            Position.Y = y;
        }

        public void SetOriginPosition(int x, int y)
        {
            SetOriginPosition(x, y, true);
        }

        public void SetOriginPosition(int x, int y, bool animation)
        {
            if (animation)
            {
                AccelerateTowardsOrigin = true;
                velocity = START_VELOCITY;
            }
            else
            {
                Position.X = x;
                Position.Y = y;
            }
            OriginPosition.X = x;
            OriginPosition.Y = y;
        }

        public void ResetPosition()
        {
            Position.X = OriginPosition.X;
            Position.Y = OriginPosition.Y;
        }

        public bool Contains(Point point)
        {
            return Position.Contains(point);
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

        public void Draw(GameTime time, SpriteBatch batch, float rotation, bool isFocus, bool isFlipped, float transparency)
        {
            const int FOCUS_OFFSET = 20;
            spriteOrigin = new Vector2(Position.Width / 2, Position.Height / 2);
            if (isFocus)
            {
                Position.Y = Position.Y - FOCUS_OFFSET;
            }
            if (isFlipped)
            {
                if (rotation == 0f)
                {
                    batch.Draw(cardBackTexture, Position, null, Color.White * transparency);
                }
                else
                {
                    batch.Draw(cardBackTexture, Position, null, Color.White * transparency, rotation, spriteOrigin, SpriteEffects.None, 0f);
                }
            }
            else
            {
                if (rotation == 0f)
                {
                    batch.Draw(cardTexture, Position, null, Color.White * transparency);
                }
                else
                {
                    batch.Draw(cardTexture, Position, null, Color.White * transparency, rotation, spriteOrigin, SpriteEffects.None, 0f);
                }
            }
            if (isFocus)
            {
                Position.Y = Position.Y + FOCUS_OFFSET;
            }
        }

        public void Draw(GameTime time, SpriteBatch batch, float rotation, bool isFocus, bool isFlipped)
        {
            Draw(time, batch, rotation, isFocus, isFlipped, 1f);
        }

        public void Draw(GameTime time, SpriteBatch batch, float rotation, bool isFocus)
        {
            Draw(time, batch, rotation, isFocus, false);
        }

        public void Draw(GameTime time, SpriteBatch batch, float rotation)
        {
            Draw(time, batch, rotation, false, false);
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
