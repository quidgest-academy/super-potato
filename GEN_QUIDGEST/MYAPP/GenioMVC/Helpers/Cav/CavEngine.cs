using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace GenioMVC.Helpers.Cav
{
	/// <summary>
	/// Cav Query engine
	/// </summary>
	public class CavEngine
	{
		private XmlCavService meta;

		public CavEngine(XmlCavService meta)
		{
			this.meta = meta;
		}

		// Lista de excepções para a função RemoveDiacritics
		// Isto está relacionado com as Collations do SQLServer
		// que identifica como iguais os caracteres "á" e "à" e "a"
		// MAS identifica como diferentes os caracteres "c" e "ç"
		// isto no futuro terá de ser adaptado a cada collation muito possivelmente
		// (espero que se encontre outra solução viável, porque esta pode requerer muito trabalho para manter)
		// TODO: experimentar como se comporta o Oracle!!
		private static List<char> DiacriticsExclude = new List<char>() { 'c', 'C' };

		// remove os diacriticos (acentuações) de uma string
		private static string RemoveDiacritics(string text)
		{
			string formD = text.Normalize(NormalizationForm.FormKD);
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < formD.Length; i++)
			{
				char ch = formD[i];
				UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
				if (uc != UnicodeCategory.NonSpacingMark || (i < (formD.Length - 1) && DiacriticsExclude.Contains(formD[i + 1])))
					sb.Append(ch);
			}

			return sb.ToString().Normalize(NormalizationForm.FormKC);
		}

		/// <summary>
		/// Percorre a definição de uma query á procura de tabelas referenciadas
		/// </summary>
		/// <param name="query">A query a analisar</param>
		/// <param name="us">utilizador</param>
		/// <returns>A lista de tabelas referenciadas pela query</returns>
		private List<string> GetTableNames(ReportDefinition query/*, CavUserSystem us*/)
		{
			List<string> res = new List<string>();
			//basetable
			res.Add(query.BaseTable.ToUpper());

			//lista de campos de detalhes
			if (query.DetailsGroup?.Fields != null)
				foreach (var f in query.DetailsGroup.Fields)
					AddUnique(res, f.TableId);

			//lista de campos de grupos
			if (query.Groups != null)
				foreach (var g in query.Groups)
					if (g.Fields != null)
						foreach (var f in g.Fields)
							AddUnique(res, f.TableId);

			//lista de ordenacoes
			if (query.Orderings != null)
				foreach (var o in query.Orderings)
				{
					string table = GetFieldMetadata(o.Field.FieldId).Alias;
					AddUnique(res, table);
				}

			//lista de condicoes
			if (query.Condition != null)
				ConditionTableNamesRecurse(res, query.Condition);

			return res;
		}

		/// <summary>
		/// Função recursiva para descobrir tabelas que são referenciadas nas condições
		/// </summary>
		/// <param name="res">[in, out] listas de tabelas referenciadas</param>
		/// <param name="reportCondition">A condição a analisar</param>
		private void ConditionTableNamesRecurse(List<string> res, ReportCondition reportCondition)
		{
			if (reportCondition == null)
				return;

			//só nos interessam as referencias a campos
			if (reportCondition.Operation == "FIELD")
			{
				string table = GetFieldMetadata(reportCondition.ValueReference).Alias;
				AddUnique(res, table);
			}

			//verificar recursivamente todos os elementos da condição
			if (reportCondition.Operands != null)
				for (int i = 0; i < reportCondition.Operands.Count; i++)
					ConditionTableNamesRecurse(res, reportCondition.Operands[i]);
		}

		private MetaCampo GetFieldMetadata(string fld)
		{
			string[] id = fld.Split('.');
			var m = meta.GetFieldInfo(id[0], id[1], new List<string>() { "Field.Id", "Field.BDname", "Field.TableId", "Field.InternalType", "Field.Array", "Field.ArrayClassName" });
			return new MetaCampo()
			{
				Nome = id[1],
				NomeCompleto = m[1],
				Alias = m[2],
				ForCampo = (FieldFormatting)Enum.Parse(typeof(FieldFormatting), m[3]),
				Array = m[4],
				ArrayClassName = m[5]
			};
		}

		private MetaTable GetTableMetadata(string tbl)
		{
			var m = meta.GetTableInfo(tbl, new List<string>() { "Table.Id", "Table.DBname", "Table.PrimaryKeyDBName" });
			return new MetaTable()
			{
				Alias = m[0],
				NomeTabela = m[1],
				NomeChavePrimaria = m[2]
			};
		}

		/// <summary>
		/// Adiciona uma string a uma lista caso ainda não esteja lá
		/// </summary>
		/// <param name="res">[in, out] A lista de strings unicas</param>
		/// <param name="s">A string a adicionar</param>
		private void AddUnique(List<string> res, string s)
		{
			if (!res.Contains(s))
				res.Add(s);
		}

		/// <summary>
		/// Compila a string de from de uma query dada a lista de tabelas a que é necessário aceder
		/// </summary>
		/// <param name="basetable">A tabela base da query</param>
		/// <param name="links">A lista de links extra que devem ser incluidos na query</param>
		/// <param name="tableNames">A lista de tabelas referenciadas pela query</param>
		/// <returns>A string de joins a ser usada</returns>
		private SelectQuery CompileJoins(ReportDefinition report, /*List<CSGenioAdtsrc> datasources,*/ List<string> tableNames)
		{
			SelectQuery sq = new SelectQuery();
			sq.noLock = true;

			var info = GetTableMetadata(report.BaseTable);
			string lastsource = null;

			//sufixo para aplicar nos alias dos joins
			string suffix = "";
			//prefixo de namespace para aplicar aos nomes das tabelas
			string schema = null;

			//calcular o/os joins de datasources
			//FROM QUI2011.dbo.[quiPROVE] quiprove2011
			//FULL OUTER JOIN QUI2010.dbo.[quiPROVE] quiprove2010 ON quiprove2011.CLIENTE = quiprove2010.CLIENTE
			//FULL OUTER JOIN QUI2009.dbo.[quiPROVE] quiprove2009 ON quiprove2011.CLIENTE = quiprove2009.CLIENTE

			// TODO
			// os joins para reports multi-datasource da forma como estão feitos actualmente não permitem mais do que uma tabela nos modos INNER e OUTER
			// considerar dividir em 2 partes, produto cartesiano de cada um dos anos no from, filtrando no where pelos campos não desdobrados
			//SELECT
			//  (COALESCE([VENCI2011].[DATA],[VENCI2010].[DATA])) AS [VENCI.DATA],
			//  ([VENCI2011].[VENCIMEN]) AS [VENCI.VENCIMEN2011],
			//  ([VENCI2010].[VENCIMEN]) AS [VENCI.VENCIMEN2010],
			//  (COALESCE([VENCI2011].[OPERCRIA],[VENCI2010].[OPERCRIA])) AS [VENCI.OPERCRIA],
			//  (COALESCE([TECNA2011].[TELEFONE],[TECNA2010].[TELEFONE])) AS [TECNA.TELEFONE]
			//FROM
			//  [QUI2011].[dbo].[QUIVENCI] AS [VENCI2011]
			//  LEFT JOIN [QUI2011].[dbo].[QUITECNI] AS [TECNA2011] ON [VENCI2011].[CODTECNA] = [TECNA2011].[CODTECNI],

			//  [QUI2010].[dbo].[QUIVENCI] AS [VENCI2010]
			//  LEFT JOIN [QUI2010].[dbo].[QUITECNI] AS [TECNA2010] ON [VENCI2010].[CODTECNA] = [TECNA2010].[CODTECNI]
			//WHERE
			//  [VENCI2011].[DATA] = [VENCI2010].[DATA] AND [VENCI2011].[OPERCRIA] = [VENCI2010].[OPERCRIA] AND [TECNA2011].[TELEFONE] = [TECNA2010].[TELEFONE]

			// analizar para os casos das checkboxes com "mostrar todos os resultados"
			// OR ([VENCI2011].[CODVENCI] IS NULL OR [VENCI2010].[CODVENCI] IS NULL OR [TECNA2011].[CODTECNI] IS NULL OR [TECNA2010].[CODTECNI] IS NULL)

			if (lastsource == null)
				sq = sq.From(schema, info.NomeTabela, info.Alias + suffix);
			else
			{
				CriteriaSet criteria = CriteriaSet.And();
				foreach (ReportField field in report.DetailsGroup.Fields)
					if (!field.MultiDatasource)
					{
						var fieldInfo = GetFieldMetadata(field.FieldId);
						criteria = criteria.Equal(lastsource, fieldInfo.NomeCompleto, info.Alias + suffix, fieldInfo.NomeCompleto);
					}
				if (report.MultiYearMode == "OUTER")
					sq = sq.Join(schema, info.NomeTabela, info.Alias + suffix, TableJoinType.Full).On(criteria);
				else
					sq = sq.Join(schema, info.NomeTabela, info.Alias + suffix, TableJoinType.Inner).On(criteria);
			}
			lastsource = info.Alias + suffix;

			List<string> visited = new List<string>();

			//caminhos extra definidos na query
			//---------------------------------------------
			if (report.ExtraPaths != null)
			{
				foreach (var l in report.ExtraPaths)
				{
					//para baixo faz inner joins
					if (l.Down)
					{
						var srcInfo = GetTableMetadata(l.DestTable);
						var path = meta.GetCaminho(srcInfo.Alias, l.SourceTable);
						//aqui so vamos suportar relações directas porque o utilizador teve de fazer uma navegação explicita
						if (path.Count > 1)
							throw new Exception("Only direct links are supported");

						if (!visited.Contains(path[0].AliasSourceTab))
						{
							visited.Add(path[0].AliasSourceTab);
							sq = sq.Join(schema, path[0].SourceTable, path[0].AliasSourceTab + suffix, l.ShowAll ? TableJoinType.Left : TableJoinType.Inner)
								.On(CriteriaSet.And()
									.Equal(path[0].AliasSourceTab + suffix, path[0].SourceRelField,
											path[0].AliasTargetTab + suffix, path[0].TargetRelField));
						}
					}
					//para cima faz left joins
					else
					{
						var srcInfo = GetTableMetadata(l.SourceTable);
						var path = meta.GetCaminho(srcInfo.Alias, l.DestTable);
						//aqui so vamos suportar relações directas porque o utilizador teve de fazer uma navegação explicita
						if (path.Count > 1)
							throw new Exception("Only direct links are supported");

						if (!visited.Contains(path[0].AliasTargetTab))
						{
							visited.Add(path[0].AliasTargetTab);

							sq = sq.Join(schema, path[0].TargetTable, path[0].AliasTargetTab + suffix, l.ShowAll ? TableJoinType.Left : TableJoinType.Inner)
								.On(CriteriaSet.And()
									.Equal(path[0].AliasSourceTab + suffix, path[0].SourceRelField,
											path[0].AliasTargetTab + suffix, path[0].TargetRelField));
						}
					}
				}
			}

			//caminhos indirectos inferidos dos metadados
			//---------------------------------------------
			foreach (var t in tableNames)
			{
				if (t != report.BaseTable && !visited.Contains(t))
				{
					//descobrir o caminho para o destino (sempre a partir da base para garantir que é o mais curto)
					var path = meta.GetCaminho(info.Alias, t);
					if (path == null)
						throw new Exception("The table " + t + " is not related to table " + report.BaseTable);

					//adicionar todos os destinos do caminho por onde ainda não passámos
					foreach (var step in path)
					{
						if (!visited.Contains(step.AliasTargetTab))
						{
							visited.Add(step.AliasTargetTab);
							sq = sq.Join(schema, step.TargetTable, step.AliasTargetTab + suffix, TableJoinType.Left)
								.On(CriteriaSet.And()
									.Equal(step.AliasSourceTab + suffix, step.SourceRelField,
											step.AliasTargetTab + suffix, step.TargetRelField));
						}
					}
				}
			}

			return sq;
		}

		private CriteriaSet CompileCondition(ReportCondition condition)
		{
			FieldFormatting f = FieldFormatting.LOGICO;
			CriteriaSet crit = CriteriaSet.Or();

			object auxCond = null;
			try
			{
				// se só tem uma datasource não modifica os aliases
				auxCond = CompileCondition_recurse( "", condition, ref f);
			}
			catch (Exception e)
			{
				throw new Exception(string.Format("Error evaluating report condition: {0}", e.Message), e);
			}
			AuxAddCriteria(crit, auxCond);

			return crit;
		}

		private CriteriaSet AuxAddCriteria(CriteriaSet crit, object obj)
		{
			Criteria x1 = obj as Criteria;
			if (x1 != null)
			{
				crit.Criterias.Add(x1);
				return crit;
			}

			CriteriaSet x2 = obj as CriteriaSet;
			if (x2 != null)
				return crit.SubSet(x2);

			throw new Exception("Invalid report condition");
		}

		private void CompileShowNulls(ReportCondition cond, ref object expr)
		{
			if (cond.Operation == "FIELD" && cond.ShowNulls)
			{
				CriteriaSet res = CriteriaSet.Or();
				res.Criterias.Add(expr as Criteria);

				var campoBD = GetFieldMetadata(cond.ValueReference);
				var tabBD = GetTableMetadata(campoBD.Alias);

				res.Criterias.Add(
					new Criteria(
						new ColumnReference(tabBD.Alias, tabBD.NomeChavePrimaria),
						CriteriaOperator.Equal,
						null));

				expr = res;
			}
		}

		private object CompileCondition_recurse(string datasource, ReportCondition condition, ref FieldFormatting tipoInferido)
		{
			if (condition == null)
			{
				tipoInferido = FieldFormatting.LOGICO;
				return CriteriaSet.And();
			}

			CriteriaSet c;
			object expr;

			switch (condition.Operation)
			{
				// Operações Binárias
				// TODO
				// 1 - mapear os operadores, e reduzir os multiplos cases
				// 2 - validar o número de operadores de cada operando
				case "AND":
					c = CriteriaSet.And();
					for (int i = 0; i < condition.Operands?.Count; i++)
						c = AuxAddCriteria(c, CompileCondition_recurse(datasource, condition.Operands[i], ref tipoInferido));
					tipoInferido = FieldFormatting.LOGICO;
					return c;
				case "OR":
					c = CriteriaSet.Or();
					for (int i = 0; i < condition.Operands?.Count; i++)
						c = AuxAddCriteria(c, CompileCondition_recurse(datasource, condition.Operands[i], ref tipoInferido));
					tipoInferido = FieldFormatting.LOGICO;
					return c;
				case "EQ":
					expr = new Criteria(
						CompileCondition_recurse(datasource, condition.Operands[0], ref tipoInferido)
						, CriteriaOperator.Equal
						, CompileCondition_recurse(datasource, condition.Operands[1], ref tipoInferido));
					tipoInferido = FieldFormatting.LOGICO;
					CompileShowNulls(condition.Operands[0], ref expr);
					return expr;
				case "GT":
					expr = new Criteria(
						CompileCondition_recurse(datasource, condition.Operands[0], ref tipoInferido)
						, CriteriaOperator.Greater
						, CompileCondition_recurse(datasource, condition.Operands[1], ref tipoInferido));
					tipoInferido = FieldFormatting.LOGICO;
					CompileShowNulls(condition.Operands[0], ref expr);
					return expr;
				case "GET":
					expr = new Criteria(
						CompileCondition_recurse(datasource, condition.Operands[0], ref tipoInferido)
						, CriteriaOperator.GreaterOrEqual
						, CompileCondition_recurse(datasource, condition.Operands[1], ref tipoInferido));
					tipoInferido = FieldFormatting.LOGICO;
					CompileShowNulls(condition.Operands[0], ref expr);
					return expr;
				case "LT":
					expr = new Criteria(
						CompileCondition_recurse(datasource, condition.Operands[0], ref tipoInferido)
						, CriteriaOperator.Lesser
						, CompileCondition_recurse(datasource, condition.Operands[1], ref tipoInferido));
					tipoInferido = FieldFormatting.LOGICO;
					CompileShowNulls(condition.Operands[0], ref expr);
					return expr;
				case "LET":
					expr = new Criteria(
						CompileCondition_recurse(datasource, condition.Operands[0], ref tipoInferido)
						, CriteriaOperator.LesserOrEqual
						, CompileCondition_recurse(datasource, condition.Operands[1], ref tipoInferido));
					tipoInferido = FieldFormatting.LOGICO;
					CompileShowNulls(condition.Operands[0], ref expr);
					return expr;
				case "NEQ":
					expr = new Criteria(
						CompileCondition_recurse(datasource, condition.Operands[0], ref tipoInferido)
						, CriteriaOperator.NotEqual
						, CompileCondition_recurse(datasource, condition.Operands[1], ref tipoInferido));
					tipoInferido = FieldFormatting.LOGICO;
					CompileShowNulls(condition.Operands[0], ref expr);
					return expr;
				case "LIKE":
					// TODO:
					// definir a sintaxe de likes a nível da interface
					// a interface deve acrescentar *'s automáticamente? aqui já está a ser feito!
					// se se retirar este automatismo do servidor a interface passa a poder
					// definir com base no LIKE 3 operações distintas, que são:
					// StartsWith, EndsWith, Contains
					if (condition.Operands[1].Operation == "LITERAL")
					{
						condition.Operands[1].ValueReference =
							condition.Operands[1].ValueReference
								.Replace('*', '%')
								.Replace('?', '_');

						if (!condition.Operands[1].ValueReference.EndsWith("%"))
							condition.Operands[1].ValueReference += "%";
						if (!condition.Operands[1].ValueReference.StartsWith("%"))
							condition.Operands[1].ValueReference = "%" + condition.Operands[1].ValueReference;
					}

					expr = new Criteria(
						CompileCondition_recurse(datasource, condition.Operands[0], ref tipoInferido)
						, CriteriaOperator.Like
						, CompileCondition_recurse(datasource, condition.Operands[1], ref tipoInferido)); //este pode precisar de um compile especial
					tipoInferido = FieldFormatting.LOGICO;
					CompileShowNulls(condition.Operands[0], ref expr);
					return expr;
				case "BETWEEN":
					// não existe CriteriaOperator.Between
					// nem sei se se devia implementar, isto dá valores diferentes consoante o DBMS
					// constrói uma condição semelhante a (C >= O1 && C < O2)
					c = CriteriaSet.And();
					AuxAddCriteria(c, new Criteria(
						CompileCondition_recurse(datasource, condition.Operands[0], ref tipoInferido),
						CriteriaOperator.GreaterOrEqual,
						CompileCondition_recurse(datasource, condition.Operands[1], ref tipoInferido)));
					AuxAddCriteria(c, new Criteria(
						CompileCondition_recurse(datasource, condition.Operands[0], ref tipoInferido),
						CriteriaOperator.Lesser,
						CompileCondition_recurse(datasource, condition.Operands[2], ref tipoInferido)));
					tipoInferido = FieldFormatting.LOGICO;
					expr = c;
					CompileShowNulls(condition.Operands[0], ref expr);
					return expr;
				case "IN":
					// faz parse ao campo para obter o tipo
					expr = CompileCondition_recurse(datasource, condition.Operands[0], ref tipoInferido);

					// faz parse a cada um dos valores a partir do 2o
					List<object> values = new List<object>();
					for (int i = 1; i < condition.Operands.Count; i++)
						values.Add(CompileCondition_recurse(datasource, condition.Operands[i], ref tipoInferido));

					tipoInferido = FieldFormatting.LOGICO;
					expr = new Criteria(expr, CriteriaOperator.In, values);
					CompileShowNulls(condition.Operands[0], ref expr);
					return expr;
				// Operações Unárias:
				case "ISNOTNULL":
				case "ISNULL":
					// este caso ignora o segundo operando
					// TODO: verificar se isto se comporta bem com todos os tipos de campos (datas, inteiros, strings...)
					CriteriaOperator op = condition.Operation == "ISNULL" ? CriteriaOperator.Equal : CriteriaOperator.NotEqual;
					expr = new Criteria(
						CompileCondition_recurse(datasource, condition.Operands[0], ref tipoInferido),
						op,
						null);
					tipoInferido = FieldFormatting.LOGICO;
					return expr;
				// Valores:
				case "FIELD":
					//ir buscar o tipo deste campo aos metadados
					var campoBD = GetFieldMetadata(condition.ValueReference);// meta.GetFieldInfo(parse[0], parse[1], ); info.CamposBD[parse[1]];
					tipoInferido = campoBD.ForCampo;

					return new ColumnReference(campoBD.Alias + datasource, campoBD.Nome);
				case "LITERAL":
					//para reaproveitar melhor o codigo existente convertemos primeiro para tipo interno
					//e depois formatamos o literal para ser usado na query.
					try
					{
						return ConversaoCav.ToInterno(condition.ValueReference, tipoInferido);
					}
					catch (Exception e)
					{
						// se der erro na conversão de um valor das condições enviamos uma mensagem mais explicita ao utilizador
						throw new Exception(string.Format("Error converting value \"{0}\" to {1} ({2})", condition.ValueReference, tipoInferido.ToString(), e.Message), e);
					}
				default:
					throw new Exception("Unknown operator type " + condition.Operation + " used in condition");
			}
		}

		/// <summary>
		/// Compila o acesso a um campo que vai servir de pivot.
		/// Este campo pode dar origem a mais que uma coluna e pode ter de executar querys para descobrir que colunas são necessárias.
		/// </summary>
		/// <param name="meta">O provider de metadados</param>
		/// <param name="field">O campo a compilar</param>
		/// <returns>A lista de expressões de select associadas a este pivot</returns>
		private List<SelectField> CompilePivotField(ReportField field, out ResultType type)
		{
			List<SelectField> res = new List<SelectField>();

			var fieldMeta = GetFieldMetadata(field.FieldId);
			ISqlExpression fielddb = new ColumnReference(fieldMeta.Alias, fieldMeta.Nome);
			var alias = fieldMeta.Alias + "." + fieldMeta.Nome;
			// tipo de dados base do campo a ser desdobrado
			type = fieldMeta.GetResultType();

			var pivotMeta = GetFieldMetadata(field.Pivot);
			ISqlExpression pivotdb = new ColumnReference(pivotMeta.Alias, pivotMeta.Nome);
			// tipo de dados da coluna pivot
			ResultType pivotType = pivotMeta.GetResultType();
			if (!string.IsNullOrEmpty(field.PivotSelectorFunction))
				pivotdb = CompileSqlFunction(field.PivotSelectorFunction, pivotdb, pivotType, out pivotType);

			//SUM(case when year(data) = 2010 then TOTALSIV end) 'sum_2010'
			if (!string.IsNullOrEmpty(field.SelectorFunction))
			{
				// aqui utiliza-se o tipo de dados da coluna pivot, embora não seja utilizado para nada, pois
				// está dentro de uma expressão de iif e não vai ser lido da BD, logo não tem de ser convertido
				pivotdb = CompileSqlFunction(field.SelectorFunction, pivotdb, pivotType, out pivotType);
				alias += "_" + field.SelectorFunction;
			}

			foreach (var value in field.PivotValues)
			{
				ISqlExpression select = SqlFunctions.Iif (CriteriaSet.And().Equal(pivotdb, value), fielddb, null);
				// aqui utiliza-se o tipo de dados base do campo, porque o resultado desta expressão será desse mesmo tipo
				select = CompileSqlFunction(field.TotalType, select, type, out type);
				res.Add(new SelectField(select, alias + "_" + field.TotalType + "_" + value));
			}

			return res;
		}

		/// <summary>
		/// Compila a string de acesso e transformação de um campo na query
		/// </summary>
		/// <param name="meta">O provider de metadados</param>
		/// <param name="field">O campo a ser incluido na query</param>
		/// <param name="type">Tipo de dados inferido para o campo</param>
		/// <returns>A string de acesso ao campo</returns>
		private SelectField CompileSimpleField(ReportField field, out ResultType type)
		{
			var fieldMeta = GetFieldMetadata(field.FieldId);
			var alias = fieldMeta.Alias + "." + fieldMeta.Nome;
			ISqlExpression fielddb = new ColumnReference(fieldMeta.Alias, fieldMeta.Nome);
			// tipo base do campo
			type = fieldMeta.GetResultType();

			if (!string.IsNullOrEmpty(field.SelectorFunction))
			{
				alias += "_" + field.SelectorFunction;
				// caso o tipo seja alterado, é actualizada a variavel type
				// é necessário passar o tipo anterior, para se poder inferir o novo tipo
				fielddb = CompileSqlFunction(field.SelectorFunction, fielddb, type, out type);
			}

			if (!string.IsNullOrEmpty(field.TotalType))
			{
				// caso o tipo seja alterado, é actualizada a variavel type
				// é necessário passar o tipo anterior, para se poder inferir o novo tipo
				fielddb = CompileSqlFunction(field.TotalType, fielddb, type, out type);
				alias += "_" + field.TotalType;
			}

			return new SelectField(fielddb, alias);
		}

		/// <summary>
		/// Compila a estrutura de acesso e transformação de um campo na query
		/// </summary>
		/// <param name="meta">O provider de metadados</param>
		/// <param name="field">O campo a ser incluido na query</param>
		/// <param name="datasources">A lista de datasources joined na query</param>
		/// <returns>A lista de select fields causada pelo pedido deste campo</returns>
		/// private List<SelectField> CompileField(ReportField field, List<CSGenioAdtsrc> datasources, out ResultType type)
		private List<SelectField> CompileField(ReportField field, out ResultType type)
		{
			if (!string.IsNullOrEmpty(field.Pivot))
				return CompilePivotField(field, out type);

			List<SelectField> res = new List<SelectField>
			{
				CompileSimpleField(field, out type)
			};
			return res;
		}

		private ResultType GetResultType(string function, ResultType baseType)
		{
			if (string.IsNullOrEmpty(function))
				return baseType;

			FieldFormatting format;
			switch (function.ToLower())
			{
				case "sum":
				case "avg":
				case "max":
				case "min":
					format = baseType.ForCampo;
					break;
				case "count":
					format = FieldFormatting.INTEIRO;
					break;
				// suponho que o tipo de retorno dos 3 seguintes seja inteiro, testar!
				case "year":
				case "month":
				case "day":
					format = FieldFormatting.INTEIRO;
					break;
				default:
					throw new Exception("Unknown funcion " + function);
			}

			string[] array;
			switch (function.ToLower())
			{
				case "sum":
				case "avg":
				case "count":
				case "year":
				case "month":
				case "day":
					array = new string[] { string.Empty, string.Empty };
					break;
				// se se aplicar a função de máximo ou mínimo o resultado continua a ser um array, nos restantes casos não se verifica
				case "min":
				case "max":
					array = new string[] { baseType.Array, baseType.ArrayClassName };
					break;
				default:
					throw new Exception("Unknown funcion " + function);
			}

			return new ResultType(format, array[0], array[1]);
		}

		/// <summary>
		/// Cria uma coluna de sort com a ordenação especificada
		/// </summary>
		/// <remarks>vale a pena mapear as duas opções ASC/DESC ou fica assim?</remarks>
		/// <param name="field">A expressao a incluir no sort</param>
		/// <param name="direction">A direcção de ordenação</param>
		/// <returns>A expressão de ordenação</returns>
		private ColumnSort CreateSort(ISqlExpression field, string direction)
		{
			return new ColumnSort(field, direction == "ASC" ? SortOrder.Ascending : SortOrder.Descending);
		}

		/// <summary>
		/// Compila uma função de agregação ou de transformação de sql para uma expressão de sql
		/// </summary>
		/// <param name="function">A função de selecção ou agregação</param>
		/// <param name="arg">O argumento da função</param>
		/// <returns>A expressão de sql</returns>
		private ISqlExpression CompileSqlFunction(string function, ISqlExpression arg, ResultType baseType, out ResultType type)
		{
			type = GetResultType(function, baseType);

			switch (function.ToLower())
			{
				case "sum":
					return SqlFunctions.Sum(arg);
				case "avg":
					return SqlFunctions.Average(arg);
				case "max":
					return SqlFunctions.Max(arg);
				case "min":
					return SqlFunctions.Min(arg);
				case "count":
					return SqlFunctions.Count(arg);
				case "year":
					return SqlFunctions.Year(arg);
				case "month":
					return SqlFunctions.Month(arg);
				case "day":
					return SqlFunctions.Day(arg);
				default:
					throw new Exception("Unknown funcion " + function);
			}
		}

		// precisamos de um join especial que não omita o separador quando existe apenas um valor
		// porque o identificador do grupo tem de ser diferente do identificador da raiz
		// (quando um dos resultados do primeiro grupo era vazio, ficavam os dois com o identificador vazio
		// ("" - e estragava a organização dos dados na resposta)
		private static string JoinSpecial(string separator, ICollection<string> values)
		{
			StringBuilder sb = new StringBuilder();

			foreach (string s in values)
			{
				sb.Append(s);
				sb.Append(separator);
			}

			return sb.ToString();
		}

		private static string GetGroupId(string val)
		{
			return GetGroupId(new List<string>() { val });
		}

		private static string GetGroupId(List<string> vals)
		{
			string key = JoinSpecial(":", vals);
			key = key.ToUpperInvariant();
			// removem-se todos os caracteres acentuados para não dar problemas com as chaves dos dicionários
			// uma vez que o SQLServer identifica como iguais as strings "ábcdéf" e "abcdef" e pode agrupar os resultados
			// de ambas no mesmo grupo (dependendo da collation definida)
			// TODO: passar isto para um parametro pré-definido por sistema (ou por campo?)
			key = RemoveDiacritics(key);

			return key;
		}

		private static string GetMainGroupId()
		{
			return GetGroupId(new List<string>());
		}

		/// <summary>
		/// Executa uma query e junta os resultados ao agrupamento correcto da resposta. Devolve um agrupamento que pode
		/// ser usado como input de processamentos de sub-grupos.
		/// </summary>
		/// <param name="res">A resposta onde se vão juntar os resultados da query</param>
		/// <param name="conn">A conexão</param>
		/// <param name="sql">A query a executar</param>
		/// <param name="dbId">O id do grupo de base de dados ou null caso não exista</param>
		/// <param name="lastGroup">O agrupamento pai caso exista</param>
		/// <param name="numGroupNames">O numero de campos que fazem a chave do agrupamento</param>
		/// <returns>O agrupamento que pode ser usado para como input de processamentos de sub-grupos.</returns>
		private int ProcessGroupQuery(User usr, ReportReply res, PersistentSupport sp, SelectQuery sql, string dbId, Dictionary<string, ReportReplyGroup> groupIndex, int numGroupNamesFather, int numGroupNamesCurrent, bool isDetails, List<ResultType> fieldTypes)
		{
			//executar a query de agrupamento
			List<ReportReplyGroup> newGroup = new List<ReportReplyGroup>();
			//var matrix = sp.executaQuery(sql);
			var matrix = sp.Execute(sql);

			for (int r = 0; r < matrix.NumRows; r++)
			{
				// obtem a chave do pai
				List<string> fatherKeyValues = new List<string>();

				if (dbId != null)
					fatherKeyValues.Add(dbId);

				for (int i = 0; i < numGroupNamesFather; i++)
					fatherKeyValues.Add(matrix.GetDirect(r, i).ToString());

				string fatherKey = GetGroupId(fatherKeyValues);

				// para cada resultado cria um novo grupo
				ReportReplyGroup row = new ReportReplyGroup();

				// se não for ao nível dos detalhes instancia-se o grupo porque vai ter valores
				if (!isDetails)
					row.Groups = new List<ReportReplyGroup>();

				ReportReplyGroup father = null;
				// adiciona-se aos grupos pai
				// isto fica protegido porque devido às collations corre-se o risco de a chave não ser encontrada no dicionário)
				if (groupIndex.TryGetValue(fatherKey, out father))
				{
					groupIndex[fatherKey].Groups.Add(row);
				}
				else
				{
					// se já falhou ao encontrar o pai deste grupo, não vale a pena seguir em frente, salta para o próximo registo
					// (aqui estava a fazer return e não pode, há registos que possivelmente pertencem a outros grupos)
					// (idependentemente disso, este sub-ramo da àrvore nunca vai ser exibido no report)
					//TODO: avisar o utilizador ?
					Log.Error(string.Format("The father key \"{0}\" was not found on the report results, this might be a collation problem", fatherKey));
					continue;
				}

				// se for uma query de detalhes o numGroupNames deve ser -1
				// (para evitar repetir código e instruções nos passos seguintes)
				if (isDetails)
					numGroupNamesCurrent = -1;

				// obtem-se a chave actual
				List<string> currentKeyValues = new List<string>(fatherKeyValues);

				// se estivermos num grupo de detalhes este ciclo não vai fazer nada
				for (int i = numGroupNamesFather; i < numGroupNamesCurrent; i++)
					currentKeyValues.Add(matrix.GetDirect(r, i).ToString());

				// se não for um grupo de detalhes adiciona-se a si próprio
				if (!isDetails)
				{
					string currentKey = GetGroupId(currentKeyValues);
					groupIndex[currentKey] = row;
				}

				List<string> allRowValues = new List<string>();

				// obtem-se a lista de valores desta row
				// volta a percorrer todos os valores para aplicar a conversão
				for (int i = 0; i < matrix.NumCols; i++)
					allRowValues.Add(ConversaoCav.FromInterno(matrix.GetDirect(r, i), meta, fieldTypes[i], usr.Language));

				row.Values = allRowValues;
			}

			// passa a devolver o número de registos que obteve
			return matrix.NumRows;
		}

		public ReportReply ExecuteQuery(User usr, ReportDefinition query)
		{
			ReportReply res = new ReportReply();

			//----------------------------------------------------------
			// Calcular os inner joins que vão ser partilhados por todas as queries
			//----------------------------------------------------------
			//calcular as tabelas a aceder a partir dos campos necessários
			List<string> tableNames = GetTableNames(query/*, us*/);

			//calcular os joins que é necessário fazer a partir das tabelas a aceder
			string tableBD = GetTableMetadata(query.BaseTable).NomeTabela;
			SelectQuery from = CompileJoins(query, /*dsJoins,*/ tableNames);
			from.noLock = true;

			//----------------------------------------------------------
			// Calcular o filtro que vai ser partilhado por todas as queries
			//----------------------------------------------------------
			from.WhereCondition = CompileCondition(query.Condition/*, dsJoins*/);

			//adicionar os EPHs
			from.WhereCondition =
				CriteriaSet.And()
				.SubSet(from.WhereCondition)
				.SubSet(GetEph(query.BaseTable,usr));

			//----------------------------------------------------------
			// Verificar se os detalhes estão sujeitos a pivot
			//----------------------------------------------------------
			bool pivot = query.DetailsGroup.Fields.Exists(x => !string.IsNullOrEmpty(x.Pivot));

			//----------------------------------------------------------
			// Agrupamentos
			//----------------------------------------------------------
			// hashtable que mapeia cada grupo indexado pela sua chave
			// a chave de um grupo é a concatenação dos valores dos campos agrupados
			Dictionary<string, ReportReplyGroup> groupIndex = new Dictionary<string, ReportReplyGroup>(StringComparer.InvariantCultureIgnoreCase);

			groupIndex[GetMainGroupId()] = res.MainGroup;

			PersistentSupport sp = null;
			try
			{
				string dbId = null;

				sp = PersistentSupport.getPersistentSupport(usr.Year);
				sp.openConnection();

				//vamos pré preencher os valores de pivot dos campos que tenham omitido essa lista.
				//no caso do pivot não especificar explicitamente os valores a perquisar temos de fazer uma query
				//para descobrir os valores distintos
				//TODO: avaliar se o interface devia indicar sempre emplicitamente quais os pivots que quer (nesse
				//  caso teria de se criar mais um serviço para devolver os valores distinct separadamente ao interface)
				foreach (ReportField field in query.DetailsGroup.Fields)
				{
					if (!string.IsNullOrEmpty(field.Pivot) && (field.PivotValues == null || field.PivotValues.Count == 0))
					{
						var pivotMeta = GetFieldMetadata(field.Pivot);
						ISqlExpression pivotdb = new ColumnReference(pivotMeta.Alias, pivotMeta.Nome);
						ResultType type = pivotMeta.GetResultType();
						if (!string.IsNullOrEmpty(field.PivotSelectorFunction))
							pivotdb = CompileSqlFunction(field.PivotSelectorFunction, pivotdb, type, out type);

						field.PivotValues = new List<string>();
						SelectQuery sq = from.Clone() as SelectQuery;
						sq.noLock = true;

						sq = sq.Distinct(true).Select(pivotdb, "value")
							.OrderBy(pivotdb, SortOrder.Ascending);
						var matrix = sp.Execute(sq);

						// TODO:
						// este campo é desmultiplicado em colunas
						// é necessário guardar o tipo de dados da coluna para mais tarde saber o que esperar do campo
						// é o mesmo tipo de dados que está na variável type? é o tipo base do campo? como fazer?
						// no ciclo seguinte, deve-se utilizar a ConversaoCav para converter os valores? pode-se fazer? deve-se?

						for (int i = 0; i < matrix.NumRows; i++)
							field.PivotValues.Add(matrix.GetDirect(i, 0).ToString());
					}
				}

				//Por cada grupo criar uma query
				List<SelectField> groupBy = new List<SelectField>();

				// lista de campos no select para depois se ter a informação necessária para os converter
				List<ResultType> selectFieldTypes = new List<ResultType>();

				if (query.Groups == null)
					query.Groups = new List<ReportGroup>();

				foreach (var group in query.Groups)
				{
					SelectQuery sql = from.Clone() as SelectQuery;
					sql.noLock = true;

					//adicionar as chaves para os grupos acima
					foreach (var g in groupBy)
						sql.SelectFields.Add(g);
					int numGroupNamesFather = groupBy.Count;

					// constrói uma nova lista para este nível que engloba todos os campos agrupados dos níveis anteriores
					// mais os campos específicos deste nível
					List<ResultType> currentSelectFieldTypes = new List<ResultType>(selectFieldTypes);

					//construir a query
					//adicionar os campos que fazem o agrupamento
					if (group.Fields != null)
					{
						foreach (var fld in group.Fields)
						{
							if (string.IsNullOrEmpty(fld.TotalType))
							{
								//var fielddb = CompileField(fld, dsJoins, out type);
								var fielddb = CompileField(fld, out ResultType type);
								//para o group by entram todas as chaves dos grupos acima mais as chaves deste
								//TODO: Se aparecer neste grupo uma chave que já foi colocada em cima devo ignorar ou dar erro?
								groupBy.AddRange(fielddb);

								// adiciona-se o tipo do campo tantas vezes como o número de colunas em que o mesmo se desdobra
								// para que se saiba formatar o seu resultado ao construir o output do relatório
								for (int i = 0; i < fielddb.Count; i++)
									selectFieldTypes.Add(type);
							}
						}
					}

					int numGroupNamesCurrent = groupBy.Count;

					// select
					if (group.Fields != null)
					{
						foreach (var fld in group.Fields)
						{
							foreach (var fielddb in CompileField(fld, out ResultType type))
							{
								sql.SelectFields.Add(fielddb);
								// adiciona-se o tipo do campo tantas vezes como o número de colunas em que o mesmo se desdobra
								// para que se saiba formatar o seu resultado ao construir o output do relatório
								currentSelectFieldTypes.Add(type);
							}
						}
					}

					// grupos
					foreach (var g in groupBy)
						sql.GroupByFields.Add(g.Expression);

					// as ordenações passam a respeitar a seguinte condição:
					// as colunas de agrupamento são sempre ordenadas primeiro
					// quando se faz drill-down nos dados não tem sentido ordenar por colunas do nível dos detalhes
					// ou seja, as colunas de um grupo A devem aparecer primeiro na ordenação do que as colunas do sub-grupo de A
					// se não se respeitar esta ordem o utilizador ao ver um relatório com grupos a ordenação vai ter dificuldades
					// na interpretação dos resultados uma vez que a organização dos dados nos níveis superiores não vai ter qualquer sentido
					// a ordenação dos groupby's também é importante na lógica de preenchimento do report com os dados de output
					// uma vez que assim permite optimizar o tempo de processamento para construir o report

					// TODO:
					// ordenar respeitando o nível do grupo e dentro do nível a ordem que o utilizador definiu
					// (pode-se obrigar na interface a fazer isto)
					foreach (var gf in groupBy)
					{
						// para cada campo do grouBy primeiro procura nos orderBy's definidos na interface
						bool found = false;
						if (query.Orderings != null)
						{
							foreach (var o in query.Orderings)
							{
								// o tipo nas ordenações é descartado, uma vez que não é necessário
								// converter nenhum valor no resultado do relatório para os order by's
								var fList = CompileField(o.Field, out ResultType type);
								if (fList.Count > 1)
									throw new NotImplementedException("Groups over pivot fields are not supported");
								if (fList[0].Alias == gf.Alias)
								{
									// se encontrou ordenação utiliza a que foi definida
									sql.OrderByFields.Add(CreateSort(fList[0].Expression, o.Direction));
									found = true;
									break;
								}
							}
						}

						// se não encontrou ordenação definida, usa a ordenação por omissão (ASC)
						if (!found)
							sql.OrderByFields.Add(CreateSort(gf.Expression, "ASC"));
					}

					//processar a query e colocar os resultados da query no grupo certo
					ProcessGroupQuery(usr, res, sp, sql, dbId, groupIndex, numGroupNamesFather, numGroupNamesCurrent, false, currentSelectFieldTypes);
				}

				//-----------------------------------------------------------------
				// executar a query de detalhes
				//-----------------------------------------------------------------
				{
					SelectQuery sql = from.Clone() as SelectQuery;
					sql.noLock = true;

					List<SelectField> fieldNames = new List<SelectField>();
					List<SelectField> detailNames = new List<SelectField>();

					//criar a query
					//adicionar os campos que pertencem aos agrupamentos
					int numGroupNames = groupBy.Count;
					foreach (var g in groupBy)
						sql.SelectFields.Add(g);

					//colocar as colunas de select
					foreach (var field in query.DetailsGroup.Fields)
					{
						// só são adicionados às queries de detalhes os campos sem totalizadores
						if (string.IsNullOrEmpty(field.TotalType))
						{
							// nos details nao repete campos que tenham sido adicionados a query e que pertencam aos groupby's (pois ja foram adicionados anteriormente)
							// aqui é importante verificar pelo alias, uma vez que aos campos podem ser aplicadas transformações
							// e cabe à responsabilidade da função CompileField ter isso em conta ao atribuir o Alias
							// (TODO: ter em conta as transformações na CompileField, caso seja necessário - por enquanto ainda não está a ser feito)
							foreach (var f in CompileField(field, out ResultType type))
							{
								if (!sql.SelectFields.Any(x => x.Alias == f.Alias))
								{
									sql.SelectFields.Add(f);

									// adiciona-se o id do campo tantas vezes como o número de colunas em que o mesmo se desdobra
									// para que se saiba formatar o seu resultado ao construir o output do relatório
									// (ATCHUNG: adicionar o fld.FieldId é deliberado! não é engano)
									selectFieldTypes.Add(type);

									if (string.IsNullOrEmpty(field.Pivot))
										detailNames.Add(f);
								}
							}
						}
					}
					//Nota: Nos detalhes os campos totalizadores são mostrados sempre
					// A query que devolve os totais globais é feita à parte

					//Nos detalhes só há agrupamentos no caso dos pivots
					if (pivot)
					{
						foreach (var g in detailNames)
						{
							sql.GroupByFields.Add(g);
							// adiciona também ao groupBy porque no calculo das ordenações é necessário saber que campos pertencem aos agrupamentos
							groupBy.Add(g);
						}
					}

					// adicionar a ordenação
					// as ordenações passam a respeitar a seguinte condição:
					// as colunas de agrupamento são sempre ordenadas primeiro
					// as colunas de ordenação definidas pelo utilizador na interface e que não correspondem a colunas de grupos
					// são acrescentadas posteriormente (só neste caso dos detalhes é que estas colunas são acrescentadas)

					// TODO:
					// ordenar respeitando o nível do grupo e dentro do nível a ordem que o utilizador definiu
					// (pode-se obrigar na interface a fazer isto)
					List<string> orders = new List<string>();

					foreach (var gf in groupBy)
					{
						// para cada campo do grouBy primeiro procura nos orderBy's definidos na interface
						bool found = false;
						if (query.Orderings != null)
						{
							foreach (var o in query.Orderings)
							{
								var fList = CompileField(o.Field, out ResultType type);
								if (fList.Count > 1) //assume-se que ninguem vai ordenar por campo de groupby que sejam pivot ao mesmo tempo
									throw new NotImplementedException("orderings over grouped pivot fields are not supported");

								if (fList[0].Alias == gf.Alias)
								{
									// se encontrou ordenação utiliza a que foi definida
									sql.OrderByFields.Add(CreateSort(fList[0].Expression, o.Direction));
									orders.Add(gf.Alias);
									found = true;
									break;
								}
							}
						}

						// se não encontrou ordenação definida, usa a ordenação por omissão (ASC)
						if (!found)
							sql.OrderByFields.Add(CreateSort(gf.Expression, "ASC"));
					}

					// os campos de ordenação definidos na interface e que ainda não foram ordenados devem ser acrescentados ao orderby
					if (query.Orderings != null)
					{
						foreach (var o in query.Orderings)
						{
							var ofList = CompileField(o.Field, out ResultType type);
							foreach (var of in ofList)
							{
								if (!orders.Contains(of.Alias))
								{
									sql.OrderByFields.Add(CreateSort(of.Expression, o.Direction));
									orders.Add(of.Alias);
								}
							}
						}
					}

					//processar a query e colocar os resultados da query no grupo certo
					// nos detalhes afecta o número total de resultados
					res.ResultCount += ProcessGroupQuery(usr, res, sp, sql, dbId, groupIndex, numGroupNames, -1, true, selectFieldTypes);

					//A mensagem de resultado de uma query bem sucedida é a query de detalhes que foi executada
					var renderer = new QueryRenderer(sp);
					var sqlPlainText = renderer.GetSql(sql);
					res.QuerySQL = sqlPlainText;
				}

				//-----------------------------------------------------------------
				// executar a query de totalizadores globais (caso existam)
				//-----------------------------------------------------------------
				if (!pivot)
				{
					List<SelectField> fieldNames = new List<SelectField>();
					List<ResultType> fieldTypeDetails = new List<ResultType>();

					foreach (var fld in query.DetailsGroup.Fields)
					{
						if (!string.IsNullOrEmpty(fld.TotalType))
						{
							foreach (var f in CompileField(fld, out ResultType type))
							{
								fieldNames.Add(f);
								fieldTypeDetails.Add(type);
							}
						}
					}

					//criar a query
					if (fieldNames.Count > 0)
					{
						SelectQuery sql = from.Clone() as SelectQuery;
						sql.noLock = true;

						foreach (var f in fieldNames)
							sql.SelectFields.Add(f);

						var matrix = sp.Execute(sql);
						for (int row = 0; row < matrix.NumRows; row++)
						{
							for (int col = 0; col < matrix.NumCols; col++)
							{
								string goupId = dbId == null ? GetMainGroupId() : GetGroupId(dbId);
								ReportReplyGroup target = groupIndex[goupId];
								target.Values.Add(ConversaoCav.FromInterno(matrix.GetDirect(row, col), meta, fieldTypeDetails[col], usr.Language));
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				throw new FrameworkException("Error processing Advanced Query", "CavEngine.ExecuteQuery", e.Message, e);
			}
			finally
			{
				sp?.closeConnection();
			}

			return res;
		}

		public CriteriaSet GetEph(string baseTable, User user)
		{
			var areaBase = CSGenio.business.Area.createArea(baseTable.ToLower(), user,user.CurrentModule);
			CriteriaSet conditions = CSGenio.business.Listing.CalculateConditionsEphGeneric(areaBase, string.Empty);
			return conditions;
		}
	}

	public class MetaCampo
	{
		public string Nome { get; set; }

		public string Alias { get; set; }

		public string NomeCompleto { get; set; }

		public FieldFormatting ForCampo { get; set; }

		public string Array { get; set; }

		public string ArrayClassName { get; set; }

		public bool IsArray()
		{
			return !string.IsNullOrEmpty(Array);
		}

		public ResultType GetResultType()
		{
			return new ResultType(ForCampo, Array, ArrayClassName);
		}
	}

	public class MetaTable
	{
		public string Alias { get; set; }

		public string NomeTabela { get; set; }

		public string NomeChavePrimaria { get; set; }
	}

	// A CavEngine e a ConversaoCav estão a depender desta classe para formatar os campos
	public class ResultType
	{
		public FieldFormatting ForCampo { get; set; }

		public string Array { get; set; }

		public string ArrayClassName { get; set; }

		public ResultType(FieldFormatting format, string array, string arrayClassName)
		{
			ForCampo = format;
			Array = array;
			ArrayClassName = arrayClassName;
		}

		public bool IsArray()
		{
			return !string.IsNullOrEmpty(Array);
		}
	}
}
