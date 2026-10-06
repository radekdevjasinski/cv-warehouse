using System;
using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class TaskQueue
    {
        private static readonly Comparison<Box> NearestTruckFirst = CompareByTruckDistance;

        private readonly List<Box> waitingBoxes = new List<Box>();
        private readonly ReservationTable reservations;

        public TaskQueue(WarehouseLayout layout, ReservationTable reservations)
        {
            this.reservations = reservations;
            waitingBoxes.AddRange(layout.Boxes);
            Refresh();
        }

        public void Refresh()
        {
            for (int index = waitingBoxes.Count - 1; index >= 0; index--)
                if (waitingBoxes[index].IsEmpty)
                    waitingBoxes.RemoveAt(index);

            waitingBoxes.Sort(NearestTruckFirst);
        }

        public bool TryAssign(Bot bot, int carryPieces)
        {
            for (int index = 0; index < waitingBoxes.Count; index++)
            {
                Box box = waitingBoxes[index];
                if (box.UnclaimedPieces == 0)
                    continue;
                if (!reservations.TryReserveNearest(box.AccessCells, bot, out GridPosition accessCell))
                    continue;

                bot.JobBox = box;
                bot.AccessCell = accessCell;
                bot.ClaimedPieces = box.Claim(carryPieces);
                return true;
            }

            return false;
        }

        private static int CompareByTruckDistance(Box left, Box right)
        {
            int byDistance = left.TruckDistance.CompareTo(right.TruckDistance);
            return byDistance != 0 ? byDistance : left.Id.CompareTo(right.Id);
        }
    }
}
