using System;
using CSGenio.business;
using System.Collections.Generic;

namespace CSGenio.framework
{
    /// <summary>
    /// Class that holds all the metainformation about a database field
    /// </summary>
    public class Field
    {
        private readonly string _fullname;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="alias">Area of the field</param>
        /// <param name="name">Name of the field</param>
        /// <param name="fieldType">Type of the field</param>
        public Field(string alias, string name, FieldType fieldType)
        {
            Alias = alias;
            Name = name;
            _fullname = Alias + "." + Name;
            FieldType = fieldType;
            FieldFormat = fieldType.GetFormatting();
            NotNull = false;
            ZeroDuplication = false;
            NotDup = false;
            IsVirtual = false;
            MQueue = true;
            Decimals = 0;
            CriaLog = false;
            VisivelCav = CavVisibilityType.Sempre;
        }
 
        /// <summary>
        /// Hashcode for the object
        /// </summary>
        /// <returns>Hashcode for the object</returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="obj">Another object</param>
        /// <returns>True if the other object is equal, false otherwise</returns>
        public override bool Equals(Object obj)
        {
            if (obj is Field c)
            {
                if (c.Name.Equals(Name) &&
                    c.FieldType.Equals(FieldType))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Field symbolic name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
		/// Table alias
		/// </summary>
		public string Alias { get; private set; }

        /// <summary>
        /// Returns the fully qualified name of this field
        /// </summary>
        public string FullName { get => _fullname; }

        /// <summary>
        /// User description of the field
        /// </summary>
        public string FieldDescription { get; set; }

        /// <summary>
        /// Field data type
        /// </summary>
        public FieldType FieldType { get; set; }

        /// <summary>
        /// Calculated field formula
        /// </summary>
        public Formula Formula { get; set; }

        /// <summary>
        /// Data format for the field
        /// </summary>
        public FieldFormatting FieldFormat { get; set; }

        /// <summary>
        /// Maximum field data size
        /// </summary>
        public int FieldSize { get; set; }

        /// <summary>
        ///  Type of default value
        /// </summary>
        public DefaultValue DefaultValue { get; set; }

        /// <summary>
        /// Name of the associated enumeration of valid values
        /// </summary>
        public string ArrayName { get; set; }

        /// <summary>
        /// The name of the associated enumeration without DBO prefix (to be used with the «new ArrayInfo(...)»)
        /// </summary>
        public string ArrayClassName { get; set; }

        /// <summary>
        /// Mandatory
        /// </summary>
        public bool NotNull { get; set; }

        /// <summary>
        /// Unique
        /// </summary>
        public bool NotDup { get; set; }

        /// <summary>
        /// Unique prefix
        /// </summary>
        public string PrefNDup { get; set; }
		
        /// <summary>
        /// Unique message
        /// </summary>
        public string Dupmsg { get; set; }

        /// <summary>
        /// Should clear field value after duplication record
        /// </summary>
        public bool ZeroDuplication { get; set; }

        /// <summary>
        /// List of replicas of this field
        /// </summary>
        public List<ReplicaDestination> ReplicaDestinationList { get; set; }

        /// <summary>
        /// Condition to inhibit field value calculation
        /// </summary>
        public ConditionFormula RecalculatesIf { get; set; }

        /// <summary>
        /// Validation rule
        /// </summary>
        public ConditionFormula WriteCondition { get; set; }

        /// <summary>
        /// Show condition
        /// </summary>
        public ConditionFormula ShowWhen { get; set; }

        /// <summary>
        /// Fill condition
        /// </summary>
        public ConditionFormula FillWhen { get; set; }

        /// <summary>
        /// Block condition
        /// </summary>
        public ConditionFormula BlockWhen { get; set; }

        /// <summary>
        /// The ecription formula/function for this field
        /// </summary>
        public InternalOperationFormula EncryptFieldValueFormula { get; set; }

        /// <summary>
        /// Formating rules
        /// IB -> NIB
        /// NC -> Nº de contribuinte
        /// CP -> Código postal
        /// SS -> Nº de Segurança Social
        /// UP -> Maiusculas
        /// MA -> Matricula
        /// FI -> File
        /// </summary>
        public Validation.FillingRule FillingRule { get; set; }

        /// <summary>
        /// Associated history table
        /// </summary>
        public string CreateHist { get; set; }

        /// <summary>
        /// Associated messaging queue
        /// </summary>
        public bool MQueue { get; set; }

        /// <summary>
        /// Decimal places for numeric fields
        /// </summary>
        public int Decimals { get; set; }

        /// <summary>
        /// Whole number places for numeric fields
        /// </summary>
        public int IntegerDigits { get; set; }

        /// <summary>
        /// Its a virtual field in the database and should not be written to
        /// </summary>
        public bool IsVirtual { get; set; }

        /// <summary>
        /// A client side field does not even exist in the database for reading
        ///  and is only calculated on demand by the client side.
        /// </summary>
        public bool IsClientSide { get; set; }

        /// <summary>
        /// Created log
        /// </summary>
        public bool CriaLog { get; set; }

        /// <summary>
        /// Pads the field with whitespace to right align it in the database
        /// </summary>
        public bool AlignRightPad { get; set; }

        /// <summary>
        /// If this field is Ordering field
        /// </summary>
        public bool HasOrdering { get; set; }

        public delegate void OnReorderHandler(Area area, persistence.PersistentSupport sp, int oldPossition, Quidgest.Persistence.GenericQuery.CriteriaSet condition);
        /// <summary>
        /// Callback invoked after neighbour re-sequencing.
        /// </summary>
        public OnReorderHandler OnReorder;

        //---------------------------------------------------------------------------------
        // For Advanced Query
        //---------------------------------------------------------------------------------
        /// <summary>
        /// Type of AQ visibility
        /// </summary>
        public CavVisibilityType VisivelCav { get; set; }

        /// <summary>
        /// Title for AQ
        /// </summary>
        public string CavDesignation { get; set; }
        //---------------------------------------------------------------------------------

        public static bool isEmptyValue(object Qvalue, FieldFormatting format)
        {
            if (Qvalue == null) return true;

            // TODO: isto devia chamar a função Conversão.isCampoNulo, estão as duas a fazer coisas diferentes

            switch (format)
            {
                case FieldFormatting.DATA:
                case FieldFormatting.DATAHORA:
                case FieldFormatting.DATASEGUNDO:
                    if (DateTime.MinValue == (DateTime)Qvalue)
                        return true;
                    break;
                case FieldFormatting.GUID:
                    if (Qvalue.ToString() == "00000000-0000-0000-0000-000000000000" || Qvalue.ToString() == "" || Qvalue.ToString() == "{00000000-0000-0000-0000-000000000000}")
                        return true;
                    break;
                case FieldFormatting.INTEIRO:
                    if ((int)Qvalue == 0)
                        return true;
                    break;
                case FieldFormatting.LOGICO:
                    //When a boolean field has a default value in the form (true or false), Qvalue is a bool (true or false).
                    if (Qvalue is Boolean)
                        return (bool)Qvalue == false;
                     //When a boolean field has a default value in the table (true or false), Qvalue is an int (1 or 0).
                    else if (Convert.ToInt32(Qvalue) == 0)
                        return true;
                    break;
                case FieldFormatting.CARACTERES:
                    if (Qvalue.ToString().Trim() == "")
                        return true;
                    break;
                case FieldFormatting.GEOGRAPHY:
                    if (Qvalue.ToString() == "")
                        return true;
                    break;
                case FieldFormatting.FLOAT:
                    if (Convert.ToDecimal(Qvalue) == 0m)
                        return true;
                    break;
                case FieldFormatting.TEMPO:
                    if (Qvalue.ToString() == "__:__" || Qvalue.ToString() == "")
                        return true;
                    break;
                case FieldFormatting.JPEG:
                    if (((System.Byte[])Qvalue).GetUpperBound(0) < 1)
                        return true;
                    break;
                case FieldFormatting.ENCRYPTED:
                    return (Qvalue as EncryptedDataType ?? new EncryptedDataType(null, Qvalue)).IsEmpty();
                default:
                    return false;
            }

            return false;
        }

        public bool isEmptyValue(object Qvalue)
        {
            return Field.isEmptyValue(Qvalue, FieldFormat);
        }

        public static object GetValorEmpty(FieldFormatting format)
        {
            switch (format)
            {
                case FieldFormatting.DATA:
                case FieldFormatting.DATAHORA:
                case FieldFormatting.DATASEGUNDO:
                    return DateTime.MinValue;
                case FieldFormatting.GUID:
                    //- AJA ao retornar a guid empty fazia com que no QWEB chaves com o parametro NOT NULL não fosse bem validade ("00000-000-" etc não é igual a "")
                    //Pelo que vi esta alteração to retornar o Qvalue GUID com zeros foi feita devido a um problema com Qvalues for defeito ultimos Qvalues.
                    //Pelos testes que fiz, em QWEB e MVC, o numeros sequencias com e sem prefixo não duplicação funciona bem. Por isso se havia algo mais em que isto não funcionava não detectei.
                    //Caso se venha a verificar o erro de novo, ter em conta que change aqui vai estragar o NOT NULL das chaves.
                    return "";
                case FieldFormatting.INTEIRO:
                    return 0;
                case FieldFormatting.LOGICO:
                    return 0;
                case FieldFormatting.CARACTERES:
                case FieldFormatting.GEOGRAPHY:
                    return "";
                case FieldFormatting.GEO_SHAPE:
                case FieldFormatting.GEOMETRIC:
                    return null;
                case FieldFormatting.FLOAT:
                    return 0m;
                case FieldFormatting.TEMPO:
                    return "__:__";
                case FieldFormatting.BINARIO:
                case FieldFormatting.JPEG:
                    return new byte[0];
                case FieldFormatting.ENCRYPTED:
                    return null;
                default:
                    throw new FrameworkException(null, "Field.GetValorEmpty", "Format not implemented: " + format.ToString());
            }
        }

        public object GetValorEmpty()
        {
            return Field.GetValorEmpty(FieldFormat);
        }

        public bool isKey()
        {
            return this.FieldType.IsKey();
        }
    }
}
