﻿using System;
using Fight_for_The_Life.Domain;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using Fight_for_The_Life.Domain.Enemies;

namespace Tests
{
    [TestFixture]
    class BloodTests
    {
        [Test]
        public void ShouldNotMoving_WhenCreatingWithZeroVelocity()
        {
            var blood = new Blood(0, 0);
            Assert.AreEqual(new Point(Game.FieldWidth - 1, 0), blood.GetLocation(1));
        }

        [Test]
        public void ShouldThrowArgumentException_WhenCreatingOutsideField()
        {
            Assert.Catch(typeof(ArgumentException), () => new Blood(Game.FieldHeight, 0));
        }

        [Test]
        public void ShouldCalculateRightLocation()
        {
            var blood = new Blood(0, 10);
            Assert.AreEqual(new Point(Game.FieldWidth - 25, 0), blood.GetLocation(2));
        }

        [Test]
        public void ShouldCreatingWithRightSize()
        {
            var blood= new Blood(0, 1);
            var model = blood.GetModel(0);
            Assert.AreEqual(75, model.Height);
            Assert.AreEqual(120, model.Width);
        }
    }
}