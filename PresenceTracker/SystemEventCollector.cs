using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresenceTracker
{
    public class SystemEventArgs : EventArgs
    {
        public State newState { get; set; }
    }

    public delegate void SystemEventHandler(object sender, SystemEventArgs e);

    class SystemEventCollector
    {
        public event SystemEventHandler SessionEvent;

        public SystemEventCollector()
        {
            SystemEvents.SessionEnding += SystemEvents_SessionEnding;
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            SystemEventArgs ea = new SystemEventArgs();
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    ea.newState = State.Resume;
                    break;
                case PowerModes.Suspend:
                    ea.newState = State.Suspend;
                    break;
            }
            SessionEvent.Raise(this, ea);
        }

        protected void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            SystemEventArgs ea = new SystemEventArgs();
            switch (e.Reason)
            {
                case SessionEndReasons.Logoff:
                    ea.newState = State.Logoff;
                    break;
                case SessionEndReasons.SystemShutdown:
                    ea.newState = State.Shutdown;
                    break;
                default:
                    ea.newState = State.Unknown;
                    break;
            }

            SessionEvent.Raise(this, ea);
        }

        protected void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            SystemEventArgs ea = new SystemEventArgs();
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    ea.newState = State.Lock;
                    break;
                case SessionSwitchReason.SessionLogoff:
                    ea.newState = State.Logoff;
                    break;
                case SessionSwitchReason.SessionLogon:
                    ea.newState = State.Logon;
                    break;
                case SessionSwitchReason.SessionUnlock:
                    ea.newState = State.Unlock;
                    break;
                default:
                    ea.newState = State.Unknown;
                    break;
            }

            SessionEvent.Raise(this, ea);
        }
    }
}
