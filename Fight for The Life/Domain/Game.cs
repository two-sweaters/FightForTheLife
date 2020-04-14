using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fight_for_The_Life.Domain
{
    class Game
    {
        // очки считаются следующим образом: пройденные главным героем пиксели * 5
        public int Score => (int) ((StartVelocity * GameTimeInSeconds +
                                    Acceleration * GameTimeInSeconds * GameTimeInSeconds / 2) * 5);
        public double VelocityInPixelsPerSecond => StartVelocity + Acceleration * GameTimeInSeconds;
        public readonly Timer Timer;
        public int GameTimeInMilliseconds { get; private set; }
        public double GameTimeInSeconds => GameTimeInMilliseconds / 1000;
        public static readonly int FieldWidth = 1920;
        public static readonly int FieldHeight = 715;
        public static readonly int StartVelocity = (int) (FieldWidth / 4);
        public static readonly double Acceleration = 1.05;
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

    class Enemy
    {
    }

    class Blood : Enemy
    {
        public readonly double Velocity;
        public Point Location { get; }

        public Blood(double spermVelocity)
        {
            Velocity = spermVelocity * 1.2;
        }
    }

    class OtherSperm : Enemy
    {
        public Point Location { get; }
    }

    class IntrauterineDevice : Enemy
    {
        public Point Location { get; }
        public readonly double Velocity;

        public IntrauterineDevice(double spermVelocity)
        {
            Velocity = spermVelocity;
        }
    }

    class BirthControl : Enemy
    {
        public Point Location { get; }
        public readonly double Velocity;

        public BirthControl(double spermVelocity)
        {
            Velocity = spermVelocity * 1.5;
        }
    }
}
