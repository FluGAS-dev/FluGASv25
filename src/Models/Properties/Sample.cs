using System;
using System.Linq;

namespace FluGASv25.Models.Properties
{
    public class Sample
    {
        public Int32 ID { get; set; }
        public string NAME { get; set; }
        public string VIEW_NAME { get; set; }
        public string STRAIN { get; set; }
        public string FILE1 { get; set; }
        public string FILE2 { get; set; }
        public string A_HA { get; set; }
        public string A_MP { get; set; }
        public string A_NA { get; set; }
        public string A_NP { get; set; }
        public string A_NS { get; set; }
        public string A_PA { get; set; }
        public string A_PB1 { get; set; }
        public string A_PB2 { get; set; }
        public string B_HA { get; set; }
        public string B_MP { get; set; }
        public string B_NA { get; set; }
        public string B_NP { get; set; }
        public string B_NS { get; set; }
        public string B_PA { get; set; }
        public string B_PB1 { get; set; }
        public string B_PB2 { get; set; }
        public string TYPE { get; set; }
        public string SUBTYPE { get; set; }
        public string A_HA_CALL { get; set; }
        public string A_NA_CALL { get; set; }
        // public string YAMAGATA_CALL { get; set; }
        // public string VICTORIA_CALL { get; set; }
        public string FILE_DATE { get; set; }
        public string COVER_RATIO_A_HA { get; set; }
        public string COVER_RATIO_A_MP { get; set; }
        public string COVER_RATIO_A_NA { get; set; }
        public string COVER_RATIO_A_NP { get; set; }
        public string COVER_RATIO_A_NS { get; set; }
        public string COVER_RATIO_A_PA { get; set; }
        public string COVER_RATIO_A_PB1 { get; set; }
        public string COVER_RATIO_A_PB2 { get; set; }
        public string COVER_RATIO_B_HA { get; set; }
        public string COVER_RATIO_B_MP { get; set; }
        public string COVER_RATIO_B_NA { get; set; }
        public string COVER_RATIO_B_NP { get; set; }
        public string COVER_RATIO_B_NS { get; set; }
        public string COVER_RATIO_B_PA { get; set; }
        public string COVER_RATIO_B_PB1 { get; set; }
        public string COVER_RATIO_B_PB2 { get; set; }
        public string AVE_COVER_A_HA { get; set; }
        public string AVE_COVER_A_MP { get; set; }
        public string AVE_COVER_A_NA { get; set; }
        public string AVE_COVER_A_NP { get; set; }
        public string AVE_COVER_A_NS { get; set; }
        public string AVE_COVER_A_PA { get; set; }
        public string AVE_COVER_A_PB1 { get; set; }
        public string AVE_COVER_A_PB2 { get; set; }
        public string AVE_COVER_B_HA { get; set; }
        public string AVE_COVER_B_MP { get; set; }
        public string AVE_COVER_B_NA { get; set; }
        public string AVE_COVER_B_NP { get; set; }
        public string AVE_COVER_B_NS { get; set; }
        public string AVE_COVER_B_PA { get; set; }
        public string AVE_COVER_B_PB1 { get; set; }
        public string AVE_COVER_B_PB2 { get; set; }
        public string CDS_REGION_A_HA { get; set; }
        public string CDS_REGION_A_MP { get; set; }
        public string CDS_REGION_A_NA { get; set; }
        public string CDS_REGION_A_NP { get; set; }
        public string CDS_REGION_A_NS { get; set; }
        public string CDS_REGION_A_PA { get; set; }
        public string CDS_REGION_A_PB1 { get; set; }
        public string CDS_REGION_A_PB2 { get; set; }
        public string CDS_REGION_B_HA { get; set; }
        public string CDS_REGION_B_MP { get; set; }
        public string CDS_REGION_B_NA { get; set; }
        public string CDS_REGION_B_NP { get; set; }
        public string CDS_REGION_B_NS { get; set; }
        public string CDS_REGION_B_PA { get; set; }
        public string CDS_REGION_B_PB1 { get; set; }
        public string CDS_REGION_B_PB2 { get; set; }
        public Int32 PRAM_ID { get; set; }
        public string SEQUENCER_ID { get; set; }
        public string ISDELETE { get; set; }
        public string DATE { get; set; }
        public string DATEONLY {
            get {
                var date = (string.IsNullOrEmpty(DATE))?  string.Empty : DATE.Split(' ').First();
                return date;
            }
            set { }
        }
        public bool IsSelected { get; set; }
        public string MEMO { get; set; }
    }
}
