using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PresenceTracker
{
    public class PresenceTrackerModel
    {
        private ObservableCollection<StateChanged> _messages = new ObservableCollection<StateChanged>();
        public ObservableCollection<StateChanged> Messages { get { return _messages; } }

        private DataAppender _appender;
        private SystemEventCollector _sysEventCollector;
        private string _dataLocation;

        public PresenceTrackerModel(string dataLocation)
        {
            _dataLocation = dataLocation;
            loadData();

            _appender = new DataAppender(_messages, dataLocation + "/statechanges.xmlpart");
            _sysEventCollector = new SystemEventCollector();
            _sysEventCollector.SessionEvent += SysEventCollector_SessionEvent;
            MidnightNotifier.DayChanged += MidnightNotifier_DayChanged;
        }

        public void addStateChange(DateTime date, State newState)
        {
            StateChanged sc = new StateChanged(DateTime.Now, newState);
            Messages.Insert(0, sc);
            _appender.append(sc);
        }

        void MidnightNotifier_DayChanged(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            TimeSpan limit = new TimeSpan(10, 0, 0, 0);

            for (int i = Messages.Count - 1; i >= 0; i--)
            {
                if (now - Messages[i].Time > limit)
                    Messages.RemoveAt(i);
            }
        }

        private void SysEventCollector_SessionEvent(object sender, SystemEventArgs e)
        {
            addStateChange(DateTime.Now, e.newState);
        }

        private void loadData()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            DateTime now = DateTime.Now;
            TimeSpan limit = new TimeSpan(10, 0, 0, 0);
            List<StateChanged> list = new List<StateChanged>();

            using (XmlReader reader = XmlReader.Create(_dataLocation + "/presence.xml", settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "StateChanged")
                            {
                                StateChanged sc = StateChanged.deserialize(reader);
                                if (sc.Time - now < limit)
                                    list.Add(sc);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            list.Reverse();
            foreach (var e in list)
                _messages.Add(e);
        }
    }
}
