namespace FluGASv25.Utils
{
    public static class ConstantValues
    {
        public const string CanceledMessage = "canceled";
        public const string ErrorMessage = "external program error";
        public const string NormalEndMessage = "normal";
        public const string ErrorEndMessage = "error.";


        public static readonly string barcode = "barcode";
        public static readonly int barcodeMax = 24;

        // Database と連動しているので注意。
        public static readonly string DefaultMinionParameterName = "init-minion";
        public static readonly string DefaultMiseqParameterName = "init-miseq";
        public static readonly string NeverAskAgain = "not-ask";

        public static readonly string ConvertDir = "fastq";
        public static readonly string MappingDir = "mapping";
        public static readonly string referencesDir = "references";

        /** Mapping **/
        public static readonly string mappingResultDirName = "aligned";
        public static readonly string fstMappingResultDirName = "1st_aligned";
        public static readonly string secMappingResultDirName = "2nd_aligned";
        public static readonly string secMappingReferenceDirName = "reference";

        public static readonly string sortedBamFooter = "-sorted.bam";
        public static readonly string secMappingFooter = "-preTop";
        public static readonly string secPileupCsvFooter = ".csv";
        public static readonly string pileupFooter = ".mpileup";
        public static readonly string vcfFooter = ".vcf";

        // 
        public const string DbDelimiter = "///";
        public const string ViewDelimiter = "-";
        public const string UNDER = "_";

        public const string MIN = "MIN";
        public const string MAX = "MAX";
        public const string minlength = "minLength";
        public const string maxlength = "maxLength";
        // public const string update = "update";
        public const string nuc5ds = "5nuc";
        public const string nuc3ds = "3nuc";
        public const string includeN = "incN";
        public const string mappingcdhit = "mapcdhit";
        public const string blastcdhit = "bltcdhit";
        public const string minCDS = "minCDS";
        public const string maxCDS = "maxCDS";


        public const string DbPropertyPrefixCoverRatio = "COVER_RATIO_";
        public const string DbPropertyPrefixCoverAvarage = "AVE_COVER_";
        public const string DbPropertyPrefixCdsRegion = "CDS_REGION_";
        public const string DbDefaultFloatValue = "0.000";
        public const int NnucDefaultIntValue = 3;
        public const double MappingCdHitDefaultValue = 0.95;  // 2020.04.07
        public const double BlastCdHitDefaultValue = 0.97;
        public const int DefaultCutoff = 30;
        public const string nuc5start = "AGC";
        public const string nuc3end = "ACT";

        public const string MainLogClear = "log-clear.";
        public const string EndAnalysis = "end analysis.";
        public const string EndAnalysisLog = "analysis-end-";

        public const string FnaFooter = ".fna";
        public const string MergeFnaFooter = "-all.fastq";
        public static readonly char delimiter = '\t';
        public static readonly char space = ' ';

    }

}
