using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestProject.Cards;

namespace TestProject.Game
{

    public class LocalGameState : GameState
    {
        public Deck Deck { get; private set; }
        public List<Player> Players { get; private set; }
        public CardPile CardPile { get; private set; }
        public Player CurrentPlayer { get; private set; }
        
        public List<GameListener> Listeners { get; private set; }

        public PlayState CurrentPlayState { get; private set; }

        private bool hasPlayerPickedFromDeck = false;
        private float elapsedTime = 0f;

        public LocalGameState(GraphicsDevice device, String name, int numberOfBots)
        {
            Players = new List<Player>();
            Listeners = new List<GameListener>();
            Deck = Deck.CreateFullDeck((device.Viewport.Bounds.Width / 2) - Deck.DECK_WIDTH, (device.Viewport.Bounds.Height / 2) - Deck.DECK_HEIGHT);
            CardPile = new CardPile(Deck.Position.X + 60, Deck.Position.Y);
            Deck.Shuffle();
            Players.Add(new LocalPlayer(PlayerPositions.South, name));
            if (numberOfBots <= 3 && numberOfBots > 0)
            {
                for (int i = 0; i < numberOfBots; i++)
                {
                    switch (i)
                    {
                        case 0:
                            if (numberOfBots == 1)
                            {
                                Players.Add(new Bot(PlayerPositions.North, "Joel"));
                            }
                            else
                            {
                                Players.Add(new Bot(PlayerPositions.West, "Kid"));
                            }
                            break;
                        case 1:
                            Players.Add(new Bot(PlayerPositions.North, "Joel"));
                            break;
                        case 2:
                            Players.Add(new Bot(PlayerPositions.East, "Lenny"));
                            break;
                    }
                }
            }
            else
            {
                throw new Exception("Bot count invalid!");
            }
            CurrentPlayState = PlayState.DEAL;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState kbState = Keyboard.GetState();
            Keys[] pressedKeys = kbState.GetPressedKeys();
            foreach (Keys key in pressedKeys)
            {
                if (key == Keys.Escape)
                {
                    Screens.ScreenManager.GotoScreen("menu");
                    return;
                }
            }
            CardPile.Update(gameTime);
            foreach (Player player in Players)
            {
                player.Update(gameTime);
            }
            if (CurrentPlayState == PlayState.DEAL)
            {
                elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                // Handle deal state
                if (elapsedTime > 0.3f)
                {
                    foreach (Player player in Players)
                    {
                        if (player.HandCardCount() < 3)
                        {
                            player.AddHandCards(Deck.DrawCard());
                        }
                        else if (player.TableFlippedCardCount() < 3)
                        {
                            player.AddTableFlippedCards(Deck.DrawCard());
                        }
                        else
                        {
                            player.AddTableCards(Deck.DrawCard());
                        }
                    }
                    if (Players.ElementAt(0).TableCardCount() == 3)
                    {
                        CurrentPlayState = PlayState.SWAP;
                        SwapState();
                    }
                    elapsedTime = 0f;
                }
            }
            else if (CurrentPlayState == PlayState.SWAP)
            {
                elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (elapsedTime > 15f)
                {
                    elapsedTime = 0f;
                    CurrentPlayState = PlayState.PLAY;
                    AddPlayedCards(Deck.DrawCard());
                }
                else
                {
                    foreach (Player player in Players)
                    {
                        PlayerAction action = player.HandleInput(gameTime, this);
                        if (action != null && action.Type == PlayerActionType.SWAP_CARD && action.Cards.Count > 1)
                        {
                            Card handCard = action.Cards[0];
                            Card swapCard = action.Cards[1];
                            player.RemoveCard(handCard);
                            player.RemoveCard(swapCard);
                            player.AddHandCards(swapCard);
                            player.AddTableCards(handCard);
                        }
                    }
                }
            }
            else if (CurrentPlayState == PlayState.PLAY)
            {
                // Handle play state
                if (CurrentPlayer != null)
                {
                    if (CurrentPlayer.HasNoCardsLeft())
                    {
                        CurrentPlayState = PlayState.WON;
                        PlayerWinner();
                    }
                    else
                    {
                        PlayerAction action = CurrentPlayer.HandleInput(gameTime, this);
                        if (action != null)
                        {
                            if (action.Type == PlayerActionType.PLAY_CARD)
                            {
                                if (!PlayCards(action.Cards.ToArray()))
                                {
                                    if (CurrentPlayer.HandCardCount() == 0 || CurrentPlayer.HandCardCount() == 0 && CurrentPlayer.TableCardCount() == 0)
                                    {
                                        // Trying to play a flipped card
                                        foreach (Card card in action.Cards)
                                        {
                                            CurrentPlayer.RemoveCard(card);
                                            CurrentPlayer.AddHandCards(card);
                                        }
                                        PlayedCards(action.Cards.ToArray());
                                        List<Card> pickedUpCards = new List<Card>();
                                        while (CardPile.Pile.Count > 0)
                                        {
                                            Card pickedUpCard = CardPile.Pile.Pop();
                                            pickedUpCards.Add(pickedUpCard);
                                            CurrentPlayer.AddHandCards(pickedUpCard);
                                        }
                                        PickedUpCards(pickedUpCards.ToArray());
                                        NextPlayer();
                                    }
                                    foreach (Card card in action.Cards)
                                    {
                                        card.ResetPosition();
                                    }
                                }
                                else
                                {
                                    // Played a normal hand
                                    while (Deck.CanDrawCard() && CurrentPlayer.HandCardCount() < 3)
                                    {
                                        CurrentPlayer.AddHandCards(Deck.DrawCard());
                                    }
                                    PlayedCards(action.Cards.ToArray());
                                    if (!EvaluateCards(action.Cards.ElementAt(0)))
                                    {
                                        NextPlayer();
                                    }
                                }
                            }
                            else if (action.Type == PlayerActionType.PICK_UP_CARDS)
                            {
                                List<Card> pickedUpCards = new List<Card>();
                                while (CardPile.Pile.Count > 0)
                                {
                                    Card pickedUpCard = CardPile.Pile.Pop();
                                    pickedUpCards.Add(pickedUpCard);
                                    CurrentPlayer.AddHandCards(pickedUpCard);
                                }
                                PickedUpCards(pickedUpCards.ToArray());
                                NextPlayer();
                            }
                            else if (action.Type == PlayerActionType.TRY_CARD)
                            {
                                Card card = DrawCard();
                                if (card != null)
                                {
                                    CurrentPlayer.AddHandCards(card);
                                    TriedFromDeck();
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Set random player as the current player
                    Random random = new Random();
                    CurrentPlayer = Players.ElementAt(random.Next(Players.Count - 1));
                    NewTurn();
                }
            }
            else if (CurrentPlayState == PlayState.WON)
            {
                elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (elapsedTime > 5f)
                {
                    foreach (Player player in Players)
                    {
                        player.ResetCards();
                    }
                    Deck = Deck.CreateFullDeck(Deck.Position.X, Deck.Position.Y);
                    Deck.Shuffle();
                    CardPile.Pile.Clear();
                    CurrentPlayState = PlayState.DEAL;
                    elapsedTime = 0f;
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch batch)
        {
            Deck.Draw(gameTime, batch);
            CardPile.Draw(gameTime, batch);
            foreach (Player player in Players)
            {
                player.Draw(gameTime, batch);
            }
        }

        public PlayState GetPlayState()
        {
            return CurrentPlayState;
        }

        public Card GetTopCard()
        {
            if (CardPile.Pile.Count > 0)
            {
                return CardPile.Pile.Peek();
            }
            else
            {
                return null;
            }
        }

        public Deck GetDeck()
        {
            return Deck;
        }

        public List<Player> GetPlayers()
        {
            return Players;
        }

        public CardPile GetPile()
        {
            return CardPile;
        }

        public Card DrawCard()
        {
            if (!hasPlayerPickedFromDeck)
            {
                Card card = Deck.DrawCard();
                hasPlayerPickedFromDeck = true;
                return card;
            }
            else
            {
                return null;
            }
        }

        private void NextPlayer()
        {
            if (CurrentPlayer.HasNoCardsLeft())
            {
                CurrentPlayState = PlayState.WON;
                PlayerWinner();
            }
            else
            {
                hasPlayerPickedFromDeck = false;
                int index = Players.IndexOf(CurrentPlayer);
                if (index + 1 == Players.Count)
                {
                    index = 0;
                }
                else
                {
                    index++;
                }
                CurrentPlayer = Players.ElementAt(index);
                NewTurn();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="card"></param>
        /// <returns>true if player can play again</returns>
        private bool EvaluateCards(Card card)
        {
            if (card.Number == CardNumber.Two)
            {
                return false;
            }
            else if (card.Number == CardNumber.Ten)
            {
                CardPile.FlipPile();
                FlippedPile();
                return true;
            }
            else
            {
                if (CardPile.Pile.Count >= 4)
                {
                    Card compareCard = null;
                    for (int i = 0; i < 4; i++)
                    {
                        Card pileCard = CardPile.Pile.ElementAt((CardPile.Pile.Count - 1) - i);
                        if (compareCard == null)
                        {
                            compareCard = pileCard;
                        }
                        else if (compareCard.CompareTo(pileCard) != 0)
                        {
                            return false;
                        }
                    }
                    CardPile.FlipPile();
                    FlippedPile();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Tries to play the cards
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        private bool PlayCards(params Card[] cards)
        {
            // Check that all cards are equal
            foreach (Card card in cards)
            {
                foreach (Card card2 in cards)
                {
                    if (card.CompareTo(card2) != 0)
                    {
                        return false;
                    }
                }
            }
            if (CardPile.Pile.Count > 0)
            {
                if (cards[0].Number == CardNumber.Two || cards[0].Number == CardNumber.Ten)
                {
                    AddPlayedCards(CurrentPlayer, cards);
                }
                else if (cards[0].CompareTo(CardPile.Pile.Peek()) > -1)
                {
                    AddPlayedCards(CurrentPlayer, cards);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                AddPlayedCards(CurrentPlayer, cards);
            }
            return true;
        }

        /// <summary>
        /// Adds cards to the played pile
        /// </summary>
        /// <param name="cards"></param>
        private void AddPlayedCards(params Card[] cards)
        {
            AddPlayedCards(null, cards);
        }

        /// <summary>
        /// Adds cards to the played pile
        /// </summary>
        /// <param name="player"></param>
        /// <param name="cards"></param>
        private void AddPlayedCards(Player player, params Card[] cards)
        {
            foreach (Card card in cards)
            {
                CardPile.AddCard(card);
                if (player != null)
                {
                    player.RemoveCard(card);
                }
            }
        }

        private void NewTurn()
        {
            foreach (GameListener listener in Listeners)
            {
                listener.NewTurn(CurrentPlayer);
            }
        }

        private void SwapState()
        {
            foreach (GameListener listener in Listeners)
            {
                listener.SwapState();
            }
        }

        private void PlayedCards(Card[] cards)
        {
            foreach (GameListener listener in Listeners)
            {
                listener.PlayedCards(CurrentPlayer, cards);
            }
        }

        private void PickedUpCards(Card[] cards)
        {
            foreach (GameListener listener in Listeners)
            {
                listener.PickedUpCards(CurrentPlayer, cards);
            }
        }

        private void FlippedPile()
        {
            foreach (GameListener listener in Listeners)
            {
                listener.FlippedPile(CurrentPlayer);
            }
        }

        private void TriedFromDeck()
        {
            foreach (GameListener listener in Listeners)
            {
                listener.TriedFromDeck(CurrentPlayer);
            }
        }

        private void PlayerWinner()
        {
            foreach (GameListener listener in Listeners)
            {
                listener.PlayerWinner(CurrentPlayer);
            }
        }

        public void AddListener(GameListener listener)
        {
            Listeners.Add(listener);
        }
    }
}
