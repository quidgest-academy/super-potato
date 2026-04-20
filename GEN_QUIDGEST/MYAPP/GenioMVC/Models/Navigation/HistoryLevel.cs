using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GenioMVC.Models.Navigation
{
	/// <summary>
	/// Represents a level of history
	/// </summary>
	public class HistoryLevel
	{
		private readonly object m_lock = new object();

		[JsonInclude]
		public string uniqueIdentifier { get; private set; }

		[JsonInclude]
		public int Level { get; private set; }

		/// <summary>
		/// The map of entries (values associated to the key)
		/// </summary>
		[JsonInclude]
		public ConcurrentDictionary<string, object> Entries { get; private set; }

		/*
			TODO:
			If we are going to refactor things it would be better that we never allowed the Entries to be iterated directly.
			We should have methods to search for an entry that avoid this need.
			For example, in this case we could have a method for CheckEntryByPrefix("_filtro") that would return a list of those keys.
			This would increase isolation of the History objects and centralize control over their access.
		*/
		[JsonIgnore]
		public ICollection<string> EntriesKeys
		{
			get
			{
				return Entries?.Keys ?? new List<string>();
			}
		}

		public ReadOnlyDictionary<string, object> GetEntries()
		{
			return new ReadOnlyDictionary<string, object>(Entries);;
		}

		public ConcurrentDictionary<string, object> GetEntriesClone()
		{
			return CloneData(Entries);
		}

		/// <summary>
		/// The mode of the form for this level
		/// </summary>
		[JsonInclude]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public FormMode FormMode { get; private set; }

		/// <summary>
		/// The location of the form for this level
		/// </summary>
		[JsonInclude]
		public NavigationLocation Location { get; private set; }

		/// <summary>
		/// Caches the human key descriptor for usage in breadcrumbs
		/// </summary>
		public string HumanRoutingDescriptionCache { get; set; }

		public HistoryLevel()
		{
			Entries = new ConcurrentDictionary<string, object>();
		}

		public HistoryLevel(NavigationLocation location, FormMode formMode, int level = 0) : this()
		{
			Location = location;
			FormMode = formMode;
			Entries = new ConcurrentDictionary<string, object>();
			Level = level;
		}

		/// <summary>
		/// Sets the location for this HistoryLevel
		/// </summary>
		/// <param name="location">The location of the form for this level</param>
		public void SetLocation(NavigationLocation location)
		{
			lock (m_lock)
			{
				this.Location = location;
			}
		}

		/// <summary>
		/// Sets the form mode for this HistoryLevel
		/// </summary>
		/// <param name="mode">The mode of the form for this level</param>
		public void SetMode(FormMode mode)
		{
			lock (m_lock)
			{
				this.FormMode = mode;
			}
		}

		/// <summary>
		/// Deep copy of any kind of object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		private T CloneData<T>(T source)
		{
			if (Object.ReferenceEquals(source, null))
				throw new ArgumentNullException("source", "Error during navigation cloning");

			var serializedData = Helpers.NavigationSerializer.Serialize(source);
			var clonedData = Helpers.NavigationSerializer.Deserialize<T>(serializedData);

			return clonedData;
		}

		/// <summary>
		/// Deep copy of the HistoryLevel object
		/// </summary>
		/// <returns>HistoryLevel</returns>
		public HistoryLevel Clone()
		{
			lock (m_lock)
			{
				return CloneData(this);
			}
		}

		/// <summary>
		/// Remove all Entries in this history level
		/// </summary>
		internal void ClearEntries()
		{
			Entries.Clear();
		}

		/// <summary>
		/// Set Entry by key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void SetEntry(string key, object value)
		{
			Entries.AddOrUpdate(key, value, (k, oldValue) => value);
		}

		/// <summary>
		/// Check if Entries contains Key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool CheckEntry(string key)
		{
			return Entries != null && Entries.ContainsKey(key);
		}

		/// <summary>
		/// Get Entry by key
		/// </summary>
		/// <param name="key"></param>
		/// <returns>return null if not contains key</returns>
		public object GetEntry(string key)
		{
			if (Entries.TryGetValue(key, out object value))
				return value;
			return null;
		}

		/// <summary>
		/// Get Entry of specific type by key
		/// </summary>
		/// <param name="key"></param>
		/// <returns>return null if not contains key</returns>
		public T GetEntry<T>(string key)
		{
			object hValue = GetEntry(key);
			var outType = typeof(T);

			if (hValue is JsonElement jObjValue)
			{
				// The «NameValueCollection» needs a special conversion from the JObject.
				if (outType == typeof(System.Collections.Specialized.NameValueCollection))
				{
					try
					{
						var result = new System.Collections.Specialized.NameValueCollection();
						var jObj = jObjValue.Deserialize<Dictionary<string, string[]>>();

						foreach (var item in jObj)
							foreach (var itemValue in item.Value)
								result.Add(item.Key, itemValue);

						return (T)(result as object);
					}
					catch
					{
						return (T)(null as object);
					}
				}

				// Check if the requested type is DateTime or DateTime? and the JsonElement is a string (assumed to be in ISO format)
				else if ((outType == typeof(DateTime) || outType == typeof(DateTime?)) && jObjValue.ValueKind == JsonValueKind.String)
				{
					// Try to parse the JsonElement string to a DateTime object
					bool parseSuccess = DateTime.TryParse(jObjValue.GetString(), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime parseResult);

					// If parsing is not successful and the expected type is DateTime?, return null
					// 	This handling for DateTime? is only for compatibility with ViewModel binding.
					// 	It needs analysis as ideally, the framework base should always operate with non-nullable DateTime.
					if (!parseSuccess && outType == typeof(DateTime?))
						return (T)(null as object);

					// If parsing is successful or the expected type is non-nullable DateTime, return the parsed result
					// Note: In case of non-nullable DateTime and unsuccessful parsing, this will return the default value of DateTime
					return (T)(parseResult as object);
				}

				// Check if the requested type is a boolean
				else if (outType == typeof(bool))
				{
					// Directly return the boolean value if the JsonElement is of boolean type
					if (jObjValue.ValueKind == JsonValueKind.True || jObjValue.ValueKind == JsonValueKind.False)
						return (T)(jObjValue.GetBoolean() as object);
					// If the JsonElement is a number, consider it true if it's equal to 1
					else if (jObjValue.ValueKind == JsonValueKind.Number)
						return (T)((jObjValue.GetDouble() == 1) as object);
					// If the JsonElement is a string, consider it true if it's equal to "1"
					else if (jObjValue.ValueKind == JsonValueKind.String)
						return (T)((jObjValue.GetString() == "1") as object);
					else return (T)(false as object);
				}

				// Handling when the method is called with a generic object type.
				//	most of the time this method gets called with T == object
				//	which makes it impossible to extract the actual value just by casting it to T
				// This code uses the ValueKind of the underlying JsonElement to retrieve
				//	a compatible object type with what the framework expects
				else if (outType == typeof(object))
				{
					// Convert JsonElement to the appropriate type based on its ValueKind
					if (jObjValue.ValueKind == JsonValueKind.String)
						return (T)(jObjValue.GetString() as object);
					else if (jObjValue.ValueKind == JsonValueKind.Number)
						return (T)(jObjValue.GetDecimal() as object);
					else if (jObjValue.ValueKind == JsonValueKind.True)
						return (T)(true as object);
					else if (jObjValue.ValueKind == JsonValueKind.False)
						return (T)(false as object);
				}

				// Default deserialization for other types
				return jObjValue.Deserialize<T>();
			}
			else if (hValue is JsonObject jTokenValue)
				return jTokenValue.Deserialize<T>();
			// Avoid casting errors when an array is expected but a different type is provided
			else if (hValue?.GetType() == typeof(object[]) && outType != typeof(object[]) && outType.IsArray)
				return (T)hValue;

			// Default case to return the value as is
			return (T)hValue;
		}

		/// <summary>
		/// Remove Entry by key
		/// </summary>
		/// <param name="key"></param>
		public void RemoveEntry(string key)
		{
			lock(m_lock)
			{
				Entries.TryRemove(key, out object value);
			}
		}

		/// <summary>
		/// Replace entries
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void ReplaceEntries(IDictionary<string, object> newEntries)
		{
			lock (m_lock)
			{
				Entries = new ConcurrentDictionary<string, object>(newEntries);
			}
		}

		/// <summary>
		/// MH (06-11-2015) - To que for possivel usar fields das tables nos titulos,
		/// apareceu necesidade de change também o string do menu de navegação.
		/// </summary>
		/// <param name="name"></param>
		public void redefineLocName(string name)
		{
			lock (m_lock)
			{
				this.Location = this.Location.redefineName(name);
			}
		}
	}
}
