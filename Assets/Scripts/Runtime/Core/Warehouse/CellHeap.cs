using System;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class CellHeap
    {
        private const int MinimumCapacity = 16;

        private int[] cells;
        private int[] priorities;

        public CellHeap(int capacity)
        {
            cells = new int[Math.Max(capacity, MinimumCapacity)];
            priorities = new int[cells.Length];
        }

        public int Count { get; private set; }

        public void Clear()
        {
            Count = 0;
        }

        public void Push(int cell, int priority)
        {
            if (Count == cells.Length)
                Grow();

            int slot = Count++;
            while (slot > 0 && priorities[(slot - 1) / 2] > priority)
            {
                int parent = (slot - 1) / 2;
                Move(parent, slot);
                slot = parent;
            }

            cells[slot] = cell;
            priorities[slot] = priority;
        }

        public int Pop(out int priority)
        {
            if (Count == 0)
                throw new InvalidOperationException("The heap is empty.");

            int topCell = cells[0];
            priority = priorities[0];
            Count--;
            if (Count > 0)
                SiftDown(cells[Count], priorities[Count]);
            return topCell;
        }

        private void SiftDown(int cell, int priority)
        {
            int slot = 0;
            while (true)
            {
                int child = SmallerChildOf(slot);
                if (child < 0 || priorities[child] >= priority)
                    break;

                Move(child, slot);
                slot = child;
            }

            cells[slot] = cell;
            priorities[slot] = priority;
        }

        private int SmallerChildOf(int slot)
        {
            int left = slot * 2 + 1;
            if (left >= Count)
                return -1;

            int right = left + 1;
            return right < Count && priorities[right] < priorities[left] ? right : left;
        }

        private void Move(int fromSlot, int toSlot)
        {
            cells[toSlot] = cells[fromSlot];
            priorities[toSlot] = priorities[fromSlot];
        }

        private void Grow()
        {
            Array.Resize(ref cells, cells.Length * 2);
            Array.Resize(ref priorities, cells.Length);
        }
    }
}
