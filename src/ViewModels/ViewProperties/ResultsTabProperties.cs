using FluGASv25.Models.Properties;
using FluGASv25.Proc.Flow;
using FluGASv25.Proc.Several;
using FluGASv25.Utils;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace FluGASv25.ViewModels.ViewProperties
{

    // Detail tab の中身。
    public class ResultsTabProperties
    {

        private IProgress<string> mainLog;
        public ResultsTabProperties(IProgress<string> progress)
        {
            this.mainLog = progress;
        }

        // alignment viewer : tablet 
        public void AlignmentViewCommand(string type_segment_idx)
        {
            System.Diagnostics.Debug.WriteLine("mapping alignment view command " + type_segment_idx);
            // サンプルが選択されていない。
            if (this.SelectedSampleId < 0) return;

            if (string.IsNullOrEmpty(type_segment_idx))
            {
                mainLog.Report("no align command parameter ...");
                System.Diagnostics.Debug.WriteLine("no align command parameter ...");
                return;
            }

            var type = type_segment_idx.Split(ConstantValues.ViewDelimiter).ElementAt(0);
            var segment = type_segment_idx.Split(ConstantValues.ViewDelimiter).ElementAt(1);
            var idx = type_segment_idx.Split(ConstantValues.ViewDelimiter).ElementAt(2);

            mainLog.Report(type + segment + " mapping view start ");

            Sample viewAlignSample =   // 選択中のSAMPLE 情報
                            Dao.SampleDao.GetSamples().Where(s => s.ID == SelectedSampleId).FirstOrDefault();
            if(viewAlignSample == null)
            {
                mainLog.Report("Sample record is not found. ID = " + SelectedSampleId);
                return;
            }

            var sampleOutDir = viewAlignSample.MEMO;  // sample ごとの outdir 
            var referenceFastaPath = Path.Combine(   // secMappingReferenceDir/// InfluenzaAHA.fna
                                                    sampleOutDir,
                                                    ConstantValues.mappingResultDirName,
                                                    ConstantValues.secMappingResultDirName,
                                                    ConstantValues.secMappingReferenceDirName,
                                                    // FluTypeSeparate.Influenza + type.ToUpper() + segment.ToLower() + ConstantValues.secMappingFooter + "1" + FluTypeSeparate.FnaFooter
                                                    FluTypeSeparate.Influenza + type.ToUpper() + segment.ToLower() + FluTypeSeparate.FnaFooter
                                                    );

            var sortedBam = Path.Combine(    // Amp-Top1-sorted.bam
                                                    sampleOutDir,
                                                    ConstantValues.mappingResultDirName,
                                                    ConstantValues.secMappingResultDirName,
                                                    type + segment.ToLower() + "-Top" + idx + "-sorted.bam"
                                                    );
            if (!File.Exists(sortedBam))
            {  // error dialog.
                MessageBox.Show("Not found bam file." + Environment.NewLine + " Did you move or delete from the original location? ",
                                            "mapping view  not start .",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Warning);
                mainLog.Report("not find bam/reference, no start mapping view.....");
                mainLog.Report(referenceFastaPath);
                mainLog.Report(sortedBam);
                mainLog.Report("open viewer fail.");
                return;
            }
            // var sortedBamPaths = string.Join(",",  bams);

            // dialog.
            // Display message box
            var res = MessageBox.Show("show mapping alignment? \n(It takes a long time to start.)",
                                                    "other program start confirm.",
                                                    MessageBoxButton.YesNo,
                                                    MessageBoxImage.Question);
            // if (ShowConfirmDialog("show alignment?" + Environment.NewLine))   // Livet Messenger 開かない
            if (res == MessageBoxResult.Yes)
            {
                mainLog.Report("find bam/reference, start ....." + Path.GetFileName(sortedBam));
                WfComponent.External.Tablet.
                        TabletStart(
                                AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'),
                                                                        referenceFastaPath,
                                                                        sortedBam,
                                                                        false);
                mainLog.Report("start mapping view :" + sortedBam);
                System.Diagnostics.Debug.WriteLine("start mapping view :" + sortedBam);
                System.Threading.Thread.Sleep(1000);
            }
            // cancel は 放って置く

            // Consensus \ xxxxxxx-InfluenzaAHA-consensus1.align
            var alignFileDir = Path.Combine(
                                                    sampleOutDir,
                                                    CommonFlow.ConsensusOutDirName);
            var alignFileBase = FluTypeSeparate.Influenza + type.ToUpper() + segment.ToUpper() + "-consensus" + idx;
            var alignFiles = WfComponent.Utils.FileUtils.PartOfSearchFile(alignFileDir, alignFileBase, CommonFlow.alignFooter);
            if (!alignFiles.Any())
            {
                mainLog.Report("not found Aa-align file(s) :" + alignFileBase);
                return;
            }

            foreach (var alignFile in alignFiles)
            {
                mainLog.Report("alignview start : " + alignFile);
                var alignRes = WfComponent.External.AliView.AliViewStart(alignFile);

                if( ! string.IsNullOrEmpty(alignRes)) 
                    mainLog.Report("alignment viewer error, " + alignRes);
                System.Threading.Thread.Sleep(1000);
            }
        }

        // 描画中のサンプルID
        public int SelectedSampleId { get; set; }

        // Tab name
        public string TabName { get; set; }

        // out view command parameter
        public string HAalign { get; set; } = string.Empty;
        public string MPalign { get; set; } = string.Empty;
        public string NAalign { get; set; } = string.Empty;
        public string NPalign { get; set; } = string.Empty;
        public string NSalign { get; set; } = string.Empty;
        public string PAalign { get; set; } = string.Empty;
        public string PB1align { get; set; } = string.Empty;
        public string PB2align { get; set; } = string.Empty;

        // include N 
        public string HAinc { get; set; } = string.Empty;
        public string MPinc { get; set; } = string.Empty;
        public string NAinc { get; set; } = string.Empty;
        public string NPinc { get; set; } = string.Empty;
        public string NSinc { get; set; } = string.Empty;
        public string PAinc { get; set; } = string.Empty;
        public string PB1inc { get; set; } = string.Empty;
        public string PB2inc { get; set; } = string.Empty;

        // ratio 
        public string HAratio { get; set; } = string.Empty;
        public string MPratio { get; set; } = string.Empty;
        public string NAratio { get; set; } = string.Empty;
        public string NPratio { get; set; } = string.Empty;
        public string NSratio { get; set; } = string.Empty;
        public string PAratio { get; set; } = string.Empty;
        public string PB1ratio { get; set; } = string.Empty;
        public string PB2ratio { get; set; } = string.Empty;

        // Avarage 
        public string HAavg { get; set; } = string.Empty;
        public string MPavg { get; set; } = string.Empty;
        public string NAavg { get; set; } = string.Empty;
        public string NPavg { get; set; } = string.Empty;
        public string NSavg { get; set; } = string.Empty;
        public string PAavg { get; set; } = string.Empty;
        public string PB1avg { get; set; } = string.Empty;
        public string PB2avg { get; set; } = string.Empty;

        // cds
        public string HAcds { get; set; } = string.Empty;
        public string MPcds { get; set; } = string.Empty;
        public string NAcds { get; set; } = string.Empty;
        public string NPcds { get; set; } = string.Empty;
        public string NScds { get; set; } = string.Empty;
        public string PAcds { get; set; } = string.Empty;
        public string PB1cds { get; set; } = string.Empty;
        public string PB2cds { get; set; } = string.Empty;

        // consensus sequence 
        public string HAseq { get; set; } = string.Empty;
        public string MPseq { get; set; } = string.Empty;
        public string NAseq { get; set; } = string.Empty;
        public string NPseq { get; set; } = string.Empty;
        public string NSseq { get; set; } = string.Empty;
        public string PAseq { get; set; } = string.Empty;
        public string PB1seq { get; set; } = string.Empty;
        public string PB2seq { get; set; } = string.Empty;
    }
}
