using CvWarehouse.Core.Warehouse;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class ReservationTableTests
    {
        private const int GridWidth = 6;
        private const int GridHeight = 3;
        private const int BotId = 0;
        private const int OtherBotId = 1;

        private static readonly GridPosition BotCell = new GridPosition(0, 0);
        private static readonly GridPosition CellBehindWall = new GridPosition(2, 2);
        private static readonly GridPosition CellAlongTheAisle = new GridPosition(5, 0);

        [Test]
        public void TryReserveNearest_CloserCellBehindAWall_ReservesTheCellWithTheShorterWalk()
        {
            var reservations = new ReservationTable(GridWithWall());

            bool isReserved = reservations.TryReserveNearest(new[] { CellBehindWall, CellAlongTheAisle }, new Bot(BotId, BotCell), out GridPosition reserved);

            Assert.IsTrue(isReserved);
            Assert.AreEqual(CellAlongTheAisle, reserved);
        }

        [Test]
        public void TryReserveNearest_NearestCellTaken_ReservesTheNextNearest()
        {
            var reservations = new ReservationTable(GridWithWall());
            reservations.TryReserve(CellAlongTheAisle, OtherBotId);

            reservations.TryReserveNearest(new[] { CellBehindWall, CellAlongTheAisle }, new Bot(BotId, BotCell), out GridPosition reserved);

            Assert.AreEqual(CellBehindWall, reserved);
        }

        [Test]
        public void TryReserveNearest_OnlyUnreachableCellsFree_ReservesNothing()
        {
            WarehouseGrid grid = GridWithWall();
            grid.SetCellType(new GridPosition(5, 1), CellType.Blocked);
            var reservations = new ReservationTable(grid);

            bool isReserved = reservations.TryReserveNearest(new[] { CellBehindWall }, new Bot(BotId, BotCell), out _);

            Assert.IsFalse(isReserved);
            Assert.IsFalse(reservations.IsReserved(CellBehindWall));
        }

        [Test]
        public void TryReserveNearest_WallOpenedAfterAnEarlierSearch_UsesTheNewShortcut()
        {
            WarehouseGrid grid = GridWithWall();
            var reservations = new ReservationTable(grid);
            reservations.TryReserveNearest(new[] { CellBehindWall, CellAlongTheAisle }, new Bot(OtherBotId, BotCell), out _);
            reservations.ReleaseAll(OtherBotId);

            grid.SetCellType(new GridPosition(2, 1), CellType.Floor);
            reservations.TryReserveNearest(new[] { CellBehindWall, CellAlongTheAisle }, new Bot(BotId, BotCell), out GridPosition reserved);

            Assert.AreEqual(CellBehindWall, reserved);
        }

        private static WarehouseGrid GridWithWall()
        {
            var grid = new WarehouseGrid(GridWidth, GridHeight);
            for (int x = 0; x < GridWidth - 1; x++)
                grid.SetCellType(new GridPosition(x, 1), CellType.Blocked);
            return grid;
        }
    }
}
