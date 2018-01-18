using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShitheadServer
{
    public static class MessageParser
    {
        public static String PlayerJoinedToString(String username)
        {
            return "NEW_PLAYER|" + username;
        }

        public static String PlayerRemovedToString(String username)
        {
            return "REMOVE_PLAYER|" + username;
        }

        public static String PlayStateToString(PlayState currentPlayState)
        {
            return "PLAY_STATE|" + currentPlayState.ToString();
        }

        public static String PlayerWonToString(Player currentPlayer)
        {
            return "PLAYER_WON|" + currentPlayer.Name;
        }

        public static string PlayersConnectedToString(List<Player> players)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("PLAYERS_CONNECTED|");
            for (int i = 0; i < players.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(",");
                }
                sb.Append(players[i].Name);
            }
            return sb.ToString();
        }

        public static String PlayerReceivedToString(Player player, Card card)
        {
            return "PLAYER_RECEIVE_CARD|" + player.Name + "|" + CardToString(card);
        }

        public static String CardToString(Card card)
        {
            return ((int)card.Suit).ToString() + "," + ((int)card.Number).ToString();
        }

        public static Card ToCard(String msg)
        {
            String suitString = msg.Substring(0, msg.IndexOf(','));
            String numberString = msg.Substring(msg.IndexOf(',') + 1);
            SuitType suit = (SuitType)Int32.Parse(suitString);
            CardNumber number = (CardNumber)Int32.Parse(numberString);
            return new Card(number, suit);
        }

        public static String ToPlayerTurn(Player currentPlayer)
        {
            return "PLAYER_TURN|" + currentPlayer.Name;
        }

        public static String ToFlippedPile()
        {
            return "FLIPPED_PILE";
        }

        public static String SwapCardToString(Player player, Card handCard, Card swapCard)
        {
            return "PLAYER_SWAP_CARD|" + player.Name + "|" + CardToString(handCard) + "|" + CardToString(swapCard);
        }

        public static PlayerAction ToPlayerAction(String message)
        {
            String[] splitMsg = message.Split('|');
            PlayerActionType type;
            Enum.TryParse(splitMsg[1], out type);
            PlayerAction action = new PlayerAction(type);
            if (splitMsg.Count() > 2)
            {
                for (int i = 2; i < splitMsg.Count(); i++)
                {
                    action.Cards.Add(ToCard(splitMsg[i]));
                }
            }
            return action;
        }

        public static String CardsPlayedToString(Player player, Card[] cards)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CARDS_PLAYED|");
            sb.Append(player.Name).Append("|");
            for (int i = 0; i < cards.Count(); i++)
            {
                if (i > 0)
                {
                    sb.Append("|");
                }
                sb.Append(CardToString(cards[i]));
            }
            return sb.ToString();
        }

        public static String FirstCardToString(Card card)
        {
            return "FIRST_CARD|" + CardToString(card);
        }

        public static String PlayerTriedFromDeckToString(Card card)
        {
            return "TRIED_FROM_DECK|" + CardToString(card);
        }

        public static String PlayerPickupCardsToString(Player player, Card[] cards)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("PICKUP_CARDS|");
            sb.Append(player.Name).Append("|");
            for (int i = 0; i < cards.Count(); i++)
            {
                if (i > 0)
                {
                    sb.Append("|");
                }
                sb.Append(CardToString(cards[i]));
            }
            return sb.ToString();
        }

        public static String PlayerDeckCardToString(Card card)
        {
            return "RECEIVE_CARD_DECK|" + CardToString(card);
        }

        public static String PlayerTriedCards(List<Card> cards)
        {
            return "TRIED_CARD|" + CardToString(cards[0]);
        }
    }
}
