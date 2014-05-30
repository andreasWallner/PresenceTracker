using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using System.Collections.ObjectModel;
using System;
using Microsoft.Win32;

namespace PresenceTracker
{
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;

        private ObservableCollection<StateChanged> _messages = new ObservableCollection<StateChanged>();
        public ObservableCollection<StateChanged> messages { get { return _messages; } }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SystemEvents.SessionEnding += new SessionEndingEventHandler(SystemEvents_SessionEnding);
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            messages.Add(new StateChanged(DateTime.Now, State.AppStart));
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
            messages.Add(new StateChanged(DateTime.Now, s));
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

            messages.Add(new StateChanged(DateTime.Now, s));
        }
    }
}
