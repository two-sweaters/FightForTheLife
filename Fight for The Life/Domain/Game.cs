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
    class Game
    {
        // очки считаются следующим образом: пройденные главным героем пиксели * 5
        public int Score
        {
            get
            {
                var segmentsAmount = (int) (GameTimeInSeconds / 15);
                var lastSegmentTime = GameTimeInSeconds % 60;
                var distance = 0;
                var velocity = StartVelocity;
                for (var segmentsCount = 0; segmentsCount < segmentsAmount; segmentsCount++)
                {
                    distance += (int) (velocity * 15);
                    velocity *= AccelerationCoefficient;
                }
                distance += (int) (lastSegmentTime * velocity);
                return distance * 5;
            }
        }
        public double VelocityInPixelsPerSecond =>
            Math.Pow(AccelerationCoefficient, (int) (GameTimeInSeconds / 15)) * StartVelocity;
        private readonly Timer Timer;
        private int GameTimeInMilliseconds { get; set; }
        private double GameTimeInSeconds => GameTimeInMilliseconds / 1000;
        public static readonly int FieldWidth = 1920;
        public static readonly int FieldHeight = 715;
        private static readonly double StartVelocity = (int) (FieldWidth / 4);
        private static readonly double AccelerationCoefficient = 1.25;
        public readonly Sperm Player = new Sperm();

        public Game()
        {
            Timer = new Timer {Interval = 1};
            Timer.Tick += (sender, args) => GameTimeInMilliseconds++;
            Timer.Start();
        }
    }

    class Sperm
    {
        public Point Location { get; set; }
        private readonly Core Core = new Core();
        public bool HasCore => Location == Core.Location;

        public Sperm()
        {
            Location = new Point(0, Game.FieldHeight / 2);
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

    class Enemy
    {
        public Point Location => new Point(Game.FieldWidth - 1 - (int) (Velocity * TimeAliveIsSeconds), Y);
        protected int Y;
        protected Timer Timer;
        protected double Velocity;
        protected int TimeAliveIsSeconds;
        private static double velocityCoefficient;

        public Enemy(int y, double spermVelocity)
        { 
        }
    }

    class Blood : Enemy
    {
        private const double velocityCoefficient = 1.2;

        public Blood(int y, double spermVelocity) : base(y, spermVelocity)
        {
            Y = y;
            Velocity = spermVelocity * velocityCoefficient;
            Timer = new Timer() { Interval = 1000 };
            Timer.Tick += (sender, args) => TimeAliveIsSeconds++;
            Timer.Start();
        }
    }

    class IntrauterineDevice : Enemy
    {
        private const double velocityCoefficient = 1;

        public IntrauterineDevice(int y, double spermVelocity) : base(y, spermVelocity)
        {
            Y = y;
            Velocity = spermVelocity * velocityCoefficient;
            Timer = new Timer() { Interval = 1000 };
            Timer.Tick += (sender, args) => TimeAliveIsSeconds++;
            Timer.Start();
        }
    }

    class BirthControl : Enemy
    {
        private const double velocityCoefficient = 1.5;


        public BirthControl(int y, double spermVelocity) : base(y, spermVelocity)
        {
            Y = y;
            Velocity = spermVelocity * velocityCoefficient;
            Timer = new Timer() { Interval = 1000 };
            Timer.Tick += (sender, args) => TimeAliveIsSeconds++;
            Timer.Start();
        }
    }
}
