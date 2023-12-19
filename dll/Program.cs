using System;

namespace SharpPath
{
    static class Program
    {
        static void Main()
        {
            Grid grid;
            Path path;
            IPathFinder pathFinder;

            grid = new Grid(new int[,]
            {
                { 0, 0, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 1, 0 },
                { 0, 1, 1, 1, 0, 0 }
            });

            pathFinder = new PathFinder8(grid);
            path = pathFinder.Run(1, 1, 5, 1);

            foreach(Node node in path)
            {
                Console.WriteLine($"[{node.X},{node.Y}]");
            }

            Console.ReadKey();
        }
    }
}
