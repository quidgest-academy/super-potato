
namespace CSGenio.core.ai;

using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;
using CSGenio.business;
using CSGenio.persistence;
using CSGenio.framework;
using Quidgest.Persistence.GenericQuery;
/// <summary>
/// Base class for list tools that query entities and return paginated results with filtering and sorting.
/// </summary>
/// <typeparam name="TEntity">The entity type that inherits from DbArea</typeparam>
public abstract class ListToolBase<TEntity> : McpTool where TEntity : DbArea
{
    public override McpSchemaBase InputSchema => new McpSchemaBase()
    {
        Properties = new Dictionary<string, McpProperty>()
        {
            {"offset" , new McpProperty() {
                Type = "integer",
                Description = "The number of items to skip before starting to collect the result set"
                }
            },
            {"numRecords" , new McpProperty() {
                Type = "integer",
                Description = "The number of records to return"
                }
            },
            {"sortBy" , new McpProperty() {
                Type = "string",
                Description = $"Column to sort by. Allowed values: {string.Join(", ", GetSortableFields())}"
                }
            },
            {"sortOrder" , new McpProperty() {
                Type = "string",
                Description = "Sort order. Allowed values: 'asc' (ascending) or 'desc' (descending). Default is 'asc'"
                }
            },
            {"filter" , new McpProperty() {
                Type = "string",
                Description = FilterDslParser.GetSyntaxDescription(GetSearchableFields())
                }
            },
        },
        Required = new List<string>() {
            "sortBy",
        }
    };

    public override McpSchemaBase OutputSchema => new McpSchemaBase()
    {
        Properties = new Dictionary<string, McpProperty>()
        {
            { "list", new McpProperty() {
                Type = "array",
                Description = "An array with the records",
                Items = new McpProperty() {
                    Type = "object",
                    Properties = GetOutputRecordProperties()
                }
            }
            },
            {
                "totalRecords", new McpProperty() {
                Type = "integer",
                Description = "The total number of records that fit the criteria, ignoring numRecords"
            } }
        },
        Required = new List<string>() {"list", "totalRecords" }
    };

    /// <summary>
    /// Returns the fields to retrieve from the database for this entity
    /// </summary>
    protected abstract Quidgest.Persistence.FieldRef[] GetEntityFields();

    /// <summary>
    /// Maps a field name string to a FieldRef for this entity
    /// </summary>
    /// <param name="fieldName">The field name (e.g., "name", "description")</param>
    /// <returns>The FieldRef for the field</returns>
    protected abstract Quidgest.Persistence.FieldRef MapFieldName(string fieldName);

    /// <summary>
    /// Returns an array of field names that can be used for sorting
    /// </summary>
    protected abstract string[] GetSortableFields();

    /// <summary>
    /// Returns an array of field names that can be used for filtering/searching
    /// </summary>
    protected abstract string[] GetSearchableFields();

    /// <summary>
    /// Executes the entity-specific search with the provided listing
    /// </summary>
    protected abstract void ExecuteSearch(PersistentSupport sp, User user, CriteriaSet filters, ListingMVC<TEntity> list);

    /// <summary>
    /// Maps an entity record to the output format
    /// </summary>
    protected abstract object MapOutputRecord(TEntity record);

    /// <summary>
    /// Returns the properties definition for the output record schema
    /// </summary>
    protected abstract Dictionary<string, McpProperty> GetOutputRecordProperties();

    private IList<ColumnSort> ParseSortParameters(JsonElement input)
    {
        // Parse sorting parameters
        string sortBy = null;
        if (input.TryGetProperty("sortBy", out var propSortBy))
            sortBy = propSortBy.GetString();

        string sortOrder = "asc";
        if (input.TryGetProperty("sortOrder", out var propSortOrder))
            sortOrder = propSortOrder.GetString()?.ToLower() ?? "asc";

        // Map sortBy to FieldRef and validate
        Quidgest.Persistence.FieldRef sortField = null;
        if (!string.IsNullOrEmpty(sortBy))
        {
            // Validate that the field is in the sortable fields list
            var sortableFields = GetSortableFields();
            if (!sortableFields.Contains(sortBy))
            {
                throw new ArgumentException($"Invalid sortBy value: '{sortBy}'. Allowed values are: {string.Join(", ", sortableFields)}");
            }

            sortField = MapFieldName(sortBy);
        }

        // Create sort specification
        if (sortField != null)
        {
            var order = sortOrder == "desc" ? SortOrder.Descending : SortOrder.Ascending;
            var sorts = new List<ColumnSort>();
            sorts.Add(new ColumnSort(new ColumnReference(sortField.Area, sortField.Field), order));
            return sorts;
        }

        return null;
    }

    public override object Execute(PersistentSupport sp, User user, JsonElement input)
    {
        //Prepare record
        int offset = 0;
        if (input.TryGetProperty("offset", out var propOffset))
            offset = propOffset.GetInt32();

        int numRecords = 10;
        if (input.TryGetProperty("numRecords", out var propNumRecords))
            numRecords = propNumRecords.GetInt32();

        var sorts = ParseSortParameters(input);

        // Parse filter if provided
        CriteriaSet filters = null;
        if (input.TryGetProperty("filter", out var propFilter))
        {
            string filterStr = propFilter.GetString();
            if (!string.IsNullOrWhiteSpace(filterStr))
            {
                var parser = new FilterDslParser(MapFieldName, GetSearchableFields());
                filters = parser.Parse(filterStr);
            }
        }

        var list = new ListingMVC<TEntity>(
            fields: GetEntityFields(),
            sorts: sorts,
            offset: offset,
            numRegs: numRecords-1, //The list returns plus 1 results to know if there are more records, but in this case we always return the total number
            distinct:false,
            user: user,
            noLock:false,
            getTotal: true
        );

        //Set record values
        ExecuteSearch(sp, user, filters, list);

        return new
        {
            content = new[]
            {
                new {
                    text = "List returned successfully",
                    type = "text"
                }
            },
            structuredContent = new
            {
                list = list.Rows.Select(r => MapOutputRecord(r)),
                totalRecords = list.TotalRecords
            }
        };
    }
}
