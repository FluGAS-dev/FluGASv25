
using System.Reflection;

namespace FluGASv25.ViewModels
{
    public partial class MainWindowViewModel 
    {

        private int _aHAminLength;
        public string AHAminLength
        {
            get { return _aHAminLength.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aHAminLength), value); }
        }

        private int _aMPminLength;
        public string AMPminLength
        {
            get { return _aMPminLength.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aMPminLength), value); }
        }

        private int _aNAminLength;
        public string ANAminLength
        {
            get { return _aNAminLength.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aNAminLength), value); }
        }

        private int _aNPminLength;
        public string ANPminLength
        {
            get { return _aNPminLength.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aNPminLength), value); }
        }

        private int _aNSminLength;
        public string ANSminLength
        {
            get { return _aNSminLength.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aNSminLength), value); }
        }

        private int _aPAminLength;
        public string APAminLength
        {
            get { return _aPAminLength.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aPAminLength), value); }
        }

        private int _aPB1minLength;
        public string APB1minLength
        {
            get { return _aPB1minLength.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aPB1minLength), value); }
        }

        private int _aPB2minLength;
        public string APB2minLength
        {
            get { return _aPB2minLength.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aPB2minLength), value); }
        }

        private int _aHAmaxLength;
        public string AHAmaxLength
        {
            get { return _aHAmaxLength.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aHAmaxLength), value); }
        }

        private int _aMPmaxLength;
        public string AMPmaxLength
        {
            get { return _aMPmaxLength.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aMPmaxLength), value); }
        }

        private int _aNAmaxLength;
        public string ANAmaxLength
        {
            get { return _aNAmaxLength.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aNAmaxLength), value); }
        }

        private int _aNPmaxLength;
        public string ANPmaxLength
        {
            get { return _aNPmaxLength.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aNPmaxLength), value); }
        }

        private int _aNSmaxLength;
        public string ANSmaxLength
        {
            get { return _aNSmaxLength.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aNSmaxLength), value); }
        }

        private int _aPAmaxLength;
        public string APAmaxLength
        {
            get { return _aPAmaxLength.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aPAmaxLength), value); }
        }

        private int _aPB1maxLength;
        public string APB1maxLength
        {
            get { return _aPB1maxLength.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aPB1maxLength), value); }
        }

        private int _aPB2maxLength;
        public string APB2maxLength
        {
            get { return _aPB2maxLength.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aPB2maxLength), value); }
        }

        private string _aHA5nuc;
        public string AHA5nuc
        {
            get { return _aHA5nuc; }
            set { RaisePropertyChangedIfSet(ref _aHA5nuc, value); }
        }

        private string _aMP5nuc;
        public string AMP5nuc
        {
            get { return _aMP5nuc; }
            set { RaisePropertyChangedIfSet(ref _aMP5nuc, value); }
        }

        private string _aNA5nuc;
        public string ANA5nuc
        {
            get { return _aNA5nuc; }
            set { RaisePropertyChangedIfSet(ref _aNA5nuc, value); }
        }

        private string _aNP5nuc;
        public string ANP5nuc
        {
            get { return _aNP5nuc; }
            set { RaisePropertyChangedIfSet(ref _aNP5nuc, value); }
        }

        private string _aNS5nuc;
        public string ANS5nuc
        {
            get { return _aNS5nuc; }
            set { RaisePropertyChangedIfSet(ref _aNS5nuc, value); }
        }

        private string _aPA5nuc;
        public string APA5nuc
        {
            get { return _aPA5nuc; }
            set { RaisePropertyChangedIfSet(ref _aPA5nuc, value); }
        }

        private string _aPB15nuc;
        public string APB15nuc
        {
            get { return _aPB15nuc; }
            set { RaisePropertyChangedIfSet(ref _aPB15nuc, value); }
        }

        private string _aPB25nuc;
        public string APB25nuc
        {
            get { return _aPB25nuc; }
            set { RaisePropertyChangedIfSet(ref _aPB25nuc, value); }
        }

        private string _aHA3nuc;
        public string AHA3nuc
        {
            get { return _aHA3nuc; }
            set { RaisePropertyChangedIfSet(ref _aHA3nuc, value); }
        }

        private string _aMP3nuc;
        public string AMP3nuc
        {
            get { return _aMP3nuc; }
            set { RaisePropertyChangedIfSet(ref _aMP3nuc, value); }
        }

        private string _aNA3nuc;
        public string ANA3nuc
        {
            get { return _aNA3nuc; }
            set { RaisePropertyChangedIfSet(ref _aNA3nuc, value); }
        }

        private string _aNP3nuc;
        public string ANP3nuc
        {
            get { return _aNP3nuc; }
            set { RaisePropertyChangedIfSet(ref _aNP3nuc, value); }
        }

        private string _aNS3nuc;
        public string ANS3nuc
        {
            get { return _aNS3nuc; }
            set { RaisePropertyChangedIfSet(ref _aNS3nuc, value); }
        }

        private string _aPA3nuc;
        public string APA3nuc
        {
            get { return _aPA3nuc; }
            set { RaisePropertyChangedIfSet(ref _aPA3nuc, value); }
        }

        private string _aPB13nuc;
        public string APB13nuc
        {
            get { return _aPB13nuc; }
            set { RaisePropertyChangedIfSet(ref _aPB13nuc, value); }
        }

        private string _aPB23nuc;
        public string APB23nuc
        {
            get { return _aPB23nuc; }
            set { RaisePropertyChangedIfSet(ref _aPB23nuc, value); }
        }

        private int _aHAincN;
        public int AHAincN
        {
            get { return _aHAincN; }
            set { RaisePropertyChangedIfSet(ref _aHAincN, value); }
        }

        private int _aMPincN;
        public int AMPincN
        {
            get { return _aMPincN; }
            set { RaisePropertyChangedIfSet(ref _aMPincN, value); }
        }

        private int _aNAincN;
        public int ANAincN
        {
            get { return _aNAincN; }
            set { RaisePropertyChangedIfSet(ref _aNAincN, value); }
        }

        private int _aNPincN;
        public int ANPincN
        {
            get { return _aNPincN; }
            set { RaisePropertyChangedIfSet(ref _aNPincN, value); }
        }

        private int _aNSincN;
        public int ANSincN
        {
            get { return _aNSincN; }
            set { RaisePropertyChangedIfSet(ref _aNSincN, value); }
        }

        private int _aPAincN;
        public int APAincN
        {
            get { return _aPAincN; }
            set { RaisePropertyChangedIfSet(ref _aPAincN, value); }
        }

        private int _aPB1incN;
        public int APB1incN
        {
            get { return _aPB1incN; }
            set { RaisePropertyChangedIfSet(ref _aPB1incN, value); }
        }

        private int _aPB2incN;
        public int APB2incN
        {
            get { return _aPB2incN; }
            set { RaisePropertyChangedIfSet(ref _aPB2incN, value); }
        }

        private string _aHAmapcdhit;
        public string AHAmapcdhit
        {
            get { return _aHAmapcdhit; }
            set { RaisePropertyChangedIfSet(ref _aHAmapcdhit, value); }
        }

        private string _aMPmapcdhit;
        public string AMPmapcdhit
        {
            get { return _aMPmapcdhit; }
            set { RaisePropertyChangedIfSet(ref _aMPmapcdhit, value); }
        }

        private string _aNAmapcdhit;
        public string ANAmapcdhit
        {
            get { return _aNAmapcdhit; }
            set { RaisePropertyChangedIfSet(ref _aNAmapcdhit, value); }
        }

        private string _aNPmapcdhit;
        public string ANPmapcdhit
        {
            get { return _aNPmapcdhit; }
            set { RaisePropertyChangedIfSet(ref _aNPmapcdhit, value); }
        }

        private string _aNSmapcdhit;
        public string ANSmapcdhit
        {
            get { return _aNSmapcdhit; }
            set { RaisePropertyChangedIfSet(ref _aNSmapcdhit, value); }
        }

        private string _aPAmapcdhit;
        public string APAmapcdhit
        {
            get { return _aPAmapcdhit; }
            set { RaisePropertyChangedIfSet(ref _aPAmapcdhit, value); }
        }

        private string _aPB1mapcdhit;
        public string APB1mapcdhit
        {
            get { return _aPB1mapcdhit; }
            set { RaisePropertyChangedIfSet(ref _aPB1mapcdhit, value); }
        }

        private string _aPB2mapcdhit;
        public string APB2mapcdhit
        {
            get { return _aPB2mapcdhit; }
            set { RaisePropertyChangedIfSet(ref _aPB2mapcdhit, value); }
        }

        private string _aHAbltcdhit;
        public string AHAbltcdhit
        {
            get { return _aHAbltcdhit; }
            set { RaisePropertyChangedIfSet(ref _aHAbltcdhit, value); }
        }

        private string _aMPbltcdhit;
        public string AMPbltcdhit
        {
            get { return _aMPbltcdhit; }
            set { RaisePropertyChangedIfSet(ref _aMPbltcdhit, value); }
        }

        private string _aNAbltcdhit;
        public string ANAbltcdhit
        {
            get { return _aNAbltcdhit; }
            set { RaisePropertyChangedIfSet(ref _aNAbltcdhit, value); }
        }

        private string _aNPbltcdhit;
        public string ANPbltcdhit
        {
            get { return _aNPbltcdhit; }
            set { RaisePropertyChangedIfSet(ref _aNPbltcdhit, value); }
        }

        private string _aNSbltcdhit;
        public string ANSbltcdhit
        {
            get { return _aNSbltcdhit; }
            set { RaisePropertyChangedIfSet(ref _aNSbltcdhit, value); }
        }

        private string _aPAbltcdhit;
        public string APAbltcdhit
        {
            get { return _aPAbltcdhit; }
            set { RaisePropertyChangedIfSet(ref _aPAbltcdhit, value); }
        }

        private string _aPB1bltcdhit;
        public string APB1bltcdhit
        {
            get { return _aPB1bltcdhit; }
            set { RaisePropertyChangedIfSet(ref _aPB1bltcdhit, value); }
        }

        private string _aPB2bltcdhit;
        public string APB2bltcdhit
        {
            get { return _aPB2bltcdhit; }
            set { RaisePropertyChangedIfSet(ref _aPB2bltcdhit, value); }
        }

        private int _aHAminCDS;
        public string AHAminCDS
        {
            get { return _aHAminCDS.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aHAminCDS), value); }
        }

        private int _aMPminCDS;
        public string AMPminCDS
        {
            get { return _aMPminCDS.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aMPminCDS), value); }
        }

        private int _aNAminCDS;
        public string ANAminCDS
        {
            get { return _aNAminCDS.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aNAminCDS), value); }
        }

        private int _aNPminCDS;
        public string ANPminCDS
        {
            get { return _aNPminCDS.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aNPminCDS), value); }
        }

        private int _aNSminCDS;
        public string ANSminCDS
        {
            get { return _aNSminCDS.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aNSminCDS), value); }
        }

        private int _aPAminCDS;
        public string APAminCDS
        {
            get { return _aPAminCDS.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aPAminCDS), value); }
        }

        private int _aPB1minCDS;
        public string APB1minCDS
        {
            get { return _aPB1minCDS.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aPB1minCDS), value); }
        }

        private int _aPB2minCDS;
        public string APB2minCDS
        {
            get { return _aPB2minCDS.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aPB2minCDS), value); }
        }

        private int _aHAmaxCDS;
        public string AHAmaxCDS
        {
            get { return _aHAmaxCDS.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aHAmaxCDS), value); }
        }

        private int _aMPmaxCDS;
        public string AMPmaxCDS
        {
            get { return _aMPmaxCDS.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aMPmaxCDS), value); }
        }

        private int _aNAmaxCDS;
        public string ANAmaxCDS
        {
            get { return _aNAmaxCDS.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aNAmaxCDS), value); }
        }

        private int _aNPmaxCDS;
        public string ANPmaxCDS
        {
            get { return _aNPmaxCDS.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aNPmaxCDS), value); }
        }

        private int _aNSmaxCDS;
        public string ANSmaxCDS
        {
            get { return _aNSmaxCDS.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aNSmaxCDS), value); }
        }

        private int _aPAmaxCDS;
        public string APAmaxCDS
        {
            get { return _aPAmaxCDS.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aPAmaxCDS), value); }
        }

        private int _aPB1maxCDS;
        public string APB1maxCDS
        {
            get { return _aPB1maxCDS.ToString(); }
            set { SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aPB1maxCDS), value); }
        }

        private int _aPB2maxCDS;
        public string APB2maxCDS
        {
            get { return _aPB2maxCDS.ToString(); }
            set 
            {
                // RaisePropertyChangedIfSet(ref _aPB2maxCDS, value);
                SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aPB2maxCDS), value);
            }
        }

        private string _typeAtes;
        public string typeAtes
        {
            get { return _typeAtes; }
            set
            {
                // RaisePropertyChangedIfSet(ref _aPB2maxCDS, value);
                SetDigitText(MethodBase.GetCurrentMethod().Name, nameof(_aPB2maxCDS), value);
            }
        }

    }
}
