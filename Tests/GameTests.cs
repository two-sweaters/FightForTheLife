using NUnit.Framework;
using Fight_for_The_Life.Domain;
using Fight_for_The_Life.Domain.GameObjects;

namespace Tests
{
    [TestFixture]
    class GameTests
    {
        [Test]
        public void ShouldCalculateRightVelocity_AfterCreation()
        {
            var game = new Game(0, 0, 1, 0, 0);
            Assert.AreEqual(480, game.GetVelocityInPixelsPerSecond());
        }

        [Test]
        public void ShouldCalculateRightVelocity_15SecondsAfterCreation()
        {
            var game = new Game(0, 0, 1, 0, 0);
            game.IncreaseGameTimeInSeconds(15);
            Assert.AreEqual(600, game.GetVelocityInPixelsPerSecond());
        }

        [Test]
        public void ShouldCalculateRightScore()
        {
            var game = new Game(0, 0, 1, 0, 0);
            game.IncreaseGameTimeInSeconds(5);
            Assert.AreEqual(1250, game.GetScore());
        }

        [Test]
        public void UpdateGame_ShouldEndGame_IfCoreFlyAway()
        {
            var game = new Game(0, 0, 1, 0, 0);
            game.Sperm.Core.Shot(2000);
            game.IncreaseGameTimeInSeconds(1);
            game.UpdateGame();
            Assert.IsTrue(game.IsGameOver);
        }

        [Test]
        public void UpdateGame_ShouldEndGame_IfOtherSpermFlyAway()
        {
            var game = new Game(0, 0, 1, 0, 0);
            game.GameObjects.Add(new OtherSperm(5, 2000));
            game.IncreaseGameTimeInSeconds(2);
            game.UpdateGame();
            Assert.IsTrue(game.IsGameOver);
        }

        [Test]
        public void UpdateGame_ShouldEndGame_AfterSpermConflictWithBlood()
        {
            var game = new Game(0, 0, 0, 0, 0);
            game.GameObjects.Add(new Blood(game.Sperm.Location.Y, 1500));
            game.IncreaseGameTimeInSeconds(1);
            game.UpdateGame();
            Assert.IsTrue(game.IsGameOver);
        }

        [Test]
        public void UpdateGame_ShouldRemoveGameObject_IfGameObjectFlyAway()
        {
            var game = new Game(0, 0, 0, 0, 0);
            game.GameObjects.Add(new Blood(game.Sperm.Location.Y, 2000));
            game.IncreaseGameTimeInSeconds(1);
            game.UpdateGame();
            Assert.IsEmpty(game.GameObjects);
        }

        [Test]
        public void UpdateGame_ShouldRemoveOtherSperm_IfCoreHitIt()
        {
            var game = new Game(0, 0, 0, 0, 0);
            game.GameObjects.Add(new OtherSperm(game.Sperm.Location.Y, 50));
            game.IncreaseGameTimeInSeconds(10);
            game.Sperm.Core.Shot(50);
            game.IncreaseGameTimeInSeconds(1);
            game.UpdateGame();
            Assert.IsEmpty(game.GameObjects);
        }

        [Test]
        public void ScoreCoefficientCost_ShouldRightCalculateCost()
        {
            var game = new Game(0, 0, 3, 0, 0);
            Assert.AreEqual(45, game.ScoreCoefficientCost);
        }

        [Test]
        public void BuffsCost_ShouldRightCalculateCost()
        {
            var game = new Game(0, 0, 1, 20, 20);
            Assert.AreEqual(45, game.MagnetMaxTimeCost);
            Assert.AreEqual(45,game.ShieldMaxTimeCost);
        }

        [Test]
        public void ScoreCoefficient_ShouldMultiplyScore()
        {
            var gameWithRegularScores = new Game(0, 0, 1, 0, 0);
            var gameWithDoubleScores = new Game(0, 0, 2, 0, 0);
            gameWithRegularScores.IncreaseGameTimeInSeconds(10);
            gameWithDoubleScores.IncreaseGameTimeInSeconds(10);
            Assert.AreEqual(gameWithRegularScores.GetScore() * 2, gameWithDoubleScores.GetScore());
        }

