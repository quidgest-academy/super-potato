using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using CSGenio.framework;

namespace CSGenio.business
{
	/// <summary>
	/// Type of Qvalues associated with an array
	/// </summary>
	public enum ArrayType
	{
		STRING,
		NUMERIC,
		LOGICAL
	}

	/// <summary>
	/// A generic array
	/// </summary>
	/// <typeparam name="T">The type of the enumeration element codes</typeparam>
	public abstract class Array<T>
	{
		/// <summary>
		/// The elements
		/// </summary>
		private Dictionary<T, ArrayElement> _elements;

		/// <summary>
		/// Gets the dictionary.
		/// </summary>
		public IDictionary<T, string> GetDictionaryImpl()
		{
			if (_elements == null)
				_elements = LoadDictionary();
			return _elements.ToDictionary(e => e.Key, e => e.Value.ResourceId);
		}

		/// <summary>
		/// Gets the elements.
		/// </summary>
		public List<T> GetElementsImpl()
		{
			return new List<T>(GetDictionaryImpl().Keys);
		}

		/// <summary>
		/// Gets the element.
		/// </summary>
		/// <param name="cod">The element's code.</param>
		public ArrayElement GetElementImpl(T cod)
		{
			if(_elements == null)
				_elements = LoadDictionary();
            if (_elements.ContainsKey(cod))
				return _elements[cod];
			return null;
		}

		/// <summary>
		/// Gets the element's description.
		/// </summary>
		/// <param name="cod">The element's code.</param>
		public string CodToDescricaoImpl(T cod)
		{
			return CodToDescricaoImpl(cod, null);
		}

		/// <summary>
		/// Gets the element's description.
		/// </summary>
		/// <param name="cod">The element's code.</param>
		/// <param name="lang">The user language.</param>
		public string CodToDescricaoImpl(T cod, string lang)
		{
			var dict = GetDictionaryImpl();

			if (dict.ContainsKey(cod))
				return Translations.GetByCode(dict[cod], lang);
			return string.Empty;
		}

		/// <summary>
		/// Gets the element's help identifier.
		/// </summary>
		/// <param name="cod">The element's code.</param>
		/// <param name="lang">The user language.</param>
		public string GetHelpIdImpl(T cod)
		{
			return GetElementImpl(cod)?.HelpId;
		}

		/// <summary>
		/// Serializes this instance.
		/// </summary>
		/// <param name="lang">The user language.</param>
		public List<object> SerializeImpl(string lang = null)
		{
			if (_elements == null)
				_elements = LoadDictionary();

			return _elements
				.Select(e => new
					{
						Key = e.Key,
						Text = GetText(e.Key, lang, e.Value),
						Group = e.Value.Group 
					}
				)
				.Cast<object>()
				.ToList();
		}

		/// <summary>
		/// Gets the text.
		/// </summary>
		/// <param name="cod">The element's code.</param>
		/// <param name="lang">The user language.</param>
		/// <param name="element">The element.</param>
		private string GetText(T cod, string lang, ArrayElement element)
		{
			if (!string.IsNullOrEmpty(element.ResourceId))
				return CodToDescricaoImpl(cod, lang);
			
			string text = element.GetText(lang);
			return string.IsNullOrEmpty(text) ? cod.ToString() : text;
		}

		/// <summary>
		/// Loads the dictionary.
		/// </summary>
		abstract protected Dictionary<T, ArrayElement> LoadDictionary();
	}

	/// <summary>
	/// An array element
	/// </summary>
	public class ArrayElement
	{
		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		/// <value>
		/// The key.
		/// </value>
		public object Key { get; set; }

		/// <summary>
		/// Gets or sets the resource identifier.
		/// </summary>
		/// <value>
		/// The resource identifier.
		/// </value>
		public string ResourceId { get; set; }

		/// <summary>
		/// Gets or sets the help identifier.
		/// </summary>
		/// <value>
		/// The help identifier.
		/// </value>
		public string HelpId { get; set; }

		/// <summary>
		/// Gets or sets the group.
		/// </summary>
		/// <value>
		/// The group.
		/// </value>
		public string Group { get; set; }

		/// <summary>
		/// Gets the texts.
		/// </summary>
		/// <value>
		/// The texts.
		/// </value>
		public Dictionary<string, string> Texts { get; } 
			= new Dictionary<string, string>();

