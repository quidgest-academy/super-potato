using System.Xml;

namespace GenioMVC;


public class WebConfigConfigurationSource : IConfigurationSource
{
    private readonly string _filename;
    public WebConfigConfigurationSource(string filename = "web.config") {
        _filename = filename;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new WebConfigConfigurationProvider(_filename);
    }
}


public class WebConfigConfigurationProvider : ConfigurationProvider
{
    private readonly string _filename;

    public WebConfigConfigurationProvider(string filename)
    {
        _filename = filename;
    }

    public override void Load()
    {
        if (!File.Exists(_filename)) return;

        XmlDocument _webConfig = new XmlDocument();
        _webConfig.Load(_filename);

        var root = _webConfig["configuration"];
        if (root != null)
        {
            var system_web = root["system.web"];
            if (system_web != null)
            {
                var ss = system_web["sessionState"];
                if (ss != null)
                {
                    MapAttribute(ss, "cookieName", "SessionOptions:Cookie:Name");
                    MapAttribute(ss, "timeout", "SessionOptions:Cookie:Timeout");
                }

                var authforms = system_web["authentication"]?["forms"];
                if(authforms != null)
                {
                    MapAttribute(authforms, "name", "LegacyFormsAuthentication:CookieName");
                }

                var machine = system_web["machineKey"];
                if (machine != null)
                {
                    MapAttribute(machine, "decryptionKey", "LegacyFormsAuthentication:DecryptionKey");
                    MapAttribute(machine, "validationKey", "LegacyFormsAuthentication:ValidationKey");
                }
            }
        }
    }

    private void MapAttribute(XmlElement element, string attribute, string key)
    {
        var value = element.Attributes[attribute]?.Value;
        if (value != null) 
            Data.Add(key, value);
    }
}
