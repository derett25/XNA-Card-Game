using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TestProject.Cards;
using System.Net.Sockets;
using TestProject.Game;
using System.Threading;

namespace TestProject.Multiplayer
{
    public class MultiplayerGameState : GameState
    {
        public Deck Deck { get; private set; }
        public List<Player> Players { get; private set; }
        public CardPile CardPile { get; private set; }
        public Player CurrentPlayer { get; private set; }

        public List<GameListener> Listeners { get; private set; }

        public PlayState CurrentPlayState { get; private set; }

        private NetworkThread networkThread;
        private Thread thread;

        public MultiplayerGameState(GraphicsDevice device, String name, String ip, int port)
        {
            Players = new List<Player>();
            Listeners = new List<GameListener>();
            Deck = Deck.CreateFullDeck((device.Viewport.Bounds.Width / 2) - Deck.DECK_WIDTH, (device.Viewport.Bounds.Height / 2) - Deck.DECK_HEIGHT);
            CardPile = new CardPile(Deck.Position.X + 60, Deck.Position.Y);
            CurrentPlayState = PlayState.DEAL;
            try
            {
                NetworkMediator mediator = new NetworkMediator();
                networkThread = new NetworkThread(mediator);
                thread = new Thread(new ThreadStart(networkThread.Run));
                mediator.Connect(ip, port);
                mediator.SendMessage(name);
                String handshake = mediator.ReceiveMessageBlocking()[0];
                if (!handshake.Equals("NOT_OK"))
                {
                    Players.Add(new LocalPlayer(PlayerPositions.South, name));
                    List<String> usernames = MessageParser.ToPlayersConnected(handshake);
                    if (usernames != null)
                    {
                        foreach (String username in usernames)
                        {
                            AddPlayer(username);
                        }
                    }
                    thread.IsBackground = true;
                    thread.Start();
                }
                else
                {
                    Screens.ScreenManager.GotoScreen("menu", "Handshaking failed");
                }
            }
            catch (Exception ex)
            {
                Screens.ScreenManager.GotoScreen("menu", ex.Message);
            }
        }

        ~MultiplayerGameState()
        {
            networkThread.Stop();
            thread.Abort();
        }

