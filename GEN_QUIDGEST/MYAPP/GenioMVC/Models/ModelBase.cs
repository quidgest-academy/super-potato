using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;
using System.Reflection;

using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Models.Navigation;
using GenioMVC.ViewModels;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

namespace GenioMVC.Models
{
	public class ModelBase(UserContext userContext) : IConditionalSerializer
	{
		protected UserContext m_userContext = userContext;

		/// <summary>
		/// The current user navigation
		/// </summary>
		protected NavigationContext Navigation => m_userContext?.CurrentNavigation;

		/// <summary>
		/// List of field values filled by history.
		/// During the insertion of a new record, defaults or replicas may remove the value of keys filled by history.
		/// This list is used to ensure that the values of these fields are maintained during the creation of the pseudo-new record.
		/// </summary>
		protected Dictionary<string, string> m_filledByHistory = [];

		/// <summary>
		/// Whether to serialize the field
		/// </summary>
		/// <param name="tag">The field</param>
		/// <returns>True if it should be serialized, false otherwise</returns>
		public bool ShouldSerialize(string tag)
		{
			return SerializeAllFields || FieldsToSerialize.Contains(tag);
		}

		/// <summary>
		/// List of fields to be serialized. If it is null, serialize all.
		/// The property exists only for compatibility with constructors and functions such as Find and Search.
		/// Due to the large amount of Reflection accessing the methods, it would be too risky to change everything to HashSet already.
		/// </summary>
		protected string[] _fieldsToSerialize;

		/// <summary>
		/// List of fields to be serialized (ShouldSerialize[...]). If it is null, serialize all.
		/// The «.Contains» in the HashSet is faster than in the Array.
		/// </summary>
		protected HashSet<string> FieldsToSerialize;
		protected bool SerializeAllFields = true;

		[JsonIgnore]
		public DbArea baseklass;

		[JsonIgnore]
		public string Identifier { get; set; }

		[JsonIgnore]
		public bool isEmptyModel { get; protected set; }

		/// <summary>
		/// Define the list of fields to be serialized
		/// </summary>
		/// <param name="fieldsToSerialize">The list of fields to be serialized</param>
		public void SetFieldsToSerialize(string[] fieldsToSerialize)
		{
			// The «_fieldsToSerialize» property exists only for compatibility with constructors and functions such as Find and Search.
			// Due to the large amount of Reflection accessing the methods, it would be too risky to change everything to HashSet already.
			_fieldsToSerialize = fieldsToSerialize;
			SerializeAllFields = fieldsToSerialize == null;
			FieldsToSerialize = SerializeAllFields ? null : new HashSet<string>(fieldsToSerialize);
		}

		public void SetIsEmptyModel(bool isEmptyModel)
		{
			this.isEmptyModel = isEmptyModel;
		}

		public T SetIsEmptyModel<T>(bool isEmptyModel) where T : ModelBase
		{
			SetIsEmptyModel(isEmptyModel);
			return (T)this;
		}

		virtual public void New(string identifier = null, PersistentSupport persistentSupport = null)
		{
			var u = m_userContext.User;
			var sp = persistentSupport ?? m_userContext.PersistentSupport;

			try
			{
				this.baseklass.fillEPH(u, sp, identifier);
				this.baseklass.insertPseud(sp, [.. m_filledByHistory.Keys], [.. m_filledByHistory.Values]);
			}
			finally
			{
				this.Identifier = identifier;
			}
		}

		public StatusMessage Save()
		{
			return this.Save(m_userContext.PersistentSupport);
		}

		public StatusMessage Save(PersistentSupport sp)
		{
			// Save self
			this.baseklass.removeCalculatedFields();
			this.baseklass.RemovePasswordFields(true);
			StatusMessage Qresult = this.baseklass.change(sp, (CriteriaSet)null);

			return Qresult;
		}

		public void Apply()
		{
			// Save self
			PersistentSupport sp = m_userContext.PersistentSupport;
			this.baseklass.removeCalculatedFields();
			this.baseklass.RemovePasswordFields(true);
			//navigation direction from wizard forward/back
			bool isGoingBack = Convert.ToBoolean(m_userContext.CurrentNavigation.GetValue("clearData"));
			//force all the missing current field values to empty
			if (isGoingBack)
				foreach (var dbfield in this.baseklass.DBFields)
					if (!this.baseklass.Fields.ContainsKey(dbfield.Value.FullName))
						this.baseklass.insertNameValueField(dbfield.Value.FullName, null);
			this.baseklass.apply(sp);
		}

