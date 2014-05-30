using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PresenceTracker
{
    public class StateChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private DateTime _time;
        private State _newState;

        public DateTime Time
        {
            get { return _time; }
            set
            {
                _time = value;
                OnPropertyChanged("Time");
            }
        }

        public State NewState
        {
            get { return _newState; }
            set
            {
                _newState = value;
                OnPropertyChanged("NewState");
            }
        }

        public StateChanged()
        {
            _time = DateTime.Now;
            _newState = State.Unknown;
        }

        public StateChanged(DateTime time, State newState)
        {
            _time = time;
            _newState = newState;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
    public class StateChangedCollection : ObservableCollection<StateChanged> { }
}
