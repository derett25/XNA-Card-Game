using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestProject.Game
{
    public static class Configuration
    {
        static public ContentManager ContentManager { get; private set; }
        static public int PLAYER_CARDS = 3;
        static public SpriteFont GameFont { get; private set; }
        static public SpriteFont ButtonFont { get; private set; }
        static public SpriteFont TitleFont { get; private set; }
        public const String RESOURCE_PATH = "Resources/";

        static public void Init(ContentManager manager)
        {
            ContentManager = manager;
            GameFont = ContentManager.Load<SpriteFont>(RESOURCE_PATH + "GameFont");
            ButtonFont = ContentManager.Load<SpriteFont>(RESOURCE_PATH + "ButtonFont");
            TitleFont = ContentManager.Load<SpriteFont>(RESOURCE_PATH + "TitleFont");
        } 
    }
}
