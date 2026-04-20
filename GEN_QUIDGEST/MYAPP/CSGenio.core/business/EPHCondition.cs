using System;

namespace CSGenio.framework;

/// <summary>
/// Defines how a set of EPH values should be fetched
/// </summary>
public class EPHCondition : ICloneable
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ephName">Id of the EPH</param>
    /// <param name="tableSystem">System of the Association table</param>
    /// <param name="table">Physical name of the Association table</param>
    /// <param name="tableAlias">Alias to use for the Association table</param>
    /// <param name="relationField">Foreign key from the Association table to the Value table</param>
    /// <param name="ephTable">Alias name of the Value table</param>
    /// <param name="ephField">Value to fetch from the Value table</param>
    /// <param name="ephFieldType">Type of value</param>
    /// <param name="intialForm">Initial form for the EPH</param>
    public EPHCondition(string ephName, string tableSystem, string table, string tableAlias, string relationField, string ephAlias, string ephField, FieldType ephFieldType, string intialForm)
    {
        EPHName = ephName;
        TableSystem = tableSystem;
        TableName = table;
        AliasTable = tableAlias;
        RelationField = relationField;
        EPHTable = ephAlias;
        EPHField = ephField;
        EPHFieldType = ephFieldType;
        IntialForm = intialForm;
    }

    /// <summary>
    /// Id of the EPH
    /// </summary>
    public string EPHName { get; }
    /// <summary>
    /// System of the Association table
    /// </summary>
    public string TableSystem { get; }
    /// <summary>
    /// Physical name of the Association table
    /// </summary>
    public string TableName { get; }
    /// <summary>
    /// Alias to use for the Association table
    /// </summary>
    public string AliasTable { get; }
    /// <summary>
    /// Foreign key from the Association table to the Value table
    /// </summary>
    public string RelationField { get; }
    /// <summary>
    /// Alias name of the Value table
    /// </summary>
    public string EPHTable { get; }
    /// <summary>
    /// Value to fetch from the Value table
    /// </summary>
    public string EPHField { get; }
    /// <summary>
    /// Type of value
    /// </summary>
    public FieldType EPHFieldType { get; }
    /// <summary>
    /// Initial form for the EPH
    /// </summary>
    /// <remarks>
    /// Leave empty for EPH's that are associated by login
    /// </remarks>
    public string IntialForm { get; }
    /// <inheritdoc/>
    public object Clone()
    {
	    return new EPHCondition(EPHName, TableSystem, TableName, AliasTable, RelationField, EPHTable, EPHField, EPHFieldType, IntialForm);
    }
}
