using System;
using System.Data;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// Helper methods to create sql functions for queries
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public class SqlFunctions
    {
        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Add"/> function with the specified arguments
        /// </summary>
        /// <param name="arguments">The arguments for the function</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Add"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Add(params object[] arguments)
        {
            return new SqlFunction(SqlFunctionType.Add, arguments);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Subtract"/> function with the specified arguments
        /// </summary>
        /// <param name="arguments">The arguments for the function</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Subtract"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Subtract(params object[] arguments)
        {
            return new SqlFunction(SqlFunctionType.Subtract, arguments);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Multiply"/> function with the specified arguments
        /// </summary>
        /// <param name="arguments">The arguments for the function</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Multiply"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Multiply(params object[] arguments)
        {
            return new SqlFunction(SqlFunctionType.Multiply, arguments);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Divide"/> function with the specified arguments
        /// </summary>
        /// <param name="dividendTableAlias">The table alias of the dividend argument</param>
        /// <param name="dividendColumn">The column of the dividend argument</param>
        /// <param name="divisorTableAlias">The table alias of the divisor argument</param>
        /// <param name="divisorColumn">The column of the divisor argument</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Divide"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Divide(string dividendTableAlias, string dividendColumn, string divisorTableAlias, string divisorColumn)
        {
            return Divide(new ColumnReference(dividendTableAlias, dividendColumn), new ColumnReference(divisorTableAlias, divisorColumn));
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Divide"/> function with the specified arguments
        /// </summary>
        /// <param name="dividendTableAlias">The table alias of the dividend argument</param>
        /// <param name="dividendColumn">The column of the dividend argument</param>
        /// <param name="divisor">The divisor argument</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Divide"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Divide(string dividendTableAlias, string dividendColumn, object divisor)
        {
            return Divide(new ColumnReference(dividendTableAlias, dividendColumn), divisor);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Divide"/> function with the specified arguments
        /// </summary>
        /// <param name="dividend">The dividend argument</param>
        /// <param name="divisor">The divisor argument</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Divide"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Divide(object dividend, object divisor)
        {
            return new SqlFunction(SqlFunctionType.Divide, dividend, divisor);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Divide"/> function with the specified arguments
        /// </summary>
        /// <param name="dividendField">The field of the dividend argument</param>
        /// <param name="divisorField">The column of the divisor argument</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Divide"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Divide(FieldRef dividendField, FieldRef divisorField)
        {
            return Divide(new ColumnReference(dividendField.Area, dividendField.Field), new ColumnReference(divisorField.Area, divisorField.Field));
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Divide"/> function with the specified arguments
        /// </summary>
        /// <param name="dividendField">The field of the dividend argument</param>
        /// <param name="divisor">The divisor argument</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Divide"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Divide(FieldRef dividendField, object divisor)
        {
            return Divide(new ColumnReference(dividendField.Area, dividendField.Field), divisor);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Count"/> function with the specified arguments
        /// </summary>
        /// <param name="arguments">The arguments for the function</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Count"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Count(params object[] parameters)
        {
            return new SqlFunction(SqlFunctionType.Count, parameters);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Average"/> function with the specified arguments
        /// </summary>
        /// <param name="tableAlias">The alias of the table</param>
        /// <param name="column">The name of the column</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Average"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Average(string tableAlias, string column)
        {
            return Average(new ColumnReference(tableAlias, column));
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Average"/> function with the specified arguments
        /// </summary>
        /// <param name="parameter">The expression for the average</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Average"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Average(object parameter)
        {
            return new SqlFunction(SqlFunctionType.Average, parameter);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Average"/> function with the specified arguments
        /// </summary>
        /// <param name="field">The field</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Average"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Average(FieldRef field)
        {
            return Average(new ColumnReference(field.Area, field.Field));
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Max"/> function with the specified arguments
        /// </summary>
        /// <param name="tableAlias">The alias of the table</param>
        /// <param name="column">The name of the column</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Max"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Max(string tableAlias, string column)
        {
            return Max(new ColumnReference(tableAlias, column));
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Max"/> function with the specified arguments
        /// </summary>
        /// <param name="parameter">The expression for the max</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Max"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Max(object parameter)
        {
            return new SqlFunction(SqlFunctionType.Max, parameter);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Max"/> function with the specified arguments
        /// </summary>
        /// <param name="field">The field</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Max"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Max(FieldRef field)
        {
            return Max(new ColumnReference(field.Area, field.Field));
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Min"/> function with the specified arguments
        /// </summary>
        /// <param name="tableAlias">The alias of the table</param>
        /// <param name="column">The name of the column</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Min"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Min(string tableAlias, string column)
        {
            return Min(new ColumnReference(tableAlias, column));
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Min"/> function with the specified arguments
        /// </summary>
        /// <param name="field">The field</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Min"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Min(FieldRef field)
        {
            return Min(new ColumnReference(field.Area, field.Field));
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Min"/> function with the specified arguments
        /// </summary>
        /// <param name="parameter">The expression for the min</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Min"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Min(object parameter)
        {
            return new SqlFunction(SqlFunctionType.Min, parameter);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Sum"/> function with the specified arguments
        /// </summary>
        /// <param name="tableAlias">The alias of the table</param>
        /// <param name="column">The name of the column</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Sum"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Sum(string tableAlias, string column)
        {
            return Sum(new ColumnReference(tableAlias, column));
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Sum"/> function with the specified arguments
        /// </summary>
        /// <param name="parameter">The expression for the min</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Sum"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Sum(object parameter)
        {
            return new SqlFunction(SqlFunctionType.Sum, parameter);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Sum"/> function with the specified arguments
        /// </summary>
        /// <param name="field">The field</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Sum"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Sum(FieldRef field)
        {
            return Sum(new ColumnReference(field.Area, field.Field));
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Sum"/> function with the specified arguments
        /// </summary>
        /// <param name="tableAlias">The table alias of the value we want the substring from</param>
        /// <param name="column">The column of the value we want the substring from</param>
        /// <param name="start">The substring start index</param>
        /// <param name="length">The substring length</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Sum"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Substring(string tableAlias, string column, object start, object length)
        {
            return Substring(new ColumnReference(tableAlias, column), start, length);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Substring"/> function with the specified arguments
        /// </summary>
        /// <param name="value">The value we want the substring from</param>
        /// <param name="start">The substring start index</param>
        /// <param name="length">The substring length</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Substring"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Substring(object value, object start, object length)
        {
            return new SqlFunction(SqlFunctionType.Substring, value, start, length);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Sum"/> function with the specified arguments
        /// </summary>
        /// <param name="field">The field we want the substring from</param>
        /// <param name="start">The substring start index</param>
        /// <param name="length">The substring length</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Sum"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Substring(FieldRef field, object start, object length)
        {
            return Substring(new ColumnReference(field.Area, field.Field), start, length);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Locate"/> function with the specified arguments
        /// </summary>
        /// <param name="tableAlias">The table alias of the value where we want to locate the search string</param>
        /// <param name="column">The column of the value where we want to locate the search string</param>
        /// <param name="search">The search string</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Locate"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Locate(string tableAlias, string column, object search)
        {
            return Locate(new ColumnReference(tableAlias, column), search, 1);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Locate"/> function with the specified arguments
        /// </summary>
        /// <param name="value">The value where we want to locate the search string</param>
        /// <param name="search">The search string</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Locate"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Locate(object value, object search)
        {
            return Locate(value, search, 1);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Locate"/> function with the specified arguments
        /// </summary>
        /// <param name="tableAlias">The table alias of the value where we want to locate the search string</param>
        /// <param name="column">The column of the value where we want to locate the search string</param>
        /// <param name="search">The search string</param>
        /// <param name="startIndex">The search start index</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Locate"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Locate(string tableAlias, string column, object search, int startIndex)
        {
            return Locate(new ColumnReference(tableAlias, column), search, startIndex);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Locate"/> function with the specified arguments
        /// </summary>
        /// <param name="value">The value where we want to locate the search string</param>
        /// <param name="search">The search string</param>
        /// <param name="startIndex">The search start index</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Locate"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.06.28
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Locate(object value, object search, int startIndex)
        {
            return new SqlFunction(SqlFunctionType.Locate, value, search, startIndex);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Locate"/> function with the specified arguments
        /// </summary>
        /// <param name="field">The field where we want to locate the search string</param>
        /// <param name="search">The search string</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Locate"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Locate(FieldRef field, object search)
        {
            return Locate(new ColumnReference(field.Area, field.Field), search, 1);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.Locate"/> function with the specified arguments
        /// </summary>
        /// <param name="field">The field where we want to locate the search string</param>
        /// <param name="search">The search string</param>
        /// <param name="startIndex">The search start index</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Locate"/> type</returns>
        /// <remarks>
        /// <!--
        /// Author: CX 2011.07.14
        /// Modified:
        /// Reviewed:
        /// -->
        /// </remarks>
        public static SqlFunction Locate(FieldRef field, object search, int startIndex)
        {
            return Locate(new ColumnReference(field.Area, field.Field), search, startIndex);
        }

        public static SqlFunction Trim(string tableAlias, string column)
        {
            return Trim(new ColumnReference(tableAlias, column));
        }

        public static SqlFunction Trim(object value)
        {
            return new SqlFunction(SqlFunctionType.Trim, value);
        }

        public static SqlFunction Trim(FieldRef field)
        {
            return Trim(new ColumnReference(field.Area, field.Field));
        }

        public static SqlFunction Length(string tableAlias, string column)
        {
            return Length(new ColumnReference(tableAlias, column));
        }

        public static SqlFunction Length(object value)
        {
            return new SqlFunction(SqlFunctionType.Length, value);
        }

        public static SqlFunction Length(FieldRef field)
        {
            return Length(new ColumnReference(field.Area, field.Field));
        }

        public static SqlFunction BitLength(string tableAlias, string column)
        {
            return BitLength(new ColumnReference(tableAlias, column));
        }

        public static SqlFunction BitLength(object value)
        {
            return new SqlFunction(SqlFunctionType.BitLength, value);
        }

        public static SqlFunction BitLength(FieldRef field)
        {
            return BitLength(new ColumnReference(field.Area, field.Field));
        }

        public static SqlFunction Coalesce(params object[] values)
        {
            return new SqlFunction(SqlFunctionType.Coalesce, values);
        }

        public static SqlFunction NullIf(string tableAlias1, string column1, string tableAlias2, string column2)
        {
            return NullIf(new ColumnReference(tableAlias1, column1), new ColumnReference(tableAlias2, column2));
        }

        public static SqlFunction NullIf(string tableAlias1, string column1, object expression2)
        {
            return NullIf(new ColumnReference(tableAlias1, column1), expression2);
        }

        public static SqlFunction NullIf(object expression1, object expression2)
        {
            return new SqlFunction(SqlFunctionType.NullIf, expression1, expression2);
        }

        public static SqlFunction NullIf(FieldRef field1, FieldRef field2)
        {
            return NullIf(new ColumnReference(field1.Area, field1.Field), new ColumnReference(field2.Area, field2.Field));
        }

        public static SqlFunction NullIf(FieldRef field, object expression2)
        {
            return NullIf(new ColumnReference(field.Area, field.Field), expression2);
        }

        public static SqlFunction Absolute(string tableAlias, string column)
        {
            return Absolute(new ColumnReference(tableAlias, column));
        }

        public static SqlFunction Absolute(object value)
        {
            return new SqlFunction(SqlFunctionType.Absolute, value);
        }

        public static SqlFunction Absolute(FieldRef field)
        {
            return Absolute(new ColumnReference(field.Area, field.Field));
        }

        public static SqlFunction Module(string dividendTableAlias, string dividendColumn, string divisorTableAlias, string divisorColumn)
        {
            return Module(new ColumnReference(dividendTableAlias, dividendColumn), new ColumnReference(divisorTableAlias, divisorColumn));
        }

        public static SqlFunction Module(string dividendTableAlias, string dividendColumn, object divisor)
        {
            return Module(new ColumnReference(dividendTableAlias, dividendColumn), divisor);
        }

        public static SqlFunction Module(object dividend, object divisor)
        {
            return new SqlFunction(SqlFunctionType.Module, dividend, divisor);
        }

        public static SqlFunction Module(FieldRef dividendField, FieldRef divisorField)
        {
            return Module(new ColumnReference(dividendField.Area, dividendField.Field), new ColumnReference(divisorField.Area, divisorField.Field));
        }

        public static SqlFunction Module(FieldRef dividendField, object divisor)
        {
            return Module(new ColumnReference(dividendField.Area, dividendField.Field), divisor);
        }

        public static SqlFunction SquareRoot(string tableAlias, string column)
        {
            return SquareRoot(new ColumnReference(tableAlias, column));
        }

        public static SqlFunction SquareRoot(object value)
        {
            return new SqlFunction(SqlFunctionType.SquareRoot, value);
        }

        public static SqlFunction SquareRoot(FieldRef field)
        {
            return SquareRoot(new ColumnReference(field.Area, field.Field));
        }

        public static SqlFunction Upper(string tableAlias, string column)
        {
            return Upper(new ColumnReference(tableAlias, column));
        }

        public static SqlFunction Upper(object value)
        {
            return new SqlFunction(SqlFunctionType.Upper, value);
        }

        public static SqlFunction Upper(FieldRef field)
        {
            return Upper(new ColumnReference(field.Area, field.Field));
        }

        public static SqlFunction Lower(string tableAlias, string column)
        {
            return Lower(new ColumnReference(tableAlias, column));
        }

        public static SqlFunction Lower(object value)
        {
            return new SqlFunction(SqlFunctionType.Lower, value);
        }

        public static SqlFunction Lower(FieldRef field)
        {
            return Lower(new ColumnReference(field.Area, field.Field));
        }

        public static SqlFunction Cast(string tableAlias, string column, DbType type)
        {
            return Cast(new ColumnReference(tableAlias, column), type);
        }

        public static SqlFunction Cast(object value, DbType type)
        {
            return new SqlFunction(SqlFunctionType.Cast, value, type);
        }

        public static SqlFunction Cast(FieldRef field, DbType type)
        {
            return Cast(new ColumnReference(field.Area, field.Field), type);
        }
		
		public static SqlFunction Cast(string tableAlias, string column, CustomDbType type)
        {
            return Cast(new ColumnReference(tableAlias, column), type);
        }

        public static SqlFunction Cast(object value, CustomDbType type)
        {
            return new SqlFunction(SqlFunctionType.Cast, value, type);
        }

        public static SqlFunction Cast(FieldRef field, CustomDbType type)
        {
            return Cast(new ColumnReference(field.Area, field.Field), type);
        }

        public static SqlFunction Extract(string part, string tableAlias, string column)
        {
            return Extract(part, new ColumnReference(tableAlias, column));
        }

        public static SqlFunction Extract(string part, object value)
        {
            return new SqlFunction(SqlFunctionType.Extract, part, value);
        }

        public static SqlFunction Extract(string part, FieldRef field)
        {
            return Extract(part, new ColumnReference(field.Area, field.Field));
        }

        public static SqlFunction Concat(params object[] values)
        {
            return new SqlFunction(SqlFunctionType.Concat, values);
        }

        public static SqlFunction CurrentTimestamp()
        {
            return new SqlFunction(SqlFunctionType.CurrentTimestamp);
        }

        public static SqlFunction SystemDate()
        {
            return new SqlFunction(SqlFunctionType.SystemDate);
        }

        public static SqlFunction Second(string tableAlias, string column)
        {
            return Second(new ColumnReference(tableAlias, column));
        }

        public static SqlFunction Second(object value)
        {
            return new SqlFunction(SqlFunctionType.Second, value);
        }

        public static SqlFunction Second(FieldRef field)
        {
            return Second(new ColumnReference(field.Area, field.Field));
        }

        public static SqlFunction Minute(string tableAlias, string column)
        {
            return Minute(new ColumnReference(tableAlias, column));
        }

        public static SqlFunction Minute(object value)
        {
            return new SqlFunction(SqlFunctionType.Minute, value);
        }

        public static SqlFunction Minute(FieldRef field)
        {
            return Minute(new ColumnReference(field.Area, field.Field));
        }

        public static SqlFunction Hour(string tableAlias, string column)
        {
            return Hour(new ColumnReference(tableAlias, column));
        }

        public static SqlFunction Hour(object value)
        {
            return new SqlFunction(SqlFunctionType.Hour, value);
        }

        public static SqlFunction Hour(FieldRef field)
        {
            return Hour(new ColumnReference(field.Area, field.Field));
        }

        public static SqlFunction Day(string tableAlias, string column)
        {
            return Day(new ColumnReference(tableAlias, column));
        }

        public static SqlFunction Day(object value)
        {
            return new SqlFunction(SqlFunctionType.Day, value);
        }

        public static SqlFunction Day(FieldRef field)
        {
            return Day(new ColumnReference(field.Area, field.Field));
        }

        public static SqlFunction Month(string tableAlias, string column)
        {
            return Month(new ColumnReference(tableAlias, column));
        }

        public static SqlFunction Month(object value)
        {
            return new SqlFunction(SqlFunctionType.Month, value);
        }

        public static SqlFunction Month(FieldRef field)
        {
            return Month(new ColumnReference(field.Area, field.Field));
        }

        public static SqlFunction Year(string tableAlias, string column)
        {
            return Year(new ColumnReference(tableAlias, column));
        }

        public static SqlFunction Year(object value)
        {
            return new SqlFunction(SqlFunctionType.Year, value);
        }

        public static SqlFunction Year(FieldRef field)
        {
            return Year(new ColumnReference(field.Area, field.Field));
        }

        public static SqlFunction ToString(string tableAlias, string column)
        {
            return ToString(new ColumnReference(tableAlias, column));
        }

        public static SqlFunction ToString(object value)
        {
            return new SqlFunction(SqlFunctionType.ToString, value);
        }

        public static SqlFunction ToString(FieldRef field)
        {
            return ToString(new ColumnReference(field.Area, field.Field));
        }

        public static SqlFunction Left(string tableAlias, string column, int length)
        {
            return Left(new ColumnReference(tableAlias, column), length);
        }

        public static SqlFunction Left(object value, int length)
        {
            return new SqlFunction(SqlFunctionType.Left, value, length);
        }

        public static SqlFunction Left(FieldRef field, int length)
        {
            return Left(new ColumnReference(field.Area, field.Field), length);
        }

        public static SqlFunction Right(string tableAlias, string column, int length)
        {
            return Right(new ColumnReference(tableAlias, column), length);
        }

        public static SqlFunction Right(object value, int length)
        {
            return new SqlFunction(SqlFunctionType.Right, value, length);
        }

        public static SqlFunction Right(FieldRef field, int length)
        {
            return Right(new ColumnReference(field.Area, field.Field), length);
        }

        public static SqlFunction Iif(CriteriaSet condition, object trueValue, object falseValue)
        {
            return new SqlFunction(SqlFunctionType.Iif, condition, trueValue, falseValue);
        }

        public static SqlFunction Custom(string functionName, params object[] arguments)
        {
            object[] realArguments = null;
            if (arguments == null || arguments.Length == 0)
            {
                realArguments = new object[] { functionName };
            }
            else
            {
                realArguments = new object[arguments.Length + 1];
                realArguments[0] = functionName;
                arguments.CopyTo(realArguments, 1);
            }
            return new SqlFunction(SqlFunctionType.Custom, realArguments);
        }

        public static SqlFunction SysCustom(string functionName, params object[] arguments)
        {
            object[] realArguments = null;
            if (arguments == null || arguments.Length == 0)
            {
                realArguments = new object[] { functionName };
            }
            else
            {
                realArguments = new object[arguments.Length + 1];
                realArguments[0] = functionName;
                arguments.CopyTo(realArguments, 1);
            }
            return new SqlFunction(SqlFunctionType.SysCustom, realArguments);
        }

        public static SqlFunction RowNumber(ColumnSort[] orderBy)
        {
            return new SqlFunction(SqlFunctionType.RowNumber, orderBy);
        }

        public static SqlFunction RowNumber(object field, SortOrder order, object pkey)// OLD Version
        {
            var orderby = new ColumnSort[2];
            
            if (field != null && field is FieldRef)
                orderby[0] = new ColumnSort(new ColumnReference((FieldRef)field), order);
            else orderby[0] = new ColumnSort((ColumnReference)field, order);

            if (pkey != null && pkey is FieldRef)
                orderby[1] = new ColumnSort(new ColumnReference((FieldRef)pkey), SortOrder.Ascending);
            else orderby[1] = new ColumnSort((ColumnReference)pkey, SortOrder.Ascending);

            return RowNumber(orderby);
        }

        public static SqlFunction RowNumber(FieldRef field, SortOrder order, FieldRef pkey)// OLD Version
        {
            return RowNumber(new ColumnReference(field.Area, field.Field), order, new ColumnReference(pkey.Area, pkey.Field));
        }

        public static SqlFunction RowNumber(string tableAlias, string column, SortOrder order, string baseTable, string pkey)// OLD Version
        {
            return RowNumber(new ColumnReference(tableAlias, column), order, new ColumnReference(baseTable, pkey));
        }
		
		public static SqlFunction Week(string tableAlias, string column)
        {
            return Week(new ColumnReference(tableAlias, column));
        }

        public static SqlFunction Week(object value)
        {
            return new SqlFunction(SqlFunctionType.Week, value);
        }

        public static SqlFunction Week(FieldRef field)
        {
            return Week(new ColumnReference(field.Area, field.Field));
        }

        /// <summary>
        /// The function rounds a number to a specified number of decimal places
        /// </summary>
        /// <param name="tableAlias">The table alias of the value to be rounded</param>
        /// <param name="column">The column of the value to be rounded</param>
        /// <param name="decimals">The number of decimal places to round number to</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Round"/> type</returns>
        public static SqlFunction Round(string tableAlias, string column, object decimals)
        {
            return Round(new ColumnReference(tableAlias, column), decimals);
        }

        /// <summary>
        /// The function rounds a number to a specified number of decimal places
        /// </summary>
        /// <param name="value">The number to be rounded</param>
        /// <param name="decimals">The number of decimal places to round number to</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Round"/> type</returns>
        public static SqlFunction Round(object value, object decimals)
        {
            return new SqlFunction(SqlFunctionType.Round, value, decimals);
        }

        /// <summary>
        /// The function rounds a number to a specified number of decimal places
        /// </summary>
        /// <param name="field">The field of the value to be rounded</param>
        /// <param name="decimals">The number of decimal places to round number to</param>
        /// <returns>A <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.Round"/> type</returns>
        public static SqlFunction Round(FieldRef field, object decimals)
        {
            return Round(new ColumnReference(field.Area, field.Field), decimals);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.GroupConcat"/> function with the specified arguments
        /// </summary>
        /// <param name="tableAlias">The table alias of the value we want to concatenate</param>
        /// <param name="column">The column of the value we want to concatenate</param>
        /// <param name="separator">The string used as separator for concatenated strings</param>
        /// <returns>An <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.GroupConcat"/> type</returns>
        public static SqlFunction GroupConcat(string tableAlias, string column, string separator)
        {
            return GroupConcat(new ColumnReference(tableAlias, column), separator);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.GroupConcat"/> function with the specified arguments
        /// </summary>
        /// <param name="value">The value we want to concatenate</param>
        /// <param name="separator">The string used as separator for concatenated strings</param>
        /// <returns>An <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.GroupConcat"/> type</returns>
        public static SqlFunction GroupConcat(object value, object separator)
        {
            return new SqlFunction(SqlFunctionType.GroupConcat, value, separator);
        }

        /// <summary>
        /// Creates a <see cref="SqlFunctionType.GroupConcat"/> function with the specified arguments
        /// </summary>
        /// <param name="field">The field we want to concatenate</param>
        /// <param name="separator">The string used as separator for concatenated strings</param>
        /// <returns>An <see cref="SqlFunction"/> of the <see cref="SqlFunctionType.GroupConcat"/> type</returns>
        public static SqlFunction GroupConcat(FieldRef field, string separator)
        {
            return GroupConcat(new ColumnReference(field.Area, field.Field), separator);
        }
    }
}