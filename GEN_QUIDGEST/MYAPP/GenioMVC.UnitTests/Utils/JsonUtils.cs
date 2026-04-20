using Microsoft.AspNetCore.Mvc;

namespace GenioMVC.UnitTests
{
    public class JsonUtils
    {
        public static void AssertJsonHasValue(string propertyName, ActionResult actualValue, object expected)
        {
            Assert.IsInstanceOf<JsonResult>(actualValue);
            var json = actualValue as JsonResult;
            Assert.IsNotNull(json);
            var value = GetPropertyObject(propertyName, json.Value);
            Assert.That(value, Is.EqualTo(expected));
        }

        public static object? GetPropertyObject(string propertyName, object? value)
        {
            Assert.IsNotNull(value);
            var prop = value.GetType().GetProperty(propertyName);
            Assert.IsNotNull(prop);
            return prop.GetValue(value);
        }

    }
}
