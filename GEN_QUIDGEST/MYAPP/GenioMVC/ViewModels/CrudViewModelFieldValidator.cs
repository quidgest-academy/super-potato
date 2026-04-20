using CSGenio.framework;
using System.Globalization;

namespace GenioMVC.ViewModels
{
    /// <summary>
    ///	Represents the result of view model validation in a CRUD scenario,
    ///	containing a dictionary of model errors.
    /// </summary>
    public class CrudViewModelValidationResult
    {
        /// <summary>
        ///	Gets the dictionary of model errors, where the key is the field name
        ///	and the value is a list of error messages associated with that field.
        /// </summary>
        public Dictionary<string, IList<string>> ModelErrors { get; } = [];

        /// <summary>
        ///	Gets a value indicating whether the validation was successful (no rules were broken).
        /// </summary>
        public bool IsValid => ModelErrors.Count == 0;

        /// <summary>
        ///	Adds a validation error message for a specific field.
        /// </summary>
        /// <param name="field">The name of the field where the error occurred.</param>
        /// <param name="errorMessage">The error message to be associated with the field.</param>
        public void AddModelError(string field, string errorMessage)
        {
            if (!ModelErrors.TryGetValue(field, out IList<string>? value))
            {
                value = [];
                ModelErrors[field] = value;
            }

            value.Add(errorMessage);
        }

        /// <summary>
        /// Merges the validation errors from another instance of CrudViewModelValidationResult into the current instance.
        /// </summary>
        /// <param name="nested">The instance of CrudViewModelValidationResult to merge into the current instance.</param>
        public void Merge(CrudViewModelValidationResult from)
        {
            Merge(from.ModelErrors);
        }

        /// <summary>
        /// Merges the validation errors from another instance of CrudViewModelValidationResult into the current instance.
        /// </summary>
        /// <param name="nested">The instance of CrudViewModelValidationResult to merge into the current instance.</param>
        /// <param name="nestingKey">The nesting key to be used for merging.</param>
        public void Merge(CrudViewModelValidationResult from, string nestingKey)
        {
            Merge(from.ModelErrors, nestingKey);
        }

        /// <summary>
        /// Merges the specified dictionary of model errors into the current instance.
        /// </summary>
        /// <param name="from">The dictionary of model errors to merge into the current instance.</param>
        /// <param name="nestingKey">The nesting key to be used for merging.</param>
        private void Merge(IDictionary<string, IList<string>> from, string nestingKey = null)
        {
            foreach (var (field, messages) in from)
            {
                string key = string.IsNullOrEmpty(nestingKey) ? field : $"{nestingKey}.{field}";

                if (!ModelErrors.TryGetValue(key, out IList<string>? value))
                {
                    value = [];
                    ModelErrors[key] = value;
                }

                foreach (var message in messages)
                {
                    value.Add(message);
                }
            }
        }
    }

    /// <summary>
    /// Provides validation methods for CRUD ViewModel fields.
    /// </summary>
    public class CrudViewModelFieldValidator
    {
        private readonly string _language;
        private readonly CrudViewModelValidationResult _aggregator = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CrudViewModelFieldValidator"/> class with the specified language.
        /// </summary>
        /// <param name="language">The language code used to translate validation error messages.</param>
        public CrudViewModelFieldValidator(string language)
        {
            _language = language;
        }

        /// <summary>
        /// Validates whether a field is required and adds an error to the aggregator if invalid.
        /// </summary>
        /// <param name="fieldName">The name of the field being validated.</param>
        /// <param name="fieldTitle">The title of the field being validated.</param>
        /// <param name="value">The value to validate for being required.</param>
        /// <param name="format">The format of the field to be validated.</param>
        public void Required(string fieldName, string fieldTitle, object? value, FieldFormatting? format = null)
        {
            bool isValid = CrudViewModelFieldValidatorImpl.Required(value, format);

            if (!isValid)
            {
                string errorMessage = string.Format(
                    Translations.Get("O campo {0} é obrigatório.", _language),
                    fieldTitle
                );

                _aggregator.AddModelError(fieldName, errorMessage);
            }
        }

