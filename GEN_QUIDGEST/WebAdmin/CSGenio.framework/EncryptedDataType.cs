namespace CSGenio.framework
{
    /// <summary>
    /// Class to store the encrypted and decrypted value of the field.
    /// TODO: Check if need to store the value in an object to allow the class to be used for other purposes in the future, 
    ///    or change the stored object's value to a String. 
    ///    It is currently only used for password fields.
    /// </summary>
    public class EncryptedDataType
    {
        /// <summary>
        /// The encrypted version of the field value
        /// </summary>
        public object EncryptedValue { get; set; }
        /// <summary>
        /// The decrypted version of the field value
        /// </summary>
        public object DecryptedValue { get; set; }

        public EncryptedDataType()
        {
            EncryptedValue = null;
            DecryptedValue = null;
        }

        public EncryptedDataType(object encryptedValue, object decryptedValue)
        {
            EncryptedValue = encryptedValue;
            DecryptedValue = decryptedValue;
        }

        /// <summary>
        /// It is considered empty if it does not have any of the value versions (encrypted or decrypted).
        /// </summary>
        /// <returns>True, if the value is considered empty</returns>
        public bool IsEmpty()
        {
            return (EncryptedValue == null || (EncryptedValue is string ev && string.IsNullOrEmpty(ev))) 
                && (DecryptedValue == null || (DecryptedValue is string uv && string.IsNullOrEmpty(uv)));
        }

        ///// <summary>
        ///// Whenever a string conversion is requested, we will only use the encrypted value.
        ///// </summary>
        ///// <param name="value">Encrypted value</param>
        public static explicit operator EncryptedDataType(string value) => new EncryptedDataType(value, null);

        /// <summary>
        /// Whenever a string conversion is requested, we will only use the encrypted value.
        /// </summary>
        /// <param name="value">Encrypted value</param>
        public static implicit operator string(EncryptedDataType value) => value?.ToString();

        /// <summary>
        /// Whenever a string conversion is requested, we will only use the encrypted value.
        /// </summary>
        public override string ToString() => EncryptedValue?.ToString() ?? string.Empty;
    }
}