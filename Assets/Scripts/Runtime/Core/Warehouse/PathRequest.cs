namespace CvWarehouse.Core.Warehouse
{
    public readonly struct PathRequest
    {
        public PathRequest(GridPosition start, GridPosition goal)
        {
            Start = start;
            Goal = goal;
        }

        public GridPosition Start { get; }

        public GridPosition Goal { get; }
    }
}
