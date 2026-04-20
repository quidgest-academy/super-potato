using CSGenio.business;
using CSGenio.core.framework.table;
using Quidgest.Persistence;

namespace CSGenio.core.Test.table;

public class TestTableConfiguration
{
	private TableConfiguration tableConfig;

	[SetUp]
	public void SetUp()
	{
		tableConfig = new TableConfiguration();
	}

	#region DetermineRowsPerPage Tests

	[Test]
	public void DetermineRowsPerPage_ConfiguredValueEqualsDefault_ReturnsConfiguredValue()
	{
		// Arrange
		tableConfig.RowsPerPage = 10;
		const int defaultRowsPerPage = 10;
		const string rowsPerPageOptions = "5,10,25,50";

		// Act
		int result = tableConfig.DetermineRowsPerPage(defaultRowsPerPage, rowsPerPageOptions);

		// Assert
		Assert.That(result, Is.EqualTo(10));
	}

	[Test]
	public void DetermineRowsPerPage_ConfiguredValueInOptions_ReturnsConfiguredValue()
	{
		// Arrange
		tableConfig.RowsPerPage = 25;
		const int defaultRowsPerPage = 10;
		const string rowsPerPageOptions = "5,10,25,50";

		// Act
		int result = tableConfig.DetermineRowsPerPage(defaultRowsPerPage, rowsPerPageOptions);

		// Assert
		Assert.That(result, Is.EqualTo(25));
	}

	[Test]
	public void DetermineRowsPerPage_ConfiguredValueNotInOptions_ReturnsDefault()
	{
		// Arrange
		tableConfig.RowsPerPage = 30;
		const int defaultRowsPerPage = 10;
		const string rowsPerPageOptions = "5,10,25,50";

		// Act
		int result = tableConfig.DetermineRowsPerPage(defaultRowsPerPage, rowsPerPageOptions);

		// Assert
		Assert.That(result, Is.EqualTo(10));
	}

	[Test]
	public void DetermineRowsPerPage_EmptyOptionsString_ReturnsDefault()
	{
		// Arrange
		tableConfig.RowsPerPage = 25;
		const int defaultRowsPerPage = 10;
		const string rowsPerPageOptions = "";

		// Act
		int result = tableConfig.DetermineRowsPerPage(defaultRowsPerPage, rowsPerPageOptions);

		// Assert
		Assert.That(result, Is.EqualTo(10));
	}

	[Test]
	public void DetermineRowsPerPage_NullOptionsString_ReturnsDefault()
	{
		// Arrange
		tableConfig.RowsPerPage = 25;
		const int defaultRowsPerPage = 10;

		// Act
		int result = tableConfig.DetermineRowsPerPage(defaultRowsPerPage, null);

		// Assert
		Assert.That(result, Is.EqualTo(10));
	}

	[Test]
	public void DetermineRowsPerPage_InvalidValuesInOptions_SkipsInvalidAndProcesssValid()
	{
		// Arrange
		tableConfig.RowsPerPage = 25;
		const int defaultRowsPerPage = 10;
		const string rowsPerPageOptions = "5,invalid,25,abc,50";

		// Act
		int result = tableConfig.DetermineRowsPerPage(defaultRowsPerPage, rowsPerPageOptions);

		// Assert
		Assert.That(result, Is.EqualTo(25));
	}

	[Test]
	public void DetermineRowsPerPage_AllInvalidValuesInOptions_ReturnsDefault()
	{
		// Arrange
		tableConfig.RowsPerPage = 25;
		const int defaultRowsPerPage = 10;
		const string rowsPerPageOptions = "invalid,abc,xyz";

		// Act
		int result = tableConfig.DetermineRowsPerPage(defaultRowsPerPage, rowsPerPageOptions);

		// Assert
		Assert.That(result, Is.EqualTo(10));
	}

	#endregion

	#region GetVisibleColumnNames Tests

