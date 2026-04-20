using System.Collections.Specialized;
using System.Net;

namespace GenioMVC;

/*
public static class GenioServerMiddlewareExtensions
{
    public static IApplicationBuilder UseGenioServer(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GenioServerMiddleware>();
    }
}
*/

public static class HttpContextUtilityExtensions
{
    public static string GetIpAddress(this HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString() ?? "";
    }

    public static string GetHostName(this HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress;
        if (ip is null)
            return string.Empty;

        try
        {
            //this is an expensive sockets operation, we should probably cache the results
            return Dns.GetHostEntry(ip).HostName;
        }
        catch
        {
            return ip.ToString();
        }
    }

    public static bool IsAjaxRequest(this HttpRequest request)
    {
        return request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }

    public static NameValueCollection QueryNameValues(this HttpRequest request)
    {
        var res = new NameValueCollection();
        foreach (var nv in request.Query)
            res.Add(nv.Key, nv.Value);
        return res;
    }

    public static void AddRange<T>(this NameValueCollection nvc, IEnumerable<KeyValuePair<string, T>> collection)
    {
        foreach (var nv in collection)
            nvc.Add(nv.Key, nv.Value?.ToString());
    }
}
