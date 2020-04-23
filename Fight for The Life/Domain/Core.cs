using System.Drawing;

namespace Fight_for_The_Life.Domain
{
    public class Core
    {
        public CoreState State { get; private set; } = CoreState.InsideSperm;
        private const double HeightCoefficient = 0.083916;
        private const double WidthCoefficient = 0.03125;
        private static readonly int ModelHeight = (int) (Game.FieldHeight * HeightCoefficient);
        private static readonly int ModelWidth = (int) (Game.FieldWidth * WidthCoefficient);
        private Point StoppedPosition { get; set; }
        private Point ShotPosition { get; set; }

        public Point GetLocation(Sperm sperm, double spermVelocity = 0, double flightTimeInSeconds = 0)
        {
            if (State == CoreState.InsideSperm)
                return new Point((int)(sperm.Location.X + Game.FieldWidth * 0.11458), 
                    (int)(sperm.Location.X + Game.FieldWidth * 0.00699));

            if (State == CoreState.Stopped)
                return StoppedPosition;

            return new Point((int) (ShotPosition.X + spermVelocity * flightTimeInSeconds * 3), ShotPosition.Y);
        }

        public Rectangle GetModel(Sperm sperm, double spermVelocity = 0, double flightTimeInSeconds = 0)
        {
            var location = GetLocation(sperm, spermVelocity, flightTimeInSeconds);
            return new Rectangle(location.X, location.Y, ModelWidth, ModelHeight);
        }

        public void Shot(Sperm sperm)
        {
            if (State == CoreState.InsideSperm)
            {
                ShotPosition = GetLocation(sperm);
                State = CoreState.Flying;
            }
        }

        public void Stop(Point currentPosition)
        {
            if (State == CoreState.Flying)
            {
                StoppedPosition = currentPosition;
                State = CoreState.Stopped;
            }
        }

        public void PickUp()
        {
            if (State == CoreState.Stopped)
                State = CoreState.InsideSperm;
        }
    }

    public enum CoreState
    {
        InsideSperm,
        Flying,
        Stopped
    }
}