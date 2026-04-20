using CSGenio.framework;
using System.Collections.Generic;

namespace CSGenio.business
{
    public abstract class ExtAppControl
    {
        /// <summary>
        /// Static object that contains all external apps controls
        /// </summary>
        protected static Dictionary<string, createExtAppControlObj> allExtAppControls;

        /// <summary>
        /// Called function to create a given external app control class implementation
        /// </summary>
        /// <param name="args">request arguments</param>
        /// <param name="utilizador">user object</param>
        /// <returns>The external control object will be initialized</returns>
        public delegate ExtAppControl createExtAppControlObj(string[] args, User user);

        /// <summary>
        /// Static constructor to add all external control implementations to an static object
        /// </summary>
        static ExtAppControl()
        {
            allExtAppControls = new Dictionary<string, createExtAppControlObj>();
        }

        /// <summary>
        /// Function to return the instance of a given control id (field name)
        /// </summary>
        /// <param name="id">control id</param>
        /// <param name="args">request arguments</param>
        /// <param name="utilizador">session user</param>
        /// <returns>the corresponding external grafics implementation instance</returns>
        public static ExtAppControl getExtAppControlObj(string id, string[] args, User user)
        {
            if (allExtAppControls.ContainsKey(id))
                return allExtAppControls[id](args, user);
            else
                throw new BusinessException(null, "ExtAppControl.getExtAppControlObj", "Control with id " + id + "not found.");
        }

        /// <summary>
        /// Function to process the request, usually there is a request type and a condition
        /// </summary>
        /// <returns>returns the resquested data</returns>
        public abstract object processRequest();
    }
}
