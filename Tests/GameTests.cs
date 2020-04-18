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
            Assert.AreEqual(480, game.GetVelocityInPixelsPerSecond(0));
        }

        [Test]
        public void ShouldCalculateRightVelocity_15SecondsAfterCreation()
        {
            var game = new Game();
            Assert.AreEqual(600, game.GetVelocityInPixelsPerSecond(15));
        }

        [Test]
        public void ShouldCalculateRightScore()
        {
            var game = new Game();
            Assert.AreEqual(1250, game.GetScore(5));
        }
    }
}
