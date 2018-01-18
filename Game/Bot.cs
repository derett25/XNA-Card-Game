using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TestProject.Cards;

namespace TestProject.Game
{
    public class Bot : Player
    {

        private float elapsedTime = 0f;
        private float randomWait = 0f;
        private bool triedFromDeck = false;

        public Bot(PlayerPosition position, String name) : base(position, name)
        {
            randomWait = CalculateNewRandomWait();
        }

        public override void Draw(GameTime time, SpriteBatch batch)
        {
            base.Draw(time, batch);
            foreach (Card card in handCards)
            {
                card.Draw(time, batch, Position.Rotation, false, true);
            }
            foreach (Card card in tableFlippedCards)
            {
                card.Draw(time, batch, Position.Rotation, false, true);
            }
            foreach (Card card in tableCards)
            {
                card.Draw(time, batch, Position.Rotation);
            }
        }

        public override PlayerAction HandleInput(GameTime gameTime, GameState state)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsedTime > randomWait)
            {
                randomWait = CalculateNewRandomWait();
                elapsedTime = 0f;
                if (state.GetPlayState() == PlayState.PLAY)
                {
                    Card card = state.GetTopCard();
                    if (card == null)
                    {
                        Card[] lowestCards = PlayLowestPossibleCards();
                        if (lowestCards == null)
                        {
                            return PlayCards(PlayTwoOrTen());
                        }
                        return PlayCards(lowestCards);
                    }
                    else
                    {
                        Card[] lowestCards = PlayLowestPossibleCards(card);
                        if (lowestCards != null)
                        {
                            return PlayCards(lowestCards);
                        }
                        else
                        {
                            Card twoOrTen = PlayTwoOrTen();
                            if (twoOrTen != null)
                            {
                                return PlayCards(twoOrTen);
                            }
                            else
                            {
                                if (!triedFromDeck)
                                {
                                    triedFromDeck = true;
                                    return new PlayerAction(PlayerActionType.TRY_CARD);
                                }
                                triedFromDeck = false;
                                return new PlayerAction(PlayerActionType.PICK_UP_CARDS);
                            }
                        }

                    }
                }
            }
            return null;
        }

        private Card[] PlayLowestPossibleCards()
        {
            triedFromDeck = false;
            return PlayLowestPossibleCards(null);
        }

        private Card[] PlayLowestPossibleCards(Card toBeat)
        {
            Card lowestCard = null;
            if (handCards.Count > 0)
            {
                foreach (Card card in handCards)
                {
                    if (card.Number != CardNumber.Two && card.Number != CardNumber.Ten)
                    {
                        if (lowestCard == null && (toBeat == null || card.CompareTo(toBeat) > -1))
                        {
                            lowestCard = card;
                        }
                        else if (lowestCard != null && lowestCard.CompareTo(card) > 0 && (toBeat == null || card.CompareTo(toBeat) > -1))
                        {
                            lowestCard = card;
                        }
                    }
                }
            }
            else if (tableCards.Count > 0)
            {
                foreach (Card card in tableCards)
                {
                    if (card.Number != CardNumber.Two && card.Number != CardNumber.Ten)
                    {
                        if (lowestCard == null && (toBeat == null || card.CompareTo(toBeat) > -1))
                        {
                            lowestCard = card;
                        }
                        else if (lowestCard != null && lowestCard.CompareTo(card) > 0 && (toBeat == null || card.CompareTo(toBeat) > -1))
                        {
                            lowestCard = card;
                        }
                    }
                }
            }
            else
            {
                Random rand = new Random();
                int index = rand.Next(tableFlippedCards.Count - 1);
                return new Card[] { tableFlippedCards.ElementAt(index) };
            }
            if (lowestCard != null)
            {
                return PlayAllEqualCards(lowestCard);
            }
            else
            {
                return null;
            }
        }

        private Card[] PlayAllEqualCards(Card lowCard)
        {
            List<Card> cards = new List<Card>();
            cards.Add(lowCard);
            foreach (Card card in handCards)
            {
                if (!card.Equals(lowCard) && card.Number == lowCard.Number)
                {
                    cards.Add(card);
                }
            }
            return cards.ToArray();
        }

        private Card PlayTwoOrTen()
        {
            Card two = null;
            Card ten = null;
            if (handCards.Count > 0)
            {
                foreach (Card card in handCards)
                {
                    if (card.Number == CardNumber.Two)
                    {
                        two = card;
                    }
                    else if (card.Number == CardNumber.Ten)
                    {
                        ten = card;
                    }
                }
            }
            else if (tableCards.Count > 0)
            {
                foreach (Card card in tableCards)
                {
                    if (card.Number == CardNumber.Two)
                    {
                        two = card;
                    }
                    else if (card.Number == CardNumber.Ten)
                    {
                        ten = card;
                    }
                }
            }
            if (ten != null)
            {
                return ten;
            }
            else
            {
                return two;
            }
        }

        private PlayerAction PlayCards(params Card[] cards)
        {
            return new PlayerAction(PlayerActionType.PLAY_CARD, cards);
        }

        private float CalculateNewRandomWait()
        {
            float min = 0.5f;
            float max = 2.5f;
            Random random = new Random();
            return (float) random.NextDouble() * (max - min) + min;
        }
    }
}
