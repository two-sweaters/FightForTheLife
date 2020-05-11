using System;

namespace Fight_for_The_Life.Domain.Enemies
{
    public class IntrauterineDevice : GameObject
    {
        public IntrauterineDevice(double spermVelocity)
        {
            HeightCoefficient = 0.139860;
            WidthCoefficient = 0.40625;
            Y = (Game.FieldHeight - 1) / 2 - (int) (Game.FieldHeight * HeightCoefficient / 2);
            Velocity = spermVelocity;
        }
    }
}