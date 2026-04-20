using System;

namespace CSGenio.framework
{
    public enum FieldType
    {
        KEY_VARCHAR,
        KEY_GUID,
        KEY_INT,
        TEXT,
        MEMO,
        MEMO_COMP_RTF,
        PATH,
        TIME_HOURS,
        NUMERIC,
        CURRENCY,
        INTEGER,
        LOGIC,
        DATE,
        DATETIME,
        DATETIMESECONDS,
        ARRAY_NUMERIC,
        ARRAY_TEXT,
        ARRAY_LOGIC,
        IMAGE,
        DOCUMENT,
        BINARY,
        GEOGRAPHY_POINT,
        GEOMETRY_SHAPE,
        GEOGRAPHY_SHAPE,
        ENCRYPTED
    }

    public static class FieldTypeUtils
    {
        private readonly struct FieldTypeInfo(FieldType fieldType, FieldFormatting formatting, string ephFunction, Type externalType)
        {
            public readonly FieldType fieldType = fieldType;
            public readonly FieldFormatting formatting = formatting;
            public readonly string ephFunction = ephFunction;
            public readonly Type externalType = externalType;
        }

        /// <summary>
        /// Register the metadata info associated with each field type
        /// The position of the items of this list MUST match the ordinal values of the FieldType enum.
        /// </summary>
        private static readonly FieldTypeInfo[] info = [
            new FieldTypeInfo(FieldType.KEY_VARCHAR     , FieldFormatting.CARACTERES    , "EMPTYG", typeof(string)),
            new FieldTypeInfo(FieldType.KEY_GUID        , FieldFormatting.GUID          , "EMPTYG", typeof(Guid)),
            new FieldTypeInfo(FieldType.KEY_INT         , FieldFormatting.CARACTERES    , "EMPTYG", typeof(int)),
            new FieldTypeInfo(FieldType.TEXT            , FieldFormatting.CARACTERES    , "EMPTYC", typeof(string)),
            new FieldTypeInfo(FieldType.MEMO            , FieldFormatting.CARACTERES    , "EMPTYC", typeof(string)),
            new FieldTypeInfo(FieldType.MEMO_COMP_RTF   , FieldFormatting.CARACTERES    , null    , typeof(string)),
            new FieldTypeInfo(FieldType.PATH            , FieldFormatting.CARACTERES    , "EMPTYC", typeof(string)),
            new FieldTypeInfo(FieldType.TIME_HOURS      , FieldFormatting.TEMPO         , "EMPTYT", typeof(string)),
            new FieldTypeInfo(FieldType.NUMERIC         , FieldFormatting.FLOAT         , "EMPTYN", typeof(decimal)),
            new FieldTypeInfo(FieldType.CURRENCY        , FieldFormatting.FLOAT         , "EMPTYN", typeof(decimal)),
            new FieldTypeInfo(FieldType.INTEGER         , FieldFormatting.INTEIRO       , "EMPTYN", typeof(int)),
            new FieldTypeInfo(FieldType.LOGIC           , FieldFormatting.LOGICO        , "EMPTYL", typeof(int)),
            new FieldTypeInfo(FieldType.DATE            , FieldFormatting.DATA          , "EMPTYD", typeof(DateTime)),
            new FieldTypeInfo(FieldType.DATETIME        , FieldFormatting.DATAHORA      , "EMPTYD", typeof(DateTime)),
            new FieldTypeInfo(FieldType.DATETIMESECONDS , FieldFormatting.DATASEGUNDO   , "EMPTYD", typeof(DateTime)),
            new FieldTypeInfo(FieldType.ARRAY_NUMERIC   , FieldFormatting.FLOAT         , "EMPTYN", typeof(decimal)),
            new FieldTypeInfo(FieldType.ARRAY_TEXT      , FieldFormatting.CARACTERES    , "EMPTYC", typeof(string)),
            new FieldTypeInfo(FieldType.ARRAY_LOGIC     , FieldFormatting.LOGICO        , "EMPTYL", typeof(int)),
            new FieldTypeInfo(FieldType.IMAGE           , FieldFormatting.BINARIO       , null    , typeof(byte[])),
            new FieldTypeInfo(FieldType.DOCUMENT        , FieldFormatting.CARACTERES    , "EMPTYC", typeof(string)),
            new FieldTypeInfo(FieldType.BINARY          , FieldFormatting.BINARIO       , null    , typeof(byte[])),
            new FieldTypeInfo(FieldType.GEOGRAPHY_POINT , FieldFormatting.GEOGRAPHY     , null    , typeof(byte[])),
            new FieldTypeInfo(FieldType.GEOMETRY_SHAPE  , FieldFormatting.GEOMETRIC     , null    , typeof(byte[])),
            new FieldTypeInfo(FieldType.GEOGRAPHY_SHAPE , FieldFormatting.GEO_SHAPE     , null    , typeof(byte[])),
            new FieldTypeInfo(FieldType.ENCRYPTED       , FieldFormatting.ENCRYPTED     , null    , typeof(EncryptedDataType)),
            ];


        /// <summary>
        /// Data type format associated with this field
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FieldFormatting GetFormatting(this FieldType type)
        {
            return info[(int)type].formatting;
        }

        /// <summary>
        /// Sql function to be used to test if this field type is empty
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string GetEPHFunction(this FieldType type)
        {
            return info[(int)type].ephFunction ?? throw new InvalidOperationException();
        }

        /// <summary>
        /// Native type associated with this data type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Type GetExternalType(this FieldType type)
        {
            return info[(int)type].externalType ?? throw new InvalidOperationException();
        }

        /// <summary>
        /// Tests if this field type is considered a database key
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsKey(this FieldType type)
        {
            return type switch
            {
                FieldType.KEY_VARCHAR => true,
                FieldType.KEY_GUID => true,
                FieldType.KEY_INT => true,
                _ => false
            };
        }

    }
}