        public void AddListener(GameListener listener)
        {
            Listeners.Add(listener);
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

        public void Update(GameTime gameTime)
        {
            CardPile.Update(gameTime);
            foreach (Player player in Players)
            {
                player.Update(gameTime);
            }
            String response = networkThread.GetLatestMessage();
            if (response != null)
            {
                if (response.StartsWith("NEW_PLAYER"))
                {
                    AddPlayer(MessageParser.ToPlayerUsername(response));
                    PlayerJoined(Players.ElementAt(Players.Count - 1));
                }
                else if (response.StartsWith("REMOVE_PLAYER"))
                {
                    String username = MessageParser.ToReceivedPlayer(response);
                    Player player = GetPlayerFromName(username);
                    Players.Remove(player);
                }
                else if (response.StartsWith("PLAYER_WON"))
                {
                    PlayerWinner();
                }
                else if (response.StartsWith("PLAYER_RECEIVE_CARD"))
                {
                    String username = MessageParser.ToReceivedPlayer(response);
                    Card card = MessageParser.ToPlayerReceivedCard(response);
                    Deck.DrawCard();
                    card.SetOriginPosition(Deck.Position.X, Deck.Position.Y, false);
                    Player player = GetPlayerFromName(username);
                    if (player != null)
                    {
                        if (CurrentPlayState == PlayState.DEAL)
                        {
                            if (player.HandCardCount() < 3)
                            {
                                player.AddHandCards(card);
                            }
                            else if (player.TableFlippedCardCount() < 3)
                            {
                                player.AddTableFlippedCards(card);
                            }
                            else
                            {
                                player.AddTableCards(card);
                            }
                        }
                        else
                        {
                            player.AddHandCards(card);
                        }
                    }
                }
                else if (response.StartsWith("PLAY_STATE"))
                {
                    CurrentPlayState = MessageParser.ToPlayState(response);
                    if (CurrentPlayState == PlayState.SWAP)
                    {
                        SwapState();
                    }
                }
                else if (response.StartsWith("PLAYER_SWAP_CARD"))
                {
                    String username = MessageParser.ToReceivedPlayer(response);
                    Player player = GetPlayerFromName(username);
                    if (player != null)
                    {
                        Card handCard = player.GetCard(MessageParser.ToPlayerSwappedHandCard(response));
                        Card tableCard = player.GetCard(MessageParser.ToPlayerSwappedTableCard(response));
                        player.RemoveCard(handCard);
                        player.RemoveCard(tableCard);
                        player.AddHandCards(tableCard);
                        player.AddTableCards(handCard);
                    }
                }
                else if (response.StartsWith("PLAYER_TURN"))
                {
                    String username = MessageParser.ToReceivedPlayer(response);
                    CurrentPlayer = GetPlayerFromName(username);
                    NewTurn();
                }
                else if (response.StartsWith("FIRST_CARD"))
                {
                    Deck.DrawCard();
                    Card card = MessageParser.ToFirstCard(response);
                    card.SetOriginPosition(Deck.Position.X, Deck.Position.Y, false);
                    CardPile.AddCard(card);
                }
                else if (response.StartsWith("FLIPPED_PILE"))
                {
                    CardPile.FlipPile();
                    FlippedPile();
                }
                else if (response.StartsWith("CARDS_PLAYED"))
                {
                    Player player = GetPlayerFromName(MessageParser.ToReceivedPlayer(response));
                    Card[] cards = MessageParser.ToCardsPlayed(response);
                    List<Card> newCards = new List<Card>();
                    foreach (Card card in cards)
                    {
                        Card newCard = player.GetCard(card);
                        newCards.Add(newCard);
                    }
                    AddPlayedCards(player, newCards.ToArray());
                }
                else if (response.StartsWith("TRIED_FROM_DECK"))
                {
                    Deck.DrawCard();
                    Card card = MessageParser.ToFirstCard(response);
                    card.SetOriginPosition(Deck.Position.X, Deck.Position.Y, false);
                    CurrentPlayer.AddHandCards(card);
                }
                else if (response.StartsWith("PICKUP_CARDS"))
                {
                    Card[] cards = MessageParser.ToCardsPlayed(response);
                    List<Card> newCards = new List<Card>();
                    foreach (Card card in cards)
                    {
                        card.SetOriginPosition(CardPile.Position.X, CardPile.Position.Y, false);
                    }
                    CurrentPlayer.AddHandCards(cards);
                    CardPile.Pile.Clear();
                }
                else if (response.StartsWith("RECEIVE_CARD_DECK"))
                {
                    Deck.DrawCard();
                    Card card = MessageParser.ToFirstCard(response);
                    card.SetOriginPosition(Deck.Position.X, Deck.Position.Y, false);
                    CurrentPlayer.AddHandCards(card);
                }
                else if (response.StartsWith("TRIED_CARD"))
                {
                    Deck.DrawCard();
                    Card card = MessageParser.ToFirstCard(response);
                    Card newCard = CurrentPlayer.GetCard(card);
                    CurrentPlayer.RemoveCard(newCard);
                    CurrentPlayer.AddHandCards(newCard);
                    PlayedCards(new Card[] { newCard });
                }
            }
            if (CurrentPlayState == PlayState.SWAP)
            {
                foreach (Player player in Players)
                {
                    if (player is LocalPlayer)
                    {
                        PlayerAction action = player.HandleInput(gameTime, this);
                        if (action != null)
                        {
                            networkThread.SendMessage(MessageParser.PlayerActionToString(action));
                        }
                    }
                }
            }
            else if (CurrentPlayState == PlayState.PLAY)
            {
                bool isLocalPlayer = false;
                if (CurrentPlayer is LocalPlayer)
                {
                    isLocalPlayer = true;
                    PlayerAction action = CurrentPlayer.HandleInput(gameTime, this);
                    if (action != null)
                    {
                        networkThread.SendMessage(MessageParser.PlayerActionToString(action));
                    }
                }
                if (!isLocalPlayer)
                {
                    foreach (Card card in GameCursor.SelectedCards)
                    {
                        card.ResetPosition();
                    }
                    GameCursor.SelectedCards.Clear();
                }
            }
        }

        private Player GetPlayerFromName(String username)
        {
            foreach (Player player in Players)
            {
                if (player.Name.Equals(username))
                {
                    return player;
                }
            }
            return null;
        }

        private void AddPlayer(String username)
        {
            switch (Players.Count)
            {
                case 1:
                    Players.Add(new Bot(PlayerPositions.West, username));
                    break;
                case 2:
                    Players.Add(new Bot(PlayerPositions.North, username));
                    break;
                case 3:
                    Players.Add(new Bot(PlayerPositions.East, username));
                    break;
            }
        }

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
            PlayedCards(cards);
        }

        private void PlayerWinner()
        {
            foreach (GameListener listener in Listeners)
            {
                listener.PlayerWinner(CurrentPlayer);
            }
        }

        private void PlayedCards(Card[] cards)
        {
            foreach (GameListener listener in Listeners)
            {
                listener.PlayedCards(CurrentPlayer, cards);
            }
        }

        private void SwapState()
        {
            foreach (GameListener listener in Listeners)
            {
                listener.SwapState();
            }
        }

        private void PlayerJoined(Player player)
        {
            foreach (GameListener listener in Listeners)
            {
                listener.PlayerJoined(player);
            }
        }

        private void NewTurn()
        {
            foreach (GameListener listener in Listeners)
            {
                listener.NewTurn(CurrentPlayer);
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
    }
}
