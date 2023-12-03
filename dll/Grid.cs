using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpPath
{
    public class Grid
    {
        public int Width { get; }
        public int Height { get; }
        public Node[,] Nodes { get; }

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

        public Node GetNextNodeNorthwest(int x, int y)
        {
            ValidatePosition(x, y);

            if (x > 0 && y > 0)
            {
                Node nextNode;

                nextNode = Nodes[x - 1, y - 1];

                if (!nextNode.IsObstacle)
                {
                    return nextNode;
                }
            }

            return null;
        }

        public Node GetNextNodeNortheast(int x, int y)
        {
            ValidatePosition(x, y);

            if (x < Width - 1 && y > 0)
            {
                Node nextNode;

                nextNode = Nodes[x + 1, y - 1];

                if (!nextNode.IsObstacle)
                {
                    return nextNode;
                }
            }

            return null;
        }

        public Node GetNextNodeSouthwest(int x, int y)
        {
            ValidatePosition(x, y);

            if (x > 0 && y < Height - 1)
            {
                Node nextNode;

                nextNode = Nodes[x - 1, y + 1];

                if (!nextNode.IsObstacle)
                {
                    return nextNode;
                }
            }

            return null;
        }

        public Node GetNextNodeSoutheast(int x, int y)
        {
            ValidatePosition(x, y);

            if (x < Width - 1 && y < Height - 1)
            {
                Node nextNode;

                nextNode = Nodes[x + 1, y + 1];

                if (!nextNode.IsObstacle)
                {
                    return nextNode;
                }
            }

            return null;
        }

        public Path FindPath(int xStart, int yStart, int xDestination, int yDestination, SearchDirections searchDirections, bool includeStart)
        {
            ValidatePosition(xStart, yStart);
            ValidatePosition(xDestination, yDestination);

            int xCurrent, yCurrent;
            Path path;
            NodeDataMatrix nodeDataMatrix;
            List<Node> nodesChecked;
            NodeData nodeDataCurrent;

            path = new Path();
            nodesChecked = new List<Node>();
            xCurrent = xStart;
            yCurrent = yStart;
            nodeDataMatrix = new NodeDataMatrix(this, xCurrent, yCurrent, xDestination, yDestination);

            while (!ProcessPath(ref xCurrent, ref yCurrent, xDestination, yDestination, nodeDataMatrix, nodesChecked, searchDirections)) { }

            nodeDataCurrent = nodeDataMatrix[xDestination, yDestination];

            while (!(nodeDataCurrent.Node.X == xStart && nodeDataCurrent.Node.Y == yStart))
            {
                path.Insert(0, nodeDataCurrent.Node);

                if (nodeDataCurrent.Parent is null)
                {
                    throw new Exception("Node in path has null parent.");
                }

                nodeDataCurrent = nodeDataCurrent.Parent;
            }

            if (includeStart)
            {
                path.Insert(0, Nodes[xStart, yStart]);
            }

            return path;
        }

        bool ProcessPath(ref int xCenter, ref int yCenter, int xDestination, int yDestination, NodeDataMatrix nodeDataMatrix, List<Node> nodesChecked, SearchDirections searchDirections)
        {
            NodeData centerNodeData;
            List<Node> adjacentNodes;
            Node mostPromisingNode;
            IOrderedEnumerable<Node> orderedNodes;

            if (xCenter == xDestination && yCenter == yDestination)
            {
                return true;
            }

            centerNodeData = nodeDataMatrix[xCenter, yCenter];

            switch (searchDirections)
            {
                case SearchDirections.Four:
                    adjacentNodes = new NodeAreaSimple(this, nodeDataMatrix, xCenter, yCenter).AdjacentNodes();
                    break;

                default:
                    adjacentNodes = new NodeArea(this, nodeDataMatrix, xCenter, yCenter).AdjacentNodes();
                    break;
            }

            foreach (Node node in adjacentNodes)
            {
                NodeData nodeData;

                nodeData = nodeDataMatrix[node];

                nodeData.UpdateGCost(centerNodeData);
                if (!nodesChecked.Contains(nodeData.Node)) nodesChecked.Add(nodeData.Node);
            }

            orderedNodes = orderNodes(nodesChecked, nodeDataMatrix);
            mostPromisingNode = orderedNodes.First();

            if ((xCenter == mostPromisingNode.X && yCenter == mostPromisingNode.Y))
            {
                nodeDataMatrix[mostPromisingNode].SetDeadEnd();
                nodesChecked.RemoveAll(node => node == mostPromisingNode);

                if (nodesChecked.Count == 0)
                {
                    throw new Exception("Path can't be found.");
                }

                mostPromisingNode = orderedNodes.First();
            }

            xCenter = mostPromisingNode.X;
            yCenter = mostPromisingNode.Y;

            return false;
        }

        void ValidatePosition(int x, int y)
        {
            if (x < 0 || x > Width - 1 || y < 0 || y > Height - 1)
            {
                throw new Exception($"position [{x}, {y}] is outside the bounds of the grid.");
            }
        }

        static IOrderedEnumerable<Node> orderNodes(IEnumerable<Node> nodes, NodeDataMatrix nodeDataMatrix)
        {
            return nodes.OrderBy(node =>
            {
                NodeData nodeData;

                nodeData = nodeDataMatrix[node];

                return nodeData.FCost + nodeData.GCost;
            });
        }

        class NodeDataMatrix
        {
            NodeData[,] matrix;

            public NodeDataMatrix(Grid grid, int xStart, int yStart, int xDestination, int yDestination)
            {
                matrix = new NodeData[grid.Width, grid.Height];
                Init(grid, xStart, yStart, xDestination, yDestination);
            }

            public NodeDataMatrix(Grid grid, Node nodeStart, Node nodeDestination)
            {
                matrix = new NodeData[grid.Width, grid.Height];
                Init(grid, nodeStart.X, nodeStart.Y, nodeDestination.X, nodeDestination.Y);
            }

            public NodeData this[int x, int y]
            {
                get => matrix[x, y];
                set => matrix[x, y] = value;
            }

            public NodeData this[Node node]
            {
                get => matrix[node.X, node.Y];
                set => matrix[node.X, node.Y] = value;
            }

            void Init(Grid grid, int xStart, int yStart, int xDestination, int yDestination)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    for (int y = 0; y < grid.Height; y++)
                    {
                        matrix[x, y] = new NodeData(grid.Nodes[x, y], xDestination, yDestination);
                    }
                }

                matrix[xStart, yStart].UpdateGCost(matrix[xStart, yStart]);
            }
        }

        class NodeData
        {
            public Node Node { get; }
            public int FCost { get => (GCost ?? 0) + HCost; }
            public int? GCost { get; private set; } = null;
            public int HCost { get; private set; }
            public NodeData Parent { get; private set; } = null;
            public bool DeadEnd { get; private set; } = false;

            public NodeData(Node node, int xDestination, int yDestination)
            {
                Node = node;
                UpdateHCost(xDestination, yDestination);
            }

            public void UpdateGCost(NodeData other)
            {
                int gcost;

                gcost = Node.GetDisplacement(other.Node.X, other.Node.Y);

                if (GCost is null || (gcost + HCost < other.FCost))
                {
                    GCost = gcost;
                    if (this != other) Parent = other;
                }
            }

            void UpdateHCost(int xDestination, int yDestination)
            {
                HCost = Node.GetDisplacement(xDestination, yDestination);
            }

            void UpdateHCost(Node destination)
            {
                UpdateHCost(destination.X, destination.Y);
            }

            public void SetDeadEnd()
            {
                DeadEnd = true;
            }
        }

        class NodeAreaSimple
        {
            public Node Center { get; }
            public Grid Grid { get; }
            public Node North { get; protected set; }
            public Node East { get; protected set; }
            public Node West { get; protected set; }
            public Node South { get; protected set; }

            public NodeAreaSimple(Grid grid, NodeDataMatrix nodeDataMatrix, int centerX, int centerY)
            {
                Grid = grid;
                Center = grid.Nodes[centerX, centerY];

                North = ClearDeadEnds(grid.GetNextNodeNorth(centerX, centerY), nodeDataMatrix);
                East = ClearDeadEnds(grid.GetNextNodeEast(centerX, centerY), nodeDataMatrix);
                West = ClearDeadEnds(grid.GetNextNodeWest(centerX, centerY), nodeDataMatrix);
                South = ClearDeadEnds(grid.GetNextNodeSouth(centerX, centerY), nodeDataMatrix);
            }

            public virtual List<Node> AdjacentNodes()
            {
                List<Node> nodes;

                nodes = new List<Node>();

                if (North != null) nodes.Add(North);
                if (East != null) nodes.Add(East);
                if (West != null) nodes.Add(West);
                if (South != null) nodes.Add(South);

                return nodes;
            }

            protected Node ClearDeadEnds(Node node, NodeDataMatrix nodeDataMatrix)
            {
                if (node != null && nodeDataMatrix[node].DeadEnd)
                {
                    return null;
                }

                return node;
            }
        }

        class NodeArea : NodeAreaSimple
        {
            public Node Northwest { get; protected set; }
            public Node Northeast { get; protected set; }
            public Node Southwest { get; protected set; }
            public Node Southeast { get; protected set; }

            public NodeArea(Grid grid, NodeDataMatrix nodeDataMatrix, int centerX, int centerY) : base(grid, nodeDataMatrix, centerX, centerY)
            {
                Northwest = ClearDeadEnds(grid.GetNextNodeNorthwest(centerX, centerY), nodeDataMatrix);
                Northeast = ClearDeadEnds(grid.GetNextNodeNortheast(centerX, centerY), nodeDataMatrix);
                Southwest = ClearDeadEnds(grid.GetNextNodeSouthwest(centerX, centerY), nodeDataMatrix);
                Southeast = ClearDeadEnds(grid.GetNextNodeSoutheast(centerX, centerY), nodeDataMatrix);
            }

            public override List<Node> AdjacentNodes()
            {
                List<Node> nodes;

                nodes = base.AdjacentNodes();

                if (Northwest != null) nodes.Add(Northwest);
                if (Northeast != null) nodes.Add(Northeast);
                if (Southwest != null) nodes.Add(Southwest);
                if (Southeast != null) nodes.Add(Southeast);

                return nodes;
            }
        }
    }
}
