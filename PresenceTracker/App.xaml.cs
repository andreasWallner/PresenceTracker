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
        private TaskbarIcon notifyIcon;

        private ObservableCollection<StateChanged> _messages = new ObservableCollection<StateChanged>();
        public ObservableCollection<StateChanged> messages { get { return _messages; } }
        private DataAppender _appender;

        public readonly string dataLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/PresenceTracker";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            checkSaveLocations();
            loadMessages();

            SystemEvents.SessionEnding += new SessionEndingEventHandler(SystemEvents_SessionEnding);
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            _appender = new DataAppender(_messages, dataLocation + "/statechanges.xmlpart");

            messages.Insert(0, new StateChanged(DateTime.Now, State.AppStart));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }

        protected void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            State s;
            switch (e.Reason)
            {
                case SessionEndReasons.Logoff:
                    s = State.Logoff;
                    break;
                case SessionEndReasons.SystemShutdown:
                    s = State.Shutdown;
                    break;
                default:
                    s = State.Unknown;
                    break;
            }
            messages.Insert(0, new StateChanged(DateTime.Now, s));
        }

        protected void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            State s;
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    s = State.Lock;
                    break;
                case SessionSwitchReason.SessionLogoff:
                    s = State.Logoff;
                    break;
                case SessionSwitchReason.SessionLogon:
                    s = State.Logon;
                    break;
                case SessionSwitchReason.SessionUnlock:
                    s = State.Unlock;
                    break;
                default:
                    s = State.Unknown;
                    break;
            }

            messages.Insert(0, new StateChanged(DateTime.Now, s));
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
        /*System.Environment.MachineName*/
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
