using System;
using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class TaskQueue
    {
        private static readonly Comparison<Box> NearestTruckFirst = CompareByTruckDistance;
        private static readonly Comparison<Box> BiggestFirst = CompareBiggestFirst;
        private static readonly Comparison<Box> SmallestFirst = CompareSmallestFirst;

        private readonly List<Box> closestBoxes = new List<Box>();
        private readonly List<Box> biggestBoxes = new List<Box>();
        private readonly List<Box> smallestBoxes = new List<Box>();
        private readonly ReservationTable reservations;
        private readonly Random random;

        public TaskQueue(WarehouseLayout layout, ReservationTable reservations, Random random)
        {
            this.reservations = reservations;
            this.random = random;
            closestBoxes.AddRange(layout.Boxes);
            biggestBoxes.AddRange(layout.Boxes);
            smallestBoxes.AddRange(layout.Boxes);
            Refresh();
        }

        public void Refresh()
        {
            RemoveEmptyBoxes(closestBoxes);
            RemoveEmptyBoxes(biggestBoxes);
            RemoveEmptyBoxes(smallestBoxes);
            closestBoxes.Sort(NearestTruckFirst);
            biggestBoxes.Sort(BiggestFirst);
            smallestBoxes.Sort(SmallestFirst);
        }

        public bool TryAssign(Bot bot)
        {
            List<Box> waitingBoxes = BoxesFor(bot.Focus);
            for (int index = 0; index < waitingBoxes.Count; index++)
            {
                Box box = waitingBoxes[index];
                if (box.UnclaimedPieces == 0)
                    continue;
                if (!reservations.TryReserveNearest(box.AccessCells, bot, out GridPosition accessCell))
                    continue;

                bot.JobBox = box;
                bot.AccessCell = accessCell;
                bot.ClaimedWords = Math.Min(bot.CarryLevel, box.OpenWords);
                bot.ClaimedPieces = box.Claim(bot.CarryLevel, random, bot.Letters);
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

        private static int CompareBiggestFirst(Box left, Box right)
        {
            int bySize = ((int)right.WeightClass).CompareTo((int)left.WeightClass);
            return bySize != 0 ? bySize : CompareByTruckDistance(left, right);
        }

        private static int CompareSmallestFirst(Box left, Box right)
        {
            int bySize = ((int)left.WeightClass).CompareTo((int)right.WeightClass);
            return bySize != 0 ? bySize : CompareByTruckDistance(left, right);
        }

        private List<Box> BoxesFor(BotFocus focus)
        {
            switch (focus)
            {
                case BotFocus.BiggestBox:
                    return biggestBoxes;
                case BotFocus.SmallestBox:
                    return smallestBoxes;
                default:
                    return closestBoxes;
            }
        }
    }
}
