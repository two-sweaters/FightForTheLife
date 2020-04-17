using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fight_for_The_Life.Domain
{
    public class Game
    {
        public double VelocityInPixelsPerSecond =>
            Math.Pow(AccelerationCoefficient, (int) (GameTimeInSeconds / 15)) * StartVelocity;

        private int GameTimeInMilliseconds => (int) (DateTime.Now - GameStartTime).TotalMilliseconds;
        private double GameTimeInSeconds => GameTimeInMilliseconds / 1000;
        private readonly DateTime GameStartTime;
        public static readonly int FieldWidth = 1920;
        public static readonly int FieldHeight = 715;
        private static readonly double StartVelocity = (int) (FieldWidth / 4);
        private static readonly double AccelerationCoefficient = 1.25;
        public readonly Sperm Player = new Sperm();
        // пришлось изменить модель подсчета очков,
        // так как в прошлой версии очки зависили от разрешения окна
        // теперь очки = кол-во пройденых окон * 1000
        public int Score
        {
            get
            {
                var segmentsAmount = (int)(GameTimeInSeconds / 15);
                var lastSegmentTime = GameTimeInSeconds % 60;
                var distance = 0.0;
                var velocity = StartVelocity;
                for (var segmentsCount = 0; segmentsCount < segmentsAmount; segmentsCount++)
                {
                    distance += (int)(velocity * 15);
                    velocity *= AccelerationCoefficient;
                }
                distance += lastSegmentTime * velocity;
                var screensAmount = distance / FieldWidth;
                return (int) (screensAmount * 1000);
            }
        }

        public Game()
        {
            GameStartTime = DateTime.Now;
        }
    }

    public class Sperm
    {
        public Point Location { get; set; }
        private readonly Core Core = new Core();
        public bool HasCore => Location == Core.Location;

        public Sperm()
        {
            Location = new Point(0, Game.FieldHeight / 2);
        }

        public Sperm(int y)
        {
            if (y > Game.FieldHeight - 1 || y < 0)
                throw new ArgumentException("Y was outside the game field!");
            Location = new Point(0, y);
        }

        public void MoveUp()
        {
            var location = new Point(Location.X, Location.Y - Game.FieldHeight / 10);
            if (location.Y >= 0) 
                Location = location;
        }

        public void MoveDown()
        {
            var location = new Point(Location.X, Location.Y + Game.FieldHeight / 10);
            if (location.Y < Game.FieldHeight)
                Location = location;
        }
    }

    class Core
    {
        public Point Location { get; }
    }

    class OtherSperm
    {
        public Point Location { get; }
    }

    public class Enemy
    {
        public Point Location => new Point(Game.FieldWidth - 1 - (int)(Velocity * TimeAliveIsSeconds), Y);
        protected int Y;
        protected double Velocity;
        protected double TimeAliveIsSeconds => (DateTime.Now - CreatingTime).TotalSeconds;
        protected DateTime CreatingTime;

        public Enemy(int y, double spermVelocity)
        { 
        }
    }

    public class Blood : Enemy
    {
        private const double VelocityCoefficient = 1.2;

        public Blood(int y, double spermVelocity) : base(y, spermVelocity)
        {
            if (y > Game.FieldHeight - 1 || y < 0)
                throw new ArgumentException("Y was outside the game field!");
            Y = y;
            CreatingTime = DateTime.Now;
            Velocity = spermVelocity * VelocityCoefficient;
        }
    }

    public class IntrauterineDevice : Enemy
    {

        public IntrauterineDevice(int y, double spermVelocity) : base(y, spermVelocity)
        {
            if (y > Game.FieldHeight - 1 || y < 0)
                throw new ArgumentException("Y was outside the game field!");
            Y = y;
            CreatingTime = DateTime.Now;
            Velocity = spermVelocity;
        }
    }

    public class BirthControl : Enemy
    {
        private const double VelocityCoefficient = 1.5;


        public BirthControl(int y, double spermVelocity) : base(y, spermVelocity)
        {
            if (y > Game.FieldHeight - 1 || y < 0)
                throw new ArgumentException("Y was outside the game field!");
            Y = y;
            CreatingTime = DateTime.Now;
            Velocity = spermVelocity * VelocityCoefficient;
        }
    }
}
