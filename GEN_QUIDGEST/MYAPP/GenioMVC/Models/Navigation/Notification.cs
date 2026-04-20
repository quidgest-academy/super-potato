using System;
using System.Collections.Generic;

using CSGenio.business;
using CSGenio.framework;

namespace GenioMVC.Models.Navigation
{
	/// <summary>
	/// Class FieldMap
	/// </summary>
	public class FieldMap
	{
		/// <summary>
		/// Field retrieved from ManualQuery
		/// </summary>
		public string FieldnameQuery { get; set; }

		/// <summary>
		/// Field to be used in app
		/// </summary>
		public string FieldnameApp { get; set; }
	}

	/// <summary>
	/// Class Tags that will be available to select from and to use on messages
	/// </summary>
	public class Tag
	{
		/// <summary>
		/// Tag mapping
		/// </summary>
		public FieldMap FieldMap { get; set; }

		/// <summary>
		/// Tag help
		/// </summary>
		public String Help { get; set; }
	}

	/// <summary>
	/// Allowed destinations that will be available
	/// </summary>
	public class AllowedDestination
	{
		/// <summary>
		/// string with destination name
		/// </summary>
		public DestinationType Destination { get; set; }

		/// <summary>
		/// Foreign key fieldname to destination table in ManualQuery
		/// </summary>
		public string FKnameQuery { get; set; }
	}

	/// <summary>
	/// Allowed destinations that will be available
	/// </summary>
	public class MessagesTableMap
	{
		public DbArea MessagesTable;
		public List<FieldMap> TableFieldMap;
	}

	/// <summary>
	/// Interface Notification
	/// </summary>
	public class Notification
	{
		/// <summary>
		/// Notification ID
		/// </summary>
		public String IDNumber { get; set; }

		/// <summary>
		/// Help text
		/// </summary>
		public String Help { get; set; }

		/// <summary>
		/// Genio query reference that controls this notification
		/// </summary>
		public ManualQuery GenioQuery { get; set; }

		/// <summary>
		/// Destinations list with mapping information and text to on each occurrence
		/// </summary>
		public List<Tag> Tags { get; set; }

		/// <summary>
		/// Tag list with mapping information and text to replace on each occurrence
		/// </summary>
		public List<AllowedDestination> AllowedDestinations { get; set; }

		/// <summary>
		/// Table class with information to store messages sent on BD
		/// </summary>
		public MessagesTableMap DatabaseMessagesMapping { get; set; }

		/// <summary>
		/// Alert to be raised
		/// </summary>
		public Alert Alert { get; set; }

		/// <summary>
		/// Flag indication to send an email
		/// </summary>
		public bool SendsEmail { get; set; }

		/// <summary>
		/// Flag indication to send an alert (on webpage alert zone)
		/// </summary>
		public bool SendsAlert { get; set; }

		/// <summary>
		/// Flag indication to register the information sent on database
		/// </summary>
		public bool SendsToDatabase { get; set; }
	}
}
