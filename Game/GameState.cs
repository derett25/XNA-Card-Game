using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestProject.Cards;

namespace TestProject.Game
{
    public interface GameState
    {
        PlayState GetPlayState();
        Card GetTopCard();
        Deck GetDeck();
        CardPile GetPile();
        List<Player> GetPlayers();
        void AddListener(GameListener listener);
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime, SpriteBatch batch);
    }
}
