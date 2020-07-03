using System;
using System.Collections;
using System.Web;

namespace PIM.Common.LocalStorage
{
    public static class Local
    {
        public static ILocalData Data { get; } = new LocalData();

        private class LocalData : ILocalData
        {
            #region Fields
            [ThreadStatic]
            private static Hashtable _localData = new Hashtable();
            private static readonly object LocalDataHashtableKey = new object();
            #endregion
            #region Properties
            private static Hashtable LocalHashtable
            {
                get
                {
                    if (!RunningInWeb)
                    {
                        if (_localData == null)
                            _localData = new Hashtable();
                        return _localData;
                    }
                    else
                    {
                        var web_hashtable = HttpContext.Current.Items[LocalDataHashtableKey] as Hashtable;
                        if (web_hashtable == null)
                        {
                            web_hashtable = new Hashtable();
                            HttpContext.Current.Items[LocalDataHashtableKey] = web_hashtable;
                        }
                        return web_hashtable;
                    }
                }
            }
            public static bool RunningInWeb
            {
                get { return HttpContext.Current != null; }
            }

            public int Count
            {
                get { return _localData.Count; }
            }
            #endregion
            #region Indexer
            public object this[object key]
            {
                get { return LocalHashtable[key]; }
                set { LocalHashtable[key] = value; }
            }
            #endregion
        }
        public interface ILocalData
        {
            object this[object key] { get; set; }
            int Count { get; }
        }
    }
}
