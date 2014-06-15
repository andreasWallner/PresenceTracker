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
            _filename = filename;
        }

        public void append(object e)
        {
            using (StreamWriter sw = File.AppendText(_filename))
            {
                StateChanged sc = (StateChanged)e;
                sw.WriteLine(sc.serialize().ToString());
                sw.Flush();
            }
        }
    }
}