        /// <summary>
        /// Validates whether a field is required and adds an error to the aggregator if invalid.
        /// For fields that may not receive a value from the client side, such as encrypted fields, 
        /// the value will be validated not only in the ViewModel but also in the Model (database value). One of them must be filled.
        /// </summary>
        /// <param name="fieldName">The name of the field being validated.</param>
        /// <param name="fieldTitle">The title of the field being validated.</param>
        /// <param name="values">The value(s) to validate for being required.</param>
        /// <param name="format">The format of the field to be validated.</param>
        public void Required(string fieldName, string fieldTitle, object?[] values, FieldFormatting? format = null)
        {
            bool isValid = values?.Any(value => CrudViewModelFieldValidatorImpl.Required(value, format)) ?? false;

            if (!isValid)
            {
                string errorMessage = string.Format(
                    Translations.Get("O campo {0} é obrigatório.", _language),
                    fieldTitle
                );

                _aggregator.AddModelError(fieldName, errorMessage);
            }
        }

        /// <summary>
        /// Validates whether a field is a valid email and adds an error to the aggregator if invalid.
        /// </summary>
        /// <param name="fieldName">The name of the field being validated.</param>
        /// <param name="value">The value to validate.</param>
        public void Email(string fieldName, string value)
        {
            bool isValid = CrudViewModelFieldValidatorImpl.Email(value);

            if (!isValid)
            {
                string errorMessage = Translations.Get("Por favor indique um email válido!", _language);
                _aggregator.AddModelError(fieldName, errorMessage);
            }
        }

        /// <summary>
        /// Validates a field using a hyperlink pattern and adds an error to the aggregator if invalid.
        /// </summary>
        /// <param name="fieldName">The name of the field being validated.</param>
        /// <param name="value">The value to validate against the hyperlink pattern.</param>
        public void Hyperlink(string fieldName, object? value)
        {
            string pattern = "^(http|ftp|https|www)://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&\\*\\(\\)_\\-\\=\\+\\\\/\\?\\.\\:\\;\\'\\,]*)?$";
            bool isValid = CrudViewModelFieldValidatorImpl.RegularExpression(value, pattern);

            if (!isValid)
            {
                string errorMessage = string.Format(Translations.Get("O campo {0} tem um endereço inválido.", _language), fieldName);
                _aggregator.AddModelError(fieldName, errorMessage);
            }
        }

        /// <summary>
        /// Validates the length of the provided string value against the specified maximum length for a given field.
        /// Adds a validation error to the aggregator if the length exceeds the specified maximum.
        /// </summary>
        /// <param name="fieldName">The name of the field being validated, used for error message identification.</param>
        /// <param name="fieldTitle">The title of the field being validated.</param>
        /// <param name="value">The string value to be validated for length.</param>
        /// <param name="maximumLength">The maximum allowed length for the string.</param>
        public void StringLength(string fieldName, string fieldTitle, object? value, int maximumLength)
        {
            bool isValid = CrudViewModelFieldValidatorImpl.StringLength(value, maximumLength);

            if (!isValid)
            {
                string errorMessage = string.Format(
                    Translations.Get(
                        "O comprimento máximo para o campo {0} é de {1} caracteres.",
                        _language
                    ),
                    fieldTitle,
                    maximumLength
                );

                _aggregator.AddModelError(fieldName, errorMessage);
            }
        }

        /// <summary>
        /// Validates a password and its confirmation for equality and adds an error message if they do not match.
        /// </summary>
        /// <param name="fieldName">The name of the password field being validated.</param>
        /// <param name="value">The password value.</param>
        /// <param name="confirmationValue">The confirmation value for the password.</param>
        public void Password(string fieldName, string value, string confirmationValue)
        {
            // Check if the password matches the confirmation
            bool isValid = Equals(value, confirmationValue);

            if (!isValid)
            {
                // If not valid, retrieve and set an error message
                string errorMessage = Translations.Get(
                    "A nova palavra-chave e a confirmação não são iguais.",
                    _language
                );

                _aggregator.AddModelError(fieldName, errorMessage);
            }
        }

