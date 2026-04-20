using System.Collections.Generic;

using CSGenio.framework;
using CSGenio.business;

namespace GenioMVC.Helpers
{
	public enum TableType
	{
		SimpleTable,
		DBedit,
		DBeditMultipleSelection,
		DBeditQuery,
		DBeditNN,
		List,
		ListUnfiltered,
		CheckList,
		CheckListLimited,
		Multiform,
		GridTableList,
		SearchList,
		Timeline
	}

	public enum LimitAreaType
	{
		AreaLimita,
		AreaLimitaN,
		AreaComparar
	}

	public enum LimitType
	{
		// Form list limits:
		A, //Area
		F, //Fixed
		N, //Fixed (new)
		H, //History
		V, //N:N
		E, //Between dates
		C, //Field

		// Menu limits:
		DB, //Area
		SC, //Condition selection
		SH, //History manipulation
		HM, //History selection
		SL, //Sub-area Logic (Boolean-int)
		SA, //Sub-area Array (Enumeration)
		AC, //Array Choice (Enumeration)
		SE, //Cross-boundary selection
		SU, //Threshold selection
		DM, //Multiple selection
		DC, //N:N List

		// Other limits:
		EPH, //EPH limits
		AFILTER, //Menu "Filter by Area": Filters out records from list that don't have at least one element on the child table related to them.
		OVERRQ //Manual routine with help for limit set in tag OVERRQ
	}

	//Enum to specify operation type (values used in SU Limit, but can be extended)
	public enum OperationType
	{
		EQUAL,
		LESS,
		LESSEQUAL,
		GREAT,
		GREATEQUAL,
		DIFF
	}

	//Class object to hold limitation settings (using same naming as in Genio classes, for clarity purpose)
	public class Limit
	{
		/// <summary>
		/// LimitType above [0]
		/// </summary>
		public LimitType TipoLimite { get; set; }

		/// <summary>
		/// Operation type [1]
		/// </summary>
		public OperationType TipoLimiteSU { get; set; }

		/// <summary>
		/// DbArea Area1 Limit [2]
		/// </summary>
		public Area AreaLimita { get; set; }

		/// <summary>
		/// Field Field 1 Limit [3]
		/// </summary>
		public Field CampoLimita { get; set; }

		/// <summary>
		/// DbArea Area2 Limit [4]
		/// </summary>
		public Area AreaLimitaN { get; set; }

		/// <summary>
		/// Field Field 2 Limit [5]
		/// </summary>
		public Field CampoLimitaN { get; set; }

		/// <summary>
		/// DbArea AreaToCompare Limit [6]
		/// </summary>
		public Area AreaComparar { get; set; }

		/// <summary>
		/// Field FieldToCompare Limit [7]
		/// </summary>
		public Field CampoComparar { get; set; }

		/// <summary>
		/// bool NaoAplicaSeNulo Limit [8]
		/// </summary>
		public bool NaoAplicaSeNulo { get; set; }

		/// <summary>
		/// string ManualHTMLText [9]
		/// </summary>
		public string ManualHTMLText { get; set; }

		/// <summary>
		/// Operation type [10]
		/// </summary>
		public string TipoLimiteOperator { get; set; }

		public Limit() { }
	}

	// Custom comparer for the Limit class
	class LimitComparer : IEqualityComparer<Limit>
	{
		// Limits are equal if their property values (except TipoLimite) are equal
		public bool Equals(Limit x, Limit y)
		{
			//Check whether the compared objects reference the same data.
			if (object.ReferenceEquals(x, y)) return true;

			//Check whether any of the compared objects is null.
			if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
				return false;

			//Check whether the limit properties are equal
			return
				((x.AreaLimita == null     && y.AreaLimita == null)     || (x.AreaLimita != null     && y.AreaLimita != null     && x.AreaLimita.Alias   == y.AreaLimita.Alias)) &&
				((x.AreaLimitaN == null    && y.AreaLimitaN == null)    || (x.AreaLimitaN != null    && y.AreaLimitaN != null    && x.AreaLimitaN.Alias  == y.AreaLimitaN.Alias)) &&
				((x.AreaComparar == null   && y.AreaComparar == null)   || (x.AreaComparar != null   && y.AreaComparar != null   && x.AreaComparar.Alias == y.AreaComparar.Alias)) &&
				((x.CampoLimita == null    && y.CampoLimita == null)    || (x.CampoLimita != null    && y.CampoLimita != null    && x.CampoLimita.Name   == y.CampoLimita.Name)) &&
				((x.CampoLimitaN == null   && y.CampoLimitaN == null)   || (x.CampoLimitaN != null   && y.CampoLimitaN != null   && x.CampoLimitaN.Name  == y.CampoLimitaN.Name)) &&
				((x.CampoComparar == null  && y.CampoComparar == null)  || (x.CampoComparar != null  && y.CampoComparar != null  && x.CampoComparar.Name == y.CampoComparar.Name)) &&
				((x.ManualHTMLText == null && y.ManualHTMLText == null) || (x.ManualHTMLText != null && y.ManualHTMLText != null && x.ManualHTMLText     == y.ManualHTMLText)) &&
				  x.NaoAplicaSeNulo == y.NaoAplicaSeNulo &&
				  x.TipoLimiteSU == y.TipoLimiteSU; //TipoLimite is ignored.
		}

