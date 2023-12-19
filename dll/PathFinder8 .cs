using SharpPath.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpPath
{
    public class PathFinder8 : PathFinder4
    {
        public bool BlockCorners { get; set; } = true;
        
        public PathFinder8(Grid grid, bool blockCorners = true, bool includeFirstNode = true) : base(grid, includeFirstNode)
        {
            BlockCorners = blockCorners;
        }

        protected override INodeArea GetNodeArea(NodeDataMatrix nodeDataMatrix, int xCenter, int yCenter)
        {
            return new NodeArea8(Grid, nodeDataMatrix, xCenter, yCenter, BlockCorners);
        }

        class NodeArea8 : NodeArea4
        {
            public Node Northwest { get; protected set; }
            public Node Northeast { get; protected set; }
            public Node Southwest { get; protected set; }
            public Node Southeast { get; protected set; }
            public bool BlockCorners { get; }

            public NodeArea8(Grid grid, NodeDataMatrix nodeDataMatrix, int centerX, int centerY, bool blockCorners) : base(grid, nodeDataMatrix, centerX, centerY)
            {
                BlockCorners = blockCorners;

                Northwest = ClearDeadEnds(grid.GetNextNodeNorthwest(centerX, centerY, !BlockCorners), nodeDataMatrix);
                Northeast = ClearDeadEnds(grid.GetNextNodeNortheast(centerX, centerY, !BlockCorners), nodeDataMatrix);
                Southwest = ClearDeadEnds(grid.GetNextNodeSouthwest(centerX, centerY, !BlockCorners), nodeDataMatrix);
                Southeast = ClearDeadEnds(grid.GetNextNodeSoutheast(centerX, centerY, !BlockCorners), nodeDataMatrix);
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
