using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class WarehouseSimulation
    {
        private readonly List<Bot> bots = new List<Bot>();
        private readonly SimulationSettings settings;
        private readonly ReservationTable reservations;
        private readonly BotMover mover;
        private readonly BotBrain brain;
        private float accumulatedSeconds;
        private int nextBotId;

        public WarehouseSimulation(WarehouseLayout layout, SimulationSettings settings)
        {
            Layout = layout;
            this.settings = settings;
            Score = new WarehouseScore(settings, layout.Boxes);
            reservations = new ReservationTable(layout.Grid);
            mover = new BotMover(layout.Grid, settings);
            brain = new BotBrain(layout, settings, reservations, Score);
        }

        public WarehouseLayout Layout { get; }

        public WarehouseScore Score { get; }

        public IReadOnlyList<Bot> Bots => bots;

        public int MaxBots => Layout.ParkingCells.Count;

        public int TickCount { get; private set; }

        public CarrySize CarrySize { get; private set; }

        public bool IsComplete => Score.DeliveredPieces == Layout.TotalPieces;

        public void Advance(float deltaTime)
        {
            accumulatedSeconds += deltaTime;
            int ticks = 0;
            while (accumulatedSeconds >= settings.TickSeconds && ticks < settings.MaxTicksPerAdvance)
            {
                accumulatedSeconds -= settings.TickSeconds;
                Tick();
                ticks++;
            }

            if (accumulatedSeconds >= settings.TickSeconds)
                accumulatedSeconds = 0f;
        }

        public void Tick()
        {
            TickCount++;
            for (int index = 0; index < bots.Count; index++)
            {
                Bot bot = bots[index];
                mover.AdvanceMove(bot);
                if (bot.IsMoving)
                    continue;

                brain.Tick(bot);
                mover.StartStep(bot);
            }
        }

        public void SetCarrySize(CarrySize carrySize)
        {
            CarrySize = carrySize;
            brain.CarryPieces = settings.PiecesPerTrip(carrySize);
        }

        public bool TryAddBot()
        {
            if (bots.Count >= MaxBots || !TryFindFreeParkingCell(out GridPosition parkingCell))
                return false;

            var bot = new Bot(nextBotId++, parkingCell)
            {
                ParkingCell = parkingCell,
                HasParkingCell = true,
                State = BotState.Parked
            };
            reservations.TryReserve(parkingCell, bot.Id);
            bots.Add(bot);
            return true;
        }

        public bool TryRemoveBot()
        {
            if (bots.Count == 0)
                return false;

            Bot bot = bots[bots.Count - 1];
            brain.AbandonWork(bot);
            bots.RemoveAt(bots.Count - 1);
            return true;
        }

        private bool TryFindFreeParkingCell(out GridPosition freeCell)
        {
            foreach (GridPosition parkingCell in Layout.ParkingCells)
            {
                if (reservations.IsReserved(parkingCell))
                    continue;

                freeCell = parkingCell;
                return true;
            }

            freeCell = default;
            return false;
        }
    }
}
