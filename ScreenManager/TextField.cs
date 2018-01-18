using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TestProject.Game;
using Microsoft.Xna.Framework.Input;

namespace TestProject.ScreenManager
{
    public class TextField
    {
        private static Texture2D textRectangleTexture = null;
        private Texture2D textTextureUp = null;
        private Texture2D textTextureDown = null;
        private Rectangle rectangle;
        public bool Focused { get; set; }
        public String Text { get; set; }
        public int MaxLength { get; set; }
        public Color BgColorUp { get; set; }
        public Color BgColorDown { get; set; }
        public Color TextColor { get; set; }
        Keys[] lastPressedKeys = new Keys[0];

        public TextField(String text,int x, int y, int width, int height)
        {
            Text = text;
            MaxLength = 10;
            Focused = false;
            rectangle = new Rectangle(x, y, width, height);
            BgColorUp = Color.White;
            BgColorDown = Color.Gray;
            TextColor = Color.Black;
        }

        public TextField(String text, int x, int y, int width, int height, Color bgColorUp, Color bgColorDown, Color textColor)
        {
            Text = text;
            MaxLength = 10;
            Focused = false;
            rectangle = new Rectangle(x, y, width, height);
            BgColorUp = bgColorUp;
            BgColorDown = bgColorDown;
            TextColor = textColor;
        }

        public TextField(String text, int x, int y, int width, int height, Texture2D textureUp, Texture2D textureDown, Color textColor)
        {
            Text = text;
            MaxLength = 10;
            Focused = false;
            rectangle = new Rectangle(x, y, width, height);
            BgColorUp = Color.White;
            BgColorDown = Color.Gray;
            TextColor = textColor;
            textTextureUp = textureUp;
            textTextureDown = textureDown;
        }

        public static void Init(GraphicsDevice device)
        {
            textRectangleTexture = new Texture2D(device, 1, 1);
            textRectangleTexture.SetData(new Color[] { Color.White });
        }

        public bool Contains(Point point)
        {
            return rectangle.Contains(point);
        }

        public void Update(GameTime gameTime)
        {
            if (Focused)
            {
                KeyboardState kbState = Keyboard.GetState();
                Keys[] pressedKeys = kbState.GetPressedKeys();

                foreach (Keys key in pressedKeys)
                {
                    if (!lastPressedKeys.Contains(key) && (((int) key >= 48 && (int) key <= 90) || key == Keys.Back || key == Keys.OemPeriod))
                    {
                        if (key == Keys.Back)
                        {
                            if (Text.Count() > 0)
                            {
                                Text = Text.Substring(0, (Text.Count() - 1));
                            }
                        }
                        else if (Text.Count() <= MaxLength)
                        {
                            String text = (key == Keys.OemPeriod) ? "." : key.ToString();
                            if (text.Count() > 1)
                            {
                                text = text.Substring(1);
                            }
                            if (lastPressedKeys.Contains(Keys.LeftShift))
                            {
                                Text += text.ToUpper();
                            }
                            else
                            {
                                Text += text.ToLower();
                            }
                        }
                    }
                }
                lastPressedKeys = pressedKeys;
            }
        }

        public void Draw(GameTime time, SpriteBatch batch)
        {
            if (textTextureUp != null && textTextureDown != null)
            {
            }
            else
            {
                if (Focused)
                {
                    batch.Draw(textRectangleTexture, rectangle, BgColorUp);
                }
                else
                {
                    batch.Draw(textRectangleTexture, rectangle, BgColorDown);
                }
            }
            batch.DrawString(Configuration.ButtonFont, Text, new Vector2(rectangle.X, rectangle.Y + (rectangle.Height / 3)), TextColor);
        }
    }
}
