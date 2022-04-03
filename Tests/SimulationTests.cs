using Battleship;
using MainObjects;
using NUnit.Framework;
using System;
using System.Linq;

namespace Tests
{
    public class SimulationTests
    {

        [Test]
        public void StartingSimulation_WithTwoNonNullablePlayers_RightShootViewModelListData()
        {
            #region Arrange
            Creation creationAlgorithm = new Creation();
            Simulation simulationAlgorithm = new Simulation();
            Player player1 = creationAlgorithm.CreatePlayer("Test player1");
            Player player2 = creationAlgorithm.CreatePlayer("Test player2");
            #endregion
            #region Act
            var shootViewModelsList = simulationAlgorithm.Start(player1, player2);
            #endregion
            #region Assert
            Assert.True(shootViewModelsList.Count() > 0);
            #endregion
        }

        [Test]
        public void StartingSimulation_WithNullablePlayer_ArgumentNullException()
        {
            #region Arrange
            Creation creationAlgorithm = new Creation();
            Simulation simulationAlgorithm = new Simulation();
            Player player1 = creationAlgorithm.CreatePlayer("Test player1");
            Player player2 = null;
            #endregion
            #region Assert
            Assert.Throws<ArgumentNullException> (() => simulationAlgorithm.Start(player1, player2));
            #endregion
        }

        [Test]
        public void StartingSimulation_WithNullablePlayers_ArgumentNullException()
        {
            #region Arrange
            Creation creationAlgorithm = new Creation();
            Simulation simulationAlgorithm = new Simulation();
            Player player1 = null;
            Player player2 = null;
            #endregion
            #region Assert
            Assert.Throws<ArgumentNullException>(() => simulationAlgorithm.Start(player1, player2));
            #endregion
        }
    }
}