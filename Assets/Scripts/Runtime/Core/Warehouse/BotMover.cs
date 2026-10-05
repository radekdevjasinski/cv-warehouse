namespace CvWarehouse.Core.Warehouse
{
    public sealed class BotMover
    {
        private readonly SimulationSettings settings;
        private readonly Pathfinder pathfinder;

        public BotMover(WarehouseGrid grid, SimulationSettings settings)
        {
            this.settings = settings;
            pathfinder = new Pathfinder(grid);
        }

        public void AdvanceMove(Bot bot)
        {
            if (!bot.IsMoving || ++bot.MoveTicks < bot.MoveDuration)
                return;

            bot.Cell = bot.NextCell;
            bot.IsMoving = false;
        }

        public void StartStep(Bot bot)
        {
            if (bot.IsMoving || bot.Cell == bot.Destination)
                return;
            if (!bot.HasPath && !TryFindPath(bot))
                return;

            BeginMove(bot, bot.Path[bot.PathIndex++]);
        }

        private bool TryFindPath(Bot bot)
        {
            bot.PathIndex = 0;
            return pathfinder.TryFindPath(new PathRequest(bot.Cell, bot.Destination), bot.Path);
        }

        private void BeginMove(Bot bot, GridPosition nextCell)
        {
            int direction = GridDirections.Between(bot.Cell, nextCell);
            bot.NextCell = nextCell;
            bot.IsMoving = true;
            bot.MoveTicks = 0;
            bot.MoveDuration = settings.TicksPerCell * GridDirections.CostOf(direction) / GridDirections.StraightCost;
        }
    }
}
