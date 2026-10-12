namespace CvWarehouse.Core.Cv
{
    public sealed class CvFollowTimer
    {
        private readonly float idleSecondsBeforeFollowing;
        private readonly float secondsBetweenScrolls;
        private float idleSeconds;
        private float secondsSinceScroll;

        public CvFollowTimer(float idleSecondsBeforeFollowing, float secondsBetweenScrolls)
        {
            this.idleSecondsBeforeFollowing = idleSecondsBeforeFollowing;
            this.secondsBetweenScrolls = secondsBetweenScrolls;
            idleSeconds = idleSecondsBeforeFollowing;
            secondsSinceScroll = secondsBetweenScrolls;
        }

        public bool IsFollowing => idleSeconds >= idleSecondsBeforeFollowing;

        public void NoteActivity()
        {
            idleSeconds = 0f;
        }

        public void Advance(float deltaSeconds)
        {
            idleSeconds += deltaSeconds;
            secondsSinceScroll += deltaSeconds;
        }

        public bool TryScroll()
        {
            if (!IsFollowing || secondsSinceScroll < secondsBetweenScrolls)
                return false;

            secondsSinceScroll = 0f;
            return true;
        }
    }
}
