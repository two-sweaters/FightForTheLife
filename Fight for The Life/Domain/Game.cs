using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Fight_for_The_Life.Domain.GameObjects;

namespace Fight_for_The_Life.Domain
{
    public class Game
    {
        public static int FieldWidth
        {
            get
            {
                if (Form.ActiveForm != null) 
                    return Form.ActiveForm.Width;
                return 1920;
            }
        }

        public static int FieldHeight
        {
            get
            {
                if (Form.ActiveForm != null) 
                    return (int) (Form.ActiveForm.Height * 0.6620370);
                return 715;
            }
        }

        public double GameTimeInSeconds { get; private set; }
        public double ShieldTimeInSeconds { get; private set; }
        public double MagnetTimeInSeconds { get; private set; }
        public double InvulnerableTimeInSeconds { get; private set; }
        private const int InvulnerableMaxTimeInSeconds = 1;
        public const int ExtraLifeCost = 80;
        public double EmptyFieldTime { get; private set; }
        private static readonly double StartVelocity = (int) (FieldWidth / 4);
        private const double AccelerationCoefficient = 1.25;
        public readonly Sperm Sperm = new Sperm();
        public HashSet<GameObject> GameObjects { get; private set; } = new HashSet<GameObject>();
        public bool IsGameOver { get; private set; }
        private readonly Random rand = new Random();
        public double ScoreCoefficient { get; set; }
        public int ShieldMaxTimeInSeconds { get; set; }
        public int MagnetMaxTimeInSeconds { get; set; }
        public int ExtraLifeAmount { get; set; }
        public int DnaAmount { get; set; }
        public int HighestScore { get; private set; }
        public int ScoreCoefficientCost => (int)(ScoreCoefficient / 0.5 - 2) * 5 + 25;
        public int ShieldMaxTimeCost
        {
            get
            {
                if (ShieldMaxTimeInSeconds == 0)
                    return 5;
                return ShieldMaxTimeInSeconds / 5 * 5 + 25;
            }
        }

        public int MagnetMaxTimeCost
        {
            get
            {
                if (MagnetMaxTimeInSeconds == 0)
                    return 5;
                return MagnetMaxTimeInSeconds / 5 * 5 + 25;
            }
        }

        public Game(int dnaAmount, int highestScore, double scoreCoefficient, 
            int shieldMaxTimeInSeconds, int magnetMaxTimeInSeconds, int extraLifeAmount)
        {
            DnaAmount = dnaAmount;
            HighestScore = highestScore;
            ScoreCoefficient = scoreCoefficient;
            ShieldMaxTimeInSeconds = shieldMaxTimeInSeconds;
            MagnetMaxTimeInSeconds = magnetMaxTimeInSeconds;
            ExtraLifeAmount = extraLifeAmount;
        }

        public int GetScore()
        {
            var segmentsAmount = (int)(GameTimeInSeconds / 15);
            var lastSegmentTime = GameTimeInSeconds % 15;
            var distance = 0.0;
            var velocity = StartVelocity;

            for (var segmentsCount = 0; segmentsCount < segmentsAmount; segmentsCount++)
            {
                distance += velocity * 15;
                velocity *= AccelerationCoefficient;
            }

            distance += lastSegmentTime * velocity;
            var screensAmount = distance / FieldWidth;
            return (int)(screensAmount * 1000 * ScoreCoefficient);
        }

        public void IncreaseGameTimeInSeconds(double seconds)
        {
            GameTimeInSeconds += seconds;
            foreach (var gameObject in GameObjects)
            {
                gameObject.TimeAliveInSeconds += seconds;
            }

            if (Sperm.IsShieldActivated)
                ShieldTimeInSeconds += seconds;

            if (Sperm.IsMagnetActivated)
                MagnetTimeInSeconds += seconds;

            if (Sperm.Core.State != CoreState.InsideSperm)
                Sperm.Core.timeAfterShotInSeconds += seconds;

            if (Sperm.Core.State == CoreState.Flying)
                Sperm.Core.flightTimeInSeconds += seconds;

            if (GameObjects.Count == 0)
                EmptyFieldTime += seconds;

            if (Sperm.IsInvulnerable)
                InvulnerableTimeInSeconds += seconds;
        }

        public double GetVelocityInPixelsPerSecond()
        {
            return Math.Pow(AccelerationCoefficient, (int)(GameTimeInSeconds / 15)) * StartVelocity;
        }

