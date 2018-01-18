using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TestProject.ScreenManager;
using TestProject.Game;

namespace TestProject.Screens
{
    public class Screen
    {
        protected GraphicsDevice device = null;
        private List<Button> buttons = new List<Button>();
        private List<TextField> textFields = new List<TextField>();
        private List<Label> labels = new List<Label>();

        public Screen(GraphicsDevice device, string name)
        {
            Name = name;
            this.device = device;
        }

        ~Screen()
        {
        }

        public string Name
        {
            get;
            set;
        }

        public virtual bool Init(params Object[] param)
        {
            return true;
        }

        public virtual void Shutdown()
        {

        }
        public virtual void Update(GameTime gameTime)
        {
            foreach (Button button in buttons)
            {
                button.Update(gameTime);
            }
            foreach (TextField field in textFields)
            {
                if (field.Contains(GameCursor.MousePosition()) && GameCursor.LeftMouseState(gameTime) == MouseAction.CLICK)
                {
                    field.Focused = true;
                }
                else if (!field.Contains(GameCursor.MousePosition()) && GameCursor.LeftMouseState(gameTime) == MouseAction.CLICK)
                {
                    field.Focused = false;
                }
                field.Update(gameTime);
            }
            foreach (Label label in labels)
            {
                label.Update(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch batch)
        {
            foreach (Button button in buttons)
            {
                button.Draw(gameTime, batch);
            }
            foreach (TextField field in textFields)
            {
                field.Draw(gameTime, batch);
            }
            foreach (Label label in labels)
            {
                label.Draw(gameTime, batch);
            }
        }

        protected void AddButton(Button button)
        {
            buttons.Add(button);
        }

        protected void RemoveButton(Button button)
        {
            buttons.Remove(button);
        }

        protected void AddTextField(TextField textField)
        {
            textFields.Add(textField);
        }

        protected void RemoveTextField(TextField textField)
        {
            textFields.Remove(textField);
        }

        protected void AddLabel(Label label)
        {
            labels.Add(label);
        }

        protected void RemoveLabel(Label label)
        {
            labels.Remove(label);
        }
    }

}