        /// <summary>
        /// Merges the validation result from another CrudViewModelValidationResult instance.
        /// </summary>
        /// <param name="nested">The CrudViewModelValidationResult instance to be merged.</param>
        public void Merge(CrudViewModelValidationResult nested)
        {
            _aggregator.Merge(nested);
        }

        /// <summary>
        /// Merges the validation result from another CrudViewModelValidationResult instance.
        /// </summary>
        /// <param name="nested">The CrudViewModelValidationResult instance to be merged.</param>
        /// <param name="nestingKey">The nesting key to be used for merging.</param>
        public void Merge(CrudViewModelValidationResult nested, string nestingKey)
        {
            _aggregator.Merge(nested, nestingKey);
        }

        /// <summary>
        /// Gets the aggregated validation result.
        /// </summary>
        /// <returns>The aggregated validation result.</returns>
        public CrudViewModelValidationResult GetResult()
        {
            return _aggregator;
        }
    }

    /// <summary>
    /// Implementation of the validation methods for CRUD ViewModel fields.
    /// </summary>
    /// <remarks>
    ///	Based on the .NET validation attribute classes.
    ///	https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations
    /// </remarks>
    public class CrudViewModelFieldValidatorImpl
    {
        /// <summary>
        /// Validates that a data field is not empty.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <param name="format">The format of the field to be validated.</param>
        /// <returns>True if the value is not empty; otherwise, false.</returns>
        public static bool Required(object? value, FieldFormatting? format)
        {
            if (value is null || (format != null && Field.isEmptyValue(value, (FieldFormatting)format)))
            {
                return false;
            }

            return value is not string stringValue || !string.IsNullOrWhiteSpace(stringValue);
        }

        /// <summary>
        /// Validates that a data field is a valid email.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <returns>True if the value is not valid; otherwise, false.</returns>
        public static bool Email(string value)
        {
            return CSGenio.business.Validation.validateEM(value);
        }

        /// <summary>
        /// Validates whether a given value matches a specified regular expression pattern.
        /// </summary>
        /// <param name="value">The value to validate against the regular expression.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>
        ///     <c>true</c> if the value matches the regular expression pattern or if the value is null or empty;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public static bool RegularExpression(object? value, string pattern)
        {
            System.Text.RegularExpressions.Regex regex = new(pattern);

            // Convert the value to a string
            string stringValue = Convert.ToString(value, CultureInfo.CurrentCulture);

            // Automatically pass if value is null or empty. `Required` should be used to assert a value is not empty.
            if (string.IsNullOrEmpty(stringValue))
            {
                return true;
            }

            System.Text.RegularExpressions.Match m = regex.Match(stringValue);

            // We are looking for an exact match, not just a search hit. This matches what
            // the RegularExpressionValidator control does
            return (m.Success && m.Index == 0 && m.Length == stringValue.Length);
        }

        /// <summary>
        /// Validates the length of the provided string value.
        /// Automatically passes if the value is null. Use `Required` to assert that a value is not null.
        /// Expects a cast exception if a non-string was passed in.
        /// </summary>
        /// <param name="value">The string value to be validated for length.</param>
        /// <param name="maximumLength">The maximum allowed length for the string.</param>
        /// <returns>
        /// Returns true if the value is null or its length is less than or equal to the specified maximum length;
        /// otherwise, returns false.
        /// </returns>
        public static bool StringLength(object value, int maximumLength)
        {
            // Automatically pass if value is null. `Required` should be used to assert a value is not null.
            // We expect a cast exception if a non-string was passed in.
            int length = value == null ? 0 : ((string)value).Length;
            return value == null || length <= maximumLength;
        }
    }
}