		// If Equals() returns true for a pair of objects
		// then GetHashCode() must return the same value for these objects.
		public int GetHashCode(Limit limit)
		{
			//Check whether the object is null
			if (limit is null)
				return 0;

			//Get hash codes for each property fields if it is not null.
			int hashAreaLimita    = limit.AreaLimita     == null ? 0 : limit.AreaLimita.GetHashCode();
			int hashAreaLimitaN   = limit.AreaLimitaN    == null ? 0 : limit.AreaLimitaN.GetHashCode();
			int hashAreaComparar  = limit.AreaComparar   == null ? 0 : limit.AreaComparar.GetHashCode();
			int hashCampoLimita   = limit.CampoLimita    == null ? 0 : limit.CampoLimita.GetHashCode();
			int hashCampoLimitaN  = limit.CampoLimitaN   == null ? 0 : limit.CampoLimitaN.GetHashCode();
			int hashCampoComparar = limit.CampoComparar  == null ? 0 : limit.CampoComparar.GetHashCode();
			int ManualHTMLText    = limit.ManualHTMLText == null ? 0 : limit.ManualHTMLText.GetHashCode();

			int hashNaoAplicaSeNulo = limit.NaoAplicaSeNulo.GetHashCode();
			int hashTipoLimiteSU    = limit.TipoLimiteSU.GetHashCode();

			//Calculate the hash code for the limit.
			return hashAreaLimita ^
				hashAreaLimitaN ^
				hashAreaComparar ^
				hashCampoLimita ^
				hashCampoLimitaN ^
				hashCampoComparar ^
				ManualHTMLText ^
				hashNaoAplicaSeNulo ^
				hashTipoLimiteSU;
		}
	}

	//Class for limit data as it will be displayed
	public class LimitDisplayData
	{
		/// <summary>
		/// Limit Type
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Area 1
		/// </summary>
		public string Area { get; set; }

		/// <summary>
		/// Area 1 Plural
		/// </summary>
		public string AreaPlural { get; set; }

		/// <summary>
		/// Field 1
		/// </summary>
		public string Field { get; set; }

		/// <summary>
		/// Field 1 Value
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Minimum Value
		/// </summary>
		public string ValueMin { get; set; }

		/// <summary>
		/// Maximum Value
		/// </summary>
		public string ValueMax { get; set; }

		/// <summary>
		/// Area 2
		/// </summary>
		public string AreaN { get; set; }

		/// <summary>
		/// Area 2 Plural
		/// </summary>
		public string AreaNPlural { get; set; }

		/// <summary>
		/// Field 2
		/// </summary>
		public string FieldN { get; set; }

		/// <summary>
		/// Field 2 Value
		/// </summary>
		public string ValueN { get; set; }

		/// <summary>
		/// Area To Compare
		/// </summary>
		public string AreaToCompare { get; set; }

		/// <summary>
		/// Area To Compare Plural
		/// </summary>
		public string AreaToComparePlural { get; set; }

		/// <summary>
		/// Field To Compare
		/// </summary>
		public string FieldToCompare { get; set; }

		/// <summary>
		/// Value To Compare
		/// </summary>
		public string ValueToCompare { get; set; }

		/// <summary>
		/// Operation type
		/// </summary>
		public string OperatorType { get; set; }

		/// <summary>
		/// Operation type for SU limits
		/// </summary>
		public string OperatorThreshold { get; set; }

		/// <summary>
		/// Manual HTML
		/// </summary>
		public string ManualHTMLText { get; set; }

		/// <summary>
		/// Apply limit only if it exists
		/// </summary>
		public string ApplyOnlyIfExists { get; set; }

		public LimitDisplayData()
		{
			this.Type = "";
			this.Area = "";
			this.AreaPlural = "";
			this.Field = "";
			this.Value = "";
			this.ValueMin = "";
			this.ValueMax = "";
			this.AreaN = "";
			this.AreaNPlural = "";
			this.FieldN = "";
			this.ValueN = "";
			this.AreaToCompare = "";
			this.AreaToComparePlural = "";
			this.FieldToCompare = "";
			this.ValueToCompare = "";
			this.OperatorType = "";
			this.OperatorThreshold = "";
			this.ManualHTMLText = "";
			this.ApplyOnlyIfExists = "";
		}
	}
}