		public StatusMessage Destroy()
		{
			// Destroy Self
			PersistentSupport sp = m_userContext.PersistentSupport;
			StatusMessage Qresult = this.baseklass.eliminate(sp);

			return Qresult;
		}

		public void Duplicate(string id)
		{
			// Duplicate
			PersistentSupport sp = m_userContext.PersistentSupport;
			this.baseklass.duplicate(sp, CriteriaSet.And().Equal(this.baseklass.PrimaryKeyName, id));
		}

		public static CriteriaSet AddEPH<A>(ref User user, CriteriaSet args, string identifier = null) where A : CSGenio.business.Area
		{
			var area = CSGenio.business.Area.createArea<A>(user, user.CurrentModule);
			return AddEPH(area, ref user, args, identifier);
		}

		public static CriteriaSet AddEPH(string areaName, ref User user, CriteriaSet args, string identifier = null)
		{
			var area = CSGenio.business.Area.createArea(areaName, user, user.CurrentModule);
			return AddEPH(area, ref user, args, identifier);
		}

		public static CriteriaSet AddEPH(CSGenio.business.Area area, ref User user, CriteriaSet args, string identifier = null)
		{
			CriteriaSet condEph = Listing.CalculateConditionsEphGeneric(area, identifier);

			if (condEph != null && (condEph.Criterias.Count > 0 || condEph.SubSets.Count > 0))
			{
				if (args == null)
					args = CriteriaSet.And();

				//garantir que não exists já um critério igual ao que vamos adicionar
				foreach (Criteria q in condEph.Criterias)
				{
					ColumnReference column = (ColumnReference)q.LeftTerm;
					Criteria criteria = args.FindCriteria(column.TableAlias, column.ColumnName, q.Operation, CriteriaSet.FindVariable.Any);
					if (criteria != null)
						condEph.Criterias.Remove(criteria);
				}

				args.SubSet(condEph);
			}

			return args;
		}

		/// <summary>
		/// Loads EPH fields into the model
		/// </summary>
		public void LoadEPH(string identifier = null)
		{
			if (baseklass == null)
				return;

			User u = m_userContext.User;
			PersistentSupport sp = m_userContext.PersistentSupport;
			baseklass.fillEPH(u, sp, identifier);
		}

		/// <summary>
		/// Gathers all the available info for a given file (versions, size, author, etc.)
		/// </summary>
		/// <param name="field">The field name that holds the docum name</param>
		/// <returns>A view model that holds all the info of a file</returns>
		public DocumsProperties_ViewModel GetInfoDoc(string field)
		{
			PersistentSupport sp = m_userContext.PersistentSupport;

			field = field[..3].ToLower() == "val" ? field[3..].ToLower() : field.ToLower();
			string documid = baseklass?.returnValueField(baseklass.Alias + "." + field + "fk") as string;

			DBFile info;
			if (string.IsNullOrEmpty(documid))
				info = DBFile.EmptyFile();
			else
				info = baseklass.infoDocum(sp, field);

			return new DocumsProperties_ViewModel(m_userContext, info);
		}

		/// <summary>
		/// Saves a document
		/// </summary>
		/// <param name="field">The field</param>
		/// <returns>True if successful</returns>
		public bool CheckoutVersion(string field)
		{
			PersistentSupport sp = m_userContext.PersistentSupport;

			try
			{
				field = field[..3].ToLower() == "val" ? field[3..].ToLower() : field.ToLower();

				sp.openTransaction();
				bool result = baseklass.checkoutDocums(sp, field, out string newcodDocums);
				baseklass.updateDirect(sp);
				sp.closeTransaction();
				return result;
			}
			catch (System.Exception)
			{
				sp.rollbackTransaction();
				return false;
			}
		}

		/// <summary>
		/// Rerturns the last version of a file
		/// </summary>
		/// <param name="field">The field name that holds the docum name</param>
		/// <returns>DBFile</returns>
		public DBFile FindDocument(string field)
		{
			PersistentSupport sp = m_userContext.PersistentSupport;
			field = field[..3].ToLower() == "val" ? field[3..].ToLower() : field.ToLower();
			return baseklass.returnLastVersionFileDocum(sp, field);
		}

