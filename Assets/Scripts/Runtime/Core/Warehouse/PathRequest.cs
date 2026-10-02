namespace CvWarehouse.Core.Warehouse
{
    public readonly struct PathRequest
    {
        public PathRequest(GridPosition start, GridPosition goal)
        {
            Start = start;
            Goal = goal;
            BotId = WarehouseGrid.NoBot;
            AvoidsBots = false;
        }

        private PathRequest(GridPosition start, GridPosition goal, int botId)
        {
            Start = start;
            Goal = goal;
            BotId = botId;
            AvoidsBots = true;
        }

        public GridPosition Start { get; }

        public GridPosition Goal { get; }

        public int BotId { get; }

        public bool AvoidsBots { get; }

        public static PathRequest AvoidingBots(GridPosition start, GridPosition goal, int botId)
        {
            return new PathRequest(start, goal, botId);
        }
    }
}
