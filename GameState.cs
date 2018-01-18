using System.Collections.Generic;

namespace ShitheadServer
{
    public interface GameState
    {
        PlayState GetPlayState();
        Card GetTopCard();
        Deck GetDeck();
        Stack<Card> GetPile();
        List<Player> GetPlayers();
        Card DrawCard();
    }
}
