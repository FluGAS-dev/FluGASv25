using FluGASv25.Models.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FluGASv25.ViewModels
{
    public partial class MainWindowViewModel 
    {

        private List<Sample> _sampleList;
        public List<Sample> SampleList
        {
            get { return _sampleList; }
            set { RaisePropertyChangedIfSet(ref _sampleList, value); }
        }

        private string _sampleName;
        public string SampleName
        {
            get { return _sampleName; }
            set { RaisePropertyChangedIfSet(ref _sampleName, value); }
        }

        private string _execDate;
        public string ExecDate
        {
            get { return _execDate; }
            set { RaisePropertyChangedIfSet(ref _execDate, value); }
        }

        private string _callType;
        public string CallType
        {
            get { return _callType; }
            set { RaisePropertyChangedIfSet(ref _callType, value); }
        }

        private string _viewName;
        public string ViewName
        {
            get { return _viewName; }
            set { RaisePropertyChangedIfSet(ref _viewName, value); }
        }

        private string _subTypes;
        public string SubTypes
        {
            get { return _subTypes; }
            set { RaisePropertyChangedIfSet(ref _subTypes, value); }
        }

        private DateTime _toDatePick;
        public DateTime ToDatePick
        {
            get { return _toDatePick; }
            set {
                if (RaisePropertyChangedIfSet(ref _toDatePick, value))
                    DateSelectSampleList();
            }
        }

        private DateTime _fromDatePick;
        public DateTime FromDatePick
        {
            get { return _fromDatePick; }
            set {
                if (RaisePropertyChangedIfSet(ref _fromDatePick, value))
                    DateSelectSampleList();
            }
        }
        private void DateSelectSampleList()
        {

            if ( this._sampleList == null || !this._sampleList.Any()) this.SampleList = Dao.SampleDao.GetSamples().ToList();

            var datePicSelectSampleList = SampleList.Where(s => this._fromDatePick <= DateTime.Parse(s.DATEONLY) &&
                                                                                   DateTime.Parse(s.DATEONLY) <= this._toDatePick)
                                                                                   .ToList();
            this.SampleList = datePicSelectSampleList;
            SetDetailClear();
        }

    }
}
