using System;

namespace FluGASv25.Models.Properties
{
    public partial class Parameters
    {

        public Int32 Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }

        public DateTime? CreateDate { get; set; }
        
        public bool IsAnalysisTop3 { get; set; }
        public string CnsMinCoverage { get; set; }
        public string CnsVariantMinFrequency { get; set; }
        public string CnsHomoFrequency { get; set; }
        public string CnsInsDelFrequency { get; set; }
        public string CnsIncludeNRatio { get; set; }
        public string BlastEvalueCutoff { get; set; }
        public string BlastResultCutoff { get; set; }
        public string ReferenceSelectBlastElement { get; set; }
        public string ReferenceSelectBlastEvalue { get; set; }
        public bool IsReferenceSelectBlastEvalue { get; set; }
        public string ReferenceSelectBlastIdentical { get; set; }
        public bool IsReferenceSelectBlastIdentical { get; set; }
        public string ReferenceSelectBlastScore { get; set; }
        public bool IsReferenceSelectBlastScore { get; set; }
        public string ReferenceSelectBlastLength { get; set; }
        public bool IsReferenceSelectBlastLength { get; set; }
        public bool IsLessThanNone1st { get; set; }
        public bool IsLessThanNone2nd { get; set; }
    }
}
