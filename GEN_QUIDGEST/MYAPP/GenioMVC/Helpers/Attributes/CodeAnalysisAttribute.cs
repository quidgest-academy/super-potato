using System;
namespace GenioMVC.Helpers
{
    /// <summary>
    /// Attribute to indicate that direct set access to the property should be validated by an analyzer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidateSetAccessAttribute : Attribute
    {
    }
}