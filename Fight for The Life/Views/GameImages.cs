using System.Drawing;
using Fight_for_The_Life.Properties;

namespace Fight_for_The_Life.Views
{
    public class GameImages
    {
        public readonly Bitmap Background;
        public readonly Bitmap Sperm;
        public readonly Bitmap Core;
        public readonly Bitmap Blood;
        public readonly Bitmap BirthControl;
        public readonly Bitmap IntrauterineDevice; 
        public readonly Bitmap OtherSperm;
        public readonly Bitmap Dna;

        public GameImages(int width, int height)
        {
            Background = new Bitmap(Resources.Background, width, height);

            Sperm = new Bitmap(Resources.MainSperm, Resources.MainSperm.Width * width / 1920,
                Resources.MainSperm.Height * height / 1080);

            Core = new Bitmap(Resources.Core, Resources.Core.Width * width / 1920,
                Resources.Core.Height * height / 1080);

            Blood = new Bitmap(Resources.Blood, Resources.Blood.Width * width / 1920,
                Resources.Blood.Height * height / 1080);

            BirthControl = new Bitmap(Resources.Pill, Resources.Pill.Width * width / 1920,
                Resources.Pill.Height * height / 1080);

            IntrauterineDevice = new Bitmap(Resources.Spiral,
                Resources.Spiral.Width * width / 1920,
                Resources.Spiral.Height * height / 1080);

            OtherSperm = new Bitmap(Resources.Sperm, Resources.Sperm.Width * width / 1920,
                Resources.Sperm.Height * height / 1080);

            Dna = new Bitmap(Resources.Dna, Resources.Dna.Width * width / 1920,
                Resources.Dna.Height * height / 1080);
        }
    }
}