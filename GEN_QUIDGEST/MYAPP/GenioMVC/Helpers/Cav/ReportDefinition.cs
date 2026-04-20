using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;

namespace GenioMVC.Helpers.Cav
{
	/// <summary>
	/// CAV Report Definition
	/// </summary>
	[DataContract, Serializable]
	public class ReportDefinition
	{
		/// <summary>
		/// Report title
		/// </summary>
		[DataMember]
		public string Title { get; set; }

		/// <summary>
		/// Access query type
		/// </summary>
		/// <remarks>
		/// PUB - Public, Everyone
		/// PES - Personal, Owner
		/// INA - Disable, nobody
		/// </remarks>
		[DataMember]
		public string Acesso { get; set; }

		/// <summary>
		/// Report base table
		/// </summary>
		[DataMember]
		public string BaseTable { get; set; }

		/// <summary>
		/// Report base table description
		/// </summary>
		[DataMember]
		public string BaseTableDescription { get; set; }

		/// <summary>
		/// Aditional connections added by manual routine
		/// </summary>
		[DataMember]
		public List<ReportLink> ExtraPaths { get; set; }

		/// <summary>
		/// Report group report
		/// </summary>
		[DataMember]
		public ReportGroup DetailsGroup { get; set; }

		/// <summary>
		/// Report groups
		/// </summary>
		[DataMember]
		public List<ReportGroup> Groups { get; set; }

		/// <summary>
		/// Report conditions
		/// </summary>
		[DataMember]
		public ReportCondition Condition { get; set; }

		/// <summary>
		/// Peport sorting sequency
		/// </summary>
		[DataMember]
		public List<ReportOrdering> Orderings { get; set; }

		/// <summary>
		/// Report orientation
		/// </summary>
		[DataMember]
		public string Orientation { get; set; }

		/// <summary>
		/// Report format (table or form)
		/// </summary>
		[DataMember]
		public string Format { get; set; }

		/// <summary>
		/// Css style
		/// </summary>
		[DataMember]
		public string StileId { get; set; }

		/// <summary>
		/// sobre que datasources incide o relatorio
		/// </summary>
		[DataMember]
		public List<string> Years { get; set; }

		/// <summary>
		/// se as datasources aparecem consecutivamente, em inner join ou outer join
		/// </summary>
		/// <remarks>
		/// SINGLE (null)
		/// PAGE
		/// INNERPIVOT
		/// OUTERPIVOT
		/// </remarks>
		[DataMember]
		public string MultiYearMode { get; set; }

		/// <summary>
		/// Devolve uma lista com todos os campos que vão ser exibidos pelo relatório
		/// </summary>
		/// <returns>Lista de campos do relatório</returns>
		public List<ReportField> GetReportFields()
		{
			List<ReportField> result = new List<ReportField>();

			// acrescenta os cabeçalhos do report para os campos dos grupos (que não são totalizadores)
			foreach (ReportGroup g in Groups)
				foreach (ReportField f in g.Fields)
					if (string.IsNullOrEmpty(f.TotalType))
						result.Add(f);

			// acrescenta os restantes cabeçalhos dos campos escolhidos (dos que não são totalizadores nem pertencem aos grupos)
			foreach (ReportField f in DetailsGroup.Fields)
			{
				if (f.MultiDatasource)
				{
					// se for multi-datasource, "desdobram-se" as colunas pelas várias datasources
					foreach (string datasource in Years)
					{
						string title = string.Format("{0} ({1})", f.GetTitle(), datasource);
						ReportField ff = new ReportField(f);
						ff.Title = title;
						if (string.IsNullOrEmpty(f.TotalType) && !result.Exists(x => x.FieldId == f.FieldId && x.Title == title))
							result.Add(ff);
					}
				}
				else
					// no caso normal, acrescenta-se o campo
					if (string.IsNullOrEmpty(f.TotalType) && !result.Exists(x => x.FieldId == f.FieldId))
						result.Add(f);
			}

			return result;
		}
	}

	/// <summary>
	/// Represents a CAV report table connection
	/// </summary>
	[DataContract, Serializable]
	public class ReportLink
	{
		/// <summary>
		/// the source table
		/// </summary>
		[DataMember]
		public string SourceTable { get; set; }
		/// <summary>
		/// The destination table
		/// </summary>
		[DataMember]
		public string DestTable { get; set; }

