using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fight_for_The_Life.Domain.Enemies;

namespace Fight_for_The_Life.Domain.GameObjects
{
    //TODO доделать класс ДНК
    class Dna : GameObject
    {
        public Dna(int y, double spermVelocity)
        {
            WidthCoefficient = 0.0375;
            HeightCoefficient = 0.0667;
            Y = y;
            Velocity = spermVelocity;
        }
    }
}
