namespace GenioMVC;

/// <summary>
/// If your application is behind a proxy, any absolute links you send as a message result need to take that external url into account
/// This class handles those use cases, like redirecting back to a client side vue page, or supplying external authentication callbacks
/// </summary>

public static class AbsoluteUrlUtils
{
    public static string ProxyUrl { get; set; }

    public static string RelativeToAbsolute(HttpRequest request, string relativeUrl)
    {
        //If there is no proxy configured try to follow the request origin
        if (string.IsNullOrEmpty(ProxyUrl)) 
        {
            return $"{request.Scheme}://{request.Host}{relativeUrl}";
        }
        //Behind a proxy, replace the absolute path with the Proxy url and add back the relative part
        else
        {
            // when deployed in iis the action path might come with the PathBase included
            // We will want to remove just that fragment of the path and keep the rest
            if (request.PathBase.HasValue && relativeUrl.StartsWith(request.PathBase, StringComparison.OrdinalIgnoreCase))
                relativeUrl = relativeUrl.Substring(request.PathBase.Value.Length);
            return ProxyUrl + relativeUrl;
        }
    }
}