		/// <summary>
		/// Saves a document
		/// </summary>
		/// <param name="field">The field</param>
		/// <param name="file">The file</param>
		/// <param name="fileName">The file name</param>
		/// <param name="coddocums">The document key</param>
		/// <param name="mode">The mode</param>
		/// <param name="version">The version</param>
		/// <returns>True if successful, false otherwise</returns>
		public bool SubmitVersion(string field, byte[] file, string fileName, string coddocums, string mode, string version)
		{
			PersistentSupport sp = m_userContext.PersistentSupport;

			try
			{
				field = field[..3].ToLower() == "val" ? field[3..].ToLower() : field.ToLower();

				sp.openTransaction();
				baseklass.submitDocum(sp, field, file, fileName + "_" + coddocums, mode, version);
				baseklass.updateDirect(sp);
				sp.closeTransaction();
				return true;
			}
			catch (System.Exception)
			{
				sp.rollbackTransaction();
				return false;
			}
		}

		/// <summary>
		/// Saves a document
		/// </summary>
		/// <param name="field">The field</param>
		/// <param name="file">The file</param>
		/// <returns>True if successful</returns>
		public bool SaveDocument(string field, DBFile file)
		{
			PersistentSupport sp = m_userContext.PersistentSupport;
			field = field[..3].ToLower() == "val" ? field[3..].ToLower() : field.ToLower();

			List<KeyValuePair<string, CSGenio.framework.Field>> fields = DbArea.GetInfoArea(baseklass.Alias).DBFields.Where(x => x.Value.FieldType.Equals(FieldType.DOCUMENT)).ToList();

			if (fields.Exists(x => x.Key.ToLower() == field) && file != null)
			{
				try
				{
					sp.openTransaction();

					if (!string.IsNullOrEmpty(baseklass.returnValueField(baseklass.Alias + "." + field + "fk").ToString()))
						baseklass.removeDocums(sp, field);
					baseklass.insertNameValueFileDB(field, file.File, file.Name + "_", "", sp, file.Version, null);
					baseklass.updateDirect(sp);

					sp.closeTransaction();
					return true;
				}
				catch (System.Exception ex)
				{
					sp.rollbackTransaction();
					throw new BusinessException("Não foi possível gravar o documento.", "ModelBase.SaveDocument", "Error saving document: " + ex, ex);
				}
			}

			return false;
		}

		/// <summary>
		/// Deletes the version history of a file
		/// </summary>
		/// <param name="field">The field name that holds the docum name</param>
		/// <param name="currentVersion">The current version that should be kept</param>
		/// <returns>True if successful, false otherwise</returns>
		public bool DeleteHistoricVersions(string field, string currentVersion = null)
		{
			PersistentSupport sp = m_userContext.PersistentSupport;

			try
			{
				sp.openConnection();
				field = field[..3].ToLower() == "val" ? field[3..].ToLower() : field.ToLower();
				string version = string.IsNullOrWhiteSpace(currentVersion) ? null : currentVersion;
				baseklass.deleteHistoryDocums(sp, field, version);
				sp.closeConnection();

				return true;
			}
			catch (System.Exception)
			{
				sp.closeConnection();
				return false;
			}
		}

		/// <summary>
		/// Deletes the last version of a file
		/// </summary>
		/// <param name="field">The field name that holds the docum name</param>
		/// <returns>True if successful, false otherwise</returns>
		public bool DeleteLastVersion(string field)
		{
			PersistentSupport sp = m_userContext.PersistentSupport;

			try
			{
				sp.openConnection();
				field = field[..3].ToLower() == "val" ? field[3..].ToLower() : field.ToLower();
				bool result = baseklass.deleteLastDocums(sp, field);
				sp.closeConnection();

				return result;
			}
			catch (System.Exception)
			{
				sp.closeConnection();
				return false;
			}
		}

		/// <summary>
		/// Deletes a document
		/// </summary>
		/// <param name="field">The field name that holds the docum name</param>
		/// <returns>True if successful</returns>
		public bool DeleteDocument(string field)
		{
			PersistentSupport sp = m_userContext.PersistentSupport;
			field = field[..3].ToLower() == "val" ? field[3..].ToLower() : field.ToLower();
			bool varOk = false;

			if (!string.IsNullOrEmpty(baseklass.returnValueField(baseklass.Alias + "." + field + "fk").ToString()))
			{
				// [RC] 06/06/2017 We must catch exceptions here, so we can rollback the transaction
				try
				{
					sp.openTransaction();

					if (baseklass.removeDocums(sp, field))
					{
						baseklass.updateDirect(sp);
						varOk = true;
					}

					sp.closeTransaction();
				}
				catch (System.Exception ex)
				{
					sp.rollbackTransaction();
					throw new BusinessException("Não foi possível apagar o documento.", "ModelBase.DeleteDocument", "Error deleting document: " + ex, ex);
				}
			}

			return varOk;
		}

