using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace GenioMVC.Helpers.Cav
{
	public class CavLimit
	{
		public string Nome { get; set; }
		public string QueryValues { get; set; }
		public List<string> Levels { get; set; }
		public List<CavLimitFilter> Filters { get; set; }

		public CavLimit(XmlCavLimit xml)
		{
			this.Nome = xml.Nome;
			this.QueryValues = xml.QueryValues;
			this.Levels = new List<string>(xml.Levels);

			this.Filters = new List<CavLimitFilter>(xml.Filters.Count);

			foreach (var xmlFilter in xml.Filters)
				this.Filters.Add(new CavLimitFilter(xmlFilter, this));
		}

		public SelectQuery GetQueryValues(string userId, string schema)
		{
			XmlDocument doc = new XmlDocument();
			XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "utf-16", null);
			XmlElement rootNode = doc.CreateElement("ControlQueryDefinitionSurrogate");
			rootNode.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
			rootNode.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
			rootNode.InnerXml = QueryValues;
			doc.InsertBefore(xmlDeclaration, doc.DocumentElement);
			doc.AppendChild(rootNode);

			// os replaces são mais eficientes com um StringBuilder
			StringBuilder queryStr = new StringBuilder(doc.InnerXml);
			queryStr.Replace("@ID", userId);
			queryStr.Replace("@SCHEMA", schema);

			XmlSerializer s = new XmlSerializer(typeof(ControlQueryDefinitionSurrogate));
			SelectQuery qValues = null;

			using (StringReader sr = new StringReader(queryStr.ToString()))
				qValues = (s.Deserialize(sr) as ControlQueryDefinitionSurrogate).Object.ToSelectQuery();

			return qValues;
		}

		public bool StoreValues()
		{
			foreach (var f in Filters)
				if (f.Repeat)
					return true;

			return false;
		}
	}

	public class CavLimitFilter
	{
		public string Area { get; set; }
		public string Campo { get; set; }
		public string Operator { get; set; }
		public string Criteria { get; set; }
		public bool Repeat { get; set; }

		private CavLimit limit;
		private List<string> limitedAreas;

		public CavLimitFilter(XmlCavLimitFilter xml, CavLimit limit)
		{
			this.Area = xml.Area;
			this.Campo = xml.Campo;
			this.Operator = xml.Operator;
			this.Criteria = xml.Criteria;
			this.Repeat = xml.Repeat;
			this.limit = limit;
			this.limitedAreas = new List<string>(xml.LimitedAreas);
		}

		public bool IsApplyedTo(List<string> queryAreas)
		{
			foreach (var a in queryAreas)
				if (limitedAreas.Contains(a))
					return true;

			return false;
		}

		private Criteria CreateCriteriaFromXml(string xml)
		{
			XmlDocument doc = new XmlDocument();
			XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "utf-16", null);
			XmlElement rootNode = doc.CreateElement("CriteriaSurrogate");
			rootNode.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
			rootNode.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
			rootNode.InnerXml = xml;
			doc.InsertBefore(xmlDeclaration, doc.DocumentElement);
			doc.AppendChild(rootNode);

			XmlSerializer s = new XmlSerializer(typeof(ControlQueryDefinitionSurrogate.CriteriaSurrogate));
			Criteria crit = null;

			using (StringReader sr = new StringReader(doc.InnerXml))
				crit = (s.Deserialize(sr) as ControlQueryDefinitionSurrogate.CriteriaSurrogate).Object;

			return crit;
		}

		// vale a pena criar duas classes para LimitFilter, cada uma com a sua implementação do GetCriteria
		// e ambas passam a devolver CriteriaSet? Ficava mais correcto e perceptivel

		public Criteria GetCriteria(string userId, string schema, string suffix)
		{
			// os replaces são mais eficientes com um StringBuilder
			StringBuilder critStr = new StringBuilder(Criteria);

			// TODO: proteger isto com CorrigeStringXml ?
			// será necessário?
			critStr.Replace("@QUERYVALUES", limit.QueryValues);
			critStr.Replace("@ID", userId);
			critStr.Replace("@TABELA", Area + suffix);
			critStr.Replace("@CAMPO", Campo);
			critStr.Replace("@SCHEMA", schema);

			Criteria crit = CreateCriteriaFromXml(critStr.ToString());

			return crit;
		}

		public List<Criteria> GetCriteria(List<object> values, string suffix)
		{
			// os replaces são mais eficientes com um StringBuilder
			StringBuilder critStr = new StringBuilder(Criteria);

			// TODO: proteger isto com CorrigeStringXml ?
			// será necessário?
			critStr.Replace("@TABELA", Area + suffix);
			critStr.Replace("@CAMPO", Campo);

			Criteria crit = CreateCriteriaFromXml(critStr.ToString());

			List<Criteria> result = new List<Criteria>();

			// neste caso assume-se que o RightTerm é sempre preenchido com o valor,
			// dado que a condição é algo do género "tabela.campo = valor"
			// faz-se assim para simplificar e não ser necessário serializar valores
			// para xml antes de reconstruir a query
			// caso seja necessário definir condições mais complexas, tem de se serializar
			// os valores ou encontrar uma forma alternativa para fazer isto
			foreach (var v in values)
			{
				Criteria critV = crit.Clone() as Criteria;

				if (critV.Operation == CriteriaOperator.Like)
					critV.RightTerm = v.ToString() + "%";
				else
					critV.RightTerm = v;

				result.Add(critV);
			}

			return result;
		}
	}
}
