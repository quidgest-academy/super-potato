using CSGenio.business;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;
using System.Text.Json.Serialization;

namespace GenioMVC.Models;

public enum KanbanEventType
{
	DragDrop
}

public enum KanbanElementType
{
	Column,
	Card
}

public class RequestKanbanModel
{
	public string SourceKey { get; set; }
	public string DestinationKey { get; set; }
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public KanbanEventType EventType { get; set; }
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public KanbanElementType ElementType { get; set; }
	public int NewOrder { get; set; }
}