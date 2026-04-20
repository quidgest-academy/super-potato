using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Administration.AuxClass
{
    [Serializable]
    public struct KeyValuePair<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
    }
}