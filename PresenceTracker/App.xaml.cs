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

        public PresenceTrackerViewModel Data;
        private PresenceTrackerModel DataModel;
        private TaskbarIcon _notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            checkSaveLocations();
            
            DataModel = new PresenceTrackerModel(dataLocation);
            DataModel.addStateChange(DateTime.Now, State.AppStart);

            Data = new PresenceTrackerViewModel(DataModel);
            
            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
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
    }
}
