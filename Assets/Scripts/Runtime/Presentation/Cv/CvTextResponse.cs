namespace CvWarehouse.Presentation.Cv
{
    public sealed class CvTextResponse
    {
        public bool IsSuccess { get; private set; }
        public string Text { get; private set; }
        public string Error { get; private set; }

        public static CvTextResponse Success(string text)
        {
            return new CvTextResponse { IsSuccess = true, Text = text, Error = string.Empty };
        }

        public static CvTextResponse Failure(string error)
        {
            return new CvTextResponse { IsSuccess = false, Text = string.Empty, Error = error };
        }
    }
}
