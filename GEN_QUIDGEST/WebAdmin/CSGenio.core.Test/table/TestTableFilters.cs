using CSGenio.business;
using CSGenio.core.framework.table;
using Quidgest.Persistence;

namespace CSGenio.core.Test.table;

public class TestTableFilters
{
	#region GroupFilter Tests

	[Test]
	public void GroupFilter_Clone_CreatesDeepCopy()
	{
		// Arrange
		var original = new GroupFilter
		{
			Name = "TestFilter",
			Key = "status",
			Value = "active",
			IsActive = true
		};

		// Act
		var cloned = original.Clone() as GroupFilter;

		// Assert
		Assert.That(cloned, Is.Not.Null);
		Assert.That(cloned.Name, Is.EqualTo(original.Name));
		Assert.That(cloned.Key, Is.EqualTo(original.Key));
		Assert.That(cloned.Value, Is.EqualTo(original.Value));
		Assert.That(cloned, Is.Not.SameAs(original));
	}

	[Test]
	public void GroupFilter_CloneWithSubFilters_ClonesSubFiltersRecursively()
	{
		// Arrange
		var subFilter = new GroupFilter { Name = "SubFilter", Key = "type", Value = "premium" };
		var original = new GroupFilter
		{
			Name = "ParentFilter",
			Key = "status",
			Value = "active",
			SubFilters = [subFilter]
		};

		// Act
		var cloned = original.Clone() as GroupFilter;

		// Assert
		Assert.That(cloned, Is.Not.Null);
		Assert.That(cloned.SubFilters, Has.Count.EqualTo(1));
		Assert.That(cloned.SubFilters[0], Is.Not.SameAs(subFilter));
		Assert.That(cloned.SubFilters[0].Name, Is.EqualTo("SubFilter"));
	}

	#endregion

	#region ActiveFilter Tests

	[Test]
	public void ActiveFilter_Clone_CreatesDeepCopy()
	{
		// Arrange
		var original = new ActiveFilter
		{
			Name = "TimeFilter",
			Date = "2024-01-01",
			Current = true,
			Previous = false,
			Upcoming = true
		};

		// Act
		var cloned = original.Clone() as ActiveFilter;

		// Assert
		Assert.That(cloned, Is.Not.Null);
		Assert.That(cloned.Name, Is.EqualTo(original.Name));
		Assert.That(cloned.Date, Is.EqualTo(original.Date));
		Assert.That(cloned.Current, Is.EqualTo(original.Current));
		Assert.That(cloned.Previous, Is.EqualTo(original.Previous));
		Assert.That(cloned.Upcoming, Is.EqualTo(original.Upcoming));
		Assert.That(cloned, Is.Not.SameAs(original));
	}

	#endregion

	#region ColumnFilter Tests

	[Test]
	public void ColumnFilter_Clone_CreatesDeepCopy()
	{
		// Arrange
		var original = new ColumnFilter
		{
			Name = "EmailFilter",
			Field = "users.email",
			Operator = "contains",
			Values = ["test@example.com", "admin@example.com"]
		};

		// Act
		var cloned = original.Clone() as ColumnFilter;

		// Assert
		Assert.That(cloned, Is.Not.Null);
		Assert.That(cloned.Name, Is.EqualTo(original.Name));
		Assert.That(cloned.Field, Is.EqualTo(original.Field));
		Assert.That(cloned.Operator, Is.EqualTo(original.Operator));
		Assert.That(cloned.Values, Has.Count.EqualTo(2));
		Assert.That(cloned, Is.Not.SameAs(original));
	}

	[Test]
	public void ColumnFilter_CloneValues_CreatesNewList()
	{
		// Arrange
		var original = new ColumnFilter
		{
			Field = "status",
			Values = ["active", "pending"]
		};

		// Act
		var cloned = original.Clone() as ColumnFilter;

		// Assert
		Assert.That(cloned, Is.Not.Null);
		cloned.Values.Add("inactive");
		Assert.Multiple(() =>
		{
			Assert.That(original.Values, Has.Count.EqualTo(2));
			Assert.That(cloned.Values, Has.Count.EqualTo(3));
		});
	}

	#endregion
}
