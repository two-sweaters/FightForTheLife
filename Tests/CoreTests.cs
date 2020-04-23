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
    class CoreTests
    {
        private Core core;
        private Sperm sperm;

        [SetUp]
        public void SetUp()
        {
            sperm = new Sperm();
            core = new Core(sperm);
        }

        [Test]
        public void ShouldBeInsideSperm_AfterCreation()
        {
            Assert.AreEqual(CoreState.InsideSperm, core.State);
        }

        [Test]
        public void GetModel_ShouldReturnModelInsideSpermModel_AfterCreation()
        {
            var model = core.GetModel();
            Assert.True(model.Top > sperm.Model.Top);
            Assert.True(model.Bottom < sperm.Model.Bottom);
            Assert.True(model.Left > sperm.Model.Left);
            Assert.True(model.Right < sperm.Model.Right);
        }

        [Test]
        public void Shot_ShouldChangeCoreState()
        {
            core.Shot(0);
            Assert.AreEqual(CoreState.Flying, core.State);
        }

        [Test]
        public void ShotPosition_ShouldBeEqualCorePosition_AfterShot()
        {
            core.Shot(0);
            Assert.AreEqual(core.GetModel().Location, core.ShotPosition);
        }

        [Test]
        public void GetModel_ShouldCalculateRightLocation_AfterShot()
        {
            core.Shot(10);
            Assert.AreEqual(new Point(core.ShotPosition.X + 300, core.ShotPosition.Y),
                core.GetModel(10).Location);
        }
    }
}
