using CSGenio.core.business;
using CSGenio.core.scheduler;
using CSGenio.framework;
using CSGenio.persistence;

namespace Administration;

/// <summary>
/// Processes a formula group
/// </summary>
public class FormulaGroupScheduledTask : IScheduledTask
{
	/// <inheritdoc/>
	public List<ScheduledTaskOption> GetOptions() =>
	[
		new ScheduledTaskOption
		{
			PropertyName = "yearapp",
			DisplayName = "Year",
			Optional = true,
			Description = "Database year."
		},
		new ScheduledTaskOption
		{
			PropertyName = "groupid",
			DisplayName = "GroupId",
			Optional = false,
			Description = "The identifier of the formula group."
		}
	];

	/// <inheritdoc/>
	public Task Process(Dictionary<string, string> options, CancellationToken stoppingToken)
	{
		string year = ScheduledTaskExtensions.GetStringOption(options, "yearapp", Configuration.DefaultYear);
		string groupId = ScheduledTaskExtensions.GetStringOption(options, "groupid", "");

		var sp = PersistentSupport.getPersistentSupport(year);

		try
		{
			sp.openTransaction();
			FormulaGroup.Execute(sp, groupId);
			sp.closeTransaction();
		}
		catch (Exception e)
		{
			sp.rollbackTransaction();
			Log.Error($"Unexpected error while processing scheduled task of formula group {groupId}: {e}");
		}

		return Task.CompletedTask;
	}
}
