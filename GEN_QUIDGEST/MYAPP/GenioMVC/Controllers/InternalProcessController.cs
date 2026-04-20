using System.Collections.Concurrent;
using System.Diagnostics;
using CSGenio.core.logger;
using CSGenio.framework;
using GenioMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenioMVC.Controllers;

public class InternalProcessController : Controller
{
	private readonly ActivitySource activitySource;
	//private readonly ConcurrentDictionary<string, Activity> activeTraces;
	private readonly ILogger _logger;

	public InternalProcessController(ILoggerFactory loggerFactory)
	{
		//activeTraces = new ConcurrentDictionary<string, Activity>();
		activitySource = new ActivitySource("Genio.Frontend");
		//we create a specialized logger here instead of geniodi so that it can have a different category for the frontend
		_logger = loggerFactory.CreateLogger("Genio.Frontend");
	}

	/// <summary>
	/// Receives the telemetry data (traces and logs) and registers them
	/// based on their type.
	/// </summary>
	/// <param name="timestamp">Timestamp to be converted</param>
	/// <returns>JsonResult with success true or false.</returns>
	[AllowAnonymous]
	public JsonResult RegisterTelemetry([FromBody]TelemetryEvents telemetryData)
	{
		if (telemetryData == null)
			return Json(new { success = true });

		IDisposable ctx = null;
		try
		{
			foreach (TelemetryEvent e in telemetryData.Events)
			{
				switch (e.TelemetryType)
				{
					case "Trace":
						HandleTrace(e);
						break;
					case "ErrorLog":
						ctx = BeginLogScope(e);
						_logger.LogError(e.Message);
						break;
					case "WarningLog":
						ctx = BeginLogScope(e);
						Log.Warning(e.Message);
						break;
					case "InfoLog":
						ctx = BeginLogScope(e);
						Log.Info(e.Message);
						break;
					default:
						Log.Error("Invalid telemetry type received in InternalProcessController RegisterTelemetry - " + e.TelemetryType);
						break;
				}
			}

			return Json(new { success = true });
		}
		catch (Exception e)
		{
			Log.Error("[InternalProcessController.RegisterTelemetry] An error ocurred reading client side telemetry: " + e.Message + "\n\n" + e.StackTrace);
			return Json(new { success = false });
		}
		finally
		{
			// Remove context after being used
			ctx?.Dispose();
		}

		//shared scope initialization for the log events
		IDisposable BeginLogScope(TelemetryEvent e)
		{
			return _logger.BeginScope(new Dictionary<string, object>
						{
							{ "ActionName", e.Origin },
							{ "CallStack", e.CallStack },
							{ "original_timestamp", e.Timestamp }
						});
		}
	}

	/// <summary>
	/// Implements logic that registers a trace depending on it's type.
	/// RequestEvent and ResponseEvent are connected by the "traceId" parameter,
	/// so when registering a request we'll save it as an activeTrace until this later
	/// becomes a parent of a response event.
	/// </summary>
	/// <param name="telemetryEvent">Telemetry event that comes from the frontend.</param>
	/// <returns></returns>
	private void HandleTrace(TelemetryEvent telemetryEvent)
	{
		//DISABLED this code until refactor of the client side traces

		/*
		// We need to force null on the curent activity the
		// method StartActivity assigns the current Activity
		// as the parent for everything, we don't want client
		// side tracing to have its, since it messes up the hierarchy
		Activity.Current = null;

		switch(telemetryEvent.Type)
		{
			case "trace":
				using (var activity = activitySource.StartActivity(telemetryEvent.Origin ?? "ResponseEvent",
					ActivityKind.Internal))
				{
					SetTraceTags(activity, telemetryEvent);
				}
				break;
			case "request":
				var requestActivity = activitySource.StartActivity(telemetryEvent.Origin ?? "RequestEvent",
					ActivityKind.Server);

				if (requestActivity == null)
					break;

				SetTraceTags(requestActivity, telemetryEvent);

				// Add the trace so it can be used as a parent in the ResponseEvent
				activeTraces[telemetryEvent.TraceId] = requestActivity;
				break;
			case "response":
				if (activeTraces.TryGetValue(telemetryEvent.TraceId, out var parentActivity))
				{
					// We don't want to register the parent as the default context
					// In this case, the default context will be the trace request to this
					// method in this controller, so ..../InternalProcess/RegisterTelemetry
					if (parentActivity?.Context != default)
					{
						using (var responseActivity = activitySource.StartActivity(telemetryEvent.Origin ?? "ResponseEvent",
									ActivityKind.Server, parentActivity.Context))
						{
							SetTraceTags(responseActivity, telemetryEvent);
						}

						// Clean up the parent trace if it's done
						if (parentActivity?.Id != null)
						{
							activeTraces.TryRemove(telemetryEvent.TraceId, out _);
							parentActivity.Dispose();
						}

						break;
					}
				}

				// This event has no parent, it shouldn't happen but might as well make sure its registered
				using (var activity = activitySource.StartActivity(telemetryEvent.Origin ?? "ResponseEvent",
					ActivityKind.Server))
				{
					SetTraceTags(activity, telemetryEvent);
				}
				break;
		}
		*/
	}

	/// <summary>
	/// Sets all the context tags in a trace, as well as the start date time and
	/// end date time. In case this information isn't present, it will use the
	/// current date and the duration will be 0.
	/// </summary>
	/// <param name="activity">Trace activity, this is where the event will be registered.</param>
	/// <param name="telemetryEvent">The telemetry event object that comes from the front-end.</param>
	/// <returns></returns>
	private void SetTraceTags(Activity activity, TelemetryEvent telemetryEvent)
	{
		if (activity == null)
			return;

		activity.SetTag("trace_id", telemetryEvent.TraceId);
		activity.SetTag("controller", telemetryEvent.ContextData?.Controller);
		activity.SetTag("action", telemetryEvent.ContextData?.Action);
		activity.SetTag("response_status", telemetryEvent.ResponseStatus);
		activity.SetTag("response_data", telemetryEvent.ResponseData?.ToString());
		activity.SetTag("request_data", telemetryEvent.RequestData?.ToString());
		activity.SetTag("request_type", telemetryEvent.RequestType?.ToString());
		activity.SetTag("request_url", telemetryEvent.RequestUrl?.ToString());
		activity.SetTag("type", telemetryEvent.Type);

		// We want to set a custom start and end time for the event
		// but based on the info we get from the frontend, only the start
		// timestamp is garanteed, there is a change Time will be null
		DateTime startTime = DateTime.UtcNow;
		if (telemetryEvent.Timestamp != null)
			TimestampToDateTime(Convert.ToInt64(telemetryEvent.Timestamp.ToString()) / 1000);

		DateTime endTime = telemetryEvent.Time.HasValue
			? startTime.AddMilliseconds(telemetryEvent.Time.Value)
			: DateTime.UtcNow;

		activity.SetStartTime(startTime);
		activity.SetEndTime(endTime);
	}

	/// <summary>
	/// Converts a Unix timestamp into a C# DateTime.
	/// </summary>
	/// <param name="timestamp">Timestamp to be converted.</param>
	/// <returns>The corresponding DateTime value.</returns>
	private DateTime TimestampToDateTime(long timestamp)
	{
		return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
	}

	~InternalProcessController()
	{
        //DISABLED this code until refactor of the client side traces
		/*
        foreach (var key in activeTraces.Keys)
		{
			if (activeTraces.TryRemove(key, out var activity)) activity?.Dispose();
		}
		*/
	}
}
