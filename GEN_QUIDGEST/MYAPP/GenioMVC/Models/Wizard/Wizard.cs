using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using GenioMVC.Models.Navigation;

namespace GenioMVC.Models
{
	/// <summary>
	/// Wizard navigation
	/// </summary>
	[Serializable]
	public class WizardNav : ISerializable
	{
		private IDictionary<string, HistoryLevel> nav;

		/// <summary>
		/// WizardNav constructor
		/// </summary>
		public WizardNav()
		{
			ResetNavigation();
		}

		/// <summary>
		/// Resets the navigation history
		/// </summary>
		public void ResetNavigation()
		{
			nav = new Dictionary<string, HistoryLevel>();
		}

		/// <summary>
		/// Stores the history for the specified step
		/// </summary>
		/// <param name="formName">The name of the main form</param>
		/// <param name="step">The current step</param>
		/// <param name="history">The current history</param>
		public void SetStepNavigation(string formName, WizardStep step, HistoryLevel history)
		{
			if (nav == null)
				ResetNavigation();

			string key = formName + "_" + step.WizardName + "_" + step.FormName;
			nav[key] = history.Clone();
		}

		/// <summary>
		/// Retrieves the history correponding to the specified step
		/// </summary>
		/// <param name="formName">The name of the main form</param>
		/// <param name="step">The current step</param>
		/// <returns>A clone of the step history, or null if it doesn't exist</returns>
		public HistoryLevel GetStepNavigation(string formName, WizardStep step)
		{
			if (nav == null)
				ResetNavigation();

			string key = formName + "_" + step.WizardName + "_" + step.FormName;

			if (nav.ContainsKey(key))
				return nav[key].Clone();
			return null;
		}

		#region Session State Serialization

		protected WizardNav(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new ArgumentNullException("info");

			try
			{
				var _value = info.GetString("NavCache");
				this.nav = Helpers.NavigationSerializer.Deserialize<Dictionary<string, HistoryLevel>>(_value);
			}
			catch (System.Exception e)
			{
				CSGenio.framework.Log.Error(string.Format("Error on serialize WizardNav. {0};{1}", e.Message, e.InnerException?.Message));
			}

			if (this.nav == null)
				this.nav = new Dictionary<string, HistoryLevel>();
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			try
			{
				var _value = Helpers.NavigationSerializer.Serialize(this.nav);
				info.AddValue("NavCache", _value);
			}
			catch (System.Exception e)
			{
				info.AddValue("NavCache", null);
				CSGenio.framework.Log.Error(string.Format("Error on serialize WizardNav. {0};{1}", e.Message, e.InnerException?.Message));
			}
		}

		#endregion
	}

	/// <summary>
	/// Class that represents a wizard step
	/// </summary>
	public class WizardStep
	{
		/// <summary>
		/// Empty wizard step constructor
		/// </summary>
		public WizardStep() : this("", "", "", -1) {}

		/// <summary>
		/// Wizard step constructor
		/// </summary>
		/// <param name="formName">The name of the associated form</param>
		/// <param name="wizardName">The name of the wizard to which it belongs</param>
		/// <param name="stepOrder">The order of the step</param>
		public WizardStep(string formName, string wizardName, int stepOrder) : this("wizard-step-" + wizardName + "-" + stepOrder, formName, wizardName, stepOrder) {}

		/// <summary>
		/// Wizard step constructor
		/// </summary>
		/// <param name="stepId">The step id (should be equal to the id in the html)</param>
		/// <param name="formName">The name of the associated form</param>
		/// <param name="wizardName">The name of the wizard to which it belongs</param>
		/// <param name="stepOrder">The order of the step</param>
		public WizardStep(string stepId, string formName, string wizardName, int stepOrder)
		{
			StepId = stepId;
			FormName = formName;
			WizardName = wizardName;
			StepOrder = stepOrder;
		}

		/// <summary>
		/// The html id of the step
		/// </summary>
		public string StepId { get; set; }

		/// <summary>
		/// The name of the associated form
		/// </summary>
		public string FormName { get; set; }

		/// <summary>
		/// The name of the wizard to which it belongs
		/// </summary>
		public string WizardName { get; set; }

		/// <summary>
		/// The order of the step
		/// </summary>
		public int StepOrder { get; set; }
	}
}
