using System;

namespace Fight_for_The_Life.Domain
{
    public class BirthControl : Enemy
    {
        private const double VelocityCoefficient = 1.5;


        public BirthControl(int y, double spermVelocity) : base(y, spermVelocity)
        {
            HeightCoefficient = WidthCoefficient = 0.16;
            if (y > Game.FieldHeight - 1 || y < 0)
                throw new ArgumentException("Y was outside the game field!");
            Y = y;
            Velocity = spermVelocity * VelocityCoefficient;
        }
    }
}