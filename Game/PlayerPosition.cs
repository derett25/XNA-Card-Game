using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestProject.Game
{
    public class PlayerPosition
    {
        public String Name { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public float Rotation { get; private set; }

        public PlayerPosition(String name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
            Rotation = 0;
        }

        public PlayerPosition(String name, int x, int y, float rotation)
        {
            Name = name;
            X = x;
            Y = y;
            Rotation = rotation;
        }
    }

    public static class PlayerPositions
    {
        public static PlayerPosition North;
        public static PlayerPosition South;
        public static PlayerPosition West;
        public static PlayerPosition East;

        public static void Init(GraphicsDevice device)
        {
            North = new PlayerPosition("north", (device.Viewport.Width / 2) - 75, 150, 3.15f);
            South = new PlayerPosition("south", (device.Viewport.Width / 2) - 75, device.Viewport.Height - 200, 0f);
            West = new PlayerPosition("west", 50, (device.Viewport.Height / 2) - 75, 1.56f);
            East = new PlayerPosition("east", device.Viewport.Width - 200, (device.Viewport.Height / 2) - 75, (float) ((Math.PI * 2) / 4) * 3);
        }
    }
}
