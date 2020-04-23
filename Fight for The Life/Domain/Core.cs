using System.Drawing;

namespace Fight_for_The_Life.Domain
{
    public class Core
    {
        public CoreState State { get; private set; } = CoreState.InsideSperm;
        private readonly Sperm sperm;
        private double ShotVelocity { get; set; }
        private const double HeightCoefficient = 0.083916;
        private const double WidthCoefficient = 0.03125;
        private static readonly int ModelHeight = (int) (Game.FieldHeight * HeightCoefficient);
        private static readonly int ModelWidth = (int) (Game.FieldWidth * WidthCoefficient);
        private Point StoppedPosition { get; set; }
        public Point ShotPosition { get; private set; }

        public Core(Sperm sperm)
        {
            this.sperm = sperm;
        }

        public Rectangle GetModel(double flightTimeInSeconds = 0)
        {
            Point location;

            if (State == CoreState.InsideSperm)
                location =  new Point((int)(sperm.Location.X + Game.FieldWidth * 0.11458),
                    (int)(sperm.Location.Y + Game.FieldHeight * 0.00699));

            else if (State == CoreState.Stopped)
                location = StoppedPosition;

            else
                location = new Point((int)(ShotPosition.X + ShotVelocity * flightTimeInSeconds * 3), ShotPosition.Y);

            return new Rectangle(location.X, location.Y, ModelWidth, ModelHeight);
        }

        public void Shot(double shotVelocity)
        {
            if (State == CoreState.InsideSperm)
            {
                ShotVelocity = shotVelocity;
                ShotPosition = GetModel().Location;
                State = CoreState.Flying;
            }
        }

        public void Stop(double flightTimeInSeconds)
        {
            if (State == CoreState.Flying)
            {
                StoppedPosition = GetModel(flightTimeInSeconds).Location;
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