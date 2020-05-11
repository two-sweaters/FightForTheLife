using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fight_for_The_Life.Domain.Enemies;
using Fight_for_The_Life.Domain.GameObjects;
using Fight_for_The_Life.Views;

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

        private double gameTimeInSeconds;
        private double shieldTimeInSeconds;
        private double emptyFieldTime;
        private static readonly double StartVelocity = (int) (FieldWidth / 4);
        private const double AccelerationCoefficient = 1.25;
        public readonly Sperm Sperm = new Sperm();
        public HashSet<GameObject> GameObjects { get; private set; } = new HashSet<GameObject>();
        public bool IsGameOver { get; private set; }
        private readonly Random rand = new Random();
        public double ScoreCoefficient { get; private set; }
        public int ShieldMaxTimeInSeconds { get; private set; }
        public int MagnetMaxTime { get; private set; }
        public int DnaAmount { get; private set; }
        public int HighestScore { get; private set; }

        public Game(int dnaAmount, int highestScore)
        {
            DnaAmount = dnaAmount;
            HighestScore = highestScore;
        }


        // пришлось изменить модель подсчета очков,
        // так как в прошлой версии очки зависили от разрешения окна
        // теперь очки = кол-во пройденых окон * 1000
        public int GetScore()
        {
            var segmentsAmount = (int)(gameTimeInSeconds / 15);
            var lastSegmentTime = gameTimeInSeconds % 60;
            var distance = 0.0;
            var velocity = StartVelocity;

            for (var segmentsCount = 0; segmentsCount < segmentsAmount; segmentsCount++)
            {
                distance += velocity * 15;
                velocity *= AccelerationCoefficient;
            }

            distance += lastSegmentTime * velocity;
            var screensAmount = distance / FieldWidth;
            return (int)(screensAmount * 1000);
        }

        public void IncreaseGameTimeInSeconds(double seconds)
        {
            gameTimeInSeconds += seconds;
            foreach (var gameObject in GameObjects)
            {
                gameObject.TimeAliveInSeconds += seconds;
            }

            if (Sperm.Core.State != CoreState.InsideSperm)
                Sperm.Core.timeAfterShotInSeconds += seconds;

            if (Sperm.Core.State == CoreState.Flying)
                Sperm.Core.flightTimeInSeconds += seconds;

            if (GameObjects.Count == 0)
                emptyFieldTime += seconds;
        }

        public double GetVelocityInPixelsPerSecond()
        {
            return Math.Pow(AccelerationCoefficient, (int)(gameTimeInSeconds / 15)) * StartVelocity;
        }

        // собираюсь вызывать из формы каждые 50мс => раз в 2с в среднем появляется враг
        public void UpdateGame(double coreFlightTime = 0)
        {
            var score = GetScore();
            if (score > HighestScore)
                HighestScore = score;

            var number = rand.Next(40);
            if (number == 1 || emptyFieldTime > 3)
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
                        IsGameOver = true;
                        return;
                    }

                    if (coreModel.IntersectsWith(objectModel) && Sperm.Core.State != CoreState.InsideSperm)
                    {
                        newGameObjects.Remove(gameObject);
                        Sperm.Core.Stop(GetVelocityInPixelsPerSecond());
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
                    if (objectModel.IntersectsWith(Sperm.Model))
                    {
                        IsGameOver = true;
                        return;
                    }

                    if (objectModel.X < -objectModel.Width || objectModel.X > FieldWidth)
                        newGameObjects.Remove(gameObject);
                }
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
                    IsGameOver = true;
                    return;
                }
                if (Sperm.Core.State == CoreState.Stopped && Sperm.Model.IntersectsWith(coreModel))
                    Sperm.Core.PickUp();
            }
        }

        public void GenerateRandomGameObject()
        {
            var number = rand.Next(5);
            var y = rand.Next((int)(FieldHeight - 1 - FieldHeight * 0.16));
            var velocity = GetVelocityInPixelsPerSecond();
            if (GameObjects.Count < 3)
            {
                if (number == 0)
                    GameObjects.Add(new BirthControl(y, velocity));

                else if (number == 1)
                    GameObjects.Add(new Blood(y, velocity));

                else if (GameObjects.All(e => !(e is OtherSperm) || e.GetLocation().X > Game.FieldWidth / 2)
                         && number == 2)
                    GameObjects.Add(new OtherSperm(y, velocity));

                else if (GameObjects.All(e => !(e is IntrauterineDevice)) && number == 3)
                    GameObjects.Add(new IntrauterineDevice(velocity));

                else
                    GameObjects.Add(new Dna(y, velocity));
            }
        }
    }
}
