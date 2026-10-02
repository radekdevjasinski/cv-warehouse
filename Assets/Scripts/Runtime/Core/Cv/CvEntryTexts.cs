namespace CvWarehouse.Core.Cv
{
    public readonly struct CvEntryTexts
    {
        public CvEntryTexts(string heading, string meta, string body)
        {
            Heading = heading;
            Meta = meta;
            Body = body;
        }

        public string Heading { get; }
        public string Meta { get; }
        public string Body { get; }
    }
}
