using System.Drawing;

namespace Fight_for_The_Life.Domain.Enemies
{
    public class GameObject
    {
        protected int Y;
        protected double Velocity;
        protected double WidthCoefficient;
        protected double HeightCoefficient;
        public double TimeAliveInSeconds { get; set; }

        public virtual Point GetLocation()
        {
            return new Point(Game.FieldWidth - 1 - (int)(Velocity * TimeAliveInSeconds), Y);
        }

        public Rectangle GetModel()
        {
            var location = GetLocation();
            return new Rectangle(location.X, location.Y, (int)(Game.FieldWidth * WidthCoefficient), 
                (int)(Game.FieldHeight * HeightCoefficient));
        }
    }
}