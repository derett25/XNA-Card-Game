using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TestProject.Game;

namespace TestProject.ScreenManager
{
    public class Button
    {
        private static Texture2D buttonRectangleTexture = null;
        private Texture2D buttonTextureUp = null;
        private Texture2D buttonTextureDown = null;
        private Rectangle rectangle;
        private bool isFocused = false;
        private List<ButtonListener> listeners = new List<ButtonListener>();
        public String Text { get; set; }
        public Color BgColorUp { get; set; }
        public Color BgColorDown { get; set; }
        public Color TextColor { get; set; }

        public Button(String text, int x, int y, int width, int height)
        {
            Text = text;
            rectangle = new Rectangle(x, y, width, height);
            BgColorUp = Color.White;
            BgColorDown = Color.Gray;
            TextColor = Color.Black;
        }

        public Button(String text, int x, int y, int width, int height, Color bgColorUp, Color bgColorDown, Color textColor)
        {
            Text = text;
            rectangle = new Rectangle(x, y, width, height);
            BgColorUp = bgColorUp;
            BgColorDown = bgColorDown;
            TextColor = textColor;
        }

        public Button(String text, int x, int y, int width, int height, Texture2D textureUp, Texture2D textureDown, Color textColor)
        {
            Text = text;
            rectangle = new Rectangle(x, y, width, height);
            BgColorUp = Color.White;
            BgColorDown = Color.Gray;
            TextColor = textColor;
            buttonTextureUp = textureUp;
            buttonTextureDown = textureDown;
        }

        public static void Init(GraphicsDevice device)
        {
            buttonRectangleTexture = new Texture2D(device, 1, 1);
            buttonRectangleTexture.SetData(new Color[] {Color.White});
        }

        public void Update(GameTime gameTime)
        {
            if (rectangle.Contains(GameCursor.MousePosition()))
            {
                if (GameCursor.LeftMouseState(gameTime) == MouseAction.CLICK)
                {
                    foreach (ButtonListener listener in listeners)
                    {
                        listener.Clicked(this);
                    }
                }
                isFocused = true;
            }
            else
            {
                isFocused = false;
            }
        }

        public void Draw(GameTime time, SpriteBatch batch)
        {
            if (buttonTextureUp != null && buttonTextureDown != null)
            {
            }
            else
            {
                if (isFocused)
                {
                    batch.Draw(buttonRectangleTexture, rectangle, BgColorUp);
                }
                else
                {
                    batch.Draw(buttonRectangleTexture, rectangle, BgColorDown);
                }
            }
            batch.DrawString(Configuration.ButtonFont, Text, new Vector2(rectangle.X, rectangle.Y + (rectangle.Height / 3)), TextColor);
        }

        public void AddButtonListener(ButtonListener listener)
        {
            listeners.Add(listener);
        }

        public void RemoveButtonListener(ButtonListener listener)
        {
            listeners.Remove(listener);
        }
    }
}
