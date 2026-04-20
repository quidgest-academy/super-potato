namespace Quidgest.Persistence.GenericQuery
{
    public enum CustomDbType
    {
        /// <summary>
        /// Should be mapped to a VARCHAR(50) or VARCHAR2(50), and it used on the cast of decimals to strings, 
        /// in order to allow the search with LIKE feature
        /// </summary>
        StandardAnsiString = 1,
        /// <summary>
        /// Should be mapped to a DECIMAL(38,10) or NUMBER(38,10), and it used on the cast of decimals to strings, 
        /// in order to allow the search with LIKE feature
        /// </summary>
        StandardDecimalSearch = 2
    }
}
