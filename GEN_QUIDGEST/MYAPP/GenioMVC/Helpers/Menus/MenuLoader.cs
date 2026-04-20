using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace GenioMVC.Helpers.Menus
{

    /// <summary>
    /// Provides access to the application's menu structure.
    /// </summary>
    public interface IMenuLoader
    {
        /// <summary>
        /// Returns all top-level menu entries, including their nested children.
        /// </summary>
        List<MenuEntry> GetAllMenus();
    }

    /// <summary>
    /// Loads and caches menu entries from the menus.xml file located in the application's base directory.
    /// </summary>
    public class XmlMenuLoaderService : IMenuLoader
    {
        private readonly Lazy<List<MenuEntry>> allMenus = new(() =>
            LoadMenuXml(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "menus.xml")));

        /// <inheritdoc />
        public List<MenuEntry> GetAllMenus() => allMenus.Value;

        /// <summary>
        /// Deserializes a list of <see cref="MenuEntry"/> from the specified XML file.
        /// </summary>
        /// <param name="fileLocation">Absolute path to the menus XML file.</param>
        public static List<MenuEntry> LoadMenuXml(string fileLocation)
        {
            XmlSerializer s = new XmlSerializer(typeof(List<MenuEntry>));
            using (StreamReader r = new StreamReader(fileLocation, Encoding.UTF8))
            {
                return (List<MenuEntry>)s.Deserialize(r);
            }
        }
    }
}