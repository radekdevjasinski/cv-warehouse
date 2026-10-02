using System;
using System.Collections.Generic;

namespace CvWarehouse.Core.Cv
{
    public sealed class CvParseResult
    {
        public bool IsValid { get; private set; }
        public CvDocument Document { get; private set; }
        public string Error { get; private set; }
        public IReadOnlyList<string> Warnings { get; private set; }

        public static CvParseResult Success(CvDocument document, IReadOnlyList<string> warnings)
        {
            return new CvParseResult
            {
                IsValid = true,
                Document = document,
                Error = string.Empty,
                Warnings = warnings
            };
        }

        public static CvParseResult Failure(string error)
        {
            return new CvParseResult
            {
                IsValid = false,
                Error = error,
                Warnings = Array.Empty<string>()
            };
        }
    }
}
