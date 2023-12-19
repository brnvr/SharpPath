using SharpPath.Exceptions;

namespace SharpPath
{
    public class Grid
    {
        public int Width { get; }
        public int Height { get; }
        public Node[,] Nodes { get; }

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;

            Nodes = new Node[Width, Height];

            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    Nodes[column, row] = new Node(column, row, false);
                }
            }
        }

        public Grid(int[,] obstacles)
        {
            Width = obstacles.GetLength(1);
            Height = obstacles.GetLength(0);

            Nodes = new Node[Width, Height];

            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    Nodes[column, row] = new Node(column, row, obstacles[row, column] != 0);
                }
            }
        }

        public Grid(byte[] buffer, int width)
        {
            int row, column;

            Width = width;
            Height = buffer.Length / Width;
            Nodes = new Node[Width, Height];
            row = column = -1;

            foreach (byte b in buffer)
            {
                if (++column % Width == 0)
                {
                    row++;
                    column = 0;
                }

                Nodes[column, row] = new Node(column, row, b != 0);
            }
        }

        public Node GetNextNodeEast(int x, int y)
        {
            ValidatePosition(x, y);

            if (x < Width - 1)
            {
                Node nextNode;

                nextNode = Nodes[x + 1, y];

                if (!nextNode.IsObstacle)
                {
                    return nextNode;
                }
            }

            return null;
        }

        public Node GetNextNodeWest(int x, int y)
        {
            ValidatePosition(x, y);

            if (x > 0)
            {
                Node nextNode;

                nextNode = Nodes[x - 1, y];

                if (!nextNode.IsObstacle)
                {
                    return nextNode;
                }
            }

            return null;
        }

        public Node GetNextNodeNorth(int x, int y)
        {
            ValidatePosition(x, y);

            if (y > 0)
            {
                Node nextNode;

                nextNode = Nodes[x, y - 1];

                if (!nextNode.IsObstacle)
                {
                    return nextNode;
                }
            }

            return null;
        }

        public Node GetNextNodeSouth(int x, int y)
        {
            ValidatePosition(x, y);

            if (y < Height - 1)
            {
                Node nextNode;

                nextNode = Nodes[x, y + 1];

                if (!nextNode.IsObstacle)
                {
                    return nextNode;
                }
            }

            return null;
        }

        public Node GetNextNodeNorthwest(int x, int y, bool ignoreCorners)
        {
            ValidatePosition(x, y);

            if (x > 0 && y > 0)
            {
                Node nextNode;

                nextNode = Nodes[x - 1, y - 1];

                if (!nextNode.IsObstacle)
                {
                    if (!ignoreCorners && (GetNextNodeNorth(x, y) is null || GetNextNodeWest(x, y) is null))
                    {
                        return null;
                    }

                    return nextNode;
                }
            }

            return null;
        }

        public Node GetNextNodeNortheast(int x, int y, bool ignoreCorners)
        {
            ValidatePosition(x, y);

            if (x < Width - 1 && y > 0)
            {
                Node nextNode;

                nextNode = Nodes[x + 1, y - 1];

                if (!nextNode.IsObstacle)
                {
                    if (!ignoreCorners && (GetNextNodeNorth(x, y) is null || GetNextNodeEast(x, y) is null))
                    {
                        return null;
                    }

                    return nextNode;
                }
            }

            return null;
        }

        public Node GetNextNodeSouthwest(int x, int y, bool ignoreCorners)
        {
            ValidatePosition(x, y);

            if (x > 0 && y < Height - 1)
            {
                Node nextNode;

                nextNode = Nodes[x - 1, y + 1];

                if (!nextNode.IsObstacle)
                {
                    if (!ignoreCorners && (GetNextNodeSouth(x, y) is null || GetNextNodeWest(x, y) is null))
                    {
                        return null;
                    }

                    return nextNode;
                }
            }

            return null;
        }

        public Node GetNextNodeSoutheast(int x, int y, bool ignoreCorners)
        {
            ValidatePosition(x, y);

            if (x < Width - 1 && y < Height - 1)
            {
                Node nextNode;

                nextNode = Nodes[x + 1, y + 1];

                if (!nextNode.IsObstacle)
                {
                    if (!ignoreCorners && (GetNextNodeSouth(x, y) is null || GetNextNodeEast(x, y) is null))
                    {
                        return null;
                    }

                    return nextNode;
                }
            }

            return null;
        }

        internal void ValidatePosition(int x, int y)
        {
            if (x < 0 || x > Width - 1 || y < 0 || y > Height - 1 || Nodes[x, y].IsObstacle)
            {
                throw new GridPositionOutOfBoundsException(x, y);
            }
        }
    }
}
