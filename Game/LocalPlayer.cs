using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestProject.Cards;
using Microsoft.Xna.Framework.Graphics;

namespace TestProject.Game
{
    public class LocalPlayer : Player
    {

        private List<Card> selectedCards = new List<Card>();

        public LocalPlayer(PlayerPosition position, String name) : base(position, name)
        {
        }

        public override void Draw(GameTime time, SpriteBatch batch)
        {
            base.Draw(time, batch);
            Card focusedCard = null;
            // One loop to find the focused card
            foreach (Card card in handCards)
            {
                if (card.Contains(GameCursor.MousePosition()) && GameCursor.SelectedCards.Count == 0)
                {
                    focusedCard = card;
                }
            }
            foreach (Card card in handCards)
            {
                if ((focusedCard != null && card.Equals(focusedCard)) || selectedCards.Contains(card))
                {
                    card.Draw(time, batch, Position.Rotation, true);
                }
                else
                {
                    card.Draw(time, batch, Position.Rotation);
                }
            }
            foreach (Card card in tableFlippedCards)
            {
                if (card.Contains(GameCursor.MousePosition()) && GameCursor.SelectedCards.Count == 0)
                {
                    card.Draw(time, batch, Position.Rotation, true, true);
                }
                else
                {
                    card.Draw(time, batch, Position.Rotation, false, true);
                }

            }
            foreach (Card card in tableCards)
            {
                if (card.Contains(GameCursor.MousePosition()) && GameCursor.SelectedCards.Count == 0)
                {
                    card.Draw(time, batch, Position.Rotation, true);
                }
                else
                {
                    card.Draw(time, batch, Position.Rotation);
                }
            }
        }

        public override PlayerAction HandleInput(GameTime gameTime, GameState state)
        {
            // Local input
            if (GameCursor.LeftMouseState(gameTime) == MouseAction.CLICK)
            {
                // Check if you can't play
                if (state.GetDeck().Contains(GameCursor.MousePosition()) && state.GetDeck().CanDrawCard() && GameCursor.SelectedCards.Count == 0)
                {
                    return new PlayerAction(PlayerActionType.TRY_CARD);
                }
                else if (state.GetPile().Contains(GameCursor.MousePosition()) && GameCursor.SelectedCards.Count == 0 && state.GetPile().Pile.Count > 0)
                {
                    return new PlayerAction(PlayerActionType.PICK_UP_CARDS);
                }
                else
                {
                    Card card = SelectedCard(GameCursor.MousePosition());
                    if (card != null && GameCursor.SelectedCards.Count == 0 && (selectedCards.Count == 0 || selectedCards.ElementAt(0).CompareTo(card) == 0))
                    {
                        List<Card> tempCards = new List<Card>();
                        tempCards.AddRange(selectedCards);
                        if (!selectedCards.Contains(card))
                        {
                            tempCards.Add(card);
                        }
                        selectedCards.Clear();
                        GameCursor.SelectedCards.AddRange(tempCards);
                    }
                }
            }
            else if (GameCursor.RightMouseState(gameTime) == MouseAction.CLICK)
            {
                Card card = SelectedCard(GameCursor.MousePosition());
                if (card != null && !GameCursor.SelectedCards.Contains(card))
                {
                    if (selectedCards.Contains(card))
                    {
                        selectedCards.Remove(card);
                    }
                    else if (selectedCards.Count == 0)
                    {
                        selectedCards.Add(card);
                    }
                    else if (selectedCards.ElementAt(0).CompareTo(card) == 0)
                    {
                        selectedCards.Add(card);
                    }
                }
            }
            else if (GameCursor.LeftMouseState(gameTime) == MouseAction.RELEASE)
            {
                if (state.GetPile().Contains(GameCursor.MousePosition()) && GameCursor.SelectedCards.Count > 0)
                {
                    Card[] cards = GameCursor.SelectedCards.ToArray();
                    foreach (Card card in cards)
                    {
                        card.ResetPosition();
                    }
                    GameCursor.SelectedCards.Clear();
                    return new PlayerAction(PlayerActionType.PLAY_CARD, cards);
                }
                else
                {
                    PlayerAction action = null;
                    foreach (Card tableCard in tableCards)
                    {
                        if (tableCard.Contains(GameCursor.MousePosition()) && GameCursor.SelectedCards.Count > 0)
                        {
                            action = new PlayerAction(PlayerActionType.SWAP_CARD, GameCursor.SelectedCards[0], tableCard);
                        }
                    }
                    foreach (Card card in GameCursor.SelectedCards)
                    {
                        card.ResetPosition();
                    }
                    GameCursor.SelectedCards.Clear();
                    return action;
                }
            }
            return null;
        }

        public Card SelectedCard(Point point)
        {
            if (handCards.Count > 0)
            {
                Card selectedCard = null;
                foreach (Card card in handCards)
                {
                    if (card.Contains(point))
                    {
                        selectedCard = card;
                    }
                }
                return selectedCard;
            }
            else if (tableCards.Count > 0)
            {
                Card selectedCard = null;
                foreach (Card card in tableCards)
                {
                    if (card.Contains(point))
                    {
                        selectedCard = card;
                    }
                }
                return selectedCard;
            }
            else
            {
                Card selectedCard = null;
                foreach (Card card in tableFlippedCards)
                {
                    if (card.Contains(point))
                    {
                        selectedCard = card;
                    }
                }
                return selectedCard;
            }
        }
    }
}
