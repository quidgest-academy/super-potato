using CSGenio.framework;
using System.Collections.Generic;

namespace CSGenio.business
{
	/// <summary>
	/// base class to implement any kind of external controls, for now, those are mostly flash controls
	/// </summary>
	public abstract class ExtControl
	{
        /// <summary>
        /// Static object that contains all external controls (mostly Flash controls)
        /// </summary>
        protected static Dictionary<string, createExtControlObj> allExtControls;
        
        /// <summary>
        /// Called function to create a given external control class implementation
        /// </summary>
        /// <param name="args">request arguments</param>
        /// <param name="utilizador">user object</param>
        /// <returns>The external control object will be initialized</returns>
        public delegate ExtControl createExtControlObj(string[] args,User user);

        /// <summary>
        /// Static constructor to add all external control implementations to an static object
        /// </summary>
        static ExtControl()
        {
            allExtControls = new Dictionary<string, createExtControlObj>();
        }

        /// <summary>
        /// Function to return the instance of a given control id (field name)
        /// </summary>
        /// <param name="id">control id</param>
        /// <param name="args">request arguments</param>
        /// <param name="utilizador">session user</param>
        /// <returns>the corresponding external grafics implementation instance</returns>
        public static ExtControl getExtControlObj(string id,string[] args,User user)
        {
            if (allExtControls.ContainsKey(id))
                return allExtControls[id](args,user);
            else
                throw new BusinessException(null, "ExtControl.getExtControlObj", "Control with id " + id + "not found.");
        }

        /// <summary>
        /// Function to process the request, usually there is a request type and a condition
        /// </summary>
        /// <returns>returns the resquested data</returns>
        public abstract object processRequest();
	}
}
