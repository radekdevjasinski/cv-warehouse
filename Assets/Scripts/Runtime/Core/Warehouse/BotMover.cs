using System;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class BotMover
    {
        private readonly WarehouseGrid grid;
        private readonly SimulationSettings settings;
        private readonly Random random;
        private readonly Pathfinder pathfinder;
        private readonly int[] freeDirections = new int[GridDirections.Count];

        public BotMover(WarehouseGrid grid, SimulationSettings settings, Random random)
        {
            this.grid = grid;
            this.settings = settings;
            this.random = random;
            pathfinder = new Pathfinder(grid);
        }

        public void AdvanceMove(Bot bot)
        {
            if (!bot.IsMoving || ++bot.MoveTicks < bot.MoveDuration)
                return;

            grid.Vacate(bot.Cell, bot.Id);
            bot.Cell = bot.NextCell;
            bot.IsMoving = false;
        }

        public void StartStep(Bot bot)
        {
            if (bot.IsMoving || bot.Cell == bot.Destination)
                return;
            if (!bot.HasPath && !TryFindPath(bot, new PathRequest(bot.Cell, bot.Destination)))
                return;

            int direction = GridDirections.Between(bot.Cell, bot.Path[bot.PathIndex]);
            if (!grid.IsStepFreeFor(bot.Cell, direction, bot.Id))
            {
                HandleBlockedStep(bot);
                return;
            }

            bot.PathIndex++;
            BeginMove(bot, direction);
        }

        private void HandleBlockedStep(Bot bot)
        {
            bot.WaitTicks++;
            if (bot.WaitTicks >= settings.SidestepWaitTicks)
                Sidestep(bot);
            else if (bot.WaitTicks % settings.RepathWaitTicks == 0)
                TryFindPath(bot, PathRequest.AvoidingBots(bot.Cell, bot.Destination, bot.Id));
        }

        private bool TryFindPath(Bot bot, PathRequest request)
        {
            bot.PathIndex = 0;
            return pathfinder.TryFindPath(request, bot.Path);
        }

        private void Sidestep(Bot bot)
        {
            bot.WaitTicks = 0;
            int freeCount = 0;
            for (int direction = 0; direction < GridDirections.Count; direction++)
                if (grid.IsStepFreeFor(bot.Cell, direction, bot.Id))
                    freeDirections[freeCount++] = direction;

            if (freeCount == 0)
                return;

            bot.ClearPath();
            BeginMove(bot, freeDirections[random.Next(freeCount)]);
        }

        private void BeginMove(Bot bot, int direction)
        {
            bot.NextCell = GridDirections.Step(bot.Cell, direction);
            grid.Occupy(bot.NextCell, bot.Id);
            bot.IsMoving = true;
            bot.MoveTicks = 0;
            bot.MoveDuration = settings.TicksPerCell * GridDirections.CostOf(direction) / GridDirections.StraightCost;
            bot.WaitTicks = 0;
        }
    }
}
