using System;
using Fight_for_The_Life.Domain;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;

namespace Tests
{
    [TestFixture]
    class IntrauterineDeviceTests
    {
        [Test]
        public void ShouldNotMoving_WhenCreatingWithZeroVelocity()
        {
            var intrauterineDevice = new IntrauterineDevice(0, 0);
            Assert.AreEqual(new Point(Game.FieldWidth - 1, 0), intrauterineDevice.GetLocation(1));
        }

        [Test]
        public void ShouldThrowArgumentException_WhenCreatingOutsideField()
        {
            Assert.Catch(typeof(ArgumentException), () => new IntrauterineDevice(Game.FieldHeight, 0));
        }

        [Test]
        public void ShouldCalculateRightLocation()
        {
            var intrauterineDevice = new IntrauterineDevice(0, 1);
            Assert.AreEqual(new Point(Game.FieldWidth - 3, 0), intrauterineDevice.GetLocation(2));
        }

        [Test]
        public void ShouldCreatingWithRightSize()
        {
            var intrauterineDevice = new IntrauterineDevice(0, 1);
            var model = intrauterineDevice.GetModel(0);
            Assert.AreEqual(99, model.Height);
            Assert.AreEqual(780, model.Width);
        }
    }
}
