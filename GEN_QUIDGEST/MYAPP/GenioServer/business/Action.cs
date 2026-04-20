using System.Collections.Generic;
using System.Linq;

using CSGenio.core.business;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business.Triggers
{
	/// <summary>
	/// Generic action interface
	/// </summary>
	public interface IAction
	{
		/// <summary>
		/// Executes the action.
		/// </summary>
		void Execute();
	}

	/// <summary>
	/// A generic action
	/// </summary>
	/// <seealso cref="CSGenio.business.IAction" />
	public abstract class Action : IAction
	{
		/// <summary>
		/// The context
		/// </summary>
		protected readonly TriggerContext _context;

		/// <summary>
		/// Initializes a new instance of the <see cref="Action" /> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="childTableWhere">The child table where.</param>
		protected Action(TriggerContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Determines whether the target table is a child table.
		/// </summary>
		/// <param name="targetArea">The target area.</param>
		/// <returns></returns>
		protected bool TargetIsChildTable(string targetArea)
		{
			// "Glob" should also be considered here, since it only has one record and any table can find it.
			return !(targetArea == "glob" || _context.Area.Alias == targetArea || _context.Area.ParentTables.ContainsKey(targetArea));
		}

		/// <summary>
		/// Gets the parent row.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <returns></returns>
		protected DbArea GetParentRow(string parent)
		{
			// Find the primary key of the parent
			Relation rel = _context.Area.ParentTables[parent.ToLower()];
			string id = (string)_context.Area.returnValueField(_context.Area.Alias + "." + rel.SourceRelField);

			if (string.IsNullOrEmpty(id))
				return null;

			// Try to get row from the context of previous actions
			DbArea area = GetDirtyRow(parent, id);

			if (area == null)
			{
				area = (DbArea)Area.createArea(parent, _context.User, _context.User.CurrentModule);

				if (_context.PersistentSupport.getRecord(area, id, null))
					return area;
			}
			else
				return area;

			return null;
		}

		/// <summary>
		/// Gets the affected rows.
		/// </summary>
		/// <param name="targetArea">The target area.</param>
		/// <returns></returns>
		protected List<DbArea> GetChildRows(string targetArea)
		{
			// The owner of the action is a child area
			List<DbArea> children = new List<DbArea>();

			foreach (var child in _context.Area.ChildTable)
			{
				if (child.ChildArea == targetArea)
				{
					// Get info about the child area
					AreaInfo info = Area.GetInfoArea(targetArea);

					CriteriaSet where = CriteriaSet.And().Equal(
						targetArea,
						info.ParentTables[_context.Area.Alias].SourceRelField,
						_context.Area.QPrimaryKey
					);

					return Area
						.searchList(targetArea, _context.PersistentSupport, _context.User, where)
						.Cast<DbArea>()
						.ToList();
				}
			}

			return children;
		}

		/// <summary>
		/// Gets the affected rows.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <returns></returns>
		public List<DbArea> GetAffectedRows(string target)
		{
			if (target == _context.Area.Alias)
				return new List<DbArea>() { _context.Area };
			else if (_context.Area.ParentTables.ContainsKey(target))
				return (GetParentRow(target) is DbArea ParentRow) ? new List<DbArea> { ParentRow } : new List<DbArea>();
			else if (target == "glob")
				return new List<DbArea>() { new GlobalFunctions(_context.User, _context.User.CurrentModule, _context.PersistentSupport).GetGlob() };
			return GetChildRows(target);
		}

		/// <summary>
		/// Sets the dirty row.
		/// </summary>
		/// <param name="area">The area.</param>
		protected void SetDirtyRow(DbArea area)
		{
			if (_context.DirtyRows.ContainsKey(area.Alias))
			{
				var rows = _context.DirtyRows[area.Alias];

				if (rows.ContainsKey(area.QPrimaryKey))
					_context.DirtyRows[area.Alias][area.QPrimaryKey] = area;
				else
					_context.DirtyRows[area.Alias].Add(area.QPrimaryKey, area);
			}
			else
			{
				_context.DirtyRows.Add(area.Alias, new Dictionary<string, DbArea>() { { area.QPrimaryKey, area } });
			}
		}

		/// <summary>
		/// Gets a dirty row in the provided table
		/// with the provided id.
		/// </summary>
		/// <param name="ndbf">The NDBF.</param>
		/// <param name="id">The identifier.</param>
		/// <returns></returns>
		protected DbArea GetDirtyRow(string ndbf, string id)
		{
			if (_context.DirtyRows.ContainsKey(ndbf))
			{
				var rows = _context.DirtyRows[ndbf];

				if (rows.ContainsKey(id))
					return rows[id];
			}

			return null;
		}

		/// <summary>
		/// Executes the action.
		/// </summary>
		public abstract void Execute();
	}

	/// <summary>
	/// An action that executes an internal import
	/// </summary>
	/// <seealso cref="CSGenio.business.Action" />
	/// <seealso cref="CSGenio.business.IAction" />
	public class InternalImportAction : Action
	{
		/// <summary>
		/// A type that encapsulates the internal import to execute.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="sp">The sp.</param>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		public delegate StatusMessage InternalImport(string id, PersistentSupport sp, User user);
		/// <summary>
		/// The internal import
		/// </summary>
		private readonly InternalImport _internalImport;
		/// <summary>
		/// The source table
		/// </summary>
		private readonly string _srcTable;

		/// <summary>
		/// Initializes a new instance of the <see cref="InternalImportAction" /> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="internalImport">The internal import.</param>
		/// <param name="parentArea">The parent area.</param>
		public InternalImportAction(TriggerContext context, InternalImport internalImport, string srcTable)
			: base(context)
		{
			_internalImport = internalImport;
			_srcTable = srcTable;
		}

		/// <inheritdoc/>
		public override void Execute()
		{
			List<DbArea> rows = GetAffectedRows(_srcTable);

			foreach (var row in rows)
				_internalImport(row.QPrimaryKey, _context.PersistentSupport, _context.User);
		}
	}

	/// <summary>
	/// An action that updates the value of a field
	/// </summary>
	/// <seealso cref="CSGenio.business.Action" />
	/// <seealso cref="CSGenio.business.IAction" />
	public class UpdateFieldValueAction : Action
	{
		/// <summary>
		/// The NDBF
		/// </summary>
		private readonly string _ndbf;

		/// <summary>
		/// The field name
		/// </summary>
		private readonly string _fieldName;

		/// <summary>
		/// The formula
		/// </summary>
		private readonly InternalOperationFormula _formula;

		/// <summary>
		/// Whether the formulas of the rows should be recalculated
		/// </summary>
		private readonly bool _recalcFormulas;

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateFieldValueAction" /> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="ndbf">The NDBF.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="formula">The formula.</param>
		/// <param name="recalcFormulas">if set to <c>true</c> [recalc formulas].</param>
		/// <param name="childTableWhere">The child table where.</param>
		public UpdateFieldValueAction(TriggerContext context, string ndbf, string fieldName,
			InternalOperationFormula formula, bool recalcFormulas)
			: base(context)
		{
			_ndbf = ndbf;
			_fieldName = fieldName;
			_formula = formula;
			_recalcFormulas = recalcFormulas;
		}

		/// <inheritdoc/>
		public override void Execute()
		{
			// Context of the formula
			FormulaDbContext formulaContext = new FormulaDbContext(_context.Area);
			formulaContext.AddFormulaSources(_formula);
			formulaContext.SetArea(_context.Area);

			// Evaluate the formula
			// within the context of the area that triggered the action
			object value = _formula.calculateInternalFormula(
				_context.Area,
				_context.PersistentSupport,
				formulaContext,
				FunctionType.ALT
			);

			if (!TargetIsChildTable(_ndbf) || _recalcFormulas)
			{
				// Determine the rows to update
				List<DbArea> rows = GetAffectedRows(_ndbf);

				foreach (var row in rows)
				{
					// Update the value of the field
					row.insertNameValueField(string.Format("{0}.{1}", _ndbf, _fieldName), value);

					// Update formulas
					if (_recalcFormulas)
						row.fillInternalOperations(_context.PersistentSupport, null);

					SetDirtyRow(row);
				}
			}
			else
			{
				// Operation on child tables without updating formulas
				// Rows can be updated using a single query
				AreaInfo info = Area.GetInfoArea(_ndbf);

				CriteriaSet filter = CriteriaSet.And();
				filter.Equal(
					info.TableName,
					info.ParentTables[_context.Area.Alias].SourceRelField,
					_context.Area.QPrimaryKey
				);

				UpdateQuery query = new UpdateQuery()
					.Update(info.TableName)
					.Set(_fieldName, value)
					.Where(filter);

				_context.PersistentSupport.Execute(query);
			}
		}
	}

	/// <summary>
	/// An action that recalcules the formulas of a table fields
	/// </summary>
	/// <seealso cref="CSGenio.business.Action" />
	/// <seealso cref="CSGenio.business.IAction" />
	public class RecalcTableAction : Action
	{
		/// <summary>
		/// The NDBF
		/// </summary>
		private readonly string _ndbf;

		/// <summary>
		/// Initializes a new instance of the <see cref="RecalcTableAction" /> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="ndbf">The NDBF.</param>
		public RecalcTableAction(TriggerContext context, string ndbf)
			: base(context)
		{
			_ndbf = ndbf;
		}

		/// <inheritdoc/>
		public override void Execute()
		{
			// Determine the rows to recalc
			List<DbArea> rows = GetAffectedRows(_ndbf);

			foreach (var row in rows)
			{
				row.fillInternalOperations(_context.PersistentSupport, null);
				SetDirtyRow(row);
			}
		}
	}

	/// <summary>
	/// An action that sends a notification.
	/// </summary>
	/// <seealso cref="CSGenio.business.Action" />
	/// <seealso cref="CSGenio.business.IAction" />
	public class SendNotificationAction : Action
	{
		/// <summary>
		/// The identifier of the notification to send.
		/// </summary>
		private readonly string _notificationId;

		/// <summary>
		/// Initializes a new instance of the <see cref="SendNotificationAction" /> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="notificationId">The identifier of the notification to send.</param>
		public SendNotificationAction(TriggerContext context, string notificationId) : base(context)
		{
			_notificationId = notificationId;
		}

		/// <inheritdoc/>
		public override void Execute()
		{
			Notification viewModel = PersistentSupport.getNotifications()[_notificationId] as Notification;
			viewModel.RunOpen(_context.PersistentSupport, _context.User);
		}
	}

	/// <summary>
	/// An action that calls an AI agent.
	/// </summary>
	public class CallAiAgentAction : Action
	{
		private readonly core.ai.ModelAiAgent _agent;

		/// <summary>
		/// Initializes a new instance of the <see cref="CallAiAgentAction" /> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="agent">The AI agent.</param>
		public CallAiAgentAction(TriggerContext context, core.ai.ModelAiAgent agent) : base(context)
		{
			_agent = agent;
		}

		/// <inheritdoc/>
		public override void Execute()
		{
			CSGenio.core.ai.AgentContextData agentContext = new CSGenio.core.ai.AgentContextData()
			{
				Username = _context.User.Name,
				AgentId = _agent.AGENT_ID,
				Module = _context.User.CurrentModule,
				Subsystem = _context.User.Year
			};
			if(!string.IsNullOrEmpty(_context.Area.QPrimaryKey))
				agentContext.CurrentRecordId = _context.Area.QPrimaryKey;

			_agent.Execute(_context.Area, _context.PersistentSupport, _context.User, agentContext);
			_agent.PersistRecord(_context.PersistentSupport);
		}
	}

	/// <summary>
	/// An action that runs a formula group.
	/// </summary>
	public class RunFormulaGroupAction : Action
	{
		private readonly string groupId;

		/// <summary>
		/// Initializes a new instance of the <see cref="RunFormulaGroupAction" /> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="groupId">The formula group identifier.</param>
		public RunFormulaGroupAction(TriggerContext context, string groupId) : base(context)
		{
			this.groupId = groupId;
		}

		/// <inheritdoc/>
		public override void Execute()
		{
			FormulaGroup.Execute(_context.PersistentSupport, groupId);
		}
	}
}
