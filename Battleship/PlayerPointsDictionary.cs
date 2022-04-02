using CommonObjects;
using MainObjects;

namespace Battleship
{
    internal class PlayerPointsDictionary : Dictionary<Point, IndexType>
    {
        private Random _random = new Random();

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

        internal void CheckDictionariesForPlayer(Point _lastPoint)
        {
            if (this.Any())
            {
                UpdateDictionariesForPlayer(_lastPoint);
            }
            else
            {
                CreateDictionaryForPlayer(_lastPoint);
            }
        }

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

        private void UpdateDictionariesForPlayer(Point _lastPoint)
        {
            IndexType indexType = this[_lastPoint];
            FindIndexes(_lastPoint, out int xIndex, out int yIndex);

            switch (indexType)
            {
                case IndexType.VerticalDown:
                    foreach (var item in this.Where(i => i.Value == IndexType.HorizontalLeft || i.Value == IndexType.HorizontalRight))
                    {
                        this.Remove(item.Key);
                    }
                    break;
                case IndexType.VerticalUp:
                    foreach (var item in this.Where(i => i.Value == IndexType.HorizontalLeft || i.Value == IndexType.HorizontalRight))
                    {
                        this.Remove(item.Key);
                    }
                    break;
                case IndexType.HorizontalLeft:
                    foreach (var item in this.Where(i => i.Value == IndexType.VerticalDown || i.Value == IndexType.VerticalUp))
                    {
                        this.Remove(item.Key);
                    }
                    break;
                case IndexType.HorizontalRight:
                    foreach (var item in this.Where(i => i.Value == IndexType.VerticalDown || i.Value == IndexType.VerticalUp))
                    {
                        this.Remove(item.Key);
                    }
                    break;
            }

            TryToAddPointToNextProbablyPointsForPlayer(indexType, xIndex, yIndex);
            this.Remove(_lastPoint);
        }

        private void FindIndexes(Point _lastPoint, out int xIndex, out int yIndex)
        {
            xIndex = Array.IndexOf(CommonVariables.DefaultXAxis, _lastPoint.X);
            yIndex = Array.IndexOf(CommonVariables.DefaultYAxis, _lastPoint.Y);
        }

        private void TryToAddPointToNextProbablyPointsForPlayer(IndexType indexType, int xIndex, int yIndex)
        {
            if (yIndex != CommonVariables.FirstIndexOfX_Y_Axis && yIndex != CommonVariables.LastIndexOfX_Y_Axis && xIndex != CommonVariables.FirstIndexOfX_Y_Axis && xIndex != CommonVariables.LastIndexOfX_Y_Axis)
            {
                DecrementOrIncrementIndex(indexType, xIndex, yIndex, out xIndex, out yIndex);
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
    }
}
