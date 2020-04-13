using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fight_for_The_Life.Domain
{
    class Game
    {
    }

    class Sperm
    {
        public Point Location { get; }
        private readonly Core Core = new Core();
        public bool HasCore => Location == Core.Location;
    }

    class Obstruction
    {
    }

    class Blood : Obstruction
    {

    }

    class OtherSperm : Obstruction
    {
        
    }

    class Core
    {
        public Point Location { get; }
    }
}
