using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace CSGenio.framework
{
    public static class UserMsg
    {
        public const int LANG_ENUS = 0;
        public const int NUM_LANGS = 1;

        private static Dictionary<int, string[]> m_messages;

        private static Dictionary<string, int> m_langix;
        private static Dictionary<string, int> m_msgix;

        static UserMsg()
        {
            //create string access dictionary
            m_langix = new Dictionary<string,int>();
            m_msgix = new Dictionary<string,int>();

            Type t = Type.GetType("CSGenio.framework.UserMsg");
            foreach(System.Reflection.FieldInfo fi in t.GetFields())
                if(fi.Name.StartsWith("LANG_"))
                    m_langix.Add(fi.Name.Substring(5).ToUpper(), (int)fi.GetValue(null));
                else if(fi.Name != "NUM_LANGS")
                    m_msgix.Add(fi.Name, (int)fi.GetValue(null));

            m_messages = new Dictionary<int, string[]>();
            //read from resource file
            //var res = System.Reflection.Assembly.GetAssembly(t).GetManifestResourceNames();
            //using (Stream res = System.Reflection.Assembly.GetAssembly(t).GetManifestResourceStream("GenioServer.Properties.UserMessages.xml"))
            using (Stream res = System.Reflection.Assembly.GetAssembly(t).GetManifestResourceStream("CSGenio.core.resources.UserMessages.xml"))
            {
                XmlReader xr = XmlReader.Create(res);
                while (xr.ReadToFollowing("msg"))
                {
                    string id = xr.GetAttribute("id").ToUpper();

                    string[] translations = new string[NUM_LANGS];
                    int index = -1;
                    if (m_msgix.TryGetValue(id, out index))
                    {
                        m_messages.Add(index, translations);

                        XmlReader sub = xr.ReadSubtree();
                        sub.ReadToFollowing("msg");
                        while (sub.Read())
                            if (sub.IsStartElement())
                            {
                                string lang = sub.Name.ToUpper();
                                string msg = sub.ReadElementContentAsString();

                                int lix = -1;
                                if (m_langix.TryGetValue(lang, out lix))
                                    translations[lix] = msg;
                            }
                        sub.Close();
                    }
                }
                xr.Close();
            }
        }

        public static string Get(int msg, int lang)
        {
            return m_messages[msg][lang];
        }

        public static string Get(string msg, string lang)
        {
            return m_messages[m_msgix[msg]][m_langix[lang]];
        }
        
        public static string Get(int msg, string lang)
        {
              string tmsg = null;
            try
            {
                tmsg = m_messages[msg][m_langix[lang]];
            }
            catch (Exception) //O idioma não está definido no Genio.
            {
                if (tmsg == null)
                    tmsg = m_messages[msg][m_langix["ENUS"]];
                if (tmsg == null)
                    return "";

                return tmsg;
            }
            return tmsg;
           // return m_messages[msg][m_langix[lang]];
        }

    }
}
