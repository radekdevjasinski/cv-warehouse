using System;
using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class TaskQueue
    {
        private static readonly Comparison<Box> NearestTruckFirst = CompareByTruckDistance;
        private static readonly Comparison<Box> BiggestFirst = CompareBySize;

        private readonly List<Box> closestBoxes = new List<Box>();
        private readonly List<Box> biggestBoxes = new List<Box>();
        private readonly ReservationTable reservations;

        public TaskQueue(WarehouseLayout layout, ReservationTable reservations)
        {
            this.reservations = reservations;
            closestBoxes.AddRange(layout.Boxes);
            biggestBoxes.AddRange(layout.Boxes);
            Refresh();
        }

        public void Refresh()
        {
            RemoveEmptyBoxes(closestBoxes);
            RemoveEmptyBoxes(biggestBoxes);
            closestBoxes.Sort(NearestTruckFirst);
            biggestBoxes.Sort(BiggestFirst);
        }

        public bool TryAssign(Bot bot, int carryPieces)
        {
            List<Box> waitingBoxes = bot.Focus == BotFocus.BiggestBox ? biggestBoxes : closestBoxes;
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

        private static void RemoveEmptyBoxes(List<Box> boxes)
        {
            for (int index = boxes.Count - 1; index >= 0; index--)
                if (boxes[index].IsEmpty)
                    boxes.RemoveAt(index);
        }

        private static int CompareByTruckDistance(Box left, Box right)
        {
            int byDistance = left.TruckDistance.CompareTo(right.TruckDistance);
            return byDistance != 0 ? byDistance : left.Id.CompareTo(right.Id);
        }

        private static int CompareBySize(Box left, Box right)
        {
            int bySize = ((int)right.WeightClass).CompareTo((int)left.WeightClass);
            return bySize != 0 ? bySize : CompareByTruckDistance(left, right);
        }
    }
}