		/// <summary>
		/// if the the relation direction is to a table below the value is "true" and "false" for table above
		/// </summary>
		[DataMember]
		public bool Down { get; set; }

		/// <summary>
		/// identify the join type
		/// If true --> "LEFT"
		/// If false --> "INNER"
		/// </summary>
		[DataMember]
		public bool ShowAll { get; set; }

		/// <summary>
		/// ReportLinks camparison
		/// </summary>
		/// <param name="obj">obj</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			ReportLink o = obj as ReportLink;
			return
				o != null
				&& o.DestTable == this.DestTable
				&& o.SourceTable == this.SourceTable
				&& o.Down == this.Down
				&& o.ShowAll == this.ShowAll;
		}

		/// <summary>
		/// object Hashcode
		/// </summary>
		/// <returns>Hashcode</returns>
		public override int GetHashCode()
		{
			return (SourceTable + DestTable + (Down ? "D" : "N") + (ShowAll ? "S" : "N")).GetHashCode();
		}
	}

	/// <summary>
	/// define the field presentation on reports
	/// </summary>
	/// <remarks>
	/// A ReportField must belongs always to a ReportGroup
	/// </remarks>
	[DataContract, Serializable]
	public class ReportField
	{
		public ReportField() { }

		public ReportField(ReportField other)
		{
			ColumnSize = other.ColumnSize;
			FieldId = other.FieldId;
			MultiDatasource = other.MultiDatasource;
			SelectorFunction = other.SelectorFunction;
			TableId = other.TableId;
			Title = other.Title;
			TotalType = other.TotalType;
		}

		/// <summary>
		/// Field Id
		/// </summary>
		[DataMember]
		public string FieldId { get; set; }

		/// <summary>
		/// Field table
		/// </summary>
		[DataMember]
		public string TableId { get; set; }

		/// <summary>
		/// Field title
		/// </summary>
		[DataMember]
		public string Title { get; set; }

		/// <summary>
		/// return field title if it is defined otherwise returns the field Id
		/// </summary>
		/// <returns>Field title</returns>
		public string GetTitle()
		{
			return string.IsNullOrEmpty(Title) ? FieldId : Title;
		}

		/// <summary>
		/// Field column size
		/// </summary>
		[DataMember]
		public int ColumnSize { get; set; }

		/// <summary>
		/// Agregation type
		/// </summary>
		/// <remarks>
		/// SUM
		/// AVG
		/// COUNT
		/// MAX
		/// MIN
		/// </remarks>
		[DataMember]
		public string TotalType { get; set; }

		/// <summary>
		/// Field sub-selection function.
		/// Example: "Year", "Month", "Day".
		/// </summary>
		[DataMember]
		public string SelectorFunction { get; set; }

		/// <summary>
		/// Nome do campo que vai servir de pivot para criar uma coluna para cada valor distinto.
		/// É obrigatório definir um TotalType para agregar os valores de FieldId.
		/// Este campo é opcional.
		/// </summary>
		/// <remarks>
		/// Basta existir um campo configurado assim para todas as outras colunas
		/// passarem a fazer parte do group by implicitamente.
		/// </remarks>
		public string Pivot { get; set; }

		/// <summary>
		/// Função de sub-selecção de para o pivot.
		/// Por exemplo year, month, day.
		/// </summary>
		public string PivotSelectorFunction { get; set; }

		/// <summary>
		/// Lista de valores explicita a ser considerada no pivot.
		/// Opcional.
		/// </summary>
		public List<string> PivotValues { get; set; }

		/// <summary>
		/// True se o campo deve apresentar valores para cada uma das datasources seleccionadas para o report
		/// </summary>
		/// <remarks>
		/// Basta existir um campo configurado assim para todas as outras colunas
		/// passarem a fazer parte da chave de agrupamento do multi-datasource
		/// </remarks>
		[DataMember]
		public bool MultiDatasource { get; set; }
	}

	/// <summary>
	/// Represents a CAV report group
	/// </summary>
	[DataContract, Serializable]
	public class ReportGroup
	{
		/// <summary>
		/// Lista de campos que vão ser mostrados no cabeçalho deste grupo
		/// </summary>
		/// <remarks>
		/// Todos os campos não totalizados vão fazer parte da chave do grupo
		/// Deve haver pelo menos um campo sem totalizador
		/// </remarks>
		[DataMember]
		public List<ReportField> Fields { get; set; }

		/// <summary>
		/// True se cada elemento do grupo provoca uma quebra de página
		/// </summary>
		[DataMember]
		public bool PageBreak { get; set; }
	}

	/// <summary>
	/// Reprensenta a report condition node
	/// </summary>
	/// <remarks>
	/// Uma condição é uma estrutura hierarquica sem ambiguidades.
	/// O nó da raiz será o ultimo a ser avaliado sendo as folhas as primeiras condições a serem executadas.
	/// </remarks>
	[DataContract, Serializable]
	public class ReportCondition
	{
		/// <summary>
		/// Tipo de operação ou de valor
		/// </summary>
		/// <remarks>
		/// Operações Binárias:
		///  AND
		///  OR
		///  EQ
		///  GT
		///  GET
		///  LT
		///  LET
		///  NEQ
		///  LIKE
		///  BETWEEN
		///  IN
		/// Operações Unárias:
		///  ISNULL
		///  NOT
		/// Valores:
		///  FIELD
		///  LITERAL
		/// </remarks>
		[DataMember]
		public string Operation { get; set; }

		/// <summary>
		/// Valor só é preenchido em nós literais e de referencia
		/// </summary>
		/// <remarks>
		/// Lista de valores para o operador IN deve vir preenchida em JSON
		/// </remarks>
		[DataMember]
		public string ValueReference { get; set; }

		// RR - uma lista de operandos facilita imenso o tratamento de AND's e OR's agrupados
		/// <summary>
		/// Operandos da operação, deve conter tantos elementos quantos o operador requer
		/// No caso dos operadores AND, OR e IN o número de Operandos é indeterminado,
		/// para os restantes operadores
		/// </summary>
		[DataMember]
		public List<ReportCondition> Operands { get; set; }

		/// <summary>
		/// Indica se os campos da tabela sobre a qual esta condição se aplica
		/// devem aparecer como linhas em branco no report, ou se o registo não aparece de todo.
		/// </summary>
		[DataMember]
		public bool ShowNulls { get; set; }
	}

	/// <summary>
	/// Ordenação do report
	/// </summary>
	[DataContract, Serializable]
	public class ReportOrdering
	{
		/// <summary>
		/// Direcção da ordenação
		/// </summary>
		/// <remarks>
		/// ASC: Ascendente
		/// DESC: Descendente
		/// </remarks>
		[DataMember]
		public string Direction { get; set; }

		/// <summary>
		/// Campo a ser ordenado
		/// </summary>
		[DataMember]
		public ReportField Field { get; set; }
	}

	//------------------------------------------------------------------------
	// Resposta
	//------------------------------------------------------------------------

	/// <summary>
	/// Resposta À execução de um relatório
	/// </summary>
	[DataContract, Serializable]
	public class ReportReply
	{
		/// <summary>
		/// The result Query
		/// </summary>
		[DataMember]
		public string QuerySQL { get; set; }

		/// <summary>
		/// Grupo de topo de resultados
		/// </summary>
		/// <remarks>
		/// Pode ser o primeiro grupo ou os detalhes caso não existam grupos
		/// </remarks>
		[DataMember]
		public ReportReplyGroup MainGroup { get; set; }

		/// <summary>
		/// Total de registos devolvidos
		/// </summary>
		[DataMember]
		public int ResultCount { get; set; }

		public ReportReply()
		{
			QuerySQL = "";
			MainGroup = new ReportReplyGroup()
			{
				//podem ser os detalhes ou o primeiro agrupamento
				Groups = new List<ReportReplyGroup>(),
				//estes vão ser totalizadores globais
				Values = new List<string>()
			};
		}
	}

	/// <summary>
	/// Representa um nível de agrupamento de um relatório
	/// </summary>
	[DataContract, Serializable]
	public class ReportReplyGroup
	{
		/// <summary>
		/// Valores de cabeçalho desta linha de resultados
		/// </summary>
		[DataMember(Name = "V", EmitDefaultValue = false)]
		public List<string> Values { get; set; }

		/// <summary>
		/// Caso seja um cabeçalho de um grupo este tem os sub-resultados guardados aqui
		/// </summary>
		[DataMember(Name = "G", EmitDefaultValue = false)]
		public List<ReportReplyGroup> Groups { get; set; }

		/// <summary>
		/// Verifica se este elemento é uma folha (não tem filhos)
		/// </summary>
		/// <returns>True se este elemento for uma folha, False caso contrário</returns>
		public bool IsLeaf()
		{
			return Groups == null || Groups.Count == 0;
		}
	}
}
