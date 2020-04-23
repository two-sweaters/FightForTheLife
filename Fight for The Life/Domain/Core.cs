using System.Drawing;

namespace Fight_for_The_Life.Domain
{
    public class Core
    {
        public bool IsInsideSperm { get; private set; }
        private const double HeightCoefficient = 0.083916;
        private const double WidthCoefficient = 0.03125;
        private static readonly int ModelHeight = (int) (Game.FieldHeight * HeightCoefficient);
        private static readonly int ModelWidth = (int) (Game.FieldWidth * WidthCoefficient);

        public Point GetLocation(Sperm sperm)
        {
            if (IsInsideSperm)
                return new Point((int)(sperm.Location.X + Game.FieldWidth * 0.11458), 
                    (int)(sperm.Location.X + Game.FieldWidth * 0.00699));
            
            if ()
        }
    }
}