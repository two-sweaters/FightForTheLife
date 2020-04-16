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
            Thread.Sleep(1000);
            Assert.AreEqual(intrauterineDevice.Location, new IntrauterineDevice(0, 0).Location);
        }

        [Test]
        public void ShouldThrowArgumentException_WhenCreatingOutsideField()
        {
            try
            {
                var intrauterineDevice = new IntrauterineDevice(Game.FieldHeight, 0);

            }
            catch (ArgumentException)
            {
            }
            catch
            {
                Assert.Fail();
            }

        }

        [Test]
        public void ShouldRightCalculateLocation()
        {
            var intrauterineDevice = new IntrauterineDevice(0, 1);
            Thread.Sleep(2000);
            Assert.AreEqual(new Point(Game.FieldWidth - 3, 0), intrauterineDevice.Location);
        }
    }
}
