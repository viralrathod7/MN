using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace MNDataSearch.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
        }
        private ObservableCollection<string> _Titles = new ObservableCollection<string>();
        private ObservableCollection<string> _Directors = new ObservableCollection<string>();
        private ObservableCollection<string> _Producers = new ObservableCollection<string>();
        private ObservableCollection<string> _Languages = new ObservableCollection<string>();
        private ObservableCollection<string> _Years = new ObservableCollection<string>();
        private ObservableCollection<string> _CastCrews = new ObservableCollection<string>();
        private ObservableCollection<string> _MainClass = new ObservableCollection<string>();
        private string _Keyword = string.Empty;
        private string all = "All";

        public ObservableCollection<string> Titles { get { return _Titles; } set { _Titles = value; NotifyPropertyChanged("Titles"); } }
        public ObservableCollection<string> Directors { get { return _Directors; } set { _Directors = value; NotifyPropertyChanged("Directors"); } }
        public ObservableCollection<string> Producers { get { return _Producers; } set { _Producers = value; NotifyPropertyChanged("Producers"); } }
        public ObservableCollection<string> Languages { get { return _Languages; } set { _Languages = value; NotifyPropertyChanged("Languages"); } }
        public ObservableCollection<string> Years { get { return _Years; } set { _Years = value; NotifyPropertyChanged("Years"); } }
        public ObservableCollection<string> CastCrews { get { return _CastCrews; } set { _CastCrews = value; NotifyPropertyChanged("CastCrews"); } }
        public ObservableCollection<string> MainClasses { get { return _MainClass; } set { _MainClass = value; NotifyPropertyChanged("MainClasses"); } }
        public string Keyword { get { return _Keyword; } set { _Keyword = value; NotifyPropertyChanged("Keyword"); } }

        public void PopulateData()
        {
            Titles = new ObservableCollection<string>(Helper.GlobalClass.Data.Where(v => !string.IsNullOrWhiteSpace(v.Title)).Select(v => v.Title).Distinct().OrderBy(o => o));
            Directors = new ObservableCollection<string>(Helper.GlobalClass.Data.Where(v => !string.IsNullOrWhiteSpace(v.Director)).Select(v => v.Director).Distinct().OrderBy(o => o));
            Producers = new ObservableCollection<string>(Helper.GlobalClass.Data.Where(v => !string.IsNullOrWhiteSpace(v.Producer)).Select(v => v.Producer).Distinct().OrderBy(o => o));
            Languages = new ObservableCollection<string>(Helper.GlobalClass.Data.Where(v => !string.IsNullOrWhiteSpace(v.Language)).Select(v => v.Language).Distinct().OrderBy(o => o));
            Years = new ObservableCollection<string>(Helper.GlobalClass.Data.Where(v => v.Year > 0).Select(v => v.Year.ToString()).Distinct().OrderBy(o => o));
            Titles.Insert(0, all);
            Directors.Insert(0, all);
            Producers.Insert(0, all);
            Languages.Insert(0, all);
            Years.Insert(0, all);
            //sliderDurationMinimum = Helper.GlobalClass.Data.Min(v => v.Duration);
            //sliderDurationMaximum = Helper.GlobalClass.Data.Max(v => v.Duration);
            //sliderDurationValue = Helper.GlobalClass.Data.Max(v => v.Duration);  
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
