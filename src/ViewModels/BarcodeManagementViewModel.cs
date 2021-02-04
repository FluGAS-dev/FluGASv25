using FluGASv25.Utils;
using FluGASv25.ViewModels.Base;
using Livet.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace FluGASv25.ViewModels
{
    public class BarcodeManagementViewModel : DialogViewModel
    {

        public static readonly string defaultNameWeight = "Normal";
        public static readonly string defaultNameColor = "LightGray";
        public static readonly string userNameWeight = "Bold";
        public static readonly string userNameColor = "Black";


        public ViewModelCommand TemplateExcelCommand { private set; get; }
        public ViewModelCommand BarcodeCommitCommand { private set; get; }

        public string FontWeight() => defaultNameColor;
        public string FontColor() => defaultNameColor;
        public BarcodeManagementViewModel()
        {
            // CommandInit();
        }

        /**
         * barcode 分割する（ isSeparate ＝ true）& folderFileList が複数なら
         * folderFile + barcode -> sample-name となる
         * 
         */
        protected IEnumerable<string> _folderFileList;
        protected bool _isBarcode;
        protected bool _isOneRun;
        public BarcodeManagementViewModel(IEnumerable<string> folderFileList,　bool isOneRun, bool isBarcode)
        {
            _folderFileList = folderFileList;
            _isBarcode = isBarcode;
            _isOneRun = isOneRun;
            CommandInit();
        }


        protected void CommandInit()
        {
            System.Diagnostics.Debug.WriteLine("Initialize");
            SetBarcodeList();
            SetConfigList();

            this.TemplateExcelCommand = new ViewModelCommand(TemplateExcel);
            this.BarcodeCommitCommand = new ViewModelCommand(BarcodeCommit); 
        }

         // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).
        public void Initialize()  // view が読み込まれてから Call される
        {

        }

        private void SetBarcodeList()
        {
            if(_folderFileList == null || !_folderFileList.Any())
            {
                _isBarcode = false;
                return;
            }

            // barcode separate?
            if ( (_isBarcode &&  _isOneRun && _folderFileList.Any() ) ||            // 3times
                 (_isBarcode && !_isOneRun && _folderFileList.Count() == 1 )) // barcode あり 1-sample
                BarcodeList = SingleBarcodeList( );

            else if (!_isOneRun && _isBarcode && _folderFileList.Count() > 1)  // 1times
                BarcodeList = MultiBarcodeList();  // barcode あり muiti-barcode-sample

            else if ( (!_isBarcode && _isOneRun && _folderFileList.Any()) ||   // 3times
                       (!_isBarcode && !_isOneRun && _folderFileList.Count() == 1)) // no-barcode 1-sample
                BarcodeList = UniqBarcodeList();                                    

            else if (!_isBarcode && !_isOneRun && _folderFileList.Count() >1 ) // 1times
                BarcodeList = UniqsBarcodeList();                                       // no-barcode multi-sample

            else
                BarcodeList = null;  // あり得ないけど
        }


        private ObservableCollection<Barcode2Name> UniqsBarcodeList()
        {
            var barcode2nameList = new ObservableCollection<Barcode2Name>();
            foreach (var ffName in _folderFileList)
            {
                /// var baseName = Path.GetFileNameWithoutExtension(ffName);
                var baseName = VariousUtils.Path2String(ffName);
                var sampleName = VariousUtils.ShortNameString(baseName);

                barcode2nameList.Add(new Barcode2Name()
                {
                    Id = sampleIds,
                    BarcodeName = baseName,
                    BarcodeNum = baseName,
                    SampleName = sampleName,
                    FontWeight = defaultNameWeight,
                    FontColor = defaultNameColor
                });
                sampleIds += 1;
            }
            return barcode2nameList;
        }

        private ObservableCollection<Barcode2Name> UniqBarcodeList()
        {

            // var name = Path.GetFileNameWithoutExtension(_folderFileList.First());
            var name = VariousUtils.Path2String(_folderFileList.First());
            var sampleName = VariousUtils.ShortNameString(name);   // 長い名前(苦肉)対策
            return new ObservableCollection<Barcode2Name>()
            {
                new Barcode2Name
                    {
                        Id = sampleIds,
                        BarcodeName = string.Empty,
                        BarcodeNum = name,
                        SampleName = sampleName,
                        FontWeight = defaultNameWeight,
                        FontColor = defaultNameColor
                    }
                };
        }

        int sampleIds = 0;  // global
        private ObservableCollection<Barcode2Name> SingleBarcodeList(string baseName = "")
        {
            var num = 0;
            var barcode2nameList = new ObservableCollection<Barcode2Name>();
            while(num < ConstantValues.barcodeMax)
            {
                num++;
                sampleIds++;
                barcode2nameList.Add(
                    new Barcode2Name
                    {
                        Id = sampleIds,
                        BarcodeName = baseName,
                        BarcodeNum = FileUtils.GetBarcodeName(baseName, num),
                        // SampleName = ConstantValues.barcode + num.ToString("00"),
                        SampleName = FileUtils.GetBarcodeName(baseName, num),
                        FontWeight = defaultNameWeight,
                        FontColor = defaultNameColor
                    });
            }

            return barcode2nameList;
        }

        private ObservableCollection<Barcode2Name> MultiBarcodeList()
        {
            var barcode2nameList = new ObservableCollection<Barcode2Name>();
            foreach (var ffName in _folderFileList)
            {
                // var baseName = Path.GetFileNameWithoutExtension(ffName);
                var baseName = Utils.VariousUtils.Path2String(ffName);
                foreach (var bn in SingleBarcodeList(baseName))
                      barcode2nameList.Add(bn);
            }

            return barcode2nameList;
        }

        private void SetConfigList()
        {
            // guppy-bin ない場合。config list は空
            if (string.IsNullOrEmpty(Properties.Settings.Default.ont_path))
            {
                ConfigList = new List<string>();
            }
            else
            {
                var searchPath = Path.GetDirectoryName(
                                            Properties.Settings.Default.ont_path.TrimEnd('\\'));

                var di = new DirectoryInfo(searchPath);
                var files = di.EnumerateFiles("dna*.cfg", SearchOption.AllDirectories);

                if (files.Any()) { 
                    ConfigList = files.Select(s => s.Name).ToList();

                    var fastCfg = files.Where(s => s.Name.Contains("fast", StringComparison.OrdinalIgnoreCase));
                    if (fastCfg.Any())
                        SelectedConfig = fastCfg
                                                    .OrderByDescending(s => s.Name)
                                                    .First().Name;
                }
            }

        }

        public bool IsCommand = false;
        private void BarcodeCommit()
        {
            this.IsCommand = true;
            ViewClose();
        }

        // download template excel-file
        private void TemplateExcel()
        {
            var dialogRes = SelectFileDialog("save excel file", false, false, "barcodeTemplate.xlsx");
            if (dialogRes.Any()) { 
                var saveLocation = dialogRes.First();  // エクセルの保存先。

                var saveRes = Proc.Several.BarcodeExcel.CreateBarcodeExcel(saveLocation, _barcodeList);
                if (! string.IsNullOrEmpty(saveRes))
                    ShowErrorDialog(saveRes);
            }
        }

        // Select Data ListView Drag & Drop
        ListenerCommand<IEnumerable<Uri>> m_addItemsCommand;
        // ICommandを公開する
        public ICommand AddItemsCommand
        {
            get
            {
                if (m_addItemsCommand == null)
                {
                    m_addItemsCommand = new ListenerCommand<IEnumerable<Uri>>(DdExcel);
                }
                return m_addItemsCommand;
            }
        }

        private void DdExcel(IEnumerable<Uri> urilist)
        {
            var excels = urilist.Select(s => s.LocalPath).ToList(); // Excel file(s) ??

            var message = string.Empty;
            var inBarcode2samples = Proc.Several.BarcodeExcel.ReadBarcodeExcels(excels, ref message);

            // エクセルのエラー
            if (!string.IsNullOrEmpty(message)) { 
                ShowErrorDialog(message);
                return;
            }

            var newBarcodes = new ObservableCollection<Barcode2Name>();
            foreach (var barcode2sample in inBarcode2samples)  // エクセルから作成した barcode->sample 
            {
                var sample = BarcodeList.Where(s => s.BarcodeNum.Equals(barcode2sample.BarcodeNum, StringComparison.OrdinalIgnoreCase))
                                                                .ToArray();

                // 元々のリストにあったサンプル名と同じサンプル名がエクセルから取得出来た場合
                if (sample.Any() && !string.IsNullOrEmpty(sample.First().SampleName)) {
                    // sample.First().SampleName = barcode2sample.SampleName;
                    sample.First().SampleName = WfComponent.Utils.FileUtils.RemoveInvalidFileChar(barcode2sample.SampleName);
                    sample.First().FontWeight = userNameWeight;
                    sample.First().FontColor = userNameColor;
                }
                else
                {
                    // エクセルからバーコード名が取れない（空セル）か、ヘッダ文字
                    if (string.IsNullOrEmpty(barcode2sample.BarcodeNum) || 
                        barcode2sample.BarcodeNum.Equals(Proc.Several.BarcodeExcel.BarcodeHeader, StringComparison.OrdinalIgnoreCase))
                        continue;


                    // エクセルから新規に追加された
                    BarcodeList.Add(new Barcode2Name()
                    {
                        BarcodeName = barcode2sample.BarcodeName,
                        BarcodeNum = barcode2sample.BarcodeNum,
                        SampleName = WfComponent.Utils.FileUtils.RemoveInvalidFileChar(barcode2sample.SampleName),
                        FontWeight = userNameWeight,
                        FontColor = userNameColor,
                    });
                }
            }
        }

        private string _selectedConfig;
        public string SelectedConfig
        {
            get => _selectedConfig;
            set { RaisePropertyChangedIfSet(ref _selectedConfig, value); }
        }
        private List<string> _configList;
        public List<string> ConfigList
        {
            get => _configList;
            set { RaisePropertyChangedIfSet(ref _configList, value); }
        }

        private ObservableCollection<Barcode2Name> _barcodeList;
        public ObservableCollection<Barcode2Name> BarcodeList
        {
            get => _barcodeList;
            private set { RaisePropertyChangedIfSet(ref _barcodeList, value); }
        }

        public bool IsBarcode => _isBarcode;

    }
}
