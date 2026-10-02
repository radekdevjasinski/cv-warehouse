namespace CvWarehouse.Core.Cv
{
    public readonly struct CvWord
    {
        public CvWord(int firstLetter, int letterCount)
        {
            FirstLetter = firstLetter;
            LetterCount = letterCount;
        }

        public int FirstLetter { get; }
        public int LetterCount { get; }
    }
}