		/// <summary>
		/// Sets all foreign keys for the nextLevel.
		/// Iterate and fill all foreign keys of this table, with history values.
		/// </summary>
		/// <param name="navigation">Navigation Context</param>
		/// <param name="level">History Level</param>
		/// <param name="changeHistory">Permitir change o Historial (no reload dos dbedits e dependentes é false)</param>
		/// <param name="allowNull">Permitir override do Qvalue na ficha com Null do Historial</param>
		/// <param name="allowOverrideComputed">Permite override do valor da ficha dos campos com formulas. Só deve ser usado nos casos como Reload do DBEdit content, para aplicar novo limite e não ter enviar e calcular a ficha inteira</param>
		public void LoadKeysFromHistory(NavigationContext navigation, int level, bool changeHistory = true, bool allowNull = false, bool allowOverrideComputed = false)
		{
			var allowOverrideMode = new object[] { FormMode.New, FormMode.Edit, FormMode.Duplicate };
			var allowOverride = allowOverrideMode.Contains(navigation.CurrentLevel.FormMode);
			LoadKeysFromHistory(navigation, level, changeHistory, allowNull, allowOverride, allowOverrideComputed);
		}

		/// <summary>
		/// Sets all foreign keys for the nextLevel.
		/// Iterate and fill all foreign keys of this table, with history values.
		/// </summary>
		/// <param name="navigation">Navigation Context</param>
		/// <param name="level">History Level</param>
		/// <param name="changeHistory">Permitir change o Historial (no reload dos dbedits e dependentes é false)</param>
		/// <param name="allowNull">Permitir override do Qvalue na ficha com Null do Historial</param>
		/// <param name="allowOverride">Permitir override do Qvalue que vem da BD com o Qvalue do Historial</param>
		/// <param name="allowOverrideComputed">Permite override do valor da ficha dos campos com formulas. Só deve ser usado nos casos como Reload do DBEdit content, para aplicar novo limite e não ter enviar e calcular a ficha inteira</param>
		public void LoadKeysFromHistory(NavigationContext navigation, int level, bool changeHistory, bool allowNull, bool allowOverride, bool allowOverrideComputed)
		{
			if (baseklass.ParentTables == null) // Caso da table PSW
				return;

			m_filledByHistory.Clear();

			foreach (var tblMae in baseklass.ParentTables)
			{
				string areaToLoad = tblMae.Value.AliasTargetTab;
				if (!tblMae.Key.Equals(areaToLoad))
					continue;

				// DB value of FK field
				if (!baseklass.DBFields.TryGetValue(tblMae.Value.SourceRelField, out Field? srcDbFld))
					continue;

				string Qfield = tblMae.Value.AliasSourceTab + "." + tblMae.Value.SourceRelField;

				object fieldValue = baseklass.returnValueField(Qfield);
				string strFieldValue = Conversion.internal2String(srcDbFld.GetValorEmpty(), srcDbFld.FieldType);

				bool isEmptyVal = GenFunctions.emptyG(fieldValue) == 1;

				var isComputedField = false;
				if (!allowOverrideComputed)
				{
					if (baseklass.RelatedSumFields != null)
						isComputedField = baseklass.RelatedSumFields.Contains(tblMae.Value.SourceRelField);

					if (baseklass.LastValueFields != null && !isComputedField)
						isComputedField = baseklass.LastValueFields.Contains(tblMae.Value.SourceRelField);

					if (baseklass.CheckTableFields != null && !isComputedField)
						isComputedField = baseklass.CheckTableFields.Contains(tblMae.Value.SourceRelField);

					if (baseklass.EndofPeriodFields != null && !isComputedField)
						isComputedField = baseklass.EndofPeriodFields.Contains(tblMae.Value.SourceRelField);

					if (baseklass.InternalOperationFields != null && !isComputedField)
						isComputedField = baseklass.InternalOperationFields.Contains(tblMae.Value.SourceRelField);
				}

				//Value do Hist
				bool hasKey = navigation.CheckKey(areaToLoad, out object hValue, level);
				bool isEmptyHistVal = GenFunctions.emptyG(hValue) == 1;

				// skip if unable to find a single value for this key
				if (hValue is Array)
				{
					// check if the value is filled and does not invalidate the EPH
					if (srcDbFld.isEmptyValue(fieldValue) && !Array.Exists<string>((string[])hValue, el => el == strFieldValue))
					{
						object emptyVal = srcDbFld.GetValorEmpty();
						string emptyStrVal = Conversion.internal2String(emptyVal, srcDbFld.FieldType);

						// clear the field with invalid value - might have been filled by a default/formula
						baseklass.insertNameValueField(Qfield, emptyVal);
						m_filledByHistory.Add(Qfield, emptyStrVal);
					}

					continue;
				}

				string hStrValue = Conversion.internal2String(hValue, srcDbFld.FieldType);

				// Override do valor da BD com valor do Historial
				if (isEmptyVal && hasKey && !isEmptyHistVal && !isComputedField)
				{ // Se o valor não for preenchido na BD e existir no Historial
					baseklass.insertNameValueField(Qfield, hValue);
					m_filledByHistory.Add(Qfield, hStrValue);
				}
				else if (((allowNull && !isEmptyVal && hasKey && hValue == null)
					|| (allowOverride && !isEmptyVal && hasKey)) && !isComputedField)
				{ // Override do valor que vem da BD com o valor do Historial
					baseklass.insertNameValueField(Qfield, hValue);
					m_filledByHistory.Add(Qfield, hStrValue);
				}
				else if (!isEmptyVal && changeHistory)
				{ // Preenche o Historial com valor da BD
					object validFieldValue = Conversion.internal2InternalValid(fieldValue, srcDbFld.FieldFormat);
					navigation.SetValue(areaToLoad, validFieldValue, level);
				}
			}
		}

