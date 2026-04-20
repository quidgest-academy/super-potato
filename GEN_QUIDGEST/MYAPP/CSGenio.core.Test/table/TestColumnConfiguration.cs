using CSGenio.business;
using CSGenio.core.framework.table;
using Quidgest.Persistence;

namespace CSGenio.core.Test.table;

public class TestColumnConfiguration
{
	#region GetTableName Tests

	[Test]
	public void GetTableName_QualifiedFieldName_ReturnsTableName()
	{
		// Arrange
		const string mainTableName = "users";
		const string fieldName = "profiles.ValName";

		// Act
		string result = ColumnConfiguration.GetTableName(mainTableName, fieldName);

		// Assert
		Assert.That(result, Is.EqualTo("profiles"));
	}

	[Test]
	public void GetTableName_UnqualifiedFieldName_ReturnsMainTableName()
	{
		// Arrange
		const string mainTableName = "users";
		const string fieldName = "ValEmail";

		// Act
		string result = ColumnConfiguration.GetTableName(mainTableName, fieldName);

		// Assert
		Assert.That(result, Is.EqualTo("users"));
	}

	[Test]
	public void GetTableName_NullMainTableName_ReturnsNull()
	{
		// Arrange
		const string fieldName = "profiles.ValName";

		// Act
		string result = ColumnConfiguration.GetTableName(null, fieldName);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public void GetTableName_NullFieldName_ReturnsNull()
	{
		// Arrange
		const string mainTableName = "users";

		// Act
		string result = ColumnConfiguration.GetTableName(mainTableName, null);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public void GetTableName_EmptyMainTableName_ReturnsNull()
	{
		// Arrange
		const string fieldName = "profiles.ValName";

		// Act
		string result = ColumnConfiguration.GetTableName("", fieldName);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public void GetTableName_EmptyFieldName_ReturnsNull()
	{
		// Arrange
		const string mainTableName = "users";

		// Act
		string result = ColumnConfiguration.GetTableName(mainTableName, "");

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public void GetTableName_MultipleDots_ReturnsFirstPart()
	{
		// Arrange
		const string mainTableName = "users";
		const string fieldName = "schema.table.ValName";

		// Act
		string result = ColumnConfiguration.GetTableName(mainTableName, fieldName);

		// Assert
		Assert.That(result, Is.EqualTo("schema"));
	}

	[Test]
	public void GetTableName_CaseSensitivity_ReturnsLowerCase()
	{
		// Arrange
		const string mainTableName = "Users";
		const string fieldName = "Profiles.ValName";

		// Act
		string result = ColumnConfiguration.GetTableName(mainTableName, fieldName);

		// Assert
		Assert.That(result, Is.EqualTo("profiles"));
	}

	#endregion

	#region GetColumnName Tests

	[Test]
	public void GetColumnName_QualifiedFieldName_ReturnsColumnName()
	{
		// Arrange
		const string fieldName = "profiles.ValName";

		// Act
		string result = ColumnConfiguration.GetColumnName(fieldName);

		// Assert
		Assert.That(result, Is.EqualTo("name"));
	}

	[Test]
	public void GetColumnName_UnqualifiedFieldName_ReturnsColumnName()
	{
		// Arrange
		const string fieldName = "ValEmail";

		// Act
		string result = ColumnConfiguration.GetColumnName(fieldName);

		// Assert
		Assert.That(result, Is.EqualTo("email"));
	}

	[Test]
	public void GetColumnName_NullFieldName_ReturnsNull()
	{
		// Arrange
		const string fieldName = "";

		// Act
		string result = ColumnConfiguration.GetColumnName(fieldName);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public void GetColumnName_EmptyFieldName_ReturnsNull()
	{
		// Arrange
		const string fieldName = "";

		// Act
		string result = ColumnConfiguration.GetColumnName(fieldName);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public void GetColumnName_NoValPrefix_ReturnsNull()
	{
		// Arrange
		const string fieldName = "InvalidFormat";

		// Act
		string result = ColumnConfiguration.GetColumnName(fieldName);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public void GetColumnName_CaseSensitivity_ReturnsLowerCase()
	{
		// Arrange
		const string fieldName = "ValFirstName";

		// Act
		string result = ColumnConfiguration.GetColumnName(fieldName);

		// Assert
		Assert.That(result, Is.EqualTo("firstname"));
	}

	[Test]
	public void GetColumnName_SpecialCharactersInColumnName_Preserved()
	{
		// Arrange
		const string fieldName = "ValEmail_Address";

		// Act
		string result = ColumnConfiguration.GetColumnName(fieldName);

		// Assert
		Assert.That(result, Is.EqualTo("email_address"));
	}

	#endregion

	#region GetFieldRef Tests

	[Test]
	public void GetFieldRef_QualifiedFieldName_ReturnsFieldRef()
	{
		// Arrange
		const string mainTableName = "users";
		const string fieldName = "profiles.ValName";

		// Act
		FieldRef result = ColumnConfiguration.GetFieldRef(mainTableName, fieldName);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Area, Is.EqualTo("profiles"));
		Assert.That(result.Field, Is.EqualTo("name"));
	}

	[Test]
	public void GetFieldRef_UnqualifiedFieldName_ReturnsFieldRef()
	{
		// Arrange
		const string mainTableName = "users";
		const string fieldName = "ValEmail";

		// Act
		FieldRef result = ColumnConfiguration.GetFieldRef(mainTableName, fieldName);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Area, Is.EqualTo("users"));
		Assert.That(result.Field, Is.EqualTo("email"));
	}

	[Test]
	public void GetFieldRef_NullMainTableName_ReturnsNull()
	{
		// Arrange
		const string fieldName = "profiles.ValName";

		// Act
		FieldRef result = ColumnConfiguration.GetFieldRef(null, fieldName);

		// Assert
		Assert.That(result, Is.Null);
	}

	[Test]
	public void GetFieldRef_NullFieldName_ReturnsNull()
	{
		// Arrange
		const string mainTableName = "users";

		// Act
		FieldRef result = ColumnConfiguration.GetFieldRef(mainTableName, null);

		// Assert
		Assert.That(result, Is.Null);
	}

	#endregion
}
