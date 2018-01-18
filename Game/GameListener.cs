using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestProject.Cards;

namespace TestProject.Game
{
    public interface GameListener
    {
        void NewTurn(Player player);
        void PlayerJoined(Player player);
        void SwapState();
        void TriedFromDeck(Player player);
        void PlayedCards(Player player, Card[] cards);
        void PickedUpCards(Player player, Card[] cards);
        void FlippedPile(Player player);
        void PlayerWinner(Player player);
    }
}
