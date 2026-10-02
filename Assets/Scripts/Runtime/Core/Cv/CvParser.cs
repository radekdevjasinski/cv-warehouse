using System;
using System.Collections.Generic;
using CvWarehouse.Core.Cv.Dto;
using UnityEngine;

namespace CvWarehouse.Core.Cv
{
    public sealed class CvParser
    {
        public CvParseResult Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return CvParseResult.Failure("The CV file is empty.");

            CvFileDto file;
            try
            {
                file = JsonUtility.FromJson<CvFileDto>(json);
            }
            catch (ArgumentException exception)
            {
                return CvParseResult.Failure("The CV file is not valid JSON. " + exception.Message);
            }

            return file == null
                ? CvParseResult.Failure("The CV file is not valid JSON.")
                : BuildResult(file);
        }

        private static CvParseResult BuildResult(CvFileDto file)
        {
            var warnings = new List<string>();
            List<CvCategory> categories = new CvCategoryMapper(warnings).MapAll(file.categories);
            List<CvSection> sections = new CvSectionMapper(categories, warnings).MapAll(file.sections);
            if (sections.Count == 0)
                return CvParseResult.Failure("The CV file has no sections with content.");

            var document = new CvDocument
            {
                Header = new CvHeaderMapper(warnings).Map(file),
                Categories = categories,
                Sections = sections,
                Footer = CvText.Clean(file.footer)
            };
            return CvParseResult.Success(document, warnings);
        }
    }
}