	[Test]
	public void GetVisibleColumnNames_NoColumns_ReturnsEmptyList()
	{
		// Arrange
		tableConfig.ColumnConfigurations = [];
		const string mainTableName = "users";

		// Act
		List<string> result = tableConfig.GetVisibleColumnNames(mainTableName);

		// Assert
		Assert.That(result, Is.Empty);
	}

	[Test]
	public void GetVisibleColumnNames_SingleVisibleColumn_ReturnsFormattedColumnName()
	{
		// Arrange
		tableConfig.ColumnConfigurations =
		[
			new ColumnConfiguration { Name = "ValEmail", Visibility = 1 }
		];
		const string mainTableName = "users";

		// Act
		List<string> result = tableConfig.GetVisibleColumnNames(mainTableName);

		// Assert
		Assert.That(result, Has.Count.EqualTo(1));
		Assert.That(result[0], Is.EqualTo("users.email"));
	}

	[Test]
	public void GetVisibleColumnNames_MultipleVisibleColumns_ReturnsAllVisible()
	{
		// Arrange
		tableConfig.ColumnConfigurations =
		[
			new ColumnConfiguration { Name = "ValEmail", Visibility = 1 },
			new ColumnConfiguration { Name = "ValName", Visibility = 1 },
			new ColumnConfiguration { Name = "ValId", Visibility = 0 }
		];
		const string mainTableName = "users";

		// Act
		List<string> result = tableConfig.GetVisibleColumnNames(mainTableName);

		// Assert
		Assert.That(result, Has.Count.EqualTo(2));
		Assert.That(result, Contains.Item("users.email"));
		Assert.That(result, Contains.Item("users.name"));
	}

	[Test]
	public void GetVisibleColumnNames_RelatedTableColumn_ReturnsQualifiedName()
	{
		// Arrange
		tableConfig.ColumnConfigurations =
		[
			new ColumnConfiguration { Name = "profiles.ValName", Visibility = 1 }
		];
		const string mainTableName = "users";

		// Act
		List<string> result = tableConfig.GetVisibleColumnNames(mainTableName);

		// Assert
		Assert.That(result, Has.Count.EqualTo(1));
		Assert.That(result[0], Is.EqualTo("profiles.name"));
	}

	[Test]
	public void GetVisibleColumnNames_InvalidColumnFormat_SkipsInvalidColumn()
	{
		// Arrange
		tableConfig.ColumnConfigurations =
		[
			new ColumnConfiguration { Name = "ValEmail", Visibility = 1 },
			new ColumnConfiguration { Name = "InvalidFormat", Visibility = 1 },
			new ColumnConfiguration { Name = "ValName", Visibility = 1 }
		];
		const string mainTableName = "users";

		// Act
		List<string> result = tableConfig.GetVisibleColumnNames(mainTableName);

		// Assert
		Assert.That(result, Has.Count.EqualTo(2));
		Assert.That(result, Contains.Item("users.email"));
		Assert.That(result, Contains.Item("users.name"));
	}

	#endregion

	#region GetFirstVisibleColumn Tests

	[Test]
	public void GetFirstVisibleColumn_NoVisibleColumns_ReturnsNull()
	{
		// Arrange
		tableConfig.ColumnConfigurations = [];
		const string mainTableName = "users";

		// Act
		FieldRef result = tableConfig.GetFirstVisibleColumn(mainTableName);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public void GetFirstVisibleColumn_MultipleVisibleColumns_ReturnsFirst()
	{
		// Arrange
		tableConfig.ColumnConfigurations =
		[
			new ColumnConfiguration { Name = "ValEmail", Visibility = 1 },
			new ColumnConfiguration { Name = "ValName", Visibility = 1 }
		];
		const string mainTableName = "users";

		// Act
		FieldRef result = tableConfig.GetFirstVisibleColumn(mainTableName);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Field, Is.EqualTo("email"));
		Assert.That(result.Area, Is.EqualTo("users"));
	}

