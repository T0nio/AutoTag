using System;
using System.Collections.Generic;

namespace AutoTagLib.ErrorManager
{
    public class Lookup
    {
        private readonly Dictionary<Type, object> References = new Dictionary<Type, object>();

        private static Lookup _instance;
        
        public static Lookup GetInstance()
        {
            return _instance ?? (_instance = new Lookup());
        }

        public void Register(Type type, object instance)
        {
            References[type] = instance;
        }

        public void Unregister(Type type)
        {
            References[type] = null;
        }

        public object Get(Type type)
        {
            object ret;
            References.TryGetValue(type, out ret);
            return ret;
        }
    }
}