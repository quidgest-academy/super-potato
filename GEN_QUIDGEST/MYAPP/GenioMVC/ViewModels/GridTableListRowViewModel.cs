using System;
using System.Collections.Specialized;

using CSGenio.business;
using CSGenio.framework;
using GenioMVC.Models;
using GenioMVC.Models.Navigation;

namespace GenioMVC.ViewModels
{
	public abstract class GridTableListRowViewModel<T> : CrudViewModel<T> where T : ModelBase
	{
		protected GridTableListRowViewModel(UserContext userContext, string? identifier = null, bool nestedForm = false) : base(userContext, identifier, nestedForm) { }

		protected GridTableListRowViewModel(UserContext userContext, string identifier, T row, bool nestedForm = false) : base(userContext, identifier, row, nestedForm) { }

		public override void Load(NameValueCollection qs, bool editable, bool ajaxRequest = false, bool lazyLoad = false)
		{
			this.editable = editable;

			Model.Identifier = Identifier;
			InitModel(qs, lazyLoad);
			if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Edit || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
			{
				// MH - Voltar calcular as formulas to "atualizar" os Qvalues dos fields fixos
				// Conexão deve estar aberta de fora. Podem haver formulas que utilizam funções "manuais".
				MapToModel(Model);
				// Preencher operações internas
				Model.baseklass.fillInternalOperations(m_userContext.PersistentSupport, null);
				MapFromModel(Model);
			}
		}

		private void MapFromClientSide()
		{
			if (Model == null)
			{
				Model = CreateModelBase();
				MapToModel(Model);
			}
			else
			{
				// Model was created by New()
				// PK is set but all other fields are empty

				// Save PK to restore later
				string pk = Model.baseklass.QPrimaryKey;

				// Fill form fields
				MapToModel(Model);

				// Restore PK
				Model.baseklass.QPrimaryKey = pk;
			}
		}

		public override void Apply()
		{
			MapFromClientSide();

			StatusMessage result = new StatusMessage();
			result = EvaluateWriteConditions(isApply: false);

			if (result.Status == Status.E)
				throw new BusinessException(result.Message, "DbArea.alterar", "Error updating record: " + result.Message);
			else
				Model.Apply();
		}

		public override void Save()
		{
			MapFromClientSide();

			if (HasWriteConditions)
			{
				StatusMessage result = new StatusMessage();
				result = EvaluateWriteConditions(isApply: false);

				if (result.Status != Status.OK)
					this.flashMessage = result;
				if (result.Status == Status.E)
					throw new BusinessException(result.Message, "DbArea.alterar", "Error updating record: " + result.Message);
				else
					this.flashMessage = Model.Save();
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
		}

		// Loads all the information needed to present the form in insert mode
		public override void NewLoad()
		{
			editable = true;
			Model = CreateModelBase();
			Model.LoadKeysFromHistory(Navigation, Navigation.CurrentLevel.Level);

			this.LoadPartial(new NameValueCollection());

			// Fill default values
			try
			{
				Model.baseklass.fillValuesDefault(m_userContext.PersistentSupport, FunctionType.INS);
			}
			catch { /* Not all formulas can be calculated without the actual record being in the database. */ }

			LoadDefaultValues();

			// Fill internal operations
			try
			{
				Model.baseklass.fillInternalOperations(m_userContext.PersistentSupport, null);
			}
			catch { /* Not all formulas can be calculated without the actual record being in the database. */ }

			MapFromModel(Model);
		}

		public override void Duplicate(string id)
		{
			throw new NotSupportedException();
		}
	}
}
