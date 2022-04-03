using CommonObjects;
using MainObjects;

namespace Battleship
{
    internal class PlayerPointsDictionary : Dictionary<Point, IndexType>
    {
        private Random _random = new Random();

        #region internal functions
        internal Point CreatePointForPlayer(PlayerPointsList playerLastHitPoints, PlayerPointsList playerPointsToShoot)
        {
            int index = _random.Next(this.Count);
            if (this.Count == CommonVariables.Zero)
            {
                return playerLastHitPoints.CreateNewPointForPlayer(playerPointsToShoot);
            }
            while (playerLastHitPoints.Contains(this.ElementAt(index).Key))
            {
                this.Remove(this.ElementAt(index).Key);
                if (this.Count == CommonVariables.Zero)
                {
                    return playerLastHitPoints.CreateNewPointForPlayer(playerPointsToShoot);
                }
                index = _random.Next(this.Count);
            }
            return this.ElementAt(index).Key;
        }

        internal void CheckDictionariesForPlayer(Point _lastPoint, PointStatus pointStatus, PlayerPointsDictionary deletedPoints)
        {
            if (this.Any())
            {
                UpdateDictionariesForPlayer(_lastPoint, pointStatus, deletedPoints);
            }
            else
            {
                CreateDictionaryForPlayer(_lastPoint);
            }
        }

        internal void ReturnDeletedPoints(PlayerPointsDictionary deletedPoints)
        {
            foreach (var item in deletedPoints)
            {
                if (!this.ContainsKey(item.Key))
                {
                    this.Add(item.Key, item.Value);
                }
            }
            deletedPoints.Clear();
        }
        #endregion

        #region private functions
        private void CreateDictionaryForPlayer(Point _lastPoint)
        {
            FindIndexes(_lastPoint, out int xIndex, out int yIndex);
            int xIndexCopy = xIndex;
            int yIndexCopy = yIndex;

            TryToAddPointToNextProbablyPointsForPlayer(IndexType.VerticalDown, xIndex, yIndexCopy);
            yIndexCopy = yIndex;
            TryToAddPointToNextProbablyPointsForPlayer(IndexType.VerticalUp, xIndex, yIndexCopy);
            TryToAddPointToNextProbablyPointsForPlayer(IndexType.HorizontalLeft, xIndexCopy, yIndex);
            xIndexCopy = xIndex;
            TryToAddPointToNextProbablyPointsForPlayer(IndexType.HorizontalRight, xIndexCopy, yIndex);
        }

        private void UpdateDictionariesForPlayer(Point _lastPoint, PointStatus pointStatus, PlayerPointsDictionary deletedPoints)
        {
            IndexType indexType = this[_lastPoint];
            FindIndexes(_lastPoint, out int xIndex, out int yIndex);

            if (pointStatus == PointStatus.Hit)
            {
                switch (indexType)
                {
                    case IndexType.VerticalDown:
                        RemoveHorizontalLeftAndRight(deletedPoints);
                        break;
                    case IndexType.VerticalUp:
                        RemoveHorizontalLeftAndRight(deletedPoints);
                        break;
                    case IndexType.HorizontalLeft:
                        RomoveVerticalUpAndDown(deletedPoints);
                        break;
                    case IndexType.HorizontalRight:
                        RomoveVerticalUpAndDown(deletedPoints);
                        break;
                }
                TryToAddPointToNextProbablyPointsForPlayer(indexType, xIndex, yIndex);
            }
            this.Remove(_lastPoint);
        }

        private void RemoveHorizontalLeftAndRight(PlayerPointsDictionary deletedPoints)
        {
            foreach (var item in this.Where(i => i.Value == IndexType.HorizontalLeft || i.Value == IndexType.HorizontalRight))
            {
                AddToDeletedPoints(deletedPoints, item);
            }
        }

        private void RomoveVerticalUpAndDown(PlayerPointsDictionary deletedPoints)
        {
            foreach (var item in this.Where(i => i.Value == IndexType.VerticalDown || i.Value == IndexType.VerticalUp))
            {
                AddToDeletedPoints(deletedPoints, item);
            }
        }

        private void AddToDeletedPoints(PlayerPointsDictionary deletedPoints, KeyValuePair<Point, IndexType> item)
        {
            this.Remove(item.Key);
            if (!deletedPoints.ContainsKey(item.Key))
            {
                deletedPoints.Add(item.Key, item.Value);
            }
        }

        private void FindIndexes(Point _lastPoint, out int xIndex, out int yIndex)
        {
            xIndex = Array.IndexOf(CommonVariables.DefaultXAxis, _lastPoint.X);
            yIndex = Array.IndexOf(CommonVariables.DefaultYAxis, _lastPoint.Y);
        }

        private void TryToAddPointToNextProbablyPointsForPlayer(IndexType indexType, int xIndex, int yIndex)
        {
            DecrementOrIncrementIndex(indexType, xIndex, yIndex, out xIndex, out yIndex);
            if (yIndex >= CommonVariables.FirstIndexOfX_Y_Axis && yIndex <= CommonVariables.LastIndexOfX_Y_Axis && xIndex >= CommonVariables.FirstIndexOfX_Y_Axis && xIndex <= CommonVariables.LastIndexOfX_Y_Axis)
            {
                this.Add(Point.CreatePoint(CommonVariables.DefaultXAxis[xIndex], CommonVariables.DefaultYAxis[yIndex]), indexType);
            }
        }

        private void DecrementOrIncrementIndex(IndexType indexType, int xIndex, int yIndex, out int newXIndex, out int newYIndex)
        {
            switch (indexType)
            {
                case IndexType.VerticalDown:
                    yIndex--;
                    break;
                case IndexType.VerticalUp:
                    yIndex++;
                    break;
                case IndexType.HorizontalLeft:
                    xIndex--;
                    break;
                case IndexType.HorizontalRight:
                    xIndex++;
                    break;
            }
            newXIndex = xIndex;
            newYIndex = yIndex;
        }
        #endregion
    }
}
