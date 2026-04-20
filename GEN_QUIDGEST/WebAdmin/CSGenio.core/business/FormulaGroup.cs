using System;
using System.Collections.Generic;

using CSGenio.core.di;
using CSGenio.persistence;

namespace CSGenio.core.business;

/// <summary>
/// Provides static methods for executing formula groups against database systems.
/// A formula group represents a collection of related formulas that are executed together
/// as a single stored procedure operation, returning results for multiple tables.
/// </summary>
public static class FormulaGroup
{
	/// <summary>
	/// List of valid formula group identifiers
	/// </summary>
	private static readonly List<string> ValidGroupIds =
	[
	];

	/// <summary>
	/// Validates that the provided group ID is in the list of valid group identifiers
	/// </summary>
	/// <param name="groupId">The group ID to validate</param>
	/// <exception cref="ArgumentException">Thrown when groupId is null, empty, or not in the valid list</exception>
	private static void ValidateGroupId(string groupId)
	{
		if (string.IsNullOrWhiteSpace(groupId))
			throw new ArgumentException("Formula group identifier cannot be null or empty.", nameof(groupId));
		if (!ValidGroupIds.Contains(groupId))
			throw new ArgumentException($"Invalid formula group identifier \"{groupId}\", valid identifiers are: {string.Join(", ", ValidGroupIds)}.", nameof(groupId));
	}

	/// <summary>
	/// Executes the formula group
	/// </summary>
	/// <param name="dataSystem">The data system identifier</param>
	/// <param name="groupId">The identifier of the formula group</param>
	/// <returns>The result of the operation, with an entry for each table in the group</returns>
	public static DataMatrix Execute(string dataSystem, string groupId)
	{
		return Execute(PersistentSupport.getPersistentSupport(dataSystem), groupId);
	}

	/// <summary>
	/// Executes the formula group
	/// </summary>
	/// <param name="sp">The persistent support</param>
	/// <param name="groupId">The identifier of the formula group</param>
	/// <returns>The result of the operation, with an entry for each table in the group</returns>
	public static DataMatrix Execute(PersistentSupport sp, string groupId)
	{
		try
		{
			ValidateGroupId(groupId);
			// Invoke the formula group stored procedure.
			return sp.ExecuteProcedure($"CalcFormulaGroup{groupId}");
		}
		catch (Exception e)
		{
			GenioDI.Log.Error($"Error in formula group \"{groupId}\": {e.Message}");
			throw;
		}
	}
}
