using System;
using System.Drawing;

namespace Fight_for_The_Life.Domain
{
    public class Sperm
    {
        public Point Location { get; private set; }
        public readonly Core Core;
        public const double HeightCoefficient = 0.1;
        public const double WidthCoefficient = 0.17;
        public static readonly int ModelHeight = (int) (Game.FieldHeight * HeightCoefficient);
        public static readonly int ModelWidth = (int) (Game.FieldWidth * WidthCoefficient);
        public Rectangle Model => new Rectangle(Location.X, Location.Y, 
            ModelWidth, ModelHeight);

        public Sperm()
        {
            Core = new Core(this);
            Location = new Point(0, Game.FieldHeight / 2);
        }

        public Sperm(int y)
        {
            if (y >= Game.FieldHeight - Model.Height || y < 0)
                throw new ArgumentException("Y was outside the game field!");
            Location = new Point(0, y);
            Core = new Core(this);
        }

        public void MoveUp()
        {
            var location = new Point(Location.X, Location.Y - Game.FieldHeight / 20);
            if (location.Y >= 0) 
                Location = location;
        }

        public void MoveDown()
        {
            var location = new Point(Location.X, Location.Y + Game.FieldHeight / 20);
            if (location.Y < Game.FieldHeight - Model.Height)
                Location = location;
        }
    }
}