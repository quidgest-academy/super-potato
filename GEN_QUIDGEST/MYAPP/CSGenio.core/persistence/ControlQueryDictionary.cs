using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace CSGenio.persistence
{
    public class ControlQueryDictionary : IDictionary<string, PersistentSupport.ControlQueryDefinition>
    {
        private object lockDict = new object();
        private IDictionary<string, PersistentSupport.ControlQueryDefinition> m_dict;
        private Assembly m_assembly;

        public ControlQueryDictionary()
        {
            m_dict = new Dictionary<string, PersistentSupport.ControlQueryDefinition>();
            m_assembly = Assembly.GetExecutingAssembly();
        }

        public void Add(string key, PersistentSupport.ControlQueryDefinition value)
        {
            lock (lockDict)
            {
                m_dict.Add(key, value);
            }
        }

        public bool ContainsKey(string key)
        {
            lock (lockDict)
            {
                return m_dict.ContainsKey(key);
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return m_dict.Keys;
            }
        }

        public bool Remove(string key)
        {
            lock (lockDict)
            {
                return m_dict.Remove(key);
            }
        }

        public bool TryGetValue(string key, out PersistentSupport.ControlQueryDefinition value)
        {
            lock (lockDict)
            {
                return m_dict.TryGetValue(key, out value);
            }
        }

        public ICollection<PersistentSupport.ControlQueryDefinition> Values
        {
            get
            {
                lock (lockDict)
                {
                    return m_dict.Values;
                }
            }
        }

        public PersistentSupport.ControlQueryDefinition this[string key]
        {
            get
            {
                lock (lockDict)
                {
                    if (!m_dict.ContainsKey(key))
                    {
                        XmlSerializer s = new XmlSerializer(typeof(ControlQueryDefinitionSurrogate));
                        using (Stream data = m_assembly.GetManifestResourceStream("CSGenio.core.resources.ControlQuery." + key + ".xml"))
                        {
                            m_dict[key] = ((ControlQueryDefinitionSurrogate)s.Deserialize(data)).Object;
                        }
                    }
                    return m_dict[key];
                }
            }
            set
            {
                lock (lockDict)
                {
                    m_dict[key] = value;
                }
            }
        }

        public void Add(KeyValuePair<string, PersistentSupport.ControlQueryDefinition> item)
        {
            lock (lockDict)
            {
                m_dict.Add(item);
            }
        }

        public void Clear()
        {
            lock (lockDict)
            {
                m_dict.Clear();
            }
        }

        public bool Contains(KeyValuePair<string, PersistentSupport.ControlQueryDefinition> item)
        {
            lock (lockDict)
            {
                return m_dict.Contains(item);
            }
        }

        public void CopyTo(KeyValuePair<string, PersistentSupport.ControlQueryDefinition>[] array, int arrayIndex)
        {
            lock (lockDict)
            {
                m_dict.CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get
            {
                lock (lockDict)
                {
                    return m_dict.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                lock (lockDict)
                {
                    return m_dict.IsReadOnly;
                }
            }
        }

        public bool Remove(KeyValuePair<string, PersistentSupport.ControlQueryDefinition> item)
        {
            lock (lockDict)
            {
                return m_dict.Remove(item);
            }
        }

        public IEnumerator<KeyValuePair<string, PersistentSupport.ControlQueryDefinition>> GetEnumerator()
        {
            lock (lockDict)
            {
                return m_dict.GetEnumerator();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            lock (lockDict)
            {
                return ((System.Collections.IEnumerable)m_dict).GetEnumerator();
            }
        }
    }
}
