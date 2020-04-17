using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Fight_for_The_Life.Domain;

namespace Tests
{
    [TestFixture]
    class GameTests
    {
        [Test]
        public void ShouldCalculateRightVelocity_AfterCreation()
        {
            var game = new Game();
            Assert.AreEqual(480, game.VelocityInPixelsPerSecond);
        }

        [Test]
        public void ShouldCalculateRightVelocity_15SecondsAfterCreation()
        {
            var game = new Game();
            Thread.Sleep(15000);
            Assert.AreEqual(600, game.VelocityInPixelsPerSecond);
        }

        [Test]
        public void ShouldCalculateRightScore()
        {
            var game = new Game();
            Thread.Sleep(5000);
            Assert.AreEqual(1250, game.Score);
        }
    }
}
