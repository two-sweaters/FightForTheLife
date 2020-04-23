﻿using System;

namespace Fight_for_The_Life.Domain.Enemies
{
    public class Blood : Enemy
    {
        private const double VelocityCoefficient = 1.2;

        public Blood(int y, double spermVelocity)
        {
            HeightCoefficient = 0.105;
            WidthCoefficient = 0.0625;
            if (y > Game.FieldHeight - 1 - Game.FieldHeight * HeightCoefficient || y < 0)
                throw new ArgumentException("Y was outside the game field!");
            Y = y;
            Velocity = spermVelocity * VelocityCoefficient;
        }
    }
}