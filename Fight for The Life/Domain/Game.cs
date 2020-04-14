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
        public int Score => (int) ((178 * GameTimeInSeconds + 1.05 * GameTimeInSeconds * GameTimeInSeconds / 2) * 5);
        public double VelocityInPixelsPerSecond => 178 + 1.05 * GameTimeInSeconds;
        public readonly Timer Timer;
        public int GameTimeInMilliseconds { get; private set; }
        public double GameTimeInSeconds => GameTimeInMilliseconds / 1000;
        public readonly int FieldWidth = 1920;
        public readonly int FieldHeight = 715;
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
            Location = new Point(0, 357);
        }

        public void MoveUp()
        {
            var location = new Point(Location.X, Location.Y - 71);
            if (location.Y >= 0) 
                Location = location;
        }

        public void MoveDown()
        {
            var location = new Point(Location.X, Location.Y + 71);
            if (location.Y < 715)
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
