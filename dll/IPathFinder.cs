namespace SharpPath
{
    public interface IPathFinder
    {
        Grid Grid { get; }
        Path Run(int xStart, int yStart, int xDestination, int yDestination);
    }
}
