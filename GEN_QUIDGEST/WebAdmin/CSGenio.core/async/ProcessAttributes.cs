using System;

namespace CSGenio.business.async
{
    /// <summary>
    /// This attribute indicates that the property/field is will be stored in the DB when the process is scheduled.
    /// </summary>
    public class GenioProcessArgument : Attribute
    {
        public GenioProcessArgument()
        {
            Hide = false;
            Docum = false;
        }
        /// <summary>
        /// Name of the argument shown to the user.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Database field to show to the user. Only works when the argument is the pirmary key of [area]
        /// Syntax: "[area].[campo]".
        /// </summary>
        public String Field { get; set; }

        /// <summary>
        /// Name of the array that translates the value to the user. If the field is also defined, it uses the given field as value.
        /// </summary>
        public String Array { get; set; }

        /// <summary>
        /// If true, the argument is hidden from the user
        /// </summary>
        /// 
        public bool Hide { get; set; }

        /// <summary>
        /// If true, the argument represents a document in the database
        /// </summary>
        /// 
        public bool Docum { get; set; }
        /// <summary>
        /// If this argument is a key, this filed indicates the key name in the database
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Name of the function defined in the type to fill the argument description.
        /// The function must have the follow signature : 
        /// public static string foo(PersistentSupport,User,String)
        /// If @Array or @Field are defined, the resulting value is passed as argument.
        /// </summary>
        public string Function { get; set; }
    }

    /// <summary>
    /// Attribute to idenfity a process as a Job. 
    /// Processes marked with this attribute will appear in the process types array
    /// </summary>
    public class GenioProcessType : Attribute
    {
        public string Id { get; set; }

        public string Resource { get; set; }

        public GenioProcessType(string id, string resource)
        {
            this.Id = id;
            this.Resource = resource;
        }
    }

    public class GenioProcessMode : Attribute
    {
        public string id;

        public GenioProcessMode(string id)
        {
            this.id = id;
        }
    }
}
