namespace CvWarehouse.Core.Warehouse
{
    public sealed class WarehouseScore
    {
        private readonly SimulationSettings settings;

        public WarehouseScore(SimulationSettings settings)
        {
            this.settings = settings;
        }

        public int DeliveredPieces { get; private set; }

        public int Points { get; private set; }

        public void Deliver(Box box, int pieces)
        {
            DeliveredPieces += pieces;
            Points += pieces * settings.PointsPerPiece(box.WeightClass);
        }
    }
}
