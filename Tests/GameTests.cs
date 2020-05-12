﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            var game = new Game();
            Assert.AreEqual(480, game.GetVelocityInPixelsPerSecond());
        }

        [Test]
        public void ShouldCalculateRightVelocity_15SecondsAfterCreation()
        {
            var game = new Game();
            Assert.AreEqual(600, game.GetVelocityInPixelsPerSecond());
        }

        [Test]
        public void ShouldCalculateRightScore()
        {
            var game = new Game();
            Assert.AreEqual(1250, game.GetScore());
        }

        [Test]
        public void GenerateRandomEnemy_ShouldCreateAnyEnemy()
        {
            var game = new Game();
            game.GenerateRandomGameObject();
            Assert.IsNotEmpty(game.GameObjects);
        }

        [Test]
        public void UpdateGame_ShouldEndGame_IfCoreFlyAway()
        {
            var game = new Game();
            game.Sperm.Core.Shot(2000);
            game.UpdateGame();
            Assert.IsTrue(game.IsGameOver);
        }

        [Test]
        public void UpdateGame_ShouldEndGame_IfOtherSpermFlyAway()
        {
            var game = new Game();
            game.GameObjects.Add(new OtherSperm(5, 2000));
            game.UpdateGame(1);
            Assert.IsTrue(game.IsGameOver);
        }

        [Test]
        public void UpdateGame_ShouldEndGame_AfterSpermConflictWithBlood()
        {
            var game = new Game();
            game.GameObjects.Add(new Blood(game.Sperm.Location.Y, 1500));
            game.UpdateGame(1);
            Assert.IsTrue(game.IsGameOver);
        }

        [Test]
        public void UpdateGame_ShouldRemoveEnemy_IfEnemyFlyAway()
        {
            var game = new Game();
            game.GameObjects.Add(new Blood(game.Sperm.Location.Y, 2000));
            game.UpdateGame(1);
            Assert.IsEmpty(game.GameObjects);
        }

        [Test]
        public void UpdateGame_ShouldCreateSomeone_AfterTwoSeconds()
        {
            var game = new Game();
            for (int i = 0; i < 80; i++)
            {
                game.UpdateGame(i * 10);
            }
            Assert.IsNotEmpty(game.GameObjects);
        }

        [Test]
        public void UpdateGame_ShouldRemoveOtherSperm_IfCoreHitIt()
        {
            var game = new Game();
            game.GameObjects.Add(new OtherSperm(game.Sperm.Location.Y, 50));
            game.Sperm.Core.Shot(50);
            game.UpdateGame(1);
            Assert.IsEmpty(game.GameObjects);
        }
    }
}
