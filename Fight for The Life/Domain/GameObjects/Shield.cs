namespace Fight_for_The_Life.Domain.GameObjects
{
    public class Shield : GameObject
    {
        public Shield(int y, double spermVelocity)
        {
            WidthCoefficient = 0.0375;
            HeightCoefficient = 0.0667;
            Y = y;
            Velocity = spermVelocity;
        }
    }
}
