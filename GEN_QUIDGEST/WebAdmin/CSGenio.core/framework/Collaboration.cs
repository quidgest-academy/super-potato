using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CSGenio.framework
{

    public class Collaboration
    {
        private static readonly string DIR_NAME = "colaboration";
        private static String GetFilePath(string location)
        {
            string dirPath = GetDirPath();
            string fileName = location + ".xml";  
            return Path.Combine(dirPath, fileName);
        }

        private static String GetDirPath()
        {
            //Since fields can change between versions, there must be a file with suggestions per version.
            int version = (int)Configuration.Version;
            string dirName = DIR_NAME + "_" + version;
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp", dirName);
        }

        public List<FieldSuggestion> FieldSuggestions = new List<FieldSuggestion>();
        public List<ArrayElementSuggestion> ArrayElementSuggestions = new List<ArrayElementSuggestion>();
        public List<OpenSuggestion> OpenSuggestions = new List<OpenSuggestion>();


        /// <summary>
        /// Serializes all suggestions and saves it in a xml file
        /// </summary>
        public void SaveXml(string location)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Collaboration));
                string dirName = GetDirPath();
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                string filePath = GetFilePath(location);
                FileMode fileMode = File.Exists(filePath) ? FileMode.Truncate : FileMode.Create;
                using (FileStream writer = File.Open(filePath, fileMode))
                {
                    serializer.Serialize(writer, this);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error writing collaboration file: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Deserializes all suggestions from a xml file
        /// </summary>
        public static Collaboration ReadXml(string location)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Collaboration));
                if (!File.Exists(GetFilePath(location)))
                {
                    return new Collaboration();
                }
                using (StreamReader reader = new StreamReader(GetFilePath(location)))
                {
                    return serializer.Deserialize(reader) as Collaboration;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error reading collaboration file: " + ex.Message);
                throw;
            }
            
        }

        public void Add(FieldSuggestion fieldSuggestion)
        {
            Add(fieldSuggestion, FieldSuggestions);
        }

        public void Add(ArrayElementSuggestion arraySuggestion)
        {
            Add(arraySuggestion, ArrayElementSuggestions);
        }

        public IEnumerable<Suggestion> AllSuggestions()
        {
            return ((IEnumerable<Suggestion>)FieldSuggestions)
                .Union(OpenSuggestions)
                .Union(ArrayElementSuggestions);
        }

        public void Add(OpenSuggestion openSuggestion)
        {
            Add(openSuggestion, OpenSuggestions);
        }

        private void Add<T>(T suggestion, List<T> suggestionList) where T : Suggestion
        {
            int index = suggestionList.FindIndex(x => x.IsSame(suggestion));
            if(index >= 0)
            {
                suggestionList[index] = suggestion;
            }
            else
            {
                suggestionList.Add(suggestion);
            }
        }
    }

    /// <summary>
    /// Represents a suggestion
    /// </summary>
    [XmlInclude(typeof(ArrayElementSuggestion))]
    [XmlInclude(typeof(FieldSuggestion))]
    public abstract class Suggestion
    {

        public string NewLabel;

        public string NewHelp;
        public string OldLabel;
        public string OldHelp;
        /// <summary>
        /// User that made the suggestion
        /// </summary>
        public string User;
        /// <summary>
        /// Language in use when the user made the suggestion
        /// </summary>
        public string Language;

        [XmlElement("Date")]
        public string DateString
        {
            get { return Date.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { Date = DateTime.Parse(value); }
        }

        [XmlIgnore]
        public DateTime Date;

        public abstract bool IsSame(Suggestion other);

        /// <summary>
        /// Location where the suggestion was made
        /// </summary>
        public String Location;

    }

    /// <summary>
    /// Suggestion for a field label or help
    /// </summary>
    public class FieldSuggestion : Suggestion
    {
        public string FieldId;

        public override bool IsSame(Suggestion other)
        {
            FieldSuggestion fieldSuggestion = other as FieldSuggestion;
            if(fieldSuggestion != null)
            {
                
                return this.User == other.User && this.Language == other.Language && this.FieldId == fieldSuggestion.FieldId;
            }
            return false;        

        }
    }

    /// <summary>
    /// Suggestion of array elements labels or helps
    /// </summary>
    public class ArrayElementSuggestion : Suggestion
    {
        public string ArrayName;
        public string ElementId;

        public override bool IsSame(Suggestion other)
        {
            ArrayElementSuggestion arraySuggestion = other as ArrayElementSuggestion;
            if (arraySuggestion != null)
            {

                return this.User == other.User && this.Language == other.Language
                    && this.ArrayName == arraySuggestion.ArrayName && this.ElementId == arraySuggestion.ElementId;
            }
            return false;

        }
    }

    public class OpenSuggestion : Suggestion
    {
        public string SuggestionText;

        public override bool IsSame(Suggestion other)
        {
            OpenSuggestion openSuggestion = other as OpenSuggestion;
            if (openSuggestion != null)
            {
                return this.User == other.User && this.Language == other.Language && this.Location == openSuggestion.Location;
            }
            return false;

        }
    }
}