		/// <summary>
		/// Check table permissions
		/// </summary>
		/// <param name="mode">Form mode</param>
		/// <returns></returns>
		public bool CheckTablePermissions(FormMode mode)
		{
			return mode switch
			{
				FormMode.List or FormMode.Show => baseklass.AccessRightsToConsult(),
				FormMode.New or FormMode.Duplicate => baseklass.AccessRightsToCreate(),
				FormMode.Edit => baseklass.accessRightsToChange(),
				FormMode.Delete => baseklass.accessRightsToDelete(),
				_ => throw new FrameworkException("FormMode not implemented.", "CheckTablePremissions", "FormMode not implemented: " + mode)
			};
		}

		/// <summary>
		/// Evaluates the table conditions
		/// </summary>
		/// <param name="type">The condition type</param>
		/// <returns>The result of the evaluation</returns>
		public StatusMessage EvaluateTableConditions(ConditionType type)
		{
			var user = m_userContext.User;
			var ps = m_userContext.PersistentSupport;
			return this.baseklass.EvaluateCrudConditions(ps, user, type);
		}

		/// <summary>
		/// MH - tentar obter FK to table do Dependente
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="alternativeAlias">Será usado quando o form não tem DB ou DL to area do F2 ou FM, mas tem ligação direta com area (CE)</param>
		/// <returns>Value da key</returns>
		public string TryGetForeignKey(string alias, string alternativeAlias = null)
		{
			dynamic areaFK = this;
			foreach (CSGenio.framework.Relation rel in baseklass.Information.GetRelations((String.IsNullOrEmpty(alternativeAlias) ? alias : alternativeAlias)))
			{
				areaFK = areaFK.GetType().GetProperty(CSGenio.framework.StringUtils.CapFirst(rel.AliasTargetTab)).GetValue(areaFK, null);
				if (areaFK == null)
					return null;
				if (rel.AliasTargetTab == (String.IsNullOrEmpty(alternativeAlias) ? alias : alternativeAlias))
				{
					if (String.IsNullOrEmpty(alternativeAlias))
						return areaFK.GetType().GetProperty("Val" + CSGenio.framework.StringUtils.CapFirst(rel.TargetRelField)).GetValue(areaFK, null);
					return areaFK.TryGetForeignKey(alias);
				}
			}

			return null;
		}

