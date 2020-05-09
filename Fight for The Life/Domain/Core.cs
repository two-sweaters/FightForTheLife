using System.Drawing;

namespace Fight_for_The_Life.Domain
{
    public class Core
    {
        public CoreState State { get; private set; } = CoreState.InsideSperm;
        private readonly Sperm sperm;
        private double shotVelocity;
        private double spermVelocityAfterStop;
        private const double HeightCoefficient = 0.083916;
        private const double WidthCoefficient = 0.03125;
        private static readonly int ModelHeight = (int) (Game.FieldHeight * HeightCoefficient);
        private static readonly int ModelWidth = (int) (Game.FieldWidth * WidthCoefficient);
        public double flightTimeInSeconds { get; set; }
        public double timeAfterShotInSeconds { get; set; }
        public Point ShotPosition { get; private set; }

        public Core(Sperm sperm)
        {
            this.sperm = sperm;
        }

        public Rectangle GetModel()
        {
            Point location;

            if (State == CoreState.InsideSperm)
                location =  new Point((int)(sperm.Location.X + Game.FieldWidth * 0.11458),
                    (int)(sperm.Location.Y + Game.FieldHeight * 0.00699));

            else if (State == CoreState.Stopped)
            {
                var stoppedTime = timeAfterShotInSeconds - flightTimeInSeconds;
                location = new Point((int)
                    (ShotPosition.X + shotVelocity * flightTimeInSeconds * 3 - spermVelocityAfterStop * stoppedTime),
                    ShotPosition.Y);
            }

            else
                location = new Point((int)(ShotPosition.X + shotVelocity * timeAfterShotInSeconds * 3), ShotPosition.Y);

            return new Rectangle(location.X, location.Y, ModelWidth, ModelHeight);
        }

        public void Shot(double spermVelocity)
        {
            if (State == CoreState.InsideSperm)
            {
                shotVelocity = spermVelocity;
                ShotPosition = GetModel().Location;
                State = CoreState.Flying;
            }
        }

        public void Stop(double spermVelocity)
        {
            if (State == CoreState.Flying)
            {
                spermVelocityAfterStop = spermVelocity;
                State = CoreState.Stopped;
            }
        }

        public void PickUp()
        {
            if (State == CoreState.Stopped)
            {
                timeAfterShotInSeconds = 0;
                flightTimeInSeconds = 0;
                State = CoreState.InsideSperm;
            }
        }
    }

    public enum CoreState
    {
        InsideSperm,
        Flying,
        Stopped
    }
}