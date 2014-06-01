using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

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

        public XElement serialize()
        {
            XElement e = new XElement("StateChanged");
            e.SetAttributeValue("time", Time);
            e.SetAttributeValue("newState", NewState);
            return e;
        }

        public static StateChanged deserialize(XElement e)
        {
            StateChanged sc = new StateChanged();
            sc.Time = DateTime.Parse(e.Attribute("time").ToString());
            sc.NewState = (State)Enum.Parse(typeof(State), e.Attribute("newState").ToString());
            return sc;
        }

        public static StateChanged deserialize(XmlReader reader)
        {
            StateChanged sc = new StateChanged();
            sc.Time = DateTime.Parse(reader.GetAttribute("time"));
            sc.NewState = (State)Enum.Parse(typeof(State), reader.GetAttribute("newState"));
            return sc;
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
