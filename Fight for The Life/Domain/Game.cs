using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fight_for_The_Life.Domain
{
    public class Game
    {
        public static readonly int FieldWidth = 1920;
        public static readonly int FieldHeight = 715;
        private static readonly double StartVelocity = (int) (FieldWidth / 4);
        private static readonly double AccelerationCoefficient = 1.25;
        public readonly Sperm Player = new Sperm();

        // пришлось изменить модель подсчета очков,
        // так как в прошлой версии очки зависили от разрешения окна
        // теперь очки = кол-во пройденых окон * 1000
        public int GetScore(double gameTimeInSeconds)
        {
            var segmentsAmount = (int)(gameTimeInSeconds / 15);
            var lastSegmentTime = gameTimeInSeconds % 60;
            var distance = 0.0;
            var velocity = StartVelocity;
            for (var segmentsCount = 0; segmentsCount < segmentsAmount; segmentsCount++)
            {
                distance += velocity * 15;
                velocity *= AccelerationCoefficient;
            }

            distance += lastSegmentTime * velocity;
            var screensAmount = distance / FieldWidth;
            return (int)(screensAmount * 1000);
        }

        public double GetVelocityInPixelsPerSecond(double gameTimeInSeconds)
        {
            return Math.Pow(AccelerationCoefficient, (int)(gameTimeInSeconds / 15)) * StartVelocity;
        }
    }
}
