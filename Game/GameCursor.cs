using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestProject.Cards;

namespace TestProject.Game
{
    static public class GameCursor
    {
        public static bool CustomMouse { get; set; }

        private static Texture2D cursor = null;
        private static Texture2D cursorClick = null;

        private static MouseState currentState;
        private static MouseState previousState;

        private const float HOLD_TIMESPAN = .25f;
        private static float holdTimer;

        public static List<Card> SelectedCards { get;  set;}

        public static void Init()
        {
            cursor = Game.Configuration.ContentManager.Load<Texture2D>(Game.Configuration.RESOURCE_PATH + "cursor");
            cursorClick = Game.Configuration.ContentManager.Load<Texture2D>(Game.Configuration.RESOURCE_PATH + "cursorClick");
            SelectedCards = new List<Card>();
        }

        public static void Draw(GameTime gameTime, SpriteBatch batch)
        {
            if (CustomMouse)
            {
                if (LeftMouseState(gameTime) == MouseAction.CLICK || LeftMouseState(gameTime) == MouseAction.HOLD)
                {
                    batch.Draw(cursorClick, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Color.White);
                }
                else
                {
                    batch.Draw(cursor, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Color.White);
                }
            }
        }

        public static void Update(GameTime time)
        {
            previousState = currentState;
            currentState = Mouse.GetState();
            if (SelectedCards != null)
            {
                foreach (Card card in SelectedCards)
                {
                    card.SetPosition(currentState.X, currentState.Y);
                }
            }
        }

        public static Point MousePosition()
        {
            return new Point(Mouse.GetState().X, Mouse.GetState().Y);
        }

        public static MouseAction LeftMouseState(GameTime gameTime)
        {
            bool isHeld = false;
            if (currentState.LeftButton == ButtonState.Pressed)
            {
                holdTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (holdTimer > HOLD_TIMESPAN)
            {
                isHeld = true;
            }
            if (currentState.LeftButton == ButtonState.Released)
            {
                if (isHeld)
                {
                    holdTimer = 0f;
                    isHeld = false;
                    return MouseAction.RELEASE;
                }
                else if (previousState.LeftButton == ButtonState.Pressed)
                {
                    return MouseAction.CLICK;
                }
                else
                {
                    return MouseAction.RELEASE;
                }
            } else if (isHeld)
            {
                return MouseAction.HOLD;
            } else if (currentState.LeftButton == ButtonState.Pressed)
            {
                return MouseAction.CLICK;
            }
            return MouseAction.NONE;
        }

        public static MouseAction RightMouseState(GameTime gameTime)
        {
            if (currentState.RightButton == ButtonState.Pressed)
            {
                return MouseAction.CLICK;
            }
            else if (currentState.RightButton == ButtonState.Released)
            {
                return MouseAction.RELEASE;
            }
            return MouseAction.NONE;
        }
    }

    public enum MouseAction
    {
        NONE,
        CLICK,
        HOLD,
        RELEASE,
    }
}
