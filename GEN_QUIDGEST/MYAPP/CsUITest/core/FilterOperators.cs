namespace quidgest.uitests.core;

public class FilterOperators
{
    public class Text
    {
        public const string Equal = "is equal to";
        public const string NotEqual = "is not equal to";
        public const string Contains = "contains";
        public const string DoesNotContain = "does not contain";
        public const string StartsWith = "starts with";
        public const string Like = "is like";
        public const string HasValue = "has value";
        public const string HasNoValue = "has no value";
    }

    public class Number
    {
        public const string Equal = "is equal to";
        public const string NotEqual = "is not equal to";
        public const string GreaterThan = "is greater than";
        public const string LessThan = "is less than";
        public const string GreaterOrEqual = "is greater than or equal to";
        public const string LessOrEqual = "is less than or equal to";
        public const string Between = "is between";
        public const string HasValue = "has value";
        public const string HasNoValue = "has no value";
    }

    public class Date
    {
        public const string Equal = "is equal to";
        public const string NotEqual = "is not equal to";
        public const string AfterOrEqual = "is after or equal to";
        public const string BeforeOrEqual = "is before or equal to";
        public const string Between = "is between";
        public const string HasValue = "has value";
        public const string HasNoValue = "has no value";
    }

    public class Enumeration
    {
        public const string Equal = "is";
        public const string NotEqual = "is not";
        public const string Between = "is in";
        public const string HasValue = "has value";
        public const string HasNoValue = "has no value";
    }
}