		public string GetText(string lang)
		{
			if (Texts.ContainsKey(lang))
				return Texts[lang];
			else if (Texts.ContainsKey(Translations.BASE_LANG))
				return Texts[Translations.BASE_LANG];
			return string.Empty;
		}
	}

	/// <summary>
	/// Classe auxiliar de acesso genérico aos metadados dos arrays
	/// </summary>
	/// <remarks>
	/// Esta classe não tem grandes preocupações de performance.
	/// Caso venha a ser usada de forma intensiva deve fazer uso de cache.
	/// </remarks>
	public class ArrayInfo
	{
		/// <summary>
		/// Constructor dos metadados de um array
		/// </summary>
		/// <param name="id">O name do array</param>
		public ArrayInfo(string id)
		{

			m_classType = QCache.Instance.Array.Get(id) as System.Type;
			if (m_classType == null)
			{
                m_classType = typeof(Area).Assembly.GetType("CSGenio.business.Array" + id, false, true);
                //if not found it will search on Dynamic array class assembly
				if (m_classType == null)				
					m_classType = System.Type.GetType("CSGenio.business.Array" + id + ", GenioServer", false, true);

                if (m_classType == null)
                    throw new BusinessException(null, "Array.ArrayInfo", "The array " + id + " has no associated class.");
                
                QCache.Instance.Array.Put(id, m_classType);
            }		
			
			Name = id;
			Type = (ArrayType)m_classType.GetProperty("Type").GetValue(null, null);
			
			//suporte to arrays com replaces de Qyear
			//To be deprecated when possible
			IsYearReplace = Elements.Any(e => e.Contains("#4#"));
		}

		/// <summary>
		/// cache da referencia to a classe
		/// </summary>
		private readonly Type m_classType;

		/// <summary>
		/// Name do array
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Type de array
		/// </summary>
		public ArrayType Type { get; set; }

		/// <summary>
		/// True se o array suporta o replace de expressões de Qyear
		/// </summary>
		/// <remarks>To be deprecated when possible</remarks>
		public bool IsYearReplace { get; set; }
		
		/// <summary>
		/// Lista de elementos do array
		/// </summary>
		public List<string> Elements
		{
			get
			{
				IEnumerable elements = m_classType.InvokeMember("GetElements", 
					System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod,
					null, 
					null,
					null) as IEnumerable;

				if (Type == ArrayType.STRING)
					return elements as List<string>;

				List<string> res = new List<string>();
				foreach(object elem in elements)
					res.Add(elem.ToString());
				return res;
			}
		}

		/// <summary>
		/// Obtem a descrição de um elemento a partir do seu codigo
		/// </summary>
		/// <param name="elem">O elemento a procurar</param>
		/// <param name="language">A lingua em que queremos a decrição do array</param>
		/// <returns>A descrição do elemento do array</returns>
		public string GetDescription(string elem, string language)
		{
			String description;
			switch (Type)
			{
				case ArrayType.STRING:
					description = m_classType.InvokeMember("CodToDescricao",
						System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod,
						null,
						null,
						new object[] { elem }) as string;
					break;
				case ArrayType.NUMERIC:
					{
						decimal val = Decimal.Parse(elem);
						description = m_classType.InvokeMember("CodToDescricao",
							System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod,
							null,
							null,
							new object[] { val }) as string;
						break;
					}
				case ArrayType.LOGICAL:
					{
						int val = int.Parse(elem);
						description = m_classType.InvokeMember("CodToDescricao",
							System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod,
							null,
							null,
							new object[] { val }) as string;
						break;
					}
				default:
					throw new BusinessException(null, "Array.GetDescription", "Unknown array type: " + Type);
			}
			return Translations.Get(description, language);
		}

		/// <summary>
		/// Returns the help of an array element in a certain language
		/// </summary>
		public string GetHelp(string elem, string lang)
		{            
			string helpId = m_classType.InvokeMember("GetHelpId",
				System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod,
				null,
				null,
				new object[] { elem }) as string;
			return Translations.GetByCode(helpId, lang);
		}
		
		/// <summary>
		/// Returns a dictionary of array elements as an object
		/// </summary>
		public object GetDictionaryObject()
		{
			return m_classType.InvokeMember("GetDictionary",
				System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod,
				null,
				null,
				null);
		}
	}
}
