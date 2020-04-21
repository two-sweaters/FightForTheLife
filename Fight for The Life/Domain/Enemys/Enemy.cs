using System.Drawing;

namespace Fight_for_The_Life.Domain
{
    public class Enemy
    {
        protected int Y;
        protected double Velocity;
        protected double WidthCoefficient;
        protected double HeightCoefficient;

        protected Enemy(int y, double spermVelocity)
        { 
        }

        public virtual Point GetLocation(double timeAliveInSeconds)
        {
            return new Point(Game.FieldWidth - 1 - (int)(Velocity * timeAliveInSeconds), Y);
        }

        public Rectangle GetModel(double timeAliveInSeconds)
        {
            var location = GetLocation(timeAliveInSeconds);
            return new Rectangle(location.X, location.Y, (int)(Game.FieldWidth * WidthCoefficient), 
                (int)(Game.FieldHeight * HeightCoefficient));
        }
    }
}