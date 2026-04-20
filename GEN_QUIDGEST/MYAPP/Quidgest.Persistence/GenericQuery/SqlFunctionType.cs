using System;

namespace Quidgest.Persistence.GenericQuery
{
    /// <summary>
    /// List of sql functions
    /// </summary>
    /// <remarks>
    /// <!--
    /// Author: CX 2011.06.28
    /// Modified:
    /// Reviewed:
    /// -->
    /// </remarks>
    public enum SqlFunctionType
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Count,
        Average,
        Max,
        Min,
        Sum,
        Substring,
        Locate,
        Trim,
        Length,
        BitLength,
        Coalesce,
        NullIf,
        Absolute,
        Module,
        SquareRoot,
        Upper,
        Lower,
        Cast,
        Extract,
        Concat,
        CurrentTimestamp,
        SystemDate,
        Second,
        Minute,
        Hour,
        Day,
        Month,
        Year,
        ToString,
        Left,
        Right,
        Iif,
        Custom,
        SysCustom,
        RowNumber,
        Week,
        Round,
        GroupConcat
    }
}
