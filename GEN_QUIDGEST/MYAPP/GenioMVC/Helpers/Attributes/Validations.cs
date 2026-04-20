using System;
using System.ComponentModel.DataAnnotations;

namespace GenioMVC.Helpers
{
	/// <summary>
	/// Check if zip code is valid
	/// </summary>
	public class CheckZipCode : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			//Regex only for PTPT
			string sPattern = @"^\d{4}(?:[-]\d{3})?$";

			if (value == null || System.Text.RegularExpressions.Regex.IsMatch((string)value, sPattern))
				return ValidationResult.Success;
			else
				return new ValidationResult(Resources.Resources.POR_FAVOR__UTILIZE_O11731);
		}
	}

	public class CheckNIF : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value == null)
				return ValidationResult.Success;

			string nif = Convert.ToString(value);
			int checkDigit;
			char firstNumber;

			if (nif != null && nif.Length == 9)
			{
				//primeiro número do NIF
				firstNumber = nif[0];

				//Calcula o CheckDigit
				checkDigit = (Convert.ToInt16(firstNumber.ToString()) * 9);
				for (int i = 2; i <= 8; i++)
				{
					checkDigit += Convert.ToInt16(nif[i - 1].ToString()) * (10 - i);
				}
				checkDigit = 11 - (checkDigit % 11);

				//Se checkDigit for superior a 10 passa a 0
				if (checkDigit >= 10)
					checkDigit = 0;

				//Compara o digito de controle com o último number do NIF
				//Se igual, o NIF é válido.
				if (checkDigit.ToString() == nif[8].ToString())
					return ValidationResult.Success;
			}

			return new ValidationResult(Resources.Resources.O_NUMERO_DE_CONTRIBU60039);
		}
	}

	public class CheckNISS : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value == null)
				return ValidationResult.Success;

			int[] factores = { 29, 23, 19, 17, 13, 11, 7, 5, 3, 2 };

			string niss = (string)value;
			int count = 0;
			int intDigit = 0;
			int sum = 0;

			if (niss == null || niss.Length != 11)
				return new ValidationResult(Resources.Resources.O_NUMERO_DE_IDENTIFI20687);

			foreach (char digit in niss)
			{
				if (!int.TryParse(digit.ToString(), out intDigit))
					return new ValidationResult(Resources.Resources.O_NUMERO_DE_IDENTIFI20687);
				if (count == 10)
				{
					return intDigit == 9 - (sum % 10) ? ValidationResult.Success : new ValidationResult(Resources.Resources.O_NUMERO_DE_IDENTIFI20687)    ;
				}
				sum += intDigit * factores[count];
				count += 1;
			}

			return new ValidationResult(Resources.Resources.O_NUMERO_DE_IDENTIFI20687);
		}
	}

	public class CheckNIB : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value == null)
				return ValidationResult.Success;

			string iban = (string)value;
			bool Qresult = false;

			if (String.IsNullOrEmpty(iban))
				return new ValidationResult(Resources.Resources.O_NIB_NAO_E_VALIDO_64143);

			if (iban.Length != 24 || iban[4] != '-' || iban[9] != '-' || iban[21] != '-')
				return new ValidationResult(Resources.Resources.O_NIB_NAO_E_VALIDO_64143);

			//remove os espaços vazios
			iban = iban.Replace("-", string.Empty);

			if (iban.Length != 21)
				return new ValidationResult(Resources.Resources.O_NIB_NAO_E_VALIDO_64143);

			//guarda o check digit
			string digito = iban.Substring(iban.Length - 2, 2);

			//substitui o checkdigit por '00'
			iban = iban.Substring(0, 19);
			iban += "00";

			int peso = 0, res, a;

			for (int i = 1; i < iban.Length; i++)
			{
				a = int.Parse(iban.Substring(i - 1, 1));
				a = peso + a;
				peso = (a * 10) % 97;
			}

			res = 98 - peso;

			if (digito == string.Format("{0:00}", res))
				Qresult = true;

			return Qresult ? ValidationResult.Success : new ValidationResult(Resources.Resources.O_NIB_NAO_E_VALIDO_64143);
		}
	}

	/// <summary>
	/// Verifica IBAN
	/// </summary>
	public class CheckIBAN : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (CSGenio.business.Validation.validateIN((string)value))
				return ValidationResult.Success;
			else
				return new ValidationResult(Resources.Resources.O_IBAN_NAO_E_VALIDO_05871);
		}
	}

	/// <summary>
	/// Checks if a Car Plate pattern is valid (PTPT)
	/// </summary>
	public class CheckCarPlatePT : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (CSGenio.business.Validation.validateMA((string)value))
				return ValidationResult.Success;
			else
				return new ValidationResult(Resources.Resources.A_MATRICULA_NAO_E_VA55283);
		}
	}

	/// <summary>
	/// Checks if an email is valid
	/// </summary>
	public class CheckEmail : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value == null || CSGenio.core.CSmail.validateMail((string)value))
				return ValidationResult.Success;
			else
				return new ValidationResult(Resources.Resources.POR_FAVOR_INDIQUE_UM47648);
		}
	}
}
