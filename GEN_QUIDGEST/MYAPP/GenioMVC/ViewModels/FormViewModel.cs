using System.Collections.Specialized;

using CSGenio.business;
using CSGenio.framework;
using GenioMVC.Models;
using GenioMVC.Models.Navigation;

namespace GenioMVC.ViewModels
{
	public abstract class FormViewModel<T> : CrudViewModel<T> where T : ModelBase
	{
		protected FormViewModel(UserContext userContext, string? identifier = null, bool nestedForm = false) : base(userContext, identifier, nestedForm) { }

		protected FormViewModel(UserContext userContext, string identifier, T row, bool nestedForm = false) : base(userContext, identifier, row, nestedForm) { }

		public override StatusMessage CheckPermissions(FormMode mode)
		{
			/*
				The validation of permissions does not require a Model with data.
					It is used solely for accessing the Role validation of CSGenioA.
				The MapToModel, which performed a hidden mapping, was removed from here,
					and the rest of the code that took advantage of this mapping now handles it explicitly.
					Thus, CheckPermissions only performs permission validation,
					and the rest of the code, whenever it needs to map data, does so explicitly.
			 */
			var baseModel = Model;
			if (baseModel == null)
			{
				baseModel = CreateModelBase();
				baseModel.Identifier = Identifier;
			}
			return base.CheckPermissions(baseModel, mode);
		}

		public override void Apply()
		{
			/*
				The loading of the Model (LoadModel), the mapping of the Model to ViewModel (MapToModel),
				 and the execution of internal formulas (ExecuteModelFormulas) must always be done before invoking Apply.
				This method no longer does this, so we can perform these types of actions before CRUD validations on the GenericHandler,
				 thereby ensuring that the data are correct and valid.
				In addition to making the values of the calculated fields valid, invoking the recalculation of the formulas after mapping
					also allows for protecting the fields that could not be filled due to the Fill When condition, but came in the ViewModel with a value.
				This also reduces the duplicate execution of record positioning, recalculation of internal formulas
					and ensures that the validation of write conditions is performed on valid data.
			*/

			var result = EvaluateWriteConditions(isApply: true);
			if (result.Status != Status.OK)
				this.flashMessage = result;
			if (result.Status == Status.E)
				throw new FieldValidationException(result, "apply");

			Model.Apply();
		}

		public override void Save()
		{
			/*
				The loading of the Model (LoadModel), the mapping of the Model to ViewModel (MapToModel),
				 and the execution of internal formulas (ExecuteModelFormulas) must always be done before invoking Save.
				This method no longer does this, so we can perform these types of actions before CRUD validations on the GenericHandler,
				 thereby ensuring that the data are correct and valid.
				In addition to making the values of the calculated fields valid, invoking the recalculation of the formulas after mapping
					also allows for protecting the fields that could not be filled due to the Fill When condition, but came in the ViewModel with a value.
				This also reduces the duplicate execution of record positioning, recalculation of internal formulas
					and ensures that the validation of write conditions is performed on valid data.
			*/

			// Write conditions
			if (HasWriteConditions)
			{
				StatusMessage result, formResult = new StatusMessage(), tblResult = new StatusMessage();
				result = EvaluateWriteConditions(isApply: false); // Comes from form conditions
				formResult.Clone(result);

				if (result.Status == Status.E)
					throw new FieldValidationException(result, string.Format("{0}.Save", Identifier));

				result = result.MergeStatusMessage(Model.Save()); // Comes from table conditions
				tblResult.Clone(result);

				// In case both tbl and form have conditions, show the form only
				if (formResult.Status == Status.OK && tblResult.Status == Status.OK)
				{
					if (!string.IsNullOrEmpty(formResult.Message))
					{
						this.flashMessage = formResult;
						return;
					}
				}

				this.flashMessage = result;
			}
			else
				this.flashMessage = Model.Save();
		}

		// Creates the pseudo-new record in the database (zzstate=1)
		public override void New()
		{
			editable = true;
			Model = CreateModelBase();
			Model.LoadKeysFromHistory(Navigation, Navigation.CurrentLevel.Level);
			Model.New(Identifier);
			// Voltar preencher as chaves a partir do Historial, caso se as replicas preencherem a null
			Model.LoadKeysFromHistory(Navigation, Navigation.CurrentLevel.Level);
			MapFromModel(Model);
		}

		// Loads all the information needed to present the form in insert mode
		public override void NewLoad()
		{
			this.LoadPartial(new NameValueCollection());

			// Fill in the event fields inserted by the calendar control
			LoadCalendarValues();
			// After the interface contextual fill, we give a last chance for the row to update internal formulas
			MapToModel(Model);
			// Fill in default values
			Model.baseklass.fillValuesDefault(m_userContext.PersistentSupport, FunctionType.INS);
			LoadDefaultValues();
			// Preencher operações internas
			Model.baseklass.fillInternalOperations(m_userContext.PersistentSupport, null);
			MapFromModel(Model);
		}

		public override void Duplicate(string id)
		{
			this.editable = true;
			Model = CreateModelBase();
			Model.Identifier = Identifier;
			Model.Duplicate(id);
			Model.LoadKeysFromHistory(this.Navigation, this.Navigation.CurrentLevel.Level);
			LoadDefaultValues();
			MapFromModel(Model);
			this.LoadPartial(new NameValueCollection());
		}

		// Fill in the event fields inserted by the calendar control
		private void LoadCalendarValues()
		{
			try
			{
				var json = Navigation.GetStrValue("CalendarOptions", true);
				if (!string.IsNullOrWhiteSpace(json))
				{
					var options = System.Text.Json.JsonSerializer.Deserialize<CalendarVariables>(json);
					if (options != null && options.HasCalendarFields)
					{
						if (!string.IsNullOrWhiteSpace(options.startDateField))
							SetViewModelValue(options.startDateField.ToLower(), options.DateStart);
						if (!string.IsNullOrWhiteSpace(options.endDateField))
							SetViewModelValue(options.endDateField.ToLower(), options.DateEnd);
						if (!string.IsNullOrWhiteSpace(options.allDayField))
							SetViewModelValue(options.allDayField.ToLower(), options.allDay);

						// Start and Ending Times
						// http://jenkinsvm/geniodoc/patterns/interface/custom-controls/fullcalendar/extra-options/nodates/nodates-starting-time
						if (!string.IsNullOrWhiteSpace(options.startTimeField))
							SetViewModelValue(options.startTimeField.ToLower(), options.minTime);
						if (!string.IsNullOrWhiteSpace(options.endTimeField))
							SetViewModelValue(options.endTimeField.ToLower(), options.maxTime);
						if (!string.IsNullOrWhiteSpace(options.selectedDateField))
							SetViewModelValue(options.selectedDateField.ToLower(), options.selectedDate);
					}
					// Remove the history entry after it has already been used.
					Navigation.ClearValue("CalendarOptions", true);
				}
			}
			catch (System.Exception e)
			{
				Log.Error("LoadCalendarValues: " + e.Message);
			}
		}
	}
}
