using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestProject.Cards;

namespace TestProject.Game
{
    public class PlayerAction
    {
        public List<Card> Cards { get; set; }
        public PlayerActionType Type { get; set; }

        public PlayerAction(PlayerActionType type)
        {
            Type = type;
            Cards = new List<Card>();
        }

        public PlayerAction(PlayerActionType type, params Card[] cards)
        {
            Type = type;
            Cards = new List<Card>();
            Cards.AddRange(cards);
        }
    }

    public enum PlayerActionType
    {
        PLAY_CARD,
        PICK_UP_CARDS,
        TRY_CARD,
        SWAP_CARD
    }
}
