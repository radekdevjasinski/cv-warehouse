using System;
using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class TaskQueue
    {
        private static readonly Comparison<Box> NearestTruckFirst = CompareByTruckDistance;
        private static readonly Comparison<Box> BiggestFirst = CompareBiggestFirst;
        private static readonly Comparison<Box> SmallestFirst = CompareSmallestFirst;
        private static readonly Comparison<Box> MostImportantFirst = CompareMostImportantFirst;

        private readonly List<Box> closestBoxes = new List<Box>();
        private readonly List<Box> biggestBoxes = new List<Box>();
        private readonly List<Box> smallestBoxes = new List<Box>();
        private readonly List<Box> importantBoxes = new List<Box>();
        private readonly ReservationTable reservations;
        private readonly Random random;

        public TaskQueue(WarehouseLayout layout, ReservationTable reservations, Random random)
        {
            this.reservations = reservations;
            this.random = random;
            closestBoxes.AddRange(layout.Boxes);
            biggestBoxes.AddRange(layout.Boxes);
            smallestBoxes.AddRange(layout.Boxes);
            importantBoxes.AddRange(layout.Boxes);
            Refresh();
        }

        public void Refresh()
        {
            RemoveEmptyBoxes(closestBoxes);
            RemoveEmptyBoxes(biggestBoxes);
            RemoveEmptyBoxes(smallestBoxes);
            RemoveEmptyBoxes(importantBoxes);
            closestBoxes.Sort(NearestTruckFirst);
            biggestBoxes.Sort(BiggestFirst);
            smallestBoxes.Sort(SmallestFirst);
            importantBoxes.Sort(MostImportantFirst);
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
            int bySize = ((int)right.SizeClass).CompareTo((int)left.SizeClass);
            return bySize != 0 ? bySize : CompareByTruckDistance(left, right);
        }

        private static int CompareSmallestFirst(Box left, Box right)
        {
            int bySize = ((int)left.SizeClass).CompareTo((int)right.SizeClass);
            return bySize != 0 ? bySize : CompareByTruckDistance(left, right);
        }

        private static int CompareMostImportantFirst(Box left, Box right)
        {
            int byImportance = ((int)right.Importance).CompareTo((int)left.Importance);
            return byImportance != 0 ? byImportance : CompareByTruckDistance(left, right);
        }

        private List<Box> BoxesFor(BotFocus focus)
        {
            switch (focus)
            {
                case BotFocus.BiggestBox:
                    return biggestBoxes;
                case BotFocus.SmallestBox:
                    return smallestBoxes;
                case BotFocus.MostImportant:
                    return importantBoxes;
                default:
                    return closestBoxes;
            }
        }
    }
}
