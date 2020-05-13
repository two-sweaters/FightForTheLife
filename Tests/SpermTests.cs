using System;
using System.Drawing;
using NUnit.Framework;
using Fight_for_The_Life.Domain;

namespace Tests
{
    [TestFixture]
    class SpermTests
    {
        [Test]
        public void ShouldNotMovingOutsideTheField()
        {
            var sperm = new Sperm(Game.FieldHeight - Sperm.ModelHeight - 1);
            sperm.MoveUp();
            Assert.AreEqual(new Point(0, Game.FieldHeight - Sperm.ModelHeight - 1), sperm.Location);
            sperm = new Sperm(0);
            sperm.MoveDown();
            Assert.AreEqual(Point.Empty, sperm.Location);
        }

        [Test]
        public void ShouldThrowArgumentException_WhenCreatingOutsideField()
        {
            Assert.Catch(typeof(ArgumentException), () => new Sperm(Game.FieldHeight));
        }

        [Test]
        public void ShouldCreatingWithRightSize()
        {
            var sperm = new Sperm();
            Assert.AreEqual(71, sperm.Model.Height);
            Assert.AreEqual(326, sperm.Model.Width);
        }
    }
}
