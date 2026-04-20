namespace CSGenio.core.ai;

using System;
using System.Collections.Generic;
using System.Text;
using Quidgest.Persistence.GenericQuery;

/// <summary>
/// Parses a filter DSL string into a CriteriaSet.
///
/// Syntax:
///   field operator value [AND|OR field operator value]...
///
/// Operators:
///   equals, notEquals, contains, notContains,
///   greaterThan, greaterOrEqual, lessThan, lessOrEqual,
///   in, notIn
///
/// Values:
///   - Strings: "value" or 'value'
///   - Numbers: 123, 45.67
///   - Booleans: true, false
///   - Null: null
///   - Arrays (for in/notIn): ["a", "b", "c"] or [1, 2, 3]
///
/// Examples:
///   name contains "test"
///   name contains "test" AND status equals "active"
///   count greaterThan 5 OR count lessThan 2
///   status in ["active", "pending", "review"]
///   (name contains "test" OR name contains "demo") AND status equals "active"
/// </summary>
public class FilterDslParser
{
    private readonly Func<string, Quidgest.Persistence.FieldRef> _fieldMapper;
    private readonly string[] _allowedFields;
    private string _input;
    private int _pos;

    public FilterDslParser(Func<string, Quidgest.Persistence.FieldRef> fieldMapper, string[] allowedFields)
    {
        _fieldMapper = fieldMapper;
        _allowedFields = allowedFields;
    }

    /// <summary>
    /// Parses a filter DSL string and returns a CriteriaSet
    /// </summary>
    /// <param name="filter">The filter string to parse</param>
    /// <returns>A CriteriaSet representing the filter, or null if the filter is empty</returns>
    public CriteriaSet Parse(string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return null;

        _input = filter.Trim();
        _pos = 0;

        return ParseExpression();
    }

    private CriteriaSet ParseExpression()
    {
        var left = ParseTerm();
        if (left == null)
            return null;

        SkipWhitespace();

        // Check for AND/OR
        while (_pos < _input.Length)
        {
            SkipWhitespace();

            if (TryConsume("AND"))
            {
                SkipWhitespace();
                var right = ParseTerm();
                if (right == null)
                    throw new ArgumentException($"Expected condition after AND at position {_pos}");

                // Combine with AND
                var combined = CriteriaSet.And();
                combined.SubSet(left);
                combined.SubSet(right);
                left = combined;
            }
            else if (TryConsume("OR"))
            {
                SkipWhitespace();
                var right = ParseTerm();
                if (right == null)
                    throw new ArgumentException($"Expected condition after OR at position {_pos}");

                // Combine with OR
                var combined = CriteriaSet.Or();
                combined.SubSet(left);
                combined.SubSet(right);
                left = combined;
            }
            else
            {
                break;
            }
        }

        return left;
    }

    private CriteriaSet ParseTerm()
    {
        SkipWhitespace();

        if (_pos >= _input.Length)
            return null;

        // Check for parenthesized expression
        if (_input[_pos] == '(')
        {
            _pos++; // consume '('
            var inner = ParseExpression();
            SkipWhitespace();
            if (_pos >= _input.Length || _input[_pos] != ')')
                throw new ArgumentException($"Expected ')' at position {_pos}");
            _pos++; // consume ')'
            return inner;
        }

        // Parse a simple condition: field operator value
        return ParseCondition();
    }

