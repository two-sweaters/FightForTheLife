using System.Drawing;

namespace Fight_for_The_Life.Domain.GameObjects
{
    class Dna : GameObject
    {
        private readonly Sperm sperm;
        public Dna(int y, double spermVelocity, Sperm sperm)
        {
            WidthCoefficient = 0.0375;
            HeightCoefficient = 0.0667;
            Y = y;
            Velocity = spermVelocity;
            this.sperm = sperm;
        }

        public override Point GetLocation()
        {
            var location = base.GetLocation();
            if (sperm.IsMagnetActivated)
                location.Y = sperm.Location.Y;

            return location;
        }
    }
}