		/// <summary>
		/// Recalculate formulas of the area. (++, CT, CS, SR, CL and U1)
		/// </summary>
		/// <param name="model">Current form data</param>
		/// <returns></returns>
		public Dictionary<string, object> RecalculateFormulas()
		{
			m_userContext.PersistentSupport.openConnection();
			baseklass.fillInternalOperations(m_userContext.PersistentSupport, null);
			m_userContext.PersistentSupport.closeConnection();

			// Return only fields with formulas
			var fields = new List<string>();
			if (baseklass.ReplicaFields != null)
				fields.AddRange(baseklass.ReplicaFields);
			if (baseklass.CheckTableFields != null)
				fields.AddRange(baseklass.CheckTableFields);
			if (baseklass.RelatedSumFields != null)
				fields.AddRange(baseklass.RelatedSumFields);
			if (baseklass.LastValueFields != null)
				fields.AddRange(baseklass.LastValueFields);
			if (baseklass.AggregateListFields != null)
				fields.AddRange(baseklass.AggregateListFields);

			var res = new Dictionary<string, object>();
			foreach (var field in fields)
			{
				var fullFieldName = baseklass.Alias.ToLowerInvariant() + "." + field;
				var value = baseklass.returnValueField(fullFieldName);
				res[fullFieldName] = value;
			}

			return res;
		}

		/// <summary>
		/// Finds a row by its primary key
		/// </summary>
		/// <typeparam name="A">Class of row to find</typeparam>
		/// <param name="id">The value of the primary key</param>
		/// <param name="userCtx">User context</param>
		/// <param name="identifier">Interface indentifier</param>
		/// <param name="fieldsToQuery">Fields that need to be queried to the database</param>
		/// <returns>The row found or null otherwise</returns>
		public static A Find<A>(string id, UserContext userCtx, string identifier = null, string[] fieldsToQuery = null) where A : CSGenio.business.Area
		{
			if (string.IsNullOrEmpty(id))
				return null;

			AreaInfo info = CSGenio.business.Area.GetInfoArea<A>();
			CriteriaSet args = CriteriaSet.And();
			args.Equal(info.Alias, info.PrimaryKeyName, id);

			User u = userCtx.User;
			CriteriaSet ephCriteria = AddEPH<A>(ref u, null, identifier);

			if (ephCriteria is not null)
			{
				CriteriaSet ephOrZzstate = new(CriteriaSetOperator.Or);
				ephOrZzstate.Equal(info.Alias, "zzstate", 1);
				ephOrZzstate.SubSet(ephCriteria);
				args.SubSet(ephOrZzstate);
			}

			var sp = userCtx.PersistentSupport;
			// Turns out this part needs to be called by reflection unfortunately, because searchListWhere is generated differently
			// for manual tables that are not Db persisted. Calling the searchListWhere directly on the sp is not equivalent.
			var method = typeof(A).GetMethod("searchList", [typeof(PersistentSupport), typeof(User), typeof(CriteriaSet), typeof(string[]), typeof(bool), typeof(bool)]);
			if (method == null)
				return null;
			var pos = method.Invoke(null, new object[] { sp, u, args, fieldsToQuery, false, true }) as List<A>;

			if (pos.Count == 0)
				return null;

			return pos[0];
		}

