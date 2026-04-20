using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace CSGenio.business
{
	public interface IMapServiceProvider
	{
		string GetToken(IDictionary<string, string> parameters);
	}

	public class ArcGisServiceProvider : IMapServiceProvider
	{
		/// <summary>
		/// Tries to obtain an access token for the ArcGis service, using the data in the specified configuration
		/// </summary>
		/// <param name="parameters">The parameters to use in the request (must contain keys for: baseUrl, tokenPath, username and password)</param>
		/// <returns>A token to access the ArcGis service, or null if some error prevents it's obtainment</returns>
		public string GetToken(IDictionary<string, string> parameters)
		{
			try
			{
				string expiration = "1440"; // Default expiration time of 1 day (in minutes).

				if (parameters.ContainsKey("expiration"))
					expiration = parameters["expiration"];

				using (var client = new HttpClient())
				{
					string url = parameters["baseUrl"] + parameters["tokenPath"];

					var data = new Dictionary<string, string>
					{
						{ "username", parameters["username"] },
						{ "password", parameters["password"] },
						{ "f", "json" },
						{ "client", "referer" },
						{ "referer", parameters["baseUrl"] },
						{ "expiration", expiration }
					};

					var response = client.PostAsync(url, new FormUrlEncodedContent(data)).Result;
					var responseContent = response.Content.ReadAsStringAsync();

					var responseData = (JObject) JsonConvert.DeserializeObject(responseContent.Result);
					return responseData?.SelectToken("token")?.Value<string>();
				}
			}
			catch
			{
				return null;
			}
		}
	}
}
