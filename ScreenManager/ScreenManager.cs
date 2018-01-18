using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestProject.Screens
{

    public static class ScreenManager
    {
        // Protected Members
        static private List<Screen> screens = new List<Screen>();
        static private bool started = false;
        static private Screen previous = null;
        // Public Members
        static public Screen ActiveScreen = null;

        static public void AddScreen(Screen screen)
        {
            foreach (Screen scr in screens)
            {
                if (scr.Name == screen.Name)
                {
                    return;
                }
            }
            screens.Add(screen);
        }

        static public int GetScreenCount()
        {
            return screens.Count;
        }

        static public Screen GetScreen(int idx)
        {
            return screens[idx];
        }

        static public void GotoScreen(string name, params Object[] param)
        {
            foreach (Screen screen in screens)
            {
                if (screen.Name == name)
                {
                    // Shutsdown Previous Screen           
                    previous = ActiveScreen;
                    if (ActiveScreen != null)
                    {
                        ActiveScreen.Shutdown();
                    }
                    // Inits New Screen
                    ActiveScreen = screen;
                    if (started) ActiveScreen.Init(param);
                    return;
                }
            }
        }

        static public void Init(params Object[] param)
        {
            started = true;
            if (ActiveScreen != null)
            {
                ActiveScreen.Init(param);
            }
        }

        static public void Shutdown()
        {
            started = false;
            if (ActiveScreen != null)
            {
                ActiveScreen.Shutdown();
            }
        }

        static public void GoToPrevious(params Object[] param)
        {
            if (previous != null)
            {
                GotoScreen(previous.Name, param);
                return;
            }
        }

        static public void Update(GameTime gameTime)
        {
            if (started == false) return;
            if (ActiveScreen != null)
            {
                ActiveScreen.Update(gameTime);
            }
        }

        static public void Draw(GameTime gameTime, SpriteBatch batch)
        {
            if (started == false) return;
            if (ActiveScreen != null)
            {
                ActiveScreen.Draw(gameTime, batch);
            }
        }
    }
}
