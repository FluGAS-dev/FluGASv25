using Livet;

namespace FluGASv25.ViewModels.Base
{
    public class Barcode2Name : ViewModel
    {

        public int Id { set; get; }
        public string BarcodeNum { set; get; }
        public string BarcodeName { set; get; }

        private string _sampleName = string.Empty;
        public string SampleName
        {
            get => _sampleName;
            set { RaisePropertyChangedIfSet(ref _sampleName, value); }
        }

        private string _fontWeight;
        public string FontWeight
        {
            get => _fontWeight;
            set { RaisePropertyChangedIfSet(ref _fontWeight, value); }
        }

        private string _fontColor;
        public string FontColor
        {
            get => _fontColor;
            set { RaisePropertyChangedIfSet(ref _fontColor, value); }
        }
    }
}
