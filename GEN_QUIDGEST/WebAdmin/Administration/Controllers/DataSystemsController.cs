using Administration.Models;
using CSGenio;
using CSGenio.core.persistence;
using CSGenio.framework;
using CSGenio.persistence;
using Microsoft.AspNetCore.Mvc;

namespace Administration.Controllers
{
	public class DataSystemsController(CSGenio.config.IConfigurationManager configManager) : ControllerBase
	{
		private ConfigurationXML configXML { 
			get 
			{
				try { 
					return configManager.GetExistingConfig(); 
				}
				catch(Exception) { 
					return null;
				}
			} 
		}

		[HttpGet]
		public IActionResult GetDataSystemsInfo()
		{
			if (configXML == null)
				return Json(new { Success = false, message = Resources.Resources.NAO_FOI_ENCONTRADO_N65044 });

			// Get data systems parameters
			List<DataSystemXml> dataSystems = configXML.DataSystems;
			// Get database versions for each data system
			Dictionary<string, int> dbVersions = GetDataSystemDbVersion();

			// Get default data system
			string defaultYear = CSGenio.framework.Configuration.DefaultYear;

			List<DataSystemParams> data = dataSystems.Select(ds => new DataSystemParams(ds.Name, ds.Schemas[0].Schema, ds.Type, ds.Server, dbVersions[ds.Name], CheckDataSystemInfo(ds))).ToList();
			return Ok(new { Success = true, data = new { dataSystemsInfo = data, defaultYear } });
		}


		/// <summary>
		/// Gets the version of a data system's database.
		/// </summary>
		/// <param name="dbYear">The year of the DB associated with the data system. If not given, the method returns the versions of all data systems in configuration.xml</param>
		/// <returns>A dictionary with the version of the given database, or all databases of existing data systems.</returns>
		private Dictionary<string, int> GetDataSystemDbVersion(string dbYear = "")
		{
			// List of years to get the database version from
			List<string> dbYears = new List<string>();
			// Dictionary with the database versions (dbName: dbVersion)
			Dictionary<string, int> dbVersions = new Dictionary<string, int>();

			if (!string.IsNullOrEmpty(dbYear))
				dbYears.Add(dbYear);
			else
				dbYears.AddRange(CSGenio.framework.Configuration.Years);

			foreach (string year in dbYears)
			{
				// If data system configuration hasn't been saved, default to DBVersion 0
				DataSystemXml dataSystemInfo = configXML.DataSystems.FirstOrDefault(ds => ds.Name == year);
				if (dataSystemInfo == null || !CheckDataSystemInfo(dataSystemInfo))
				{
					dbVersions.Add(year, 0);
					continue;
				}

				PersistentSupport ps = PersistentSupport.getPersistentSupport(year);

				// If the DB doesn't exist, default to DBVersion 0
				if (!ps.CheckIfDatabaseExists())
				{
					dbVersions.Add(year, 0);
					continue;
				}

				DatabaseVersionReader dbVersionReader = new DatabaseVersionReader(ps);
				try
				{
					ps.openConnection();
					dbVersions.Add(year, dbVersionReader.GetDbVersion());
				}
				catch (Exception)
				{
					dbVersions.Add(year, 0);
				}
				finally
				{
					ps.closeConnection();
				}
			}

			return dbVersions;
		}


		/// <summary>
		/// Checks the validity of the configuration of a given data system.
		/// </summary>
		/// <param name="dataSystemInfo">The data system configuration</param>
		/// <returns>True if the data system has all mandatory fields configured, false otherwise.</returns>		
		private bool CheckDataSystemInfo(DataSystemXml dataSystemInfo)
		{
			return !string.IsNullOrEmpty(dataSystemInfo.Type) 
				&& !string.IsNullOrEmpty(dataSystemInfo.Server) 
				&& !string.IsNullOrEmpty(dataSystemInfo.Login) 
				&& !string.IsNullOrEmpty(dataSystemInfo.Password);
		}
	}
}