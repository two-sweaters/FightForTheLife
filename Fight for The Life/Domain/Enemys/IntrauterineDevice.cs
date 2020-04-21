using System;

namespace Fight_for_The_Life.Domain
{
    public class IntrauterineDevice : Enemy
    {
        public IntrauterineDevice(int y, double spermVelocity) : base(y, spermVelocity)
        {
            HeightCoefficient = 0.139860;
            WidthCoefficient = 0.40625;
            if (y > Game.FieldHeight - 1 || y < 0)
                throw new ArgumentException("Y was outside the game field!");
            Y = y;
            Velocity = spermVelocity;
        }
    }
}