using System;
using System.ComponentModel;
using System.Windows.Threading;


namespace PresenceTracker
{
    public static class EventExtensions
    {
        public static void Raise(this EventHandler ev, object sender, EventArgs e)
        {
            var handler = ev;
            if (handler != null)
                handler(sender, e);
        }

        public static void Raise<T>(this EventHandler<T> ev, object sender, EventArgs e)
        {
            var handler = ev;
            if (handler != null)
                handler(sender, e);
        }

        // Extension method which marshals events back onto the main thread
        public static void RaiseMarshalled(this MulticastDelegate multicast, object sender, EventArgs args)
        {
            foreach (Delegate del in multicast.GetInvocationList())
            {
                // Try for WPF first
                DispatcherObject dispatcherTarget = del.Target as DispatcherObject;
                if (dispatcherTarget != null && !dispatcherTarget.Dispatcher.CheckAccess())
                {
                    // WPF target which requires marshaling
                    dispatcherTarget.Dispatcher.BeginInvoke(del, sender, args);
                }
                else
                {
                    // Maybe its WinForms?
                    ISynchronizeInvoke syncTarget = del.Target as ISynchronizeInvoke;
                    if (syncTarget != null && syncTarget.InvokeRequired)
                    {
                        // WinForms target which requires marshaling
                        syncTarget.BeginInvoke(del, new object[] { sender, args });
                    }
                    else
                    {
                        // Just do it.
                        del.DynamicInvoke(sender, args);
                    }
                }
            }
        }
        // Extension method which marshals actions back onto the main thread
        public static void RaiseMarshalled<T>(this Action<T> action, T args)
        {
            // Try for WPF first
            DispatcherObject dispatcherTarget = action.Target as DispatcherObject;
            if (dispatcherTarget != null && !dispatcherTarget.Dispatcher.CheckAccess())
            {
                // WPF target which requires marshaling
                dispatcherTarget.Dispatcher.BeginInvoke(action, args);
            }
            else
            {
                // Maybe its WinForms?
                ISynchronizeInvoke syncTarget = action.Target as ISynchronizeInvoke;
                if (syncTarget != null && syncTarget.InvokeRequired)
                {
                    // WinForms target which requires marshaling
                    syncTarget.BeginInvoke(action, new object[] { args });
                }
                else
                {
                    // Just do it.
                    action.DynamicInvoke(args);
                }
            }
        }
    }
}