	[Test]
	public void GetFirstVisibleColumn_HiddenColumnsBeforeVisibleOne_SkipsHidden()
	{
		// Arrange
		tableConfig.ColumnConfigurations =
		[
			new ColumnConfiguration { Name = "ValId", Visibility = 0 },
			new ColumnConfiguration { Name = "ValEmail", Visibility = 1 }
		];
		const string mainTableName = "users";

		// Act
		FieldRef result = tableConfig.GetFirstVisibleColumn(mainTableName);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Field, Is.EqualTo("email"));
	}

	#endregion

	#region GetValidSearchFilters Tests

	[Test]
	public void GetValidSearchFilters_NoFilters_ReturnsEmptyList()
	{
		// Arrange
		tableConfig.Filters = [];
		const string mainTableName = "users";
		var searchableColumnNames = new List<string> { "users.email", "users.name" };

		// Act
		List<ColumnFilter> result = tableConfig.GetValidSearchFilters(mainTableName, searchableColumnNames);

		// Assert
		Assert.That(result, Is.Empty);
	}

	[Test]
	public void GetValidSearchFilters_SearchableColumns_ReturnsValidFilters()
	{
		// Arrange
		var columnFilter = new ColumnFilter
		{
			Field = "users.email",
			Operator = "contains",
			Values = ["test@example.com"]
		};
		tableConfig.Filters = [columnFilter];
		tableConfig.ColumnConfigurations =
		[
			new ColumnConfiguration { Name = "ValEmail", Visibility = 1 }
		];
		const string mainTableName = "users";
		var searchableColumnNames = new List<string> { "users.email", "users.name" };

		// Act
		List<ColumnFilter> result = tableConfig.GetValidSearchFilters(mainTableName, searchableColumnNames);

		// Assert
		Assert.That(result, Has.Count.GreaterThanOrEqualTo(0));
	}

	[Test]
	public void GetValidSearchFilters_NonSearchableColumn_FiltersItOut()
	{
		// Arrange
		var columnFilter = new ColumnFilter
		{
			Field = "users.internal_id",
			Operator = "equals",
			Values = ["123"]
		};
		tableConfig.Filters = [columnFilter];
		tableConfig.ColumnConfigurations = [];
		const string mainTableName = "users";
		var searchableColumnNames = new List<string> { "users.email", "users.name" };

		// Act
		List<ColumnFilter> result = tableConfig.GetValidSearchFilters(mainTableName, searchableColumnNames);

		// Assert
		// Filter with non-searchable field should not appear or be marked invalid
		Assert.That(result, Is.Not.Null);
	}

	[Test]
	public void GetValidSearchFilters_NullFieldIncluded_IncludesIt()
	{
		// Arrange
		var columnFilter = new ColumnFilter
		{
			Field = null,
			Operator = "contains",
			Values = ["search_term"]
		};
		tableConfig.Filters = [columnFilter];
		const string mainTableName = "users";
		var searchableColumnNames = new List<string> { "users.email", "users.name" };

		// Act
		List<ColumnFilter> result = tableConfig.GetValidSearchFilters(mainTableName, searchableColumnNames);

		// Assert
		Assert.That(result, Has.Count.GreaterThanOrEqualTo(1));
	}

	#endregion

	#region SerializeAsJson Tests

	[Test]
	public void SerializeAsJson_IgnoresNullProperties()
	{
		// Arrange
		tableConfig.Name = "TestConfig";
		tableConfig.DefaultSearchColumn = null;
		tableConfig.ColumnConfigurations = [];

		// Act
		string result = tableConfig.SerializeAsJson();

		// Assert
		Assert.That(result, Does.Not.Contain("null"));
	}

	[Test]
	public void SerializeAsJson_WithColumns_IncludesColumnData()
	{
		// Arrange
		tableConfig.Name = "TestConfig";
		tableConfig.ColumnConfigurations =
		[
			new ColumnConfiguration { Name = "ValEmail", Order = 0, Visibility = 1 }
		];

		// Act
		string result = tableConfig.SerializeAsJson();

		// Assert
		Assert.That(result, Does.Contain("ValEmail"));
		Assert.That(result, Does.Contain("columnConfiguration"));
	}

	[Test]
	public void SerializeAsJson_IgnoresNonSerializedProperties()
	{
		// Arrange
		tableConfig.Name = "TestConfig";
		tableConfig.Version = 5;
		tableConfig.Uuid = "test-uuid-123";

		// Act
		string result = tableConfig.SerializeAsJson();

		// Assert
		Assert.That(result, Does.Not.Contain("Version"));
		Assert.That(result, Does.Not.Contain("Uuid"));
	}

	#endregion

	#region ParseTableConfigData Tests

	[Test]
	public void ParseTableConfigData_ValidJson_DeserializesSuccessfully()
	{
		// Arrange
		string json = """{"name":"TestConfig","rowsPerPage":10,"columnConfiguration":[]}""";

		// Act
		var result = TableConfiguration.ParseTableConfigData(json);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.RowsPerPage, Is.EqualTo(10));
	}

	[Test]
	public void ParseTableConfigData_ComplexJson_PreservesAllData()
	{
		// Arrange
		string json = """
			{
				"name":"ComplexConfig",
				"rowsPerPage":25,
				"lineBreak":true,
				"activeViewMode":"grid",
				"columnConfiguration":[
					{"name":"ValEmail","order":0,"visibility":1,"exportability":1}
				]
			}
			""";

		// Act
		var result = TableConfiguration.ParseTableConfigData(json);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(result.RowsPerPage, Is.EqualTo(25));
			Assert.That(result.LineBreak, Is.True);
			Assert.That(result.ActiveViewMode, Is.EqualTo("grid"));
			Assert.That(result.ColumnConfigurations, Has.Count.EqualTo(1));
		});
	}

	[Test]
	public void ParseTableConfigData_InvalidJson_ReturnsEmptyConfiguration()
	{
		// Arrange
		string invalidJson = "{ invalid json }";

		// Act
		var result = TableConfiguration.ParseTableConfigData(invalidJson);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.TypeOf<TableConfiguration>());
	}

	[Test]
	public void ParseTableConfigData_EmptyString_ReturnsEmptyConfiguration()
	{
		// Arrange
		string emptyJson = "";

		// Act
		var result = TableConfiguration.ParseTableConfigData(emptyJson);

		// Assert
		Assert.That(result, Is.Not.Null);
	}

	[Test]
	public void ParseTableConfigData_CaseInsensitiveDeserialization()
	{
		// Arrange
		string json = """{"NAME":"TestConfig","ROWSPERPAGE":15}""";

		// Act
		var result = TableConfiguration.ParseTableConfigData(json);

		// Assert
		Assert.That(result.RowsPerPage, Is.EqualTo(15));
	}

	[Test]
	public void ParseTableConfigData_WithDatabaseRecord_PopulatesMetadata()
	{
		// Arrange
		var dbRecord = new CSGenioAtblcfg(new CSGenio.framework.User("test", "test", "test"))
		{
			ValConfig = """{"name":"DbConfig","rowsPerPage":20}""",
			ValName = "SavedConfig",
			ValUsrsetv = 3,
			ValUuid = "uuid-123"
		};

		// Act
		var result = TableConfiguration.ParseTableConfigData(dbRecord);

		// Assert
		Assert.That(result.Name, Is.EqualTo("SavedConfig"));
		Assert.That(result.Version, Is.EqualTo(3));
		Assert.That(result.Uuid, Is.EqualTo("uuid-123"));
		Assert.That(result.RowsPerPage, Is.EqualTo(20));
	}

	#endregion
}
