using System;
using System.Drawing;

namespace Fight_for_The_Life.Domain
{
    public class OtherSperm : Enemy
    {

        public OtherSperm(int y, double spermVelocity) : base(y, spermVelocity)
        {
            HeightCoefficient = 0.1;
            WidthCoefficient = 0.17;
            if (y > Game.FieldHeight - 1 || y < 0)
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