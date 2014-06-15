using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace PresenceTracker
{
    public class PresenceTrackerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<StateChanged> Messages { get; private set; }

        private ObservableCollection<StateChanged> _filteredMessages;
        public ObservableCollection<StateChanged> FilteredMessages
        {
            get { return _filteredMessages; }
            private set { _filteredMessages = value; NotifyPropertyChanged("FilteredMessages"); }
        }

        private Boolean _filterDuringDay;
        public Boolean FilterDuringDay
        {
            get { return _filterDuringDay; }
            set { _filterDuringDay = value; NotifyPropertyChanged("FilterDuringDay"); updateFilter(); }
        }

        private PresenceTrackerModel _data;

        public PresenceTrackerViewModel(PresenceTrackerModel data)
        {
            _filterDuringDay = false;
            _data = data;
            _filteredMessages = new ObservableCollection<StateChanged>();
            _data.Messages.CollectionChanged += messages_CollectionChanged;
            Messages = _data.Messages;

            updateFilter();
        }

        void messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            updateFilter();
        }

        private void updateFilter()
        {
            if (_filterDuringDay)
            {
                ObservableCollection<StateChanged> newColl = new ObservableCollection<StateChanged>();
                DateTime currDate = DateTime.MinValue;
                bool first = true;
                foreach (var sc in _data.Messages)
                {
                    if (sc.Time.Date != currDate.Date)
                    {
                        newColl.Add(sc);
                        currDate = sc.Time.Date;
                        first = true;
                    }
                    else if (sc.Time.Date == currDate.Date && first)
                    {
                        newColl.Add(sc);
                        first = false;
                    }
                    else
                    {
                        newColl.RemoveAt(newColl.Count - 1);
                        newColl.Add(sc);
                    }
                }
                FilteredMessages.Clear();
                foreach(var sc in newColl)
                    FilteredMessages.Add(sc);
            }
            else
            {
                FilteredMessages.Clear();
                foreach (var sc in Messages)
                    FilteredMessages.Add(sc);
            }
        }

        protected void NotifyPropertyChanged(string propName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs("propName"));
        }
    }
}
