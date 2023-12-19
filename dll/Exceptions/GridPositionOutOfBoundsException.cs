using System;

namespace SharpPath.Exceptions
{
    public class GridPositionOutOfBoundsException : Exception
    {
        public GridPositionOutOfBoundsException(int x, int y) : base($"Position [{x},{y}] is out of grid bounds.") { }
    }
}