        public void UpdateGame()
        {
            var score = GetScore();
            if (score > HighestScore)
                HighestScore = score;

            if (ShieldTimeInSeconds > ShieldMaxTimeInSeconds)
                Sperm.IsShieldActivated = false;

            if (MagnetTimeInSeconds > MagnetMaxTimeInSeconds)
                Sperm.IsMagnetActivated = false;

            if (InvulnerableTimeInSeconds > InvulnerableMaxTimeInSeconds)
                Sperm.IsInvulnerable = false;

            var number = rand.Next(40);
            if (number == 1 || EmptyFieldTime > 3)
                GenerateRandomGameObject();

            CheckAndUpdateCore();
            if (IsGameOver)
                return;

            CheckAndUpdateGameObjects();
        }

        private void CheckAndUpdateGameObjects()
        {
            var coreModel = Sperm.Core.GetModel();
            var newGameObjects = new HashSet<GameObject>(GameObjects);
            foreach (var gameObject in GameObjects)
            {
                var objectModel = gameObject.GetModel();
                if (gameObject is OtherSperm)
                {
                    if (objectModel.X > FieldWidth)
                    {
                        if (ExtraLifeAmount == 0)
                        {
                            IsGameOver = true;
                            return;
                        }
                        ExtraLifeAmount--;
                        Sperm.IsInvulnerable = true;
                        InvulnerableTimeInSeconds = 0;
                    }

                    if (coreModel.IntersectsWith(objectModel) && Sperm.Core.State != CoreState.InsideSperm)
                    {
                        newGameObjects.Remove(gameObject);
                        Sperm.Core.Stop(GetVelocityInPixelsPerSecond());
                    }
                }
                else if (gameObject is Shield)
                {
                    if (objectModel.IntersectsWith(Sperm.Model))
                    {
                        newGameObjects.Remove(gameObject);
                        Sperm.IsShieldActivated = true;
                        ShieldTimeInSeconds = 0;
                    }
                }
                else if (gameObject is Magnet)
                {
                    if (objectModel.IntersectsWith(Sperm.Model))
                    {
                        newGameObjects.Remove(gameObject);
                        Sperm.IsMagnetActivated = true;
                        MagnetTimeInSeconds = 0;
                    }
                }
                else if (gameObject is Dna)
                {
                    if (objectModel.IntersectsWith(Sperm.Model))
                    {
                        DnaAmount++;
                        newGameObjects.Remove(gameObject);
                    }
                }
                else 
                {
                    if (objectModel.IntersectsWith(Sperm.Model) && !Sperm.IsShieldActivated && !Sperm.IsInvulnerable)
                    {
                        if (ExtraLifeAmount == 0)
                        {
                            IsGameOver = true;
                            return;
                        }
                        ExtraLifeAmount--;
                        Sperm.IsInvulnerable = true;
                        InvulnerableTimeInSeconds = 0;
                    }
                }

                if (objectModel.X < -objectModel.Width || objectModel.X > FieldWidth)
                    newGameObjects.Remove(gameObject);
            }

            GameObjects = newGameObjects;
        }

        private void CheckAndUpdateCore()
        {
            var coreModel = Sperm.Core.GetModel();
            if (Sperm.Core.State != CoreState.InsideSperm)
            {
                if (coreModel.X < 0 || coreModel.X > FieldWidth)
                {
                    if (ExtraLifeAmount == 0)
                    {
                        IsGameOver = true;
                        return;
                    }
                    ExtraLifeAmount--;
                    Sperm.IsInvulnerable = true;
                    InvulnerableTimeInSeconds = 0;
                    Sperm.Core.PickUp();
                }
                if (Sperm.Core.State == CoreState.Stopped && Sperm.Model.IntersectsWith(coreModel))
                    Sperm.Core.PickUp();
            }
        }

        private void GenerateRandomGameObject()
        {
            var number = rand.Next(100);
            var y = rand.Next((int)(FieldHeight - 1 - FieldHeight * 0.16));
            var velocity = GetVelocityInPixelsPerSecond();
            if (GameObjects.Count < 3)
            {
                if (number < 15)
                    GameObjects.Add(new BirthControl(y, velocity));

                else if (number < 30)
                    GameObjects.Add(new Blood(y, velocity));

                else if (CanOtherSpermSpawn() && number < 45)
                    GameObjects.Add(new OtherSperm(y, velocity));

                else if (GameObjects.All(e => !(e is IntrauterineDevice)) && number < 60)
                    GameObjects.Add(new IntrauterineDevice(velocity));

                else if (number < 80)
                    GameObjects.Add(new Dna(y, velocity, Sperm));

                else if (number < 90 && ShieldMaxTimeInSeconds > 0 && !Sperm.IsShieldActivated)
                    GameObjects.Add(new Shield(y, velocity));

                else if (number < 100 && MagnetMaxTimeInSeconds > 0 && !Sperm.IsMagnetActivated)
                    GameObjects.Add(new Magnet(y, velocity));
            }
        }

        private bool CanOtherSpermSpawn()
        {
            return GameObjects
                .All(e => !(e is OtherSperm) && !(e is IntrauterineDevice));
        }
    }
}
