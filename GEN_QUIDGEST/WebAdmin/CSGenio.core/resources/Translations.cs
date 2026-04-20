using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;

namespace CSGenio.framework
{
    public abstract class Translations
    {
        public const int LANG_ENUS = 0;
        public const int NUM_LANGS = 1;
		public const string BASE_LANG = "ENUS";
		public const string DEFAULT_BASE_LANG = "ENUS";

        private static Dictionary<string, string[]> m_messages;
		private static Dictionary<string, string[]> m_messagesCode;

        private static Dictionary<string, int> m_langix;

        static Translations()
        {
            //create string access dictionary
            m_langix = new Dictionary<string, int>();

            Type t = Type.GetType("CSGenio.framework.Translations");
            foreach (var fi in t.GetFields().Where(fi => fi.Name.StartsWith("LANG_")))
                m_langix.Add(fi.Name.Substring(5), (int)fi.GetValue(null));

            m_messages = new Dictionary<string, string[]>();
			m_messagesCode = new Dictionary<string, string[]>();
			
            //read from resource file
            using (Stream res = System.Reflection.Assembly.GetAssembly(t).GetManifestResourceStream("CSGenio.core.resources.Translations.xml"))
            {
                XmlReader xr = XmlReader.Create(res);
                while (xr.ReadToFollowing("msg"))
                {
                    string id = xr.GetAttribute("id");
					string code = xr.GetAttribute("code");

                    string[] translations = new string[NUM_LANGS];
                    m_messages.Add(id, translations);
					m_messagesCode.Add(code, translations);

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
                xr.Close();
            }
        }

        private static string getText(string msg, string lang, Dictionary<string, string[]> messages)
        {
            if (String.IsNullOrEmpty(msg) || String.IsNullOrEmpty(lang))
            {
                return msg;
            }
            try
            {
                if (m_langix.ContainsKey(lang))
                {
                    string key = msg;
                    if (messages.ContainsKey(key) && messages[key].Length > m_langix[lang])
                    {
                        string tmsg = messages[key][m_langix[lang]];
                        if (tmsg == null)
                            tmsg = messages[key][m_langix[DEFAULT_BASE_LANG]];
                        if (tmsg == null)
                            return msg;
                        return tmsg;
                    }
                }
                else // This might happen because there's currently no distinction between "PTPT" and "PPPP", they both translate to pt-PT.
                {
                    string key = msg;
                    if (messages.ContainsKey(key))
                    {
                        string tmsg = messages[key][m_langix[BASE_LANG]]; // Try to use the client's default language
                        if (tmsg == null)
                            tmsg = messages[key][m_langix[DEFAULT_BASE_LANG]]; // Try to use Genio's default language
                        if (tmsg == null)
                            return msg;
                        return tmsg;
                    }
                }

                return msg;
            }
            catch (Exception)
            {
                return msg;
            }
        }

		public static string Get(string msg, string lang = null)
        {
			return getText(msg, lang ?? BASE_LANG, m_messages);
		}

		public static string GetByCode(string code, string lang = null)
        {
			return getText(code, lang ?? BASE_LANG, m_messagesCode);
		}
    }
}
