using System;

namespace CvWarehouse.Core.Menu
{
    public sealed class MenuBotLoop
    {
        private readonly MenuBotLoopSettings settings;
        private float phaseSeconds;

        public MenuBotLoop(MenuBotLoopSettings settings)
        {
            if (settings.TakeOutSeconds <= 0f || settings.WalkSeconds <= 0f || settings.PutInSeconds <= 0f)
                throw new ArgumentException("Every phase of the menu bot loop needs a positive duration.", nameof(settings));
            if (settings.CargoWordsPerTrip == null || settings.CargoWordsPerTrip.Length == 0)
                throw new ArgumentException("The menu bot needs at least one cargo size.", nameof(settings));

            this.settings = settings;
        }

        public MenuBotPhase Phase { get; private set; } = MenuBotPhase.TakingOut;

        public int DeliveredTrips { get; private set; }

        public float PhaseProgress => Math.Min(1f, phaseSeconds / DurationOf(Phase));

        public float Position
        {
            get
            {
                switch (Phase)
                {
                    case MenuBotPhase.Carrying:
                        return PhaseProgress;
                    case MenuBotPhase.PuttingIn:
                        return 1f;
                    case MenuBotPhase.Returning:
                        return 1f - PhaseProgress;
                    default:
                        return 0f;
                }
            }
        }

        public bool IsMoving => Phase == MenuBotPhase.Carrying || Phase == MenuBotPhase.Returning;

        public bool IsCarrying => Phase == MenuBotPhase.Carrying || Phase == MenuBotPhase.PuttingIn;

        public int CargoWords => IsCarrying ? settings.CargoWordsPerTrip[DeliveredTrips % settings.CargoWordsPerTrip.Length] : 0;

        public void Advance(float deltaSeconds)
        {
            if (deltaSeconds <= 0f)
                return;

            phaseSeconds += deltaSeconds;
            while (phaseSeconds >= DurationOf(Phase))
            {
                phaseSeconds -= DurationOf(Phase);
                EnterNextPhase();
            }
        }

        private void EnterNextPhase()
        {
            switch (Phase)
            {
                case MenuBotPhase.TakingOut:
                    Phase = MenuBotPhase.Carrying;
                    break;
                case MenuBotPhase.Carrying:
                    Phase = MenuBotPhase.PuttingIn;
                    break;
                case MenuBotPhase.PuttingIn:
                    DeliveredTrips++;
                    Phase = MenuBotPhase.Returning;
                    break;
                default:
                    Phase = MenuBotPhase.TakingOut;
                    break;
            }
        }

        private float DurationOf(MenuBotPhase phase)
        {
            switch (phase)
            {
                case MenuBotPhase.TakingOut:
                    return settings.TakeOutSeconds;
                case MenuBotPhase.PuttingIn:
                    return settings.PutInSeconds;
                default:
                    return settings.WalkSeconds;
            }
        }
    }
}