    private CriteriaSet ParseCondition()
    {
        SkipWhitespace();

        // Parse field name
        string field = ParseIdentifier();
        if (string.IsNullOrEmpty(field))
            return null;

        // Validate field
        if (!Array.Exists(_allowedFields, f => f.Equals(field, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException($"Invalid field: '{field}'. Allowed fields: {string.Join(", ", _allowedFields)}");
        }

        SkipWhitespace();

        // Parse operator
        string op = ParseOperator();
        if (string.IsNullOrEmpty(op))
            throw new ArgumentException($"Expected operator after field '{field}' at position {_pos}");

        SkipWhitespace();

        // Parse value
        object value = ParseValue();

        // Build criteria
        var fieldRef = _fieldMapper(field);
        var criteria = CriteriaSet.And();

        switch (op.ToLower())
        {
            case "equals":
            case "=":
            case "==":
                criteria.Equal(fieldRef, value);
                break;
            case "notequals":
            case "!=":
            case "<>":
                criteria.NotEqual(fieldRef, value);
                break;
            case "greaterthan":
            case ">":
                criteria.Greater(fieldRef, value);
                break;
            case "greaterorequal":
            case ">=":
                criteria.GreaterOrEqual(fieldRef, value);
                break;
            case "lessthan":
            case "<":
                criteria.Lesser(fieldRef, value);
                break;
            case "lessorequal":
            case "<=":
                criteria.LesserOrEqual(fieldRef, value);
                break;
            case "contains":
            case "like":
                criteria.Like(fieldRef, $"%{value}%");
                break;
            case "notcontains":
            case "notlike":
                criteria.NotLike(fieldRef, $"%{value}%");
                break;
            case "startswith":
                criteria.Like(fieldRef, $"{value}%");
                break;
            case "endswith":
                criteria.Like(fieldRef, $"%{value}");
                break;
            case "in":
                if (value is List<object> inList)
                    criteria.In(fieldRef, inList);
                else
                    throw new ArgumentException("The 'in' operator requires an array value like [\"a\", \"b\"]");
                break;
            case "notin":
                if (value is List<object> notInList)
                    criteria.NotIn(fieldRef, notInList);
                else
                    throw new ArgumentException("The 'notIn' operator requires an array value like [\"a\", \"b\"]");
                break;
            default:
                throw new ArgumentException($"Invalid operator: '{op}'. Allowed: equals, notEquals, contains, notContains, greaterThan, greaterOrEqual, lessThan, lessOrEqual, startsWith, endsWith, in, notIn (or symbols: =, !=, <>, >, >=, <, <=)");
        }

        return criteria;
    }

    private string ParseIdentifier()
    {
        var sb = new StringBuilder();
        while (_pos < _input.Length && (char.IsLetterOrDigit(_input[_pos]) || _input[_pos] == '_'))
        {
            sb.Append(_input[_pos]);
            _pos++;
        }
        return sb.ToString();
    }

    private string ParseOperator()
    {
        SkipWhitespace();

        // Try symbol operators first
        string[] symbolOps = { ">=", "<=", "!=", "<>", "==", "=", ">", "<" };
        foreach (var op in symbolOps)
        {
            if (TryConsume(op))
                return op;
        }

        // Try word operators
        string[] wordOps = { "equals", "notEquals", "contains", "notContains",
                            "greaterThan", "greaterOrEqual", "lessThan", "lessOrEqual",
                            "startsWith", "endsWith", "in", "notIn", "like", "notLike" };

        int startPos = _pos;
        string word = ParseIdentifier();

        foreach (var op in wordOps)
        {
            if (word.Equals(op, StringComparison.OrdinalIgnoreCase))
                return op;
        }

        // Not a valid operator, reset position
        _pos = startPos;
        return null;
    }

    private object ParseValue()
    {
        SkipWhitespace();

        if (_pos >= _input.Length)
            throw new ArgumentException("Expected value at end of input");

        char c = _input[_pos];

        // String value (double or single quotes)
        if (c == '"' || c == '\'')
            return ParseString(c);

        // Array value
        if (c == '[')
            return ParseArray();

        // Number, boolean, or null
        return ParseLiteralValue();
    }

    private string ParseString(char quote)
    {
        var startingPos = _pos;
        _pos++; // consume opening quote
        var sb = new StringBuilder();

        while (_pos < _input.Length)
        {
            char c = _input[_pos];

            if (c == '\\' && _pos + 1 < _input.Length)
            {
                // Escape sequence
                _pos++;
                char escaped = _input[_pos];
                switch (escaped)
                {
                    case 'n': sb.Append('\n'); break;
                    case 't': sb.Append('\t'); break;
                    case 'r': sb.Append('\r'); break;
                    case '\\': sb.Append('\\'); break;
                    case '"': sb.Append('"'); break;
                    case '\'': sb.Append('\''); break;
                    default: sb.Append(escaped); break;
                }
                _pos++;
            }
            else if (c == quote)
            {
                _pos++; // consume closing quote
                return sb.ToString();
            }
            else
            {
                sb.Append(c);
                _pos++;
            }
        }

        throw new ArgumentException($"Unterminated string starting at position {startingPos}");
    }

    private List<object> ParseArray()
    {
        _pos++; // consume '['
        var list = new List<object>();

        SkipWhitespace();

        if (_pos < _input.Length && _input[_pos] == ']')
        {
            _pos++; // empty array
            return list;
        }

        while (_pos < _input.Length)
        {
            SkipWhitespace();
            list.Add(ParseValue());
            SkipWhitespace();

            if (_pos >= _input.Length)
                throw new ArgumentException("Unterminated array");

            if (_input[_pos] == ']')
            {
                _pos++;
                return list;
            }

            if (_input[_pos] == ',')
            {
                _pos++;
                continue;
            }

            throw new ArgumentException($"Expected ',' or ']' in array at position {_pos}");
        }

        throw new ArgumentException("Unterminated array");
    }

    private object ParseLiteralValue()
    {
        // Check for boolean/null keywords
        if (TryConsume("true"))
            return true;
        if (TryConsume("false"))
            return false;
        if (TryConsume("null"))
            return null;

        // Parse number
        var sb = new StringBuilder();
        bool hasDecimal = false;
        bool hasSign = false;

        if (_pos < _input.Length && (_input[_pos] == '-' || _input[_pos] == '+'))
        {
            sb.Append(_input[_pos]);
            _pos++;
            hasSign = true;
        }

        while (_pos < _input.Length)
        {
            char c = _input[_pos];
            if (char.IsDigit(c))
            {
                sb.Append(c);
                _pos++;
            }
            else if (c == '.' && !hasDecimal)
            {
                sb.Append(c);
                _pos++;
                hasDecimal = true;
            }
            else
            {
                break;
            }
        }

        string numStr = sb.ToString();
        if (string.IsNullOrEmpty(numStr) || (hasSign && numStr.Length == 1))
            throw new ArgumentException($"Expected value at position {_pos}");

        if (hasDecimal)
        {
            if (double.TryParse(numStr, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double d))
                return d;
        }
        else
        {
            if (int.TryParse(numStr, out int i))
                return i;
            if (long.TryParse(numStr, out long l))
                return l;
        }

        throw new ArgumentException($"Invalid number: '{numStr}'");
    }

    private void SkipWhitespace()
    {
        while (_pos < _input.Length && char.IsWhiteSpace(_input[_pos]))
            _pos++;
    }

    private bool TryConsume(string text)
    {
        if (_pos + text.Length > _input.Length)
            return false;

        // For word tokens, make sure they're not part of a larger word
        bool isWordToken = char.IsLetter(text[0]);

        for (int i = 0; i < text.Length; i++)
        {
            if (char.ToUpperInvariant(_input[_pos + i]) != char.ToUpperInvariant(text[i]))
                return false;
        }

        // If it's a word token, check that it's followed by a non-letter/digit
        if (isWordToken && _pos + text.Length < _input.Length)
        {
            char next = _input[_pos + text.Length];
            if (char.IsLetterOrDigit(next) || next == '_')
                return false;
        }

        _pos += text.Length;
        return true;
    }

    /// <summary>
    /// Generates a description of the DSL syntax for use in tool schemas
    /// </summary>
    public static string GetSyntaxDescription(string[] fields)
    {
        return $@"Filter expression. Syntax: field operator value [AND|OR field operator value]...
Operators: equals (=), notEquals (!=), contains, notContains, greaterThan (>), greaterOrEqual (>=), lessThan (<), lessOrEqual (<=), startsWith, endsWith, in, notIn.
Values: strings in quotes (""text""), numbers (123), booleans (true/false), arrays for in/notIn ([""a"",""b""]).
Use parentheses for grouping: (a OR b) AND c.
Fields: {string.Join(", ", fields)}.
Examples: name contains ""test"" | status equals ""active"" | count > 5 | status in [""a"",""b""] | (name contains ""x"" OR name contains ""y"") AND active equals true";
    }
}
