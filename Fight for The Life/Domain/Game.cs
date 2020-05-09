using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fight_for_The_Life.Domain.Enemies;
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
        private static readonly double StartVelocity = (int) (FieldWidth / 4);
        private const double AccelerationCoefficient = 1.25;
        public readonly Sperm Sperm = new Sperm();
        public HashSet<Enemy> LivingEnemies { get; private set; } = new HashSet<Enemy>();
        public bool IsGameOver { get; private set; }
        private readonly Random rand = new Random();

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
            foreach (var enemy in LivingEnemies)
            {
                enemy.TimeAliveInSeconds += seconds;
            }

            if (Sperm.Core.State != CoreState.InsideSperm)
                Sperm.Core.timeAfterShotInSeconds += seconds;

            if (Sperm.Core.State == CoreState.Flying)
                Sperm.Core.flightTimeInSeconds += seconds;
        }

        public double GetVelocityInPixelsPerSecond()
        {
            return Math.Pow(AccelerationCoefficient, (int)(gameTimeInSeconds / 15)) * StartVelocity;
        }

        // собираюсь вызывать из формы каждые 50мс => раз в 2с в среднем появляется враг
        public void UpdateGame(double coreFlightTime = 0)
        {
            var number = rand.Next(40);
            if (number == 1)
                GenerateRandomEnemy();

            CheckAndUpdateCore();
            if (IsGameOver)
                return;

            CheckAndUpdateLivingEnemies();
        }

        private void CheckAndUpdateLivingEnemies()
        {
            var coreModel = Sperm.Core.GetModel();
            var newLivingEnemies = new HashSet<Enemy>(LivingEnemies);
            foreach (var enemy in LivingEnemies)
            {
                var enemyModel = enemy.GetModel();
                if (enemy is OtherSperm)
                {
                    if (enemyModel.X > FieldWidth)
                    {
                        IsGameOver = true;
                        return;
                    }

                    if (coreModel.IntersectsWith(enemyModel) && Sperm.Core.State != CoreState.InsideSperm)
                    {
                        newLivingEnemies.Remove(enemy);
                        Sperm.Core.Stop(GetVelocityInPixelsPerSecond());
                    }
                }

                else
                {
                    if (enemyModel.IntersectsWith(Sperm.Model))
                    {
                        IsGameOver = true;
                        return;
                    }

                    if (enemyModel.X < -enemyModel.Width || enemyModel.X > FieldWidth)
                        newLivingEnemies.Remove(enemy);
                }
            }

            LivingEnemies = newLivingEnemies;
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

        public void GenerateRandomEnemy()
        {
            var number = rand.Next(4);
            var y = rand.Next((int)(FieldHeight - 1 - FieldHeight * 0.16));
            var velocity = GetVelocityInPixelsPerSecond();

            if (number == 0)
                LivingEnemies.Add(new BirthControl(y, velocity));

            else if (number == 1)
                LivingEnemies.Add(new Blood(y, velocity));

            else if (LivingEnemies.All(e => !(e is OtherSperm) || e.GetLocation().X > Game.FieldWidth / 2) 
                     && number == 2)
                LivingEnemies.Add(new OtherSperm(y, velocity));

            else if (LivingEnemies.All(e => !(e is IntrauterineDevice)))
                LivingEnemies.Add(new IntrauterineDevice(velocity));
        }
    }
}
