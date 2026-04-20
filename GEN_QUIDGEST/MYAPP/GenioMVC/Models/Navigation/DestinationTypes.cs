using System;
using System.Collections.Generic;

using CSGenio.business;
using CSGenio.framework;

namespace GenioMVC.Models.Navigation
{
	/// <summary>
	/// Class Destination
	/// </summary>
	public class DestinationType
	{
		/// <summary>
		/// string with destination name
		/// </summary>
		public string DestinationName { get; set; }

		/// <summary>
		/// Table name
		/// </summary>
		public DbArea DestinationTablename { get; set; }

		/// <summary>
		/// Email field in destination table
		/// </summary>
		public string EmailField { get; set; }
	}

	/// <summary>
	/// Class DestinationTypes
	/// </summary>
	public class DestinationTypes
	{
		/// <summary>
		/// List with destination types
		/// </summary>
		public List<DestinationType> ListDestinationTypes { get; set; }
	}
}
