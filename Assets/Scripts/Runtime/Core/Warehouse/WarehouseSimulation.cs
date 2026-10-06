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

        public bool IsPaused { get; private set; }

        public bool CanRemoveBot => bots.Count > 1;

        public bool IsComplete => Score.DeliveredPieces == Layout.TotalPieces;

        public void TogglePause()
        {
            IsPaused = !IsPaused;
        }

        public void Advance(float deltaTime)
        {
            if (IsPaused)
                return;

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

        public void SetCarrySize(Bot bot, CarrySize carrySize)
        {
            bot.CarrySize = carrySize;
        }

        public void SetFocus(Bot bot, BotFocus focus)
        {
            if (bot.Focus == focus)
                return;

            bot.Focus = focus;
            if (brain.TryRetarget(bot))
                mover.PlanRoute(bot);
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

        public bool TryRemoveBot(Bot bot)
        {
            if (!CanRemoveBot || !bots.Remove(bot))
                return false;

            brain.AbandonWork(bot);
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
