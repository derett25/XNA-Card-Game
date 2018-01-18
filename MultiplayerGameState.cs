using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace ShitheadServer
{
    public class MultiplayerGameState : GameState
    {
        public Deck Deck { get; private set; }
        public List<Player> Players { get; private set; }
        public Stack<Card> Pile { get; private set; }
        public Player CurrentPlayer { get; private set; }

        public PlayState CurrentPlayState { get; private set; }

        private bool hasPlayerPickedFromDeck = false;
        private Stopwatch stopWatch = new Stopwatch();

        public MultiplayerGameState()
        {
            Pile = new Stack<Card>();
            Players = new List<Player>();
            CurrentPlayState = PlayState.NONE;
            Deck = Deck.CreateFullDeck();
            Deck.Shuffle();
        }

        public void Shutdown()
        {
            foreach (Player player in Players)
            {
                if (player is NetworkPlayer)
                {
                    NetworkPlayer networkPlayer = (NetworkPlayer)player;
                    networkPlayer.Shutdown();
                }
            }
        }

        public bool AddBot(String name)
        {
            if ((Players.Count + 1) <= 4 && CurrentPlayState == PlayState.NONE && UsernameIsUnique(name))
            {
                Players.Add(new Bot(name));
                Broadcast(MessageParser.PlayerJoinedToString(name));
                return true;
            }
            return false;
        }

        public bool AddPlayer(String username, TcpClient client)
        {
            if ((Players.Count + 1) <= 4 && CurrentPlayState == PlayState.NONE && UsernameIsUnique(username))
            {
                Broadcast(MessageParser.PlayerJoinedToString(username));
                NetworkPlayer player = new NetworkPlayer(username, client);
                player.SendMessage(MessageParser.PlayersConnectedToString(Players));
                Players.Add(player);
                return true;
            }
            return false;
        }

        public void Update()
        {
            if (CurrentPlayState == PlayState.NONE && Players.Count == 4)
            {
                StartGame();
            }
            foreach (Player player in Players.ToList())
            {
                if (player is NetworkPlayer)
                {
                    NetworkPlayer networkPlayer = (NetworkPlayer)player;
                    if (!networkPlayer.IsConnected())
                    {
                        Broadcast(MessageParser.PlayerRemovedToString(player.Name));
                        Players.Remove(player);
                        Console.WriteLine("Player {0} left", player.Name);
                        if (CurrentPlayer == player)
                        {
                            NextPlayer();
                        }
                    }
                }
            }
            if (CurrentPlayState == PlayState.DEAL)
            {
                if (stopWatch.ElapsedMilliseconds > 300)
                {
                    foreach (Player player in Players)
                    {
                        if (player.HandCardCount() < 3)
                        {
                            Card card = Deck.DrawCard();
                            player.AddHandCards(card);
                            Console.WriteLine("{0} received {1}", player.Name, card.ToString());
                            Broadcast(MessageParser.PlayerReceivedToString(player, card));
                        }
                        else if (player.TableFlippedCardCount() < 3)
                        {
                            Card card = Deck.DrawCard();
                            player.AddTableFlippedCards(card);
                            Console.WriteLine("{0} received {1}", player.Name, card.ToString());
                            Broadcast(MessageParser.PlayerReceivedToString(player, card));
                        }
                        else
                        {
                            Card card = Deck.DrawCard();
                            player.AddTableCards(card);
                            Console.WriteLine("{0} received {1}", player.Name, card.ToString());
                            Broadcast(MessageParser.PlayerReceivedToString(player, card));
                        }
                    }
                    if (Players.ElementAt(0).TableCardCount() == 3)
                    {
                        CurrentPlayState = PlayState.SWAP;
                        Console.WriteLine("Changed STATE to SWAP!");
                        Broadcast(MessageParser.PlayStateToString(CurrentPlayState));
                    }
                    stopWatch.Restart();
                }
            }
            else if (CurrentPlayState == PlayState.SWAP)
            {
                if (stopWatch.ElapsedMilliseconds > 15000)
                {
                    CurrentPlayState = PlayState.PLAY;
                    Card drawCard = Deck.DrawCard();
                    AddPlayedCards(drawCard);
                    Console.WriteLine("Changed STATE to PLAY!");
                    Broadcast(MessageParser.FirstCardToString(drawCard));
                    Console.WriteLine("First card is {0}", drawCard.ToString());
                    Broadcast(MessageParser.PlayStateToString(CurrentPlayState));
                    stopWatch.Stop();
                }
                else
                {
                    foreach (Player player in Players)
                    {
                        PlayerAction action = player.HandleInput(this);
                        if (action != null && action.Type == PlayerActionType.SWAP_CARD && action.Cards.Count > 1)
                        {
                            Card handCard = action.Cards[0];
                            Card swapCard = action.Cards[1];
                            player.RemoveCard(handCard);
                            player.RemoveCard(swapCard);
                            player.AddHandCards(swapCard);
                            player.AddTableCards(handCard);
                            Console.WriteLine("{0} swapped {1} with {2}", player.Name, handCard.ToString(), swapCard.ToString());
                            Broadcast(MessageParser.SwapCardToString(player, handCard, swapCard));
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
                        Console.WriteLine("Changed STATE to WON!");
                        stopWatch.Start();
                        Broadcast(MessageParser.PlayerWonToString(CurrentPlayer));
                    }
                    else
                    {
                        PlayerAction action = CurrentPlayer.HandleInput(this);
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
                                        Console.WriteLine("{0} tried {1}", CurrentPlayer.Name, action.Cards[0].ToString());
                                        Broadcast(MessageParser.PlayerTriedCards(action.Cards));
                                        List<Card> pickedUpCards = new List<Card>();
                                        while (Pile.Count > 0)
                                        {
                                            Card pickedUpCard = Pile.Pop();
                                            pickedUpCards.Add(pickedUpCard);
                                            CurrentPlayer.AddHandCards(pickedUpCard);
                                        }
                                        Console.WriteLine("{0} picked up cards", CurrentPlayer.Name);
                                        Broadcast(MessageParser.PlayerPickupCardsToString(CurrentPlayer, pickedUpCards.ToArray()));
                                        NextPlayer();
                                    }
                                }
                                else
                                {
                                    // Played a normal hand
                                    while (Deck.CanDrawCard() && CurrentPlayer.HandCardCount() < 3)
                                    {
                                        Card card = Deck.DrawCard();
                                        CurrentPlayer.AddHandCards(card);
                                        Broadcast(MessageParser.PlayerDeckCardToString(card));
                                    }
                                    Console.WriteLine("{0} played {1}", CurrentPlayer.Name, action.Cards[0].ToString());
                                    Broadcast(MessageParser.CardsPlayedToString(CurrentPlayer, action.Cards.ToArray()));
                                    if (!EvaluateCards(action.Cards.ElementAt(0)))
                                    {
                                        NextPlayer();
                                    }
                                }
                            }
                            else if (action.Type == PlayerActionType.PICK_UP_CARDS)
                            {
                                List<Card> pickedUpCards = new List<Card>();
                                while (Pile.Count > 0)
                                {
                                    Card pickedUpCard = Pile.Pop();
                                    pickedUpCards.Add(pickedUpCard);
                                    CurrentPlayer.AddHandCards(pickedUpCard);
                                }
                                Console.WriteLine("{0} picked up cards", CurrentPlayer.Name);
                                Broadcast(MessageParser.PlayerPickupCardsToString(CurrentPlayer, pickedUpCards.ToArray()));
                                NextPlayer();
                            }
                            else if (action.Type == PlayerActionType.TRY_CARD)
                            {
                                Card card = DrawCard();
                                if (card != null)
                                {
                                    CurrentPlayer.AddHandCards(card);
                                    Console.WriteLine("{0} tried {1}", CurrentPlayer.Name, card.ToString());
                                    Broadcast(MessageParser.PlayerTriedFromDeckToString(card));
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
                    Broadcast(MessageParser.ToPlayerTurn(CurrentPlayer));
                }
            }
            else if (CurrentPlayState == PlayState.WON)
            {
                if (stopWatch.ElapsedMilliseconds > 5000)
                {
                    foreach (Player player in Players)
                    {
                        player.ResetCards();
                    }
                    Deck = Deck.CreateFullDeck();
                    Deck.Shuffle();
                    Pile.Clear();
                    CurrentPlayState = PlayState.DEAL;
                    stopWatch.Restart();
                }
            }
        }

        public void StartGame()
        {
            Console.WriteLine("Game has started!");
            stopWatch.Start();
            CurrentPlayState = PlayState.DEAL;
        }

        public PlayState GetPlayState()
        {
            return CurrentPlayState;
        }

        public Card GetTopCard()
        {
            if (Pile.Count > 0)
            {
                return Pile.Peek();
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

        public Stack<Card> GetPile()
        {
            return Pile;
        }

        public List<Player> GetPlayers()
        {
            return Players;
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
                stopWatch.Stop();
                stopWatch.Start();
                Broadcast(MessageParser.PlayerWonToString(CurrentPlayer));
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
                Broadcast(MessageParser.ToPlayerTurn(CurrentPlayer));
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
                Pile.Clear();
                Broadcast(MessageParser.ToFlippedPile());
                return true;
            }
            else
            {
                if (Pile.Count >= 4)
                {
                    Card compareCard = null;
                    for (int i = 0; i < 4; i++)
                    {
                        Card pileCard = Pile.ElementAt((Pile.Count - 1) - i);
                        if (compareCard == null)
                        {
                            compareCard = pileCard;
                        }
                        else if (compareCard.CompareTo(pileCard) != 0)
                        {
                            return false;
                        }
                    }
                    Pile.Clear();
                    Broadcast(MessageParser.ToFlippedPile());
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
            if (Pile.Count > 0)
            {
                if (cards[0].Number == CardNumber.Two || cards[0].Number == CardNumber.Ten)
                {
                    AddPlayedCards(CurrentPlayer, cards);
                }
                else if (cards[0].CompareTo(Pile.Peek()) > -1)
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
                Pile.Push(card);
                if (player != null)
                {
                    player.RemoveCard(card);
                }
            }
        }

        private bool UsernameIsUnique(String username)
        {
            foreach (Player player in Players)
            {
                if (player.Name.Equals(username))
                {
                    return false;
                }
            }
            return true;
        }

        private void Broadcast(params Object[] param)
        {
            String message = (String) param[0];
            foreach (Player player in Players)
            {
                if (player is NetworkPlayer)
                {
                    NetworkPlayer networkPlayer = (NetworkPlayer)player;
                    if (networkPlayer.IsConnected())
                    {
                        new Thread(() =>
                        {
                            lock (networkPlayer)
                            {
                                Thread.CurrentThread.IsBackground = true;
                                networkPlayer.SendMessage("^");
                                networkPlayer.SendMessage(message);
                            }
                        }).Start();
                    }
                }
            }
        }
    }

    public enum PlayState
    {
        DEAL,
        SWAP,
        PLAY,
        WON,
        NONE
    }
}