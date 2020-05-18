using System.Drawing;
using NUnit.Framework;
using Fight_for_The_Life.Domain;

namespace Tests
{
    [TestFixture]
    class CoreTests
    {
        private Game game;

        [SetUp]
        public void SetUp()
        {
            game = new Game(0, 0, 1, 0, 0);
        }

        [Test]
        public void ShouldBeInsideSperm_AfterCreation()
        {
            Assert.AreEqual(CoreState.InsideSperm, game.Sperm.Core.State);
        }

        [Test]
        public void GetModel_ShouldReturnModelInsideSpermModel_AfterCreation()
        {
            var model = game.Sperm.Core.GetModel();
            Assert.True(model.Top > game.Sperm.Model.Top);
            Assert.True(model.Bottom < game.Sperm.Model.Bottom);
            Assert.True(model.Left > game.Sperm.Model.Left);
            Assert.True(model.Right < game.Sperm.Model.Right);
        }

        [Test]
        public void GetModel_ShouldReturnModelInsideSpermModel_AfterMoveUp()
        {
            game.Sperm.MoveUp();
            game.Sperm.MoveUp();
            game.Sperm.MoveUp();
            var model = game.Sperm.Core.GetModel();
            Assert.True(model.Top > game.Sperm.Model.Top);
            Assert.True(model.Bottom < game.Sperm.Model.Bottom);
            Assert.True(model.Left > game.Sperm.Model.Left);
            Assert.True(model.Right < game.Sperm.Model.Right);
        }

        [Test]
        public void Shot_ShouldChangeCoreState_IfCoreInsideSperm()
        {
            game.Sperm.Core.Shot(0);
            Assert.AreEqual(CoreState.Flying, game.Sperm.Core.State);
        }

        [Test]
        public void Shot_ShouldNotChangeCoreState_IfCoreIsNotInsideSperm()
        {
            game.Sperm.Core.Shot(0);
            game.Sperm.Core.Stop( 0);
            Assert.AreEqual(CoreState.Stopped, game.Sperm.Core.State);
        }

        [Test]
        public void ShotPosition_ShouldBeEqualCorePosition_AfterShot()
        {
            game.Sperm.Core.Shot(0);
            Assert.AreEqual(game.Sperm.Core.GetModel().Location, game.Sperm.Core.ShotPosition);
        }

        [Test]
        public void GetModel_ShouldCalculateRightLocation_AfterShot()
        {
            game.Sperm.Core.Shot(10);
            game.IncreaseGameTimeInSeconds(10);
            Assert.AreEqual(new Point(game.Sperm.Core.ShotPosition.X + 300, game.Sperm.Core.ShotPosition.Y),
                game.Sperm.Core.GetModel().Location);
        }

        [Test]
        public void Stop_ShouldChangeCoreState()
        {
            game.Sperm.Core.Shot(0);
            game.Sperm.Core.Stop( 0);
            Assert.AreEqual(CoreState.Stopped, game.Sperm.Core.State);
        }

        [Test]
        public void Stop_ShouldNotChangeCoreState_IfCoreIsNotFlying()
        {
            game.Sperm.Core.Stop( 0);
            Assert.AreEqual(CoreState.InsideSperm, game.Sperm.Core.State);
        }

        [Test]
        public void GetModel_ShouldCalculateRightLocation_AfterStop()
        {
            game.Sperm.Core.Shot(10);
            game.IncreaseGameTimeInSeconds(10);
            game.Sperm.Core.Stop(10);
            game.IncreaseGameTimeInSeconds(15);
            Assert.AreEqual(new Point(game.Sperm.Core.ShotPosition.X + 150, game.Sperm.Core.ShotPosition.Y),
                game.Sperm.Core.GetModel().Location);
        }

        [Test]
        public void PickUp_ShouldChangeCoreState_IfCoreIsStopped()
        {
            game.Sperm.Core.Shot(0);
            game.Sperm.Core.Stop(0);
            game.Sperm.Core.PickUp();
            Assert.AreEqual(CoreState.InsideSperm, game.Sperm.Core.State);
        }

        [Test]
        public void PickUp_ShouldNotChangeCoreState_IfCoreIsFlying()
        {
            game.Sperm.Core.Shot(0);
            game.Sperm.Core.PickUp();
            Assert.AreEqual(CoreState.Flying, game.Sperm.Core.State);
        }
    }
}
