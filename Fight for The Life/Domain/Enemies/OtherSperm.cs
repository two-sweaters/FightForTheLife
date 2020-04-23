using System;
using System.Drawing;

namespace Fight_for_The_Life.Domain.Enemies
{
    public class OtherSperm : Enemy
    {

        public OtherSperm(int y, double spermVelocity)
        {
            HeightCoefficient = Sperm.HeightCoefficient;
            WidthCoefficient = Sperm.WidthCoefficient;
            if (y > Game.FieldHeight - 1 - Game.FieldHeight * HeightCoefficient || y < 0)
                throw new ArgumentException("Y was outside the game field!");
            Y = y;
            Velocity = spermVelocity;
        }

        public override Point GetLocation(double timeAliveInSeconds)
        {
            return new Point((int)(Velocity * timeAliveInSeconds), Y);
        }
    }
}