using System;

namespace SharpPath
{
    public class Node
    {
        public bool IsObstacle { get; set; }
        public int X { get; }
        public int Y { get; }

        public Node(int x, int y, bool isObstacle)
        {
            X = x;
            Y = y;
            IsObstacle = isObstacle;
        }

        public int GetDisplacement(int xDestination, int yDestination)
        {
            int xDistance, yDistance;

            xDistance = Math.Abs(X - xDestination);
            yDistance = Math.Abs(Y - yDestination);

            return Math.Min(xDistance, yDistance) * 14 + Math.Abs(xDistance - yDistance) * 10;
        }

        public int GetDisplacement(Node destination)
        {
            return GetDisplacement(destination.X, destination.Y);
        }
    }
}
