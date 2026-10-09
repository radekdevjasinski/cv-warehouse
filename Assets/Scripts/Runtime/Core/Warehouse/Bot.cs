using System.Collections.Generic;

namespace CvWarehouse.Core.Warehouse
{
    public sealed class Bot
    {
        public const int FirstLevel = 1;

        private const float HalfCell = 0.5f;

        public Bot(int id, GridPosition cell)
        {
            Id = id;
            Cell = cell;
            NextCell = cell;
            Destination = cell;
        }

        public int Id { get; }

        public GridPosition Cell { get; internal set; }

        public GridPosition NextCell { get; internal set; }

        public bool IsMoving { get; internal set; }

        public int MoveTicks { get; internal set; }

        public int MoveDuration { get; internal set; }

        public BotState State { get; internal set; }

        public GridPosition Destination { get; private set; }

        public Box JobBox { get; internal set; }

        public GridPosition AccessCell { get; internal set; }

        public GridPosition DropCell { get; internal set; }

        public GridPosition ParkingCell { get; internal set; }

        public bool HasParkingCell { get; internal set; }

        public int ClaimedPieces { get; internal set; }

        public int CargoPieces { get; internal set; }

        public int CarryLevel { get; internal set; } = FirstLevel;

        public int SpeedLevel { get; internal set; } = FirstLevel;

        public BotFocus Focus { get; internal set; }

        public float MoveFraction => IsMoving ? (float)MoveTicks / MoveDuration : 0f;

        public float CentreX => Cell.X + (NextCell.X - Cell.X) * MoveFraction + HalfCell;

        public float CentreY => Cell.Y + (NextCell.Y - Cell.Y) * MoveFraction + HalfCell;

        public bool IsAtDestination => !IsMoving && Cell == Destination;

        public int RouteLength => Path.Count - PathIndex;

        internal List<GridPosition> Path { get; } = new List<GridPosition>();

        internal int PathIndex { get; set; }

        internal List<int> Letters { get; } = new List<int>();

        internal int WorkTicksLeft { get; set; }

        internal bool HasPath => PathIndex < Path.Count;

        public int LevelOf(BotUpgrade upgrade)
        {
            return upgrade == BotUpgrade.Speed ? SpeedLevel : CarryLevel;
        }

        public GridPosition RouteCell(int index)
        {
            return Path[PathIndex + index];
        }

        internal void SetDestination(GridPosition destination)
        {
            Destination = destination;
            ClearPath();
        }

        internal void ClearPath()
        {
            Path.Clear();
            PathIndex = 0;
        }
    }
}
