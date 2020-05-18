using Fight_for_The_Life.Domain;
using Fight_for_The_Life.Domain.GameObjects;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    class DnaTests
    {
        [Test]
        public void DnaShouldHaveSameYCoordinateWithSperm_AfterMagnetActivated()
        {
            var sperm = new Sperm(500);
            var dna = new Dna(0, 0, sperm);
            sperm.IsMagnetActivated = true;
            Assert.AreEqual(sperm.Location.Y, dna.GetLocation().Y);
        }
    }
}
