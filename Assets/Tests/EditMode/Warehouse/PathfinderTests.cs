using System.Collections.Generic;
using CvWarehouse.Core.Warehouse;
using NUnit.Framework;

namespace CvWarehouse.Tests.EditMode.Warehouse
{
    public sealed class PathfinderTests
    {
        private const int GridSide = 5;
        private const int WallX = 2;
        private const int MovingBot = 0;
        private const int StandingBot = 1;

        private WarehouseGrid grid;
        private Pathfinder pathfinder;
        private List<GridPosition> path;

        [SetUp]
        public void SetUp()
        {
            grid = new WarehouseGrid(GridSide, GridSide);
            pathfinder = new Pathfinder(grid);
            path = new List<GridPosition>();
        }

        [Test]
        public void TryFindPath_OpenFloor_MovesDiagonally()
        {
            var goal = new GridPosition(3, 3);

            Assert.IsTrue(pathfinder.TryFindPath(new PathRequest(new GridPosition(0, 0), goal), path));

            Assert.AreEqual(3, path.Count);
            Assert.AreEqual(goal, path[path.Count - 1]);
        }

        [Test]
        public void TryFindPath_StartIsGoal_ReturnsEmptyPath()
        {
            var cell = new GridPosition(2, 2);

            Assert.IsTrue(pathfinder.TryFindPath(new PathRequest(cell, cell), path));

            Assert.IsEmpty(path);
        }

        [Test]
        public void TryFindPath_WallWithGap_GoesThroughTheGapWithoutCuttingCorners()
        {
            BuildWall(gapY: 4);
            var start = new GridPosition(0, 0);

            Assert.IsTrue(pathfinder.TryFindPath(new PathRequest(start, new GridPosition(4, 0)), path));

            CollectionAssert.Contains(path, new GridPosition(WallX, 4));
            AssertEveryStepIsLegal(start);
        }

        [Test]
        public void TryFindPath_GoalWalledOff_ReturnsFalse()
        {
            BuildWall(gapY: -1);

            Assert.IsFalse(pathfinder.TryFindPath(new PathRequest(new GridPosition(0, 0), new GridPosition(4, 0)), path));

            Assert.IsEmpty(path);
        }

        [Test]
        public void TryFindPath_AvoidingBots_GoesAroundAStandingBot()
        {
            var start = new GridPosition(0, 2);
            var goal = new GridPosition(4, 2);
            var standing = new GridPosition(2, 2);
            grid.Occupy(standing, StandingBot);

            Assert.IsTrue(pathfinder.TryFindPath(PathRequest.AvoidingBots(start, goal, MovingBot), path));

            CollectionAssert.DoesNotContain(path, standing);
            Assert.AreEqual(goal, path[path.Count - 1]);
        }

        [Test]
        public void TryFindPath_NotAvoidingBots_IgnoresAStandingBot()
        {
            var standing = new GridPosition(2, 2);
            grid.Occupy(standing, StandingBot);

            pathfinder.TryFindPath(new PathRequest(new GridPosition(0, 2), new GridPosition(4, 2)), path);

            CollectionAssert.Contains(path, standing);
        }

        private void BuildWall(int gapY)
        {
            for (int y = 0; y < GridSide; y++)
                if (y != gapY)
                    grid.SetCellType(new GridPosition(WallX, y), CellType.Blocked);
        }

        private void AssertEveryStepIsLegal(GridPosition start)
        {
            GridPosition current = start;
            foreach (GridPosition next in path)
            {
                Assert.IsTrue(grid.CanStep(current, GridDirections.Between(current, next)), "Illegal step to " + next);
                current = next;
            }
        }
    }
}
