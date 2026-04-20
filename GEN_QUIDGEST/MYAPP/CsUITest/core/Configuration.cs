using System.IO;
using System.Text.Json;

namespace quidgest.uitests.core;

public class Configuration
{
    public string Browser { get; set; }
    public string BaseUrl { get; set; }
    public bool? Headless { get; set; }
    public int? ImplicitWait { get; set; }
    public int? ExplicitWait { get; set; }

    public int? WindowWidth { get; set; }
    public int? WindowHeight { get; set; }

    private static Configuration _instance;

    //should be private but this is just a quick way to allow the configuration to be deserialized
    public Configuration() {}

    public static Configuration Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new Configuration
                {
                    Browser = "chrome",
                    BaseUrl = "https://localhost:5173/",
                    Headless = true,
                    ImplicitWait = 100,
                    ExplicitWait = 1000,
                    WindowWidth = 1920,
                    WindowHeight = 1080
                };

                //config file overrides defaults
                if(File.Exists("SeleniumWebTest.json"))
                {
                    var settings = File.ReadAllText("SeleniumWebTest.json");
                    var f = JsonSerializer.Deserialize<Configuration>(settings);
                    if(f.Browser != null)
                        _instance.Browser = f.Browser;
                    if(f.BaseUrl != null)
                        _instance.BaseUrl = f.BaseUrl;
                    if(f.Headless != null)
                        _instance.Headless = f.Headless;
                    if(f.ImplicitWait != null)
                        _instance.ImplicitWait = f.ImplicitWait;
                    if(f.ExplicitWait != null)
                        _instance.ExplicitWait = f.ExplicitWait;
                    if (f.WindowWidth != null)
                        _instance.WindowWidth = f.WindowWidth;
                    if (f.WindowHeight != null)
                        _instance.WindowHeight = f.WindowHeight;
                }

                //environment variables override file
                var b = Environment.GetEnvironmentVariable("selenium.browser");
                if(b != null)
                    _instance.Browser = b;
                var u = Environment.GetEnvironmentVariable("selenium.baseurl");
                if(u != null)
                    _instance.BaseUrl = u;
                var h = Environment.GetEnvironmentVariable("selenium.headless");
                if(h != null)
                    _instance.Headless = Boolean.Parse(h);
                var w = Environment.GetEnvironmentVariable("selenium.implicitwait");
                if(w != null)
                    _instance.ImplicitWait = Int32.Parse(w);
                var ew = Environment.GetEnvironmentVariable("selenium.explicitwait");
                if(ew != null)
                    _instance.ExplicitWait = Int32.Parse(ew);
                var ww = Environment.GetEnvironmentVariable("selenium.windowwidth");
                if (ww != null)
                    _instance.WindowWidth = Int32.Parse(ww);
                var wh = Environment.GetEnvironmentVariable("selenium.windowheight");
                if (wh != null)
                    _instance.WindowHeight = Int32.Parse(wh);
            }
            return _instance;
        }
    }
}