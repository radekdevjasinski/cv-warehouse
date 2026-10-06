namespace CvWarehouse.Core.Warehouse
{
    public sealed class BotBrain
    {
        private readonly WarehouseLayout layout;
        private readonly SimulationSettings settings;
        private readonly ReservationTable reservations;
        private readonly WarehouseScore score;
        private readonly TaskQueue taskQueue;

        public BotBrain(WarehouseLayout layout, SimulationSettings settings, ReservationTable reservations, WarehouseScore score)
        {
            this.layout = layout;
            this.settings = settings;
            this.reservations = reservations;
            this.score = score;
            taskQueue = new TaskQueue(layout, reservations);
            CarryPieces = settings.PiecesPerTrip(CarrySize.Letter);
        }

        public int CarryPieces { get; set; }

        public void Tick(Bot bot)
        {
            switch (bot.State)
            {
                case BotState.GoingToBox:
                    if (bot.IsAtDestination)
                        StartWork(bot, BotState.Extracting, settings.ExtractTicks);
                    break;
                case BotState.Extracting:
                    TickExtracting(bot);
                    break;
                case BotState.GoingToDrop:
                    if (bot.IsAtDestination)
                        StartWork(bot, BotState.Unloading, settings.UnloadTicks);
                    break;
                case BotState.Unloading:
                    TickUnloading(bot);
                    break;
                default:
                    TickWithoutJob(bot);
                    break;
            }
        }

        public void AbandonWork(Bot bot)
        {
            if (bot.JobBox != null)
            {
                bot.JobBox.ReleaseClaim(bot.ClaimedPieces);
                score.Deliver(bot.JobBox, bot.CargoPieces);
            }

            bot.JobBox = null;
            bot.ClaimedPieces = 0;
            bot.CargoPieces = 0;
            bot.HasParkingCell = false;
            bot.State = BotState.Idle;
            reservations.ReleaseAll(bot.Id);
        }

        private static void StartWork(Bot bot, BotState workState, int workTicks)
        {
            bot.State = workState;
            bot.WorkTicksLeft = workTicks;
        }

        private void TickWithoutJob(Bot bot)
        {
            if (taskQueue.TryAssign(bot, CarryPieces))
            {
                LeaveParking(bot);
                bot.State = BotState.GoingToBox;
                bot.SetDestination(bot.AccessCell);
            }
            else if (bot.State == BotState.Idle)
            {
                TryGoToParking(bot);
            }
            else if (bot.State == BotState.GoingToParking && bot.IsAtDestination)
            {
                bot.State = BotState.Parked;
            }
        }

        private void LeaveParking(Bot bot)
        {
            if (!bot.HasParkingCell)
                return;

            reservations.Release(bot.ParkingCell, bot.Id);
            bot.HasParkingCell = false;
        }

        private void TryGoToParking(Bot bot)
        {
            if (!reservations.TryReserveNearest(layout.ParkingCells, bot, out GridPosition parkingCell))
                return;

            bot.ParkingCell = parkingCell;
            bot.HasParkingCell = true;
            bot.State = BotState.GoingToParking;
            bot.SetDestination(parkingCell);
        }

        private void TickExtracting(Bot bot)
        {
            if (bot.WorkTicksLeft > 0)
            {
                bot.WorkTicksLeft--;
                return;
            }

            if (!reservations.TryReserveNearest(layout.DropCells, bot, out GridPosition dropCell))
                return;

            TakeCargo(bot);
            reservations.Release(bot.AccessCell, bot.Id);
            bot.DropCell = dropCell;
            bot.State = BotState.GoingToDrop;
            bot.SetDestination(dropCell);
        }

        private void TakeCargo(Bot bot)
        {
            bot.JobBox.TakeClaimed(bot.ClaimedPieces);
            bot.CargoPieces = bot.ClaimedPieces;
            bot.ClaimedPieces = 0;
            if (!bot.JobBox.IsEmpty)
                return;

            layout.RemoveEmptyBox(bot.JobBox);
            taskQueue.Refresh();
        }

        private void TickUnloading(Bot bot)
        {
            if (bot.WorkTicksLeft > 0)
            {
                bot.WorkTicksLeft--;
                return;
            }

            score.Deliver(bot.JobBox, bot.CargoPieces);
            reservations.Release(bot.DropCell, bot.Id);
            bot.CargoPieces = 0;
            bot.JobBox = null;
            bot.State = BotState.Idle;
        }
    }
}
