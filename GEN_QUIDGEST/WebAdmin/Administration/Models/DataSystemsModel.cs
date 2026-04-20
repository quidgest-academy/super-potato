namespace Administration.Models
{
	/// <summary>
	/// Auxiliary class that encapsulates a dataSystem's information.
	/// </summary>
	public class DataSystemParams
	{
		public string Year { get; set; }
		public string DbName { get; set; }
		public string DbType { get; set; }
		public string DbServer { get; set; }
		public int DbVersion { get; set; }
		public bool Configured { get; set; }

		internal DataSystemParams(string year, string dbName, string dbType, string dbServer, int dbVersion, bool configured)
		{
			Year = year;
			DbName = dbName;
			DbType = dbType ?? "";
			DbServer = dbServer ?? "";
			DbVersion = dbVersion;
			Configured = configured;
		}
	}    
}