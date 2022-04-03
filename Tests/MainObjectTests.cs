using Battleship;
using CommonObjects;
using MainObjects;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class MainObjectTests
    {

        [Test]
        public void ExtraPointCreation_WithOnlyCreatedPoint_RightExtraPoint()
        {
            #region Arrange
            Point point = Point.CreatePoint(1, 'a');
            #endregion
            #region Act
            ExtraPoint extraPoint = ExtraPoint.CreateExtraPoint(point);
            #endregion
            #region Assert
            Assert.AreEqual(point, extraPoint.Point);
            Assert.AreEqual(PointStatus.Free, extraPoint.Status);
            #endregion
        }

        [Test]
        public void ExtraPointCreation_WithCreatedPointAndStatus_RightExtraPoint()
        {
            #region Arrange
            Point point = Point.CreatePoint(1, 'a');
            #endregion
            #region Act
            ExtraPoint extraPoint = ExtraPoint.CreateExtraPoint(point, PointStatus.Taken);
            #endregion
            #region Assert
            Assert.AreEqual(point, extraPoint.Point);
            Assert.AreEqual(PointStatus.Taken, extraPoint.Status);
            #endregion
        }

        [Test]
        public void ExtraPointCreation_WithXAndY_RightExtraPoint()
        {
            #region Act
            ExtraPoint extraPoint = ExtraPoint.CreateExtraPoint(1, 'a');
            #endregion
            #region Assert
            Assert.AreEqual(1, extraPoint.Point.X);
            Assert.AreEqual('a', extraPoint.Point.Y);
            #endregion
        }

        [Test]
        public void ShipCreation_WithCreatedPoints_RightShip()
        {
            #region Arrange
            ICollection<ExtraPoint> points = new List<ExtraPoint> { ExtraPoint.CreateExtraPoint(1, 'a'), ExtraPoint.CreateExtraPoint(1, 'b'), ExtraPoint.CreateExtraPoint(1, 'c') };
            #endregion
            #region Act
            Ship ship = Ship.CreateShip("shipName", points);
            #endregion
            #region Assert
            Assert.AreEqual("shipName", ship.ShipName);
            Assert.AreEqual(3, ship.Points.Count);
            #endregion
        }

        [Test]
        public void ShipCreation_WithoutPoints_ArgumentNullException()
        {
            #region Assert
            Assert.Throws<ArgumentNullException>(() => Ship.CreateShip("shipName", null));
            #endregion
        }

        [Test]
        public void PlayerCreation_WithoutValues_CreatedPlayerWithRightValues()
        {
            #region Arrange
            ICollection<ExtraPoint> points = new List<ExtraPoint> { ExtraPoint.CreateExtraPoint(1, 'a'), ExtraPoint.CreateExtraPoint(1, 'b'), ExtraPoint.CreateExtraPoint(1, 'c') };
            Ship ship = Ship.CreateShip("shipName", points);
            #endregion
            #region Act
            Player player = Player.CreatePlayer("player", new List<Ship> { ship }, new ExtraPoint[10], new ExtraPoint[10]);
            #endregion
            #region Assert
            Assert.AreEqual("player", player.NickName);
            Assert.AreEqual(1, player.Ships.Count);
            Assert.AreEqual(10, player.OwnMap.Length);
            #endregion
        }

        [Test]
        public void PlayerCreation_WithoutValues_ArgumentNullException()
        {
            #region Assert
            Assert.Throws<ArgumentNullException>(() => Player.CreatePlayer("shipName", null, null, null));
            #endregion
        }
    }
}