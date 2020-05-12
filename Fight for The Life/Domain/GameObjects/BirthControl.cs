using System;

namespace Fight_for_The_Life.Domain.GameObjects
{
    public class BirthControl : GameObject
    {
        private const double VelocityCoefficient = 1.5;

        public BirthControl(int y, double spermVelocity)
        {
            HeightCoefficient = 0.16;
            WidthCoefficient = 0.05958;
            if (y > Game.FieldHeight - 1 - Game.FieldHeight * HeightCoefficient || y < 0)
                throw new ArgumentException("Y was outside the game field!");
            Y = y;
            Velocity = spermVelocity * VelocityCoefficient;
        }
    }
}