		/// <summary>
		/// This method extends the sorts with prefix and suffix fields for non-duplication (if have)
		/// and also with primary key if no unique field exists, when select is not with distinct.
		/// Otherwise, if no sort fields is provided, it returns sorts based on the first visible column
		/// that is sortable.
		/// </summary>
		/// <typeparam name="A">The class representing the rows</typeparam>
		/// <param name="distinct">Does it perform a distinct operation</param>
		/// <param name="sorts">Fields the result should be sorted by</param>
		/// <param name="firstVisibleColumn">First visible column</param>
		/// <returns>The list of ColumnSort items to apply to the list control's query</returns>
		private static List<ColumnSort> ExtendListSortingColumns<A>(bool distinct, List<ColumnSort> sorts = null, FieldRef firstVisibleColumn = null) where A : CSGenio.business.Area
		{
			// `sorts` may arrive null.
			sorts ??= [];

			// No user-selected sorting method
			if (sorts.Count == 0)
			{
				// Condition for field type added because sorting by an image field causes an error
				if (firstVisibleColumn != null
					&& CSGenio.business.Area.GetFieldInfo(firstVisibleColumn).FieldType != FieldType.IMAGE
					&& CSGenio.business.Area.GetFieldInfo(firstVisibleColumn).FieldType != FieldType.GEOGRAPHY_POINT
					&& CSGenio.business.Area.GetFieldInfo(firstVisibleColumn).FieldType != FieldType.GEOMETRY_SHAPE
					&& CSGenio.business.Area.GetFieldInfo(firstVisibleColumn).FieldType != FieldType.GEOGRAPHY_SHAPE)
				{
					ColumnSort sortFirstVisibleColumn = new(new ColumnReference(firstVisibleColumn), SortOrder.Ascending);
					sorts.Add(sortFirstVisibleColumn);
				}
			}

			if (!distinct)
			{
				// Make sure at least one of the fields or combination of fields is unique
				bool hasUniqueField = false;
				AreaInfo areaInfo = CSGenio.business.Area.GetInfoArea<A>();

				// Iterate a copy of the sorts because fields can be added to sorts during this
				List<ColumnSort> originalSorts = new(sorts);
				foreach (ColumnSort sort in originalSorts)
				{
					// Check if this field is unique
					ColumnReference sortColumnReference = sort.Expression as ColumnReference;
					if (sortColumnReference == null)
						continue;

					Field field = CSGenio.business.Area.GetFieldInfo(new FieldRef(sortColumnReference.TableAlias, sortColumnReference.ColumnName));
					if (
						// Field has unique property
						field.NotDup
						// Field is the table's primary key
						|| (field.Alias != null && field.Alias.Equals(areaInfo.Alias) && field.Name != null && field.Name.Equals(areaInfo.PrimaryKeyName))
						// Field is a sequential
						|| (areaInfo.SequentialDefaultValues != null && areaInfo.SequentialDefaultValues.Contains(field.Name))
					)
						hasUniqueField = true;

					// If field has a "prefix to be unique" field, add it to the ordering
					if (!string.IsNullOrEmpty(field.PrefNDup))
					{
						ColumnReference prefixColumnRef = new(field.Alias, field.PrefNDup);
						ColumnSort prefixColumnSort = new(prefixColumnRef, SortOrder.Ascending);
						if (!sorts.Contains(prefixColumnSort))
							sorts.Add(prefixColumnSort);
					}
				}

				// If ordering does not have a unique column or combination of columns, add the primary key column
				// to keep the order of records consistent
				if (!hasUniqueField)
				{
					ColumnSort pkColumnSort = new(new ColumnReference(areaInfo.Alias, areaInfo.PrimaryKeyName), SortOrder.Ascending);
					sorts.Add(pkColumnSort);
				}
			}

			return sorts;
		}

        /// <summary>
        /// The method that builds a ListingMVC object to be used by the Export class 
		/// for data export in the list control.
        /// </summary>
        /// <typeparam name="A">Class of rows</typeparam>
        /// <param name="distinct">Does it perform a distinct operation</param>
        /// <param name="args">Criteria for the search. Note: The CriteriaSet will be extended with PHE limits.</param>
        /// <param name="fields">Fields that we want to retrieve from the database</param>
        /// <param name="offset">Pagination offset</param>
        /// <param name="numRegs">Pagination size</param>
        /// <param name="sorts">Fields the result should be sorted by</param>
        /// <param name="identifier">Interface indentifier</param>
        /// <param name="noLock">True if dirty reads are allowed</param>
        /// <param name="pagingPosEPHs">EPH positioning data</param>
        /// <param name="firstVisibleColumn">First visible column</param>
        /// <returns>Initialized ListingMVC object rteady to be used by the Export class.</returns>
        public static ListingMVC<A> BuildListingForExport<A>(UserContext ctx, bool distinct, ref CriteriaSet args, FieldRef[] fields = null, int offset = 0, int numRegs = 0, List<ColumnSort> sorts = null, string identifier = null, bool noLock = true, CriteriaSet pagingPosEPHs = null, FieldRef firstVisibleColumn = null) where A : CSGenio.business.Area
        {
            User u = ctx.User;

            // EPH
            args = AddEPH<A>(ref u, args, identifier);

			// Sorting columns
			sorts = ExtendListSortingColumns<A>(distinct, sorts, firstVisibleColumn);

			ListingMVC<A> listing = new(fields, sorts, offset, numRegs, distinct, u, noLock, identifier, pagingPosEPHs: pagingPosEPHs);

            return listing;
        }

