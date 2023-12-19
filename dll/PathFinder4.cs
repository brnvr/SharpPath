using SharpPath.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpPath
{
    public class PathFinder4 : IPathFinder
    {
        public Grid Grid { get; }
        public bool IncludeFirstNode { get; set; } = true;
        
        public PathFinder4(Grid grid, bool includeFirstNode = true)
        {
            Grid = grid;
            IncludeFirstNode = includeFirstNode;
        }

        public Path Run(int xStart, int yStart, int xDestination, int yDestination)
        {
            Grid.ValidatePosition(xStart, yStart);
            Grid.ValidatePosition(xDestination, yDestination);

            int xCurrent, yCurrent;
            Path path;
            NodeDataMatrix nodeDataMatrix;
            List<Node> nodesChecked;
            NodeData nodeDataCurrent;

            path = new Path();
            nodesChecked = new List<Node>();
            xCurrent = xStart;
            yCurrent = yStart;
            nodeDataMatrix = new NodeDataMatrix(Grid, xCurrent, yCurrent, xDestination, yDestination);

            while (!ProcessPath(ref xCurrent, ref yCurrent, xDestination, yDestination, nodeDataMatrix, nodesChecked)) { }

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

            if (IncludeFirstNode)
            {
                path.Insert(0, Grid.Nodes[xStart, yStart]);
            }

            return path;
        }

        bool ProcessPath(ref int xCenter, ref int yCenter, int xDestination, int yDestination, NodeDataMatrix nodeDataMatrix, List<Node> nodesChecked)
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
            adjacentNodes = GetNodeArea(nodeDataMatrix, xCenter, yCenter).AdjacentNodes();

            foreach (Node node in adjacentNodes)
            {
                NodeData nodeData;

                nodeData = nodeDataMatrix[node];

                nodeData.UpdateGCost(centerNodeData);
                if (!nodesChecked.Contains(nodeData.Node)) nodesChecked.Add(nodeData.Node);
            }

            orderedNodes = OrderNodes(nodesChecked, nodeDataMatrix);
            mostPromisingNode = orderedNodes.First();

            if ((xCenter == mostPromisingNode.X && yCenter == mostPromisingNode.Y))
            {
                nodeDataMatrix[mostPromisingNode].SetDeadEnd();
                nodesChecked.RemoveAll(node => node == mostPromisingNode);

                if (nodesChecked.Count == 0)
                {
                    throw new PathNotFoundException();
                }

                mostPromisingNode = orderedNodes.First();
            }

            xCenter = mostPromisingNode.X;
            yCenter = mostPromisingNode.Y;

            return false;
        }

        protected static IOrderedEnumerable<Node> OrderNodes(IEnumerable<Node> nodes, NodeDataMatrix nodeDataMatrix)
        {
            return nodes.OrderBy(node =>
            {
                NodeData nodeData;

                nodeData = nodeDataMatrix[node];

                return nodeData.FCost + nodeData.HCost;
            });
        }

        protected virtual INodeArea GetNodeArea(NodeDataMatrix nodeDataMatrix, int xCenter, int yCenter)
        {
            return new NodeArea4(Grid, nodeDataMatrix, xCenter, yCenter);
        }

        protected class NodeData
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

                if (this == other)
                {
                    GCost = 0;
                    return;
                }

                if (other.GCost is null)
                {
                    throw new Exception("other NodeData's GCost can't be null.");
                }

                gcost = Node.GetDisplacement(other.Node.X, other.Node.Y) + (int)other.GCost;

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

        protected class NodeDataMatrix
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

        protected interface INodeArea
        {
            Node Center { get; }
            Grid Grid { get; }
            List<Node> AdjacentNodes();
        }

        protected class NodeArea4 : INodeArea
        {
            public Node Center { get; }
            public Grid Grid { get; }
            public Node North { get; protected set; }
            public Node East { get; protected set; }
            public Node West { get; protected set; }
            public Node South { get; protected set; }

            public NodeArea4(Grid grid, NodeDataMatrix nodeDataMatrix, int centerX, int centerY)
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
    }
}