        [Test]
        public void IncreaseTimeInSeconds_ShouldIncreaseAllCounters()
        {
            var game = new Game(0, 0, 0, 5, 5);
            game.Sperm.IsMagnetActivated = true;
            game.Sperm.IsShieldActivated = true;
            game.Sperm.Core.Shot(0);
            game.IncreaseGameTimeInSeconds(1);
            Assert.AreEqual(1, game.GameTimeInSeconds);
            Assert.AreEqual(1, game.EmptyFieldTime);
            Assert.AreEqual(1, game.MagnetTimeInSeconds);
            Assert.AreEqual(1, game.ShieldTimeInSeconds);
            Assert.AreEqual(1, game.Sperm.Core.flightTimeInSeconds);
            Assert.AreEqual(1, game.Sperm.Core.timeAfterShotInSeconds);
            var blood = new Blood(0, 0);
            game.GameObjects.Add(blood);
            game.Sperm.Core.Stop(0);
            game.IncreaseGameTimeInSeconds(1);
            Assert.AreEqual(1, blood.TimeAliveInSeconds);
            Assert.AreEqual(2, game.Sperm.Core.timeAfterShotInSeconds);
        }

        [Test]
        public void UpdateGame_ShouldUpdateHighestScore()
        {
            var game = new Game(0, 0, 1, 0, 0);
            game.IncreaseGameTimeInSeconds(10);
            game.UpdateGame();
            Assert.AreEqual(game.GetScore(), game.HighestScore);
        }

        [Test]
        public void UpdateGame_ShouldRemoveBuffs()
        {
            var game = new Game(0, 0, 0, 5, 5);
            game.Sperm.IsMagnetActivated = true;
            game.Sperm.IsShieldActivated = true;
            game.IncreaseGameTimeInSeconds(6);
            game.UpdateGame();
            Assert.IsFalse(game.Sperm.IsMagnetActivated);
            Assert.IsFalse(game.Sperm.IsShieldActivated);
        }

        [Test]
        public void UpdateGame_ShouldActivateBuffsAndRemoveItFromField_AfterSpermPickItUp()
        {
            var game = new Game(0, 0, 0, 1, 1);
            game.GameObjects.Add(new Shield(game.Sperm.Location.Y, 1900));
            game.IncreaseGameTimeInSeconds(1);
            game.UpdateGame();
            Assert.IsTrue(game.Sperm.IsShieldActivated);
            Assert.IsEmpty(game.GameObjects);
            game.GameObjects.Add(new Magnet(game.Sperm.Location.Y, 1900));
            game.IncreaseGameTimeInSeconds(1);
            game.UpdateGame();
            Assert.IsTrue(game.Sperm.IsMagnetActivated);
            Assert.IsEmpty(game.GameObjects);
        }

        [Test]
        public void UpdateGame_ShouldIncreaseDnaAmountAndRemoveIt_AfterSpermPickItUp()
        {
            var game = new Game(0, 0, 0, 0, 0);
            game.GameObjects.Add(new Dna(game.Sperm.Location.Y, 1900, game.Sperm));
            game.IncreaseGameTimeInSeconds(1);
            game.UpdateGame();
            Assert.AreEqual(1, game.DnaAmount);
            Assert.IsEmpty(game.GameObjects);
        }

        [Test]
        public void UpdateGame_ShouldNotEndGame_AfterSpermConflictWithBlood_IfShieldIsActivated()
        {
            var game = new Game(0, 0, 0, 5, 0);
            game.Sperm.IsShieldActivated = true;
            game.GameObjects.Add(new Blood(game.Sperm.Location.Y, 1500));
            game.IncreaseGameTimeInSeconds(1);
            game.UpdateGame();
            Assert.IsFalse(game.IsGameOver);
        }

        [Test]
        public void UpdateGame_ShouldCreateGameObject_IfFieldIsEmptyMoreThen3Seconds()
        {
            var game = new Game(0, 0, 0, 0, 0);
            game.IncreaseGameTimeInSeconds(4);
            game.UpdateGame();
            Assert.IsNotEmpty(game.GameObjects);
        }
    }
}
