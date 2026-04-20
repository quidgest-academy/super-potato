using System;

namespace CSGenio.framework
{
	/// <summary>
	/// Possible states for request results
	/// </summary>
	[Serializable]
	public sealed class Status
	{
        /// <summary>
        /// Error
        /// </summary>
        public readonly static Status E = new Status("E");
        /// <summary>
        /// Success
        /// </summary>
        public readonly static Status OK = new Status("OK");
        /// <summary>
        /// Success and there are more pages of records
        /// </summary>
        public readonly static Status OK_MAIS = new Status("OK+");
        /// <summary>
        /// Success and there are more pages of records and previous pages of records
        /// </summary>
        public readonly static Status OK_MAIS_MENOS = new Status("OK+-");
		/// <summary>
		/// Success and there are previous pages of records
		/// </summary>
		public readonly static Status OK_MENOS = new Status("OK-");
		/// <summary>
		/// Used in null requests or empty function
		/// </summary>
		public readonly static Status INIT = new Status("INIT");
		/// <summary>
		/// Warnings
		/// </summary>
        public readonly static Status W = new Status("W");
		/// <summary>
		/// Error with warning
		/// </summary>
        public readonly static Status EW = new Status("EW");
		/// <summary>
		/// Success but shows warning
		/// </summary>
        public readonly static Status OK_MAIS_W = new Status("OK+W");
		/// <summary>
		/// Empty state, only used when there is no state defined
		/// </summary>
        public readonly static Status VAZ = new Status("");

		private readonly string id;	

		public Status(string anID)
		{
			id = anID;
		}

		public override string ToString()
		{
			return id;
		}

		public override bool Equals(object obj)
		{
			if(obj is Status s)
				return s.ToString().Equals(id);
			return false;
		}

		public override int GetHashCode()
		{
			return id.GetHashCode();
		}
	}
}
