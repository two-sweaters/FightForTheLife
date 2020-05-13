﻿using System.Drawing;
using Fight_for_The_Life.Properties;

namespace Fight_for_The_Life.Views
{
    public class GameImages
    {
        public readonly Bitmap Background;
        public readonly Bitmap Sperm;
        public readonly Bitmap SpermWithShield;
        public readonly Bitmap SpermWithMagnet;
        public readonly Bitmap SpermWithShieldAndMagnet;
        public readonly Bitmap Core;
        public readonly Bitmap Blood;
        public readonly Bitmap BirthControl;
        public readonly Bitmap IntrauterineDevice; 
        public readonly Bitmap OtherSperm;
        public readonly Bitmap Dna;
        public readonly Bitmap Shield;
        public readonly Bitmap Magnet;

        public GameImages(double widthCoefficient, double heightCoefficient)
        {
            Background = new Bitmap(Resources.Background, 
                (int)(widthCoefficient * 1920), 
                (int)(heightCoefficient * 1080));

            Sperm = new Bitmap(Resources.MainSperm, 
                (int)(Resources.MainSperm.Width * widthCoefficient),
                (int)(Resources.MainSperm.Height * heightCoefficient));

            Core = new Bitmap(Resources.Core, 
                (int)(Resources.Core.Width * widthCoefficient),
                (int)(Resources.Core.Height * heightCoefficient));

            Blood = new Bitmap(Resources.Blood, 
                (int)(Resources.Blood.Width * widthCoefficient),
                (int)(Resources.Blood.Height * heightCoefficient));

            BirthControl = new Bitmap(Resources.Pill, 
                (int)(Resources.Pill.Width * widthCoefficient),
                (int)(Resources.Pill.Height * heightCoefficient));

            IntrauterineDevice = new Bitmap(Resources.Spiral, 
                (int)(Resources.Spiral.Width * widthCoefficient),
                (int)(Resources.Spiral.Height * heightCoefficient));

            OtherSperm = new Bitmap(Resources.Sperm, 
                (int)(Resources.Sperm.Width * widthCoefficient),
                (int)(Resources.Sperm.Height * heightCoefficient));

            Dna = new Bitmap(Resources.Dna, 
                (int)(Resources.Dna.Width * widthCoefficient),
                (int)(Resources.Dna.Height * heightCoefficient));

            Shield = new Bitmap(Resources.Shield, 
                (int)(Resources.Shield.Width * widthCoefficient),
                (int)(Resources.Shield.Height * heightCoefficient));

            Magnet = new Bitmap(Resources.Magnet, 
                (int)(Resources.Magnet.Width * widthCoefficient),
                (int)(Resources.Magnet.Height * heightCoefficient));

            SpermWithShield = new Bitmap(Resources.MainSpermWithShield, 
                (int)(Resources.Sperm.Width * widthCoefficient),
                (int)(Resources.Sperm.Height * heightCoefficient));

            SpermWithMagnet = new Bitmap(Resources.MainSpermWithMagnet, 
                (int)(Resources.Sperm.Width * widthCoefficient),
                (int)(Resources.Sperm.Height * heightCoefficient));

            SpermWithShieldAndMagnet = new Bitmap(Resources.MainSpermWithShieldAndMagnet, 
                (int)(Resources.Sperm.Width * widthCoefficient),
                (int)(Resources.Sperm.Height * heightCoefficient));
        }
    }
}