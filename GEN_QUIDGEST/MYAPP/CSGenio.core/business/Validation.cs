using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business
{
    /// <summary>
    /// Collection of methods to verify specific field formats.
    /// </summary>
    public class Validation
    {
        /// <summary>
        /// Delegate function with the filling rule validation method.
        /// Existing filling rules (aRegrasp):
        /// CP -> Código postal
        /// NC -> Nº de Contribuinte (NIF)
        /// IB -> NIB
        /// IN -> IBAN
        /// SS -> Nº de Segurança Social
        /// UP -> Maiúsculas
        /// MA -> Matrícula
        /// EM -> Email
        /// FI -> Ficheiro
        /// MP -> Máscara Manual
        /// </summary>
        /// <param name="arg">argument that will be passed to the validation method</param>
        /// <returns>true if valid, false otherwise</returns>
        public delegate bool FillingRule(string arg);


        /// <summary>
        /// Validate a Qfield from a record that has been changed
        /// </summary>
        /// <param name="area"></param>
        /// <param name="Qfield">field to validate</param>
        /// <param name="fieldValue">value of the field</param>
        /// <param name="sp">persistent support</param>
        /// <param name="tpFunction">function type</param>
        /// <param name="user">user</param>
        /// <returns>"OK" if valid, otherwise return a error message</returns>
        public static string validateFieldChange(Area area, Field Qfield, object fieldValue, PersistentSupport sp, FunctionType tpFunction, User user)
        {
            // validate if the field value can not be null
            if (Qfield.NotNull && area.ValidateIfIsNull)
            {
                if (Qfield.isEmptyValue(fieldValue))
                    return string.Format(Translations.Get("O campo {0} é obrigatório.", user.Language), Qfield.FieldDescription);
            }

            string descTrans = Translations.Get(Qfield.FieldDescription, user.Language);
            if (Qfield.FieldSize > 0 && Qfield.FieldType == FieldType.TEXT && Qfield.FieldSize < fieldValue.ToString().Length)
                return string.Format(Translations.Get("O tamanho do campo {0} excede o valor máximo permitido ({1}).", user.Language), descTrans, Qfield.FieldSize);

            // validate if the field value is unique (prefNdup)
            if (!validateNonDuplication(area, Qfield, fieldValue, sp))
            {
                // Last updated by [CJP] at [2015.12.07]
                // Prevent returning codes / guids in error messages to the user

                bool isKey = true;
                if (Qfield.FieldType != FieldType.KEY_VARCHAR && Qfield.FieldType != FieldType.KEY_GUID && Qfield.FieldType != FieldType.KEY_VARCHAR && Qfield.FieldType != FieldType.KEY_GUID)
                    isKey = false;

                if (!String.IsNullOrEmpty(Qfield.ArrayName) && !isKey)
                {
                    // Convert array name
                    string arrayPrefix = string.Empty;
                    if (Qfield.FieldType == FieldType.ARRAY_NUMERIC)
                        arrayPrefix = "dbo.GetValArrayN";
                    else if (Qfield.FieldType == FieldType.ARRAY_LOGIC)
                        arrayPrefix = "dbo.GetValArrayL";
                    else
                        arrayPrefix = "dbo.GetValArrayC";

                    string arrayName = Qfield.ArrayName.Replace(arrayPrefix, "");
                    arrayName = char.ToUpper(arrayName[0]) + arrayName.Substring(1);
                    // Get array description from value
                    var array = new ArrayInfo(arrayName);
                    fieldValue = array.GetDescription(fieldValue.ToString(), "");
                    fieldValue = CSGenio.framework.Translations.Get(fieldValue.ToString(), user.Language); //convert to user's language
                }
                if (string.IsNullOrEmpty(Qfield.Dupmsg))
                {
                    if (!isKey)
                        return string.Format(Translations.Get("O campo {0} não pode ter o valor {1} porque já existe outra ficha com o mesmo valor.", user.Language), descTrans, fieldValue);
                    else
                        return string.Format(Translations.Get("O campo {0} não pode ter o valor atual porque já existe outra ficha com o mesmo valor.", user.Language), descTrans);
                }
                else
                {
                    try
                    {
                        //Parse the message to obtain the field's values if there are any
                        string dupeMsg = Translations.Get(Qfield.Dupmsg, user.Language);
                        int messageCharCnt = dupeMsg.Length;
                        string currentField = "";
                        for (int i = dupeMsg.IndexOf('[') + 1; i < messageCharCnt; i++)
                        {
                            if (i == dupeMsg.IndexOf(']'))
                            {
                                // When we have a whole field name,
                                // check if there is a field with that name in the current table

                                foreach (var fld in area.Fields)
                                    if(currentField.Equals(fld.Value.Name, StringComparison.OrdinalIgnoreCase))
                                    {
                                        //If we confirm there is a field in the current table with this name
                                        //Fetch its value and replace it in the string
                                        dupeMsg = dupeMsg.Replace("[" + currentField + "]", (fld.Value as RequestedField).Value.ToString());
                                        break;
                                    }

                                currentField = ""; //Clear currentField even if nothing was replaced

                                //If there are no more fields, finish
                                if (!dupeMsg.Contains("["))
                                    break;
                                else //Otherwise hop onto the next one
                                    i = dupeMsg.IndexOf("[") + 1;
                            }

                            currentField += dupeMsg[i];
                        }
                        return dupeMsg;
                    }
                    catch (Exception)
                    {
                        if (isKey)
                            return string.Format(Translations.Get("O campo {0} não pode ter o valor atual porque já existe outra ficha com o mesmo valor.", user.Language), descTrans);
                        else
                            return string.Format(Translations.Get("O campo {0} não pode ter o valor {1} porque já existe outra ficha com o mesmo valor.", user.Language), descTrans, fieldValue);
                    }
                }
            }

            // validate field filling rule
            if (fieldValue != null && Qfield.FillingRule != null && !Qfield.FillingRule.Invoke(fieldValue.ToString()))
                return string.Format(Translations.Get("O campo {0} não respeita a regra de preenchimento.", user.Language), descTrans);

            return "OK";
        }


        /// <summary>
        /// Validate each Qfield from a record that has been changed
        /// </summary>
        /// <param name="isApply">true if the function is called by "Apply"</param>
        /// <returns>"OK" status message if all fields are valid, or a list with error status messages</returns>
        public static StatusMessage validateFieldsChange(Area area, PersistentSupport sp, User user, bool isApply = false)
        {
            //RS(2009.01.20) Como a lista de fields preenchidos pode mudar durante a validacao primeiro
            // temos de obter uma copia dessa lista to nao causar excepção
            ArrayList camposPreenchidos = new ArrayList(area.Fields.Values);
            StatusMessage result = StatusMessage.GetAggregator();
            foreach (object ocampoPedido in camposPreenchidos)
            {
                RequestedField campoPedido = (RequestedField)ocampoPedido;
                Field Qfield = (Field)area.DBFields[campoPedido.Name];
                string Qresult = validateFieldChange(area, Qfield, campoPedido.Value, sp, FunctionType.ALT, user);

                if (!Qresult.Equals("OK") && !Qresult.Equals("W"))
                {
                    //JGF 2020.12.10 Instead of returning it nows add the errors to the list to be aggregated later.
                    var error = StatusMessage.Error(msg: Qresult, origin: campoPedido.Name);
                    result.MergeStatusMessage(error);
                }
                else if (Qresult.Equals("W"))
                {
                    var aviso = Translations.Get(Qfield.WriteCondition.ErrorWarning, user.Language);
                    result.MergeStatusMessage(StatusMessage.Warning(aviso, origin: campoPedido.Name));
                }
            }

            // Evaluate table conditions
            var conditionResults = area.ValidateConditions(sp, isApply);
            result.MergeStatusMessage(conditionResults);

            if (result.Status == Status.OK && String.IsNullOrEmpty(result.Message))
            {
                result.MergeStatusMessage(StatusMessage.OK());
            }

            return result;
        }


        /// <summary>
        /// Validate if a Qfield has to be unique (non-duplication)
        /// </summary>
        /// <param name="area">Qfield's area</param>
        /// <param name="Qfield">Qfield to validate</param>
        /// <param name="fieldValue">Qvalue of the Qfield</param>
        /// <param name="sp">persistent support</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool validateNonDuplication(Area area, Field Qfield, object fieldValue, PersistentSupport sp)
        {
            // Check if the field is marked as unique (non-duplication)
            if (!Qfield.NotDup)
                return true;

            // Check if the field is marked as not null
            if (!Qfield.NotNull && Qfield.isEmptyValue(fieldValue))
                return true;

            // Only validate if there was a relevant change of the record
            var prefndupInfo = String.IsNullOrEmpty(Qfield.PrefNDup) ? null : area.DBFields[Qfield.PrefNDup];
            if (!area.Fields[area.DBFields["zzstate"].FullName].IsDirty()
                && !area.Fields[Qfield.FullName].IsDirty()
                && (prefndupInfo is null || !area.Fields[prefndupInfo.FullName].IsDirty()))
                return true;

            // Query to check if the value is not unique
            CriteriaSet criteria = CriteriaSet.And()
                    .Equal(area.Alias, Qfield.Name, fieldValue)
                    .Equal(area.Alias, "zzstate", 0);
            string codIntValue = area.QPrimaryKey;
            if (!string.IsNullOrEmpty(codIntValue))
                criteria = criteria.NotEqual(area.Alias, area.PrimaryKeyName, codIntValue);

            SelectQuery qs = new SelectQuery()
                .Select(area.Alias, Qfield.Name)
                .From(area.QSystem, area.TableName, area.Alias)
                .Where(criteria);

            // Check if Qfield sets another Qfield as prefix for non-duplication
            if (!String.IsNullOrEmpty(Qfield.PrefNDup))
            {
                // Retrieve the Qvalue of the prefix Qfield
                object nDupPrefValue;
                if (area.Fields.ContainsKey(prefndupInfo.FullName))
                    nDupPrefValue = area.returnValueField(prefndupInfo.FullName);
                else
                {
                    // Retrieve the Qvalue of the prefix Qfield from the database
                    qs = new SelectQuery()
                        .Select(area.Alias, Qfield.PrefNDup)
                        .From(area.QSystem, area.TableName, area.Alias)
                        .Where(CriteriaSet.And()
                            .Equal(area.Alias, area.PrimaryKeyName, codIntValue));

                    nDupPrefValue = sp.ExecuteScalar(qs);
                }

                // Prefix Qfield does not have a Qvalue
                if (nDupPrefValue.Equals(""))
                    return true;

                // Add a condition for the non-duplication prefix to the query
                qs.WhereCondition.Equal(area.Alias, Qfield.PrefNDup, nDupPrefValue);
            }

            // Execute the query
            object Qresult = sp.ExecuteScalar(qs);
            return Qresult == null;
        }

        /// <summary>
        /// Validate write condition on a specific Qfield
        /// </summary>
        /// <param name="area">Qfield's area</param>
        /// <param name="Qfield">Qfield</param>
        /// <param name="fieldValue">Qvalue of the Qfield</param>
        /// <param name="sp">persistent support</param>
        /// <param name="tpFunction">function type</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool validateWriteCondition(Area area, Field Qfield, object fieldValue, PersistentSupport sp, FunctionType tpFunction)
        {
            ConditionFormula formula = Qfield.WriteCondition;

            object[] fieldsValue = formula.returnValueFieldsInternalFormula(area, formula.ByAreaArguments, sp, formula.ParameterCount, tpFunction);
            return formula.calculateFormulaCondition(fieldsValue, area.User, area.Module, sp);
        }

        /// <summary>
        /// Validate Not-Null Qfields
        /// </summary>
        /// <param name="area">area</param>
        /// <param name="user">user</param>
        /// <returns>"OK" status message if all fields are valid, or a list with error status messages</returns>
        public static StatusMessage ValidateNotNull(Area area, User user)
        {
            ArrayList camposPreenchidos = new ArrayList(area.Fields.Values);
            foreach (object ocampoPedido in camposPreenchidos)
            {
                RequestedField campoPedido = (RequestedField)ocampoPedido;
                Field Qfield = (Field)area.DBFields[campoPedido.Name];

                if (Qfield.NotNull)
                {
                    if (Qfield.isEmptyValue(campoPedido.Value))
                        return StatusMessage.Error(string.Format(Translations.Get("O campo {0} ({1}.{2}) é obrigatório e não está preenchido.", user.Language), Qfield.FieldDescription, Qfield.Alias, Qfield.Name));
                }
            }

            return StatusMessage.OK(Translations.Get("Alteração bem sucedida.", user.Language));
        }

        /// <summary>
        /// Validate a NIF
        /// </summary>
        /// <param name="nif">NIF</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool validateNC(string nif)
        {
            // Empty value
            if (String.IsNullOrEmpty(nif))
                return true;

            if (nif.Length != 9)
                return false;

            Regex expReg = new Regex("[0-9]{9}");
            if (!expReg.Match(nif).Success)
                return false;

            char[] nifChar = nif.ToCharArray();

            int[] nifInt = new int[9];
            for (int i = 0; i < 9; i++)
                nifInt[i] = int.Parse(nif.Substring(i, 1));

            int num = nifInt[0] * 9 + nifInt[1] * 8 + nifInt[2] * 7 + nifInt[3] * 6 + nifInt[4] * 5 + nifInt[5] * 4 + nifInt[6] * 3 + nifInt[7] * 2;
            num = 11 - (num % 11);
            if (num >= 10)
                num = 0;
            if (num != nifInt[8])
                return false;
            return true;
        }

        /// <summary>
        /// Validate a NIB
        /// NIB format: CCCC-BBBB-12345678901-DD
        /// CCCC = bank code (4 digits)
        /// BBBB = branch code (4 digits)
        /// 12345678901 = account number (11 digits)
        /// DD = control code (2 digits)
        /// </summary>
        /// <param name="nib">NIB</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool validateIB(string nib)
        {
            // Empty value
            if (String.IsNullOrEmpty(nib))
                return true;

            // Check length and position of hyphens
            if (nib.Length != 24 || nib[4] != '-' || nib[9] != '-' || nib[21] != '-')
                return false;

            // Remove hyphens
            nib = nib.Replace("-", "");
            char[] chrNIB = nib.ToCharArray();

            // Convert characters into integers
            int i;
            int[] intNIB = new int[21];
            for (i = 0; i < 21; i++)
                intNIB[i] = chrNIB[i] - '0';

            // Validation logic
            int s = 0, p = 1, c;
            c = intNIB[19] * 10 + intNIB[20];
            intNIB[19] = 0;
            intNIB[20] = 0;

            for (i = 20; i >= 0; i--)
            {
                s += intNIB[i] * p;
                p = (p * 10) % 97;
            }
            s = 98 - (s % 97);

            if (s != c)
                return false;
            return true;
        }

        /// <summary>
        /// Validate a portuguese postal code.
        /// </summary>
        /// <param name="zipCode">postal code</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool validateCP(string zipCode)
        {
            // Empty value
            if (String.IsNullOrEmpty(zipCode))
                return true;

            if (zipCode.Length != 8 && zipCode.Length != 4)
                return false;

            Regex expReg = new Regex("^([0-9]{4})(-([0-9]{3}))?$");
            if (!expReg.Match(zipCode).Success)
                return false;
            return true;
        }

        /// <summary>
        /// Validate a portuguese social security number
        /// </summary>
        /// <param name="nrSegSoc">social security number</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool validateSS(string nrSegSoc)
        {
            // Empty value
            if (String.IsNullOrEmpty(nrSegSoc))
                return true;

            // Social security number new format
            if (nrSegSoc.Length == 11)
            {
                Regex expReg = new Regex("([0-9]{11})");
                if (expReg.Match(nrSegSoc).Success)
                {
                    int tamNr = nrSegSoc.Length;
                    Int64 nrSegSocInt = Int64.Parse(nrSegSoc);
                    Int64[] alg = new Int64[tamNr];
                    for (int i = 0; i < tamNr; i++)
                    {
                        Int64 potencia = (Int64)Math.Pow(10, (tamNr - i - 1));
                        alg[i] = nrSegSocInt / potencia;
                        nrSegSocInt -= alg[i] * potencia;
                    }

                    Int64 validacao = alg[0] * 29 + alg[1] * 23 + alg[2] * 19 + alg[3] * 17 + alg[4] * 13 + alg[5] * 11 + alg[6] * 7 + alg[7] * 5 + alg[8] * 3 + alg[9] * 2;

                    validacao = 9 - (validacao % 10);
                    if (alg[0] != 1 && alg[0] != 2 || validacao != alg[10])
                        return false;
                    return true;
                }
                else
                    return false;
            }
            // Social security number old format
            else if (nrSegSoc.Length == 9)
            {
                Regex expReg = new Regex("([0-9]{9})");
                if (!expReg.Match(nrSegSoc).Success)
                    return false;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Validate if a text is filled with capital letters.
        /// This method only considers letter characters.
        /// Symbols and special characters are ignored.
        /// </summary>
        /// <param name="text">text</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool validateUP(string text)
        {
            // If null or empty it does not have invalid chars
            if (String.IsNullOrEmpty(text))
                return true;

            char[] accepted_chars = { 'º', 'ª' };

            // Validate if each character is uppercase
            for (int i = 0; i < text.Length; i++)
            {
                // Skip non-letter characters
                if (!Char.IsLetter(text[i]))
                    continue;

                // Skip some special characters considered letters
                if (Array.Exists(accepted_chars, c => c == text[i]))
                    continue;

                // Only validates the characters if it is a letter
                if (!Char.IsUpper(text[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Validate if a text is filled with lower letters.
        /// This method only considers letter characters.
        /// Symbols and special characters are ignored.
        /// </summary>
        /// <param name="text">text</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool validateLO(string text)
        {
            // If null or empty it does not have invalid chars
            if (String.IsNullOrEmpty(text))
                return true;

            char[] accepted_chars = { 'º', 'ª' };

            // Validate if each character is lowercase
            for (int i = 0; i < text.Length; i++)
            {
                // Skip non-letter characters
                if (!Char.IsLetter(text[i]))
                    continue;

                // Skip some special characters considered letters
                if (Array.Exists(accepted_chars, c => c == text[i]))
                    continue;

                // Only validates the characters if it is a letter
                if (!Char.IsLower(text[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Validate a portuguese license plate
        /// </summary>
        /// <param name="license">license plate</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool validateMA(string license)
        {
            // Match 3 blocks made up from 2 letters or 2 digits, separated by an optional '-' or ' ' character
            Regex expReg = new Regex("^([A-Za-z]{2}|[0-9]{2})(-| )?([A-Za-z]{2}|[0-9]{2})(-| )?([A-Za-z]{2}|[0-9]{2})$");
            if (string.IsNullOrEmpty(license))
                return true;
            else if (expReg.Match(license).Success)
                return true;
            return false;
        }

        /// <summary>
        /// Validate a file
        /// </summary>
        /// <param name="file">filename</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool validateFI(string file)
        {
            return true;
        }

        /// <summary>
        /// Validate a Qfield with its current mask
        /// </summary>
        /// <param name="value">The provided value</param>
        /// <param name="mask">The mask</param>
        /// <param name="validation">Validate if there are any mandatory fields</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool validateMP(string value, string mask, string validation)
        {
            if (string.IsNullOrEmpty(value))
                return true;

            // The '&' character means it's a required digit, while the '*' is for optional ones.
            int mandatoryChars = validation.Count(c => c == '&');

            if (value.Length < mandatoryChars || value.Length > mask.Length)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                switch (mask[i])
                {
                    case '0':
                        if (!char.IsNumber(value[i]))
                            return false;
                        break;
                    case 'a':
                        if (!char.IsLower(value[i]))
                            return false;
                        break;
                    case 'A':
                        if (!char.IsUpper(value[i]))
                            return false;
                        break;
                    case 'S':
                        if (!char.IsLetter(value[i]))
                            return false;
                        break;
                    case 'X':
                        if (!char.IsLetterOrDigit(value[i]))
                            return false;
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Validate an email address
        /// </summary>
        /// <param name="email">email address</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool validateEM(string email)
        {
            /*
            string sPattern = @"^[a-zA-Z0-9_+&*-]+(?>\.[a-zA-Z0-9_+&*-]+)*@(?>[a-zA-Z0-9-]+\.)+[a-zA-Z]{2,7}$";
            if (string.IsNullOrEmpty(email) || System.Text.RegularExpressions.Regex.IsMatch(email, sPattern))
                return true;
            else
                return false;
             */
            // The above pattern doesn't work in some cases, like the one reported here: https://forum.quidgest.net/forum/q-a-2/question/email-validator-544
            // Since C# already validates the email address in the MailAdress constructor, implementing our own regex expression, that works in every case, is probably not worth the hassle.
            try
            {
                if(string.IsNullOrEmpty(email))
                    return true;
				
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address == email;
            }
            catch
            {
                return string.IsNullOrEmpty(email);
            }
        }

        /// <summary>
        /// Validate a IBAN (ISO standard 13616)
        /// IBAN format: XXZZ CCCC BBBB 12345678901 DD
        /// XX = country code (2 letters)
        /// ZZ = control code (2 digits)
        /// CCCC BBBB 12345678901 DD = Basic Bank Account Number (country specific)
        /// </summary>
        /// <param name="iban">IBAN</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool validateIN(string iban)
        {
            // Empty value
            if (String.IsNullOrEmpty(iban))
                return true;

            // Remove hyphens and blank spaces
            iban = iban.Replace("-", String.Empty).Replace(" ", String.Empty);
            iban = iban.ToUpper();

            if (iban.Length < 4 || iban.Length > 34)
                return false;

            // Check if IBAN starts with 2 letters followed by 2 digits
            if (!Regex.IsMatch(iban, "^[A-Z]{2}[0-9]{2}"))
                return false;

            string bank = iban.Substring(4, iban.Length - 4) + iban.Substring(0, 4);
            int asciiShift = 55;
            StringBuilder sb = new StringBuilder();
            foreach (char c in bank)
            {
                int v;
                if (Char.IsLetter(c)) v = c - asciiShift;
                else v = int.Parse(c.ToString());
                sb.Append(v);
            }
            string checkSumString = sb.ToString();
            int checksum = int.Parse(checkSumString.Substring(0, 1));
            for (int i = 1; i < checkSumString.Length; i++)
            {
                int v = int.Parse(checkSumString.Substring(i, 1));
                checksum *= 10;
                checksum += v;
                checksum %= 97;
            }

            // Invalid IBAN
            if (checksum != 1)
                return false;

            return true;
        }
    }
}
