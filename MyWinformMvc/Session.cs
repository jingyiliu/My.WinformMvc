using System.Collections.Generic;
using My.Helpers;

namespace My.WinformMvc
{
    /// <summary>
    /// Stores application-wide data, eg., the user principal.
    /// </summary>
    public class Session
    {
        readonly object _syncObj = new object();
        readonly Dictionary<string, object> _dataItems = new Dictionary<string, object>();

        public IEnumerable<KeyValuePair<string, object>> DataItems 
        {
            get
            {
                lock (_syncObj) 
                    return _dataItems;
            }
        }

        public object this[string key]
        {
            get
            {
                Requires.NotNullOrWhiteSpace(key, "key"); 
                lock (_syncObj) 
                    return _dataItems[key];
            }
            set 
            { 
                Requires.NotNullOrWhiteSpace(key, "key"); 
                lock (_syncObj)
                    _dataItems[key] = value; 
            }
        }

        public void AddData(string key, object data)
        {
            Requires.NotNullOrWhiteSpace(key, "key"); 
            lock (_syncObj)
                _dataItems.Add(key, data);
        }

        public void RemoveData(string key)
        {
            Requires.NotNullOrWhiteSpace(key, "key"); 
            lock (_syncObj)
                _dataItems.Remove(key);
        }

        public void ClearData()
        {
            lock (_syncObj)
                _dataItems.Clear();
        }

        public bool TryGetData(string key, out object data)
        {
            Requires.NotNullOrWhiteSpace(key, "key"); 
            lock (_syncObj)
                return _dataItems.TryGetValue(key, out data);
        }

        public object GetData(string key)
        {
            Requires.NotNullOrWhiteSpace(key, "key"); 
            lock (_syncObj)
                return _dataItems[key];
        }
    }
}
