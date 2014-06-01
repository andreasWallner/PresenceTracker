using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresenceTracker
{
    class DataAppender
    {
        private string _filename;

        public DataAppender(INotifyCollectionChanged sender, string filename)
        {
            sender.CollectionChanged += sender_CollectionChanged;
            _filename = filename;
        }

        void sender_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems.Count != 0)
            {
                using (StreamWriter sw = File.AppendText(_filename))
                {
                    foreach (var item in e.NewItems)
                    {
                        StateChanged sc = (StateChanged)item;
                        sw.WriteLine(sc.serialize().ToString());
                        sw.Flush();
                    }
                }
            }
        }
    }
}
