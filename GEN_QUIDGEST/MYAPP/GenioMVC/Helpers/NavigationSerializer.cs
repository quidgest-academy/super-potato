using System.Text.Json;

namespace GenioMVC.Helpers
{
	public class NavigationSerializer
	{

		public static string Serialize<T>(T source)
		{
            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = null;
            options.Converters.Add(new DateTimeJsonConverter());
            options.Converters.Add(new BoolJsonConverter());
            options.Converters.Add(new SelectListJsonConverter());
            options.Converters.Add(new NameValueCollectionJsonConverter());
            options.Converters.Add(new ConcurrentStackJsonConverter<GenioMVC.Models.Navigation.HistoryLevel>());

            return JsonSerializer.Serialize<T>(source, options);
		}

		public static T? Deserialize<T>(string source)
		{
            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = null;
            options.Converters.Add(new DateTimeJsonConverter());
            options.Converters.Add(new BoolJsonConverter());
            options.Converters.Add(new SelectListJsonConverter());
            options.Converters.Add(new NameValueCollectionJsonConverter());
            options.Converters.Add(new ConcurrentStackJsonConverter<GenioMVC.Models.Navigation.HistoryLevel>());

            return JsonSerializer.Deserialize<T>(source, options);
		}
	}
}
