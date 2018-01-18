using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestProject.Game;

namespace TestProject.ScreenManager
{
    public class Label
    {
        public String Text { get; set; }
        public Vector2 Position { get; set; }
        public Color Color { get; set; }

        public bool Animate { get; set; }
        public float Transparency { get; set; }

        private SpriteFont labelFont;

        public Label(String text, int x, int y)
        {
            Text = text;
            Color = Color.White;
            Position = new Vector2(x, y);
            labelFont = Configuration.GameFont;
            Transparency = 1f;
        }

        public Label(String text, int x, int y, Color color)
        {
            Text = text;
            Color = color;
            Position = new Vector2(x, y);
            labelFont = Configuration.GameFont;
            Transparency = 1f;
        }

        public Label(String text, int x, int y, SpriteFont font)
        {
            Text = text;
            Color = Color.White;
            Position = new Vector2(x, y);
            labelFont = font;
            Transparency = 1f;
        }

        public Label(String text, int x, int y, Color color, SpriteFont font)
        {
            Text = text;
            Color = color;
            Position = new Vector2(x, y);
            labelFont = font;
            Transparency = 1f;
        }

        public void Update(GameTime gameTime)
        {
            // Add timing?
            if (Animate && Transparency > 0)
            {
                Transparency -= 0.0010f;
                if (Transparency < 0f)
                {
                    Transparency = 0f;
                } 
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch batch)
        {
            batch.DrawString(labelFont, Text, Position, Color * Transparency);
        }
    }
}
