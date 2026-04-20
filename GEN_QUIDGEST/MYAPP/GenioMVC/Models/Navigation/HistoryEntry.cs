using System;
using System.Collections.Generic;
using System.Linq;

namespace GenioMVC.Models.Navigation
{
	/// <summary>
	/// Represents an entry with a key and a set of values in a history level.
	/// Last updated by [CJP] at [2014.11.04]
	/// The key appears only in the HistoryLevel correspondent to this HistoryEntry.
	/// </summary>
	[Serializable]
	public class HistoryEntry
	{
		/// <summary>
		/// The value for this entry
		/// </summary>
		/// <remarks>
		/// When multiple values exists, returns the first
		/// </remarks>
		public object Value
		{
			get
			{
				return m_values.FirstOrDefault();
			}
			set
			{
				m_values.Clear();
				m_values.Add(value);
			}
		}

		private IList<object> m_values;
		/// <summary>
		/// The collection of values for this entry
		/// </summary>
		public IEnumerable<object> Values
		{
			get
			{
				return m_values;
			}
		}

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="key">The key for this entry</param>
		public HistoryEntry()
		{
			m_values = new List<object>();
		}

		protected HistoryEntry(HistoryEntry another)
		{
			m_values = new List<object>(another.m_values);
		}

		public Object Clone()
		{
			return new HistoryEntry(this);
		}

		/// <summary>
		/// Adds a value to the collection of values of this entry
		/// </summary>
		/// <param name="value">The value to add</param>
		public void AddValue(object value)
		{
			if (m_values.Count > 0)
				m_values.Clear();
			m_values.Add(value);
		}
	}
}
