using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Fight_for_The_Life.Domain;
using Fight_for_The_Life.Views;

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
            core = sperm.Core;
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
        public void GetModel_ShouldReturnModelInsideSpermModel_AfterMoveUp()
        {
            sperm.MoveUp();
            sperm.MoveUp();
            sperm.MoveUp();
            var model = core.GetModel();
            Assert.True(model.Top > sperm.Model.Top);
            Assert.True(model.Bottom < sperm.Model.Bottom);
            Assert.True(model.Left > sperm.Model.Left);
            Assert.True(model.Right < sperm.Model.Right);
        }

        [Test]
        public void Shot_ShouldChangeCoreState_IfCoreInsideSperm()
        {
            core.Shot(0);
            Assert.AreEqual(CoreState.Flying, core.State);
        }

        [Test]
        public void Shot_ShouldNotChangeCoreState_IfCoreIsNotInsideSperm()
        {
            core.Shot(0);
            core.Stop(0, 0);
            Assert.AreEqual(CoreState.Stopped, core.State);
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

        [Test]
        public void Stop_ShouldChangeCoreState()
        {
            core.Shot(0);
            core.Stop(0, 0);
            Assert.AreEqual(CoreState.Stopped, core.State);
        }

        [Test]
        public void Stop_ShouldNotChangeCoreState_IfCoreIsNotFlying()
        {
            core.Stop(0, 0);
            Assert.AreEqual(CoreState.InsideSperm, core.State);
        }

        [Test]
        public void GetModel_ShouldCalculateRightLocation_AfterStop()
        {
            core.Shot(10);
            core.Stop(10, 10);
            Assert.AreEqual(new Point(core.ShotPosition.X + 200, core.ShotPosition.Y), 
                core.GetModel(20).Location);
        }

        [Test]
        public void PickUp_ShouldChangeCoreState_IfCoreIsStopped()
        {
            core.Shot(0);
            core.Stop(0, 0);
            core.PickUp();
            Assert.AreEqual(CoreState.InsideSperm, core.State);
        }

        [Test]
        public void PickUp_ShouldNotChangeCoreState_IfCoreIsFlying()
        {
            core.Shot(0);
            core.PickUp();
            Assert.AreEqual(CoreState.Flying, core.State);
        }
    }
}
