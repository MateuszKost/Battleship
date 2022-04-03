using Battleship;
using MainObjects;
using NUnit.Framework;
using System.Linq;

namespace Tests
{
    public class CreationTests
    {
        [Test]
        public void CreatingPlayer_AllRightData()
        {
            #region Arrange
            CreationAlgorithm creationAlgorithm = new CreationAlgorithm();
            Player player = creationAlgorithm.CreatePlayer("Test player");
            #endregion
            #region Assert
            Assert.AreEqual("Test player", player.NickName);
            Assert.AreEqual(5, player.Ships.Count);
            Assert.AreEqual(100, player.OwnMap.Length);
            Assert.AreEqual(17, player.OwnMap.Where(p => p.Status == CommonObjects.PointStatus.Taken).Count());
            #endregion
        }

        [Test]
        public void CreatingPlayer_WithNullName_RightDataWithEmptyNickName()
        {
            #region Arrange
            CreationAlgorithm creationAlgorithm = new CreationAlgorithm();
            Player player = creationAlgorithm.CreatePlayer(null);
            #endregion
            #region Assert
            Assert.AreEqual(string.Empty, player.NickName);
            Assert.AreEqual(5, player.Ships.Count);
            Assert.AreEqual(100, player.OwnMap.Length);
            Assert.AreEqual(17, player.OwnMap.Where(p => p.Status == CommonObjects.PointStatus.Taken).Count());
            #endregion
        }
    }
}