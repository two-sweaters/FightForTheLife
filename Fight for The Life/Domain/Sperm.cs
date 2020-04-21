using System;
using System.Drawing;

namespace Fight_for_The_Life.Domain
{
    public class Sperm
    {
        public Point Location { get; set; }
        private readonly Core Core = new Core();
        public static readonly int ModelHeight = (int) (Game.FieldHeight * 0.1);
        public static readonly int ModelWidth = (int) (Game.FieldWidth * 0.17);
        public Rectangle Model => new Rectangle(Location.X, Location.Y, 
            ModelWidth, ModelHeight);

        public Sperm()
        {
            Location = new Point(0, Game.FieldHeight / 2);
        }

        public Sperm(int y)
        {
            if (y >= Game.FieldHeight - Model.Height || y < 0)
                throw new ArgumentException("Y was outside the game field!");
            Location = new Point(0, y);
        }

        public void MoveUp()
        {
            var location = new Point(Location.X, Location.Y - Game.FieldHeight / 10);
            if (location.Y >= 0) 
                Location = location;
        }

        public void MoveDown()
        {
            var location = new Point(Location.X, Location.Y + Game.FieldHeight / 10);
            if (location.Y < Game.FieldHeight - Model.Height)
                Location = location;
        }
    }
}