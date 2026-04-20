using System;
using System.IO;
using System.Xml.Serialization;

namespace CSGenio
{
    /// <summary>
    /// Allows the definition of which files should be redirected
    /// </summary>
    [XmlRoot("Redirect")]
    public class RedirectXML
    {

        [XmlElement("File")]
        public FileRedirect[] files = new FileRedirect[0];

        public static RedirectXML ReadRedirectFile(string filePath)
        {
            using (System.IO.StreamReader input = new System.IO.StreamReader(filePath))
            {
                XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(RedirectXML));
                return (RedirectXML)serializer.Deserialize(input);
            }           
        }
        
        
        /// <summary>
        /// Serializes the xml to a file
        /// </summary>
        /// <param name="xml">The object to be serialized</param>
        /// <param name="path">The path where the file will be saved</param>
        public static void WriteRedirectFile(RedirectXML xml, string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(RedirectXML));
                serializer.Serialize(writer, xml);
            }
        }
    }

    /// <summary>
    /// Represents a file to be redirected
    /// </summary>
    public class FileRedirect
    {
        /// <summary>
        /// Final destination path
        /// </summary>
        [XmlText]
        public string path = "";

        /// <summary>
        /// Name of the file to be redirected
        /// </summary>
        [XmlAttribute]
        public string file = "";

        /// <summary>
        /// If set the path should be interperted as relative to a given location
        /// </summary>
        [XmlAttribute]
        public bool relative = false;

        /// <summary>
        /// Gets the full path of this file. If the given path is relative, resolves the path into a full path.
        /// </summary>
        /// <param name="relativeTo">Indicates the origin path to solve into a full path. If empty uses the current base directory </param>
        /// <returns>Allways returns a full path</returns>
        public string GetFullPath(string relativeTo="")
        {
            if(relative)
            {
                if(String.IsNullOrEmpty(relativeTo))
                {
                    return GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
                }
                else
                {
                    string relativePath = path;
                    string fullPath = relativeTo.TrimEnd('\\');
                    while(relativePath.StartsWith("..\\"))
                    {
                        fullPath = Directory.GetParent(fullPath).FullName;
                        relativePath = relativePath.Remove(0, 3);
                    }
                    return Path.Combine(fullPath, relativePath);
                }
            }
            else
            {
                return path;
            }
        }

    }
}