		/// <summary>
		/// Finds rows that obey to a criteria
		/// </summary>
		/// <typeparam name="A">Class of rows to find</typeparam>
		/// <param name="distinct">Does it perform a distinct operation</param>
		/// <param name="args">Criteria for the search</param>
		/// <param name="fields">Fields that we want to retrieve from the database</param>
		/// <param name="offset">Pagination offset</param>
		/// <param name="numRegs">Pagination size</param>
		/// <param name="sorts">Fields the result should be sorted by</param>
		/// <param name="identifier">Interface indentifier</param>
		/// <param name="noLock">True if dirty reads are allowed</param>
		/// <param name="getTotal">True if we want to retreive a total record found value that ignores the pagination</param>
		/// <param name="selectrow">Primary key of a row to highlight</param>
		/// <param name="pagingPosEPHs">EPH positioning data</param>
		/// <param name="firstVisibleColumn">First visible column</param>
		/// <returns>A listing containing all the rows and information retrieved</returns>
		public static ListingMVC<A> Where<A>(UserContext ctx, bool distinct, CriteriaSet args = null, Quidgest.Persistence.FieldRef[] fields = null, int offset = 0, int numRegs = 0, List<ColumnSort> sorts = null, string identifier = null, bool noLock = true, bool getTotal = false, string selectrow = "", CriteriaSet pagingPosEPHs = null, Quidgest.Persistence.FieldRef firstVisibleColumn = null, List<FieldRef> fieldsWithTotalizer = null, List<string> selectedRecords = null) where A : CSGenio.business.Area
		{
			User u = ctx.User;
			PersistentSupport sp = ctx.PersistentSupport;

			// EPH
			args = AddEPH<A>(ref u, args, identifier);

			// Sorting columns
			sorts = ExtendListSortingColumns<A>(distinct, sorts, firstVisibleColumn);

			// `fieldsWithTotalizer` and `selectedRecords` may arrive null.
			fieldsWithTotalizer ??= [];
			selectedRecords ??= [];

			ListingMVC<A> listing = new(fields, sorts, offset, numRegs, distinct, u, noLock, identifier, getTotal, selectrow, pagingPosEPHs, fieldsWithTotalizer, selectedRecords);

			// Turns out this part needs to be called by reflection unfortunately, because searchListWhere is generated differently
			// for manual tables that are not Db persisted. Calling the searchListAdvancedWhere directly on the sp is not equivalent.
			var method = typeof(A).GetMethod("searchListAdvancedWhere", BindingFlags.Public | BindingFlags.Static);
			if (method == null)
				return listing;
			method.Invoke(null, new object[] { sp, u, args, listing });

			return listing;
		}

		/// <summary>
		/// Finds rows that obey to a criteria
		/// </summary>
		/// <typeparam name="A">Class of rows to find</typeparam>
		/// <param name="args">Criteria for the search</param>
		/// <returns>A listing containing all the rows and information retrieved</returns>
		public static ListingMVC<A> All<A>(UserContext ctx, CriteriaSet args = null) where A : CSGenio.business.Area
		{
			return Where<A>(ctx, false, args, numRegs: -1);
		}

		public static ModelBase FindGeneric(string modelName, string pk, UserContext userContext, string uid, string[] fieldsToSerialize = null, string[] fieldsToQuery = null)
		{
			// TODO: Analyze whether using the cache can improve performance here in the case of multiple simultaneous requests, for example of GetImage.
			Type type = Type.GetType("GenioMVC.Models." + StringUtils.CapFirst(modelName));
			MethodInfo methodInfo = type.GetMethod("Find", [typeof(string), typeof(UserContext), typeof(string), typeof(string[]), typeof(string[])]);
			var row = methodInfo.Invoke(null, new object[] { pk, userContext, uid, fieldsToSerialize, fieldsToQuery }) as ModelBase;
			return row;
		}

		public object GetValueGeneric(string propertyName)
		{
			// TODO: Analyze whether using the cache can improve performance here in the case of multiple simultaneous requests, for example of GetImage.
			Type type = this.GetType();
			PropertyInfo prop = type.GetProperty(propertyName);
			return prop.GetValue(this, null);
		}

		public void SetValueGeneric(string propertyName, object? value)
		{
			Type type = this.GetType();
			PropertyInfo prop = type.GetProperty(propertyName);
			prop.SetValue(this, value, null);
		}
	}
}
