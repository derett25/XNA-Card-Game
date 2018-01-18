using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestProject.Cards;
using TestProject.Game;

namespace TestProject.Multiplayer
{
    public static class MessageParser
    {
        public static String ToPlayerUsername(String msg)
        {
            return msg.Substring(msg.IndexOf('|') + 1);
        }

        public static PlayState ToPlayState(String msg)
        {
            String playState = msg.Substring(msg.IndexOf('|') + 1);
            switch (playState)
            {
                default:
                case "DEAL":
                    return PlayState.DEAL;
                case "SWAP":
                    return PlayState.SWAP;
                case "PLAY":
                    return PlayState.PLAY;
                case "WON":
                    return PlayState.WON;
            }
        }

        public static List<String> ToPlayersConnected(String msg)
        {
            String[] players = msg.Substring(msg.IndexOf('|') + 1).Split(',');
            if (players.Count() > 1 || players[0].Length > 0)
            {
                return players.ToList();
            }
            else
            {
                return null;
            }
        }

        public static String ToReceivedPlayer(String msg)
        {
            return msg.Split('|')[1];
        }

        public static Card ToPlayerReceivedCard(String msg)
        {
            String[] splitMsgType = msg.Split('|');
            return ToCard(splitMsgType[2]);
        }

        // Played swap card
        public static Card ToPlayerSwappedHandCard(String msg)
        {
            String[] splitMsg = msg.Split('|');
            return ToCard(splitMsg[2]);
        }

        public static Card ToPlayerSwappedTableCard(String msg)
        {
            String[] splitMsg = msg.Split('|');
            return ToCard(splitMsg[3]);
        }
        //

        public static Card ToCard(String msg)
        {
            String suitString = msg.Substring(0, msg.IndexOf(','));
            String numberString = msg.Substring(msg.IndexOf(',') + 1);
            SuitType suit = (SuitType)Int32.Parse(suitString);
            CardNumber number = (CardNumber)Int32.Parse(numberString);
            return new Card(number, suit);
        }

        public static String CardToString(Card card)
        {
            return ((int)card.Suit).ToString() + "," + ((int)card.Number).ToString();
        }

        public static string PlayerActionToString(PlayerAction action)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("PLAYER_ACTION|");
            sb.Append(action.Type.ToString());
            if (action.Cards.Count > 0)
            {
                sb.Append("|");
                for (int i = 0; i < action.Cards.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append("|");
                    }
                    sb.Append(CardToString(action.Cards[i]));
                }
            }
            return sb.ToString();
        }

        public static Card[] ToCardsPlayed(String msg)
        {
            List<Card> cardList = new List<Card>();
            String[] splitMsg = msg.Split('|');
            for (int i = 2; i < splitMsg.Count(); i++)
            {
                cardList.Add(ToCard(splitMsg[i]));
            }
            return cardList.ToArray();
        }

        public static Card ToFirstCard(String msg)
        {
            return ToCard(msg.Split('|')[1]);
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
