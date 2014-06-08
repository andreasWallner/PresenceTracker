using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using System.Collections.ObjectModel;
using System;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;

namespace PresenceTracker
{


    public partial class App : Application
    {
        public readonly string dataLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/PresenceTracker";

        private TaskbarIcon _notifyIcon;
        private DataAppender _appender;
        private SystemEventCollector _sysEventCollector;

        private ObservableCollection<StateChanged> _messages = new ObservableCollection<StateChanged>();
        public ObservableCollection<StateChanged> messages { get { return _messages; } }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            checkSaveLocations();
            loadMessages();

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            _appender = new DataAppender(_messages, dataLocation + "/statechanges.xmlpart");
            _sysEventCollector = new SystemEventCollector();
            _sysEventCollector.SessionEvent += SysEventCollector_SessionEvent;

            StateChanged sc = new StateChanged(DateTime.Now, State.AppStart));
            messages.Insert(0, sc);
            _appender.append(sc);
        }

        private void SysEventCollector_SessionEvent(object sender, SystemEventArgs e)
        {
            StateChanged sc = new StateChanged(DateTime.Now, e.newState);
            messages.Insert(0, sc);
            _appender.append(sc);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose();
            base.OnExit(e);
        }

        protected void checkSaveLocations()
        {
            if( !Directory.Exists(dataLocation))
                Directory.CreateDirectory(dataLocation);
            if (!File.Exists(dataLocation + "/presence.xml"))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\"?>");
                sb.AppendLine("<!DOCTYPE presence [");
                sb.AppendLine("<!ENTITY statechanges    ");
                sb.AppendLine("SYSTEM \"./statechanges.xmlpart\">");
                sb.AppendLine("]>");
                sb.AppendLine("<presence version=\"1\">");
                sb.AppendLine("&statechanges;");
                sb.AppendLine("</presence>");

                StreamWriter sw = new StreamWriter(dataLocation + "/presence.xml");
                sw.Write(sb);
                sw.Close();
            }
            if (!File.Exists(dataLocation + "/statechanges.xmlpart"))
                File.Create(dataLocation + "/statechanges.xmlpart").Close();
        }

        private void loadMessages()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            DateTime now = DateTime.Now;
            TimeSpan limit = new TimeSpan(10,0,0,0);
            List<StateChanged> list = new List<StateChanged>();

            using (XmlReader reader = XmlReader.Create(dataLocation + "/presence.xml", settings))
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
