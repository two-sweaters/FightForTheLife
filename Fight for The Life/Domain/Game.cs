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
        public static int FieldWidth => Form.ActiveForm.Width;
        public static int FieldHeight => (int) (Form.ActiveForm.Height * 0.66203703703703703703703703703704);
        private static readonly double StartVelocity = (int) (FieldWidth / 4);
        private const double AccelerationCoefficient = 1.25;
        public readonly Sperm Sperm = new Sperm();
        public HashSet<Enemy> LivingEnemies { get; private set; } = new HashSet<Enemy>();
        public bool IsGameOver { get; private set; }
        private readonly Random rand = new Random(12345511);

        // пришлось изменить модель подсчета очков,
        // так как в прошлой версии очки зависили от разрешения окна
        // теперь очки = кол-во пройденых окон * 1000
        public int GetScore(double gameTimeInSeconds)
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

        public double GetVelocityInPixelsPerSecond(double gameTimeInSeconds)
        {
            return Math.Pow(AccelerationCoefficient, (int)(gameTimeInSeconds / 15)) * StartVelocity;
        }

        // собираюсь вызывать из формы каждые 10мс => раз в 2с в среднем появляется враг
        public void UpdateGame(double gameTimeInSeconds, double coreFlightTime = 0)
        {
            var number = rand.Next(200);
            if (number == 1)
                GenerateRandomEnemy(gameTimeInSeconds);

            CheckAndUpdateCore(coreFlightTime);
            if (IsGameOver)
                return;

            CheckAndUpdateLivingEnemies(gameTimeInSeconds, coreFlightTime);
        }

        private void CheckAndUpdateLivingEnemies(double gameTimeInSeconds, double coreFlightTime)
        {
            var coreModel = Sperm.Core.GetModel(coreFlightTime);
            var newLivingEnemies = new HashSet<Enemy>(LivingEnemies);
            foreach (var enemy in LivingEnemies)
            {
                var enemyModel = enemy.GetModel(gameTimeInSeconds);
                if (enemy is OtherSperm)
                {
                    if (enemyModel.X < 0 || enemyModel.X > FieldWidth)
                    {
                        IsGameOver = true;
                        return;
                    }

                    if (coreModel.IntersectsWith(enemyModel))
                    {
                        newLivingEnemies.Remove(enemy);
                        Sperm.Core.Stop(coreFlightTime, GetVelocityInPixelsPerSecond(gameTimeInSeconds));
                    }
                }

                else
                {
                    if (enemyModel.IntersectsWith(Sperm.Model))
                    {
                        IsGameOver = true;
                        return;
                    }

                    if (enemyModel.X < 0 || enemyModel.X > FieldWidth)
                        newLivingEnemies.Remove(enemy);
                }
            }

            LivingEnemies = newLivingEnemies;
        }

        private void CheckAndUpdateCore(double coreFlightTime)
        {
            var coreModel = Sperm.Core.GetModel(coreFlightTime);
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

        public void GenerateRandomEnemy(double gameTimeInSeconds)
        {
            var number = rand.Next(4);
            var y = rand.Next((int)(FieldHeight - 1 - FieldHeight * 0.16));
            var velocity = GetVelocityInPixelsPerSecond(gameTimeInSeconds);

            if (number == 0)
                LivingEnemies.Add(new BirthControl(y, velocity));

            else if (number == 1)
                LivingEnemies.Add(new Blood(y, velocity));

            else if (number == 2) 
                LivingEnemies.Add(new IntrauterineDevice(velocity));

            else
                LivingEnemies.Add(new OtherSperm(y, velocity));
        }
    }
}
