namespace CvWarehouse.Presentation.Warehouse
{
    public readonly struct BotPose
    {
        public BotPose(bool isStepping, bool isBlinking)
        {
            IsStepping = isStepping;
            IsBlinking = isBlinking;
        }

        public bool IsStepping { get; }

        public bool IsBlinking { get; }
    }
}
