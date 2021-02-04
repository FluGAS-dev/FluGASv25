using System;

namespace FluGASv25.Models.Properties
{
    public class MiseqParameters : Parameters
    {
        // for Miseq
        public string Bowtie2Mp { get; set; }
        public string Bowtie2Np { get; set; }
        public string Bowtie2Rdg { get; set; }
        public string Bowtie2Rfg { get; set; }
        public string Bowtie2ScoreMin { get; set; }
        public string Bowtie2Gbar { get; set; }
        public string Bowtie2Nceil { get; set; }
        public string Bowtie2D { get; set; }
        public string Bowtie2R { get; set; }
        public string Bowtie2N { get; set; }
        public string Bowtie2L { get; set; }
        public string Bowtie2I { get; set; }

        // for fastqc
        public bool IsFastQC { get; set; }
        public string FastQCMinPhredScore { get; set; }
        public string FastQCWindowSize { get; set; }
        public string FastQCMinLength { get; set; }

        // seqkit sampling sequence count
        public bool IsSampling { get; set; }
        public string MappingSeqCount { get; set; }
    }
}
