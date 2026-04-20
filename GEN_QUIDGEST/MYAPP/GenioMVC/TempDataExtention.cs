using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;

namespace GenioMVC;

public static class TempDataExtention
{
    /// <summary>
    /// Puts an object into the TempData by first serializing it to JSON.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tempData"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void SetObject<T>(this ITempDataDictionary tempData, string key, T value) where T : class
    {
        if (typeof(T) == typeof(string))
            //string optimized to not pass through serialization
            tempData[key] = value;
        else
            tempData[key] = JsonSerializer.Serialize(value);
    }

    /// <summary>
    /// Gets an object from the TempData by deserializing it from JSON.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tempData"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T? GetObject<T>(this ITempDataDictionary tempData, string key) where T : class
    {
        tempData.TryGetValue(key, out object? o);
        if(o == null)
            return null;
        //string optimized to not pass through serialization
        if (typeof(T) == typeof(string))
            return o as T;
        if (o is string os)
            return JsonSerializer.Deserialize<T>(os);
        else 
            return null;
    }
}
