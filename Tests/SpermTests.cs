using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var sperm = new Sperm(Game.FieldHeight - 1);
            sperm.MoveDown();
            Assert.AreEqual(new Point(0, Game.FieldHeight - 1), sperm.Location);
            sperm = new Sperm(0);
            sperm.MoveUp();
            Assert.AreEqual(Point.Empty, sperm.Location);
        }

        [Test]
        public void ShouldThrowArgumentException_WhenCreatingOutsideField()
        {
            try
            {
                var sperm = new Sperm(Game.FieldHeight);
            }
            catch (ArgumentException)
            {
            }
            catch
            {
                Assert.Fail();
            }
        }
    }
}
