using System;

namespace FluGASv25.Models.Properties
{
    public class MinionParameters : Parameters
    {

        // selet mapper
        public bool IsGuppyAlign { get; set; }
        public bool IsMinimap2 { get; set; }
        
        // for MinION
        public string GuppyMinCover { get; set; }

        // for minimap2
        public string Minimap2KmerSize { get; set; }
        public string Minimap2MinWindowSize { get; set; }
        public string Minimap2MatchingScore { get; set; }
        public string Minimap2MismachPenalty { get; set; }
        public string Minimap2GapOpenPenalty { get; set; }
        public string Minimap2GapExtentionPenalty { get; set; }

    }
}
