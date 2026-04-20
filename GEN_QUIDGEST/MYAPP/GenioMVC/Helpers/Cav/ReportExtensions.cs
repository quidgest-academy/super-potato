using OfficeOpenXml;
using OfficeOpenXml.Style;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using GenioMVC.ViewModels.Cav;

namespace GenioMVC.Helpers.Cav
{
	/// <summary>
	/// Classe global para agregar definições de reports
	/// </summary>
	public static class ReportExtensions
	{
		/// <summary>
		/// Matriz de tipos de totais, para se pode precorrer por ordem por prioridades dos operadores
		/// </summary>
		/// <remarks>
		/// TODO: pode-se ler isto de algumas definições para permitir definir prioridades dos operadores
		/// </remarks>
		public static string[][] totaTypeLabels = new string[][]
		{
			new string[] { "SUM", "Soma" },
			new string[] { "AVG", "Média" },
			new string[] { "COUNT", "Total de Elementos" },
			new string[] { "MAX", "Máximo" },
			new string[] { "MIN", "Mínimo" }
		};
	}

	public class ReportExcel
	{
		ResultModel Model;

		public ReportExcel(ResultModel model)
		{
			this.Model = model;
		}

		public ExcelPackage SimpleExample()
		{
			// Library http://epplus.codeplex.com/
			ExcelPackage pck = new ExcelPackage();

			// Example
			DataTable table = new DataTable();
			table.Columns.Add("Dosage", typeof(int));
			table.Columns.Add("Drug", typeof(string));
			table.Columns.Add("Patient", typeof(string));
			table.Columns.Add("Date", typeof(DateTime));
			table.Rows.Add(25, "Indocin", "David", DateTime.Now);
			table.Rows.Add(50, "Enebrel", "Sam", DateTime.Now);
			table.Rows.Add(10, "Hydralazine", "Christoff", DateTime.Now);
			table.Rows.Add(21, "Combivent", "Janet", DateTime.Now);
			table.Rows.Add(100, "Dilantin", "Melanie", DateTime.Now);

			//Create the worksheet
			ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Consulta");

			// Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
			ws.Cells["A1"].LoadFromDataTable(table, true);

			// Format the header for column 1-3
			using (ExcelRange rng = ws.Cells["A1:D1"])
			{
				rng.Style.Font.Bold = true;
				rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
				rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
				rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
			}

			// Example how to Format Column 1 as numeric
			using (ExcelRange col = ws.Cells[2, 1, 2 + table.Rows.Count, 1])
			{
				col.Style.Numberformat.Format = "#,##0.00";
				col.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
			}

			return pck;
		}
		public ExcelPackage GenerateExcel()
		{
			// Library http://epplus.codeplex.com/
			ExcelPackage pck = new ExcelPackage();

			//Create the worksheet
			ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Resources.Resources.CONSULTA23186);

			Create_Results(Model.Result.MainGroup, Model.Query, ws);

			return pck;
		}

		/// <summary>
		/// Get the byte array from exel document
		/// </summary>
		/// <returns></returns>
		public byte[] GenerateExcelBytes()
		{
			return GenerateExcel().GetAsByteArray();
		}

		public void Create_Results(ReportReplyGroup group, ReportDefinition query, ExcelWorksheet ws)
		{
			// obtêm-se os campos do report
			List<ReportField> allFields = query.GetReportFields();

			// TODO: este caso pode passar para dentro da função? como se faz isto?
			// pode-se acrescentar um parametro extra que vem de cima e que diz se tem pagebreak ou não
			// cujo valor por omissão quando se invoca a função aqui é true, o que obriga a escrever sempre os cabeçalhos
			// testar para verificar se isto funciona bem (aproveitar a oportunidade quando se implementar os pagebreaks)

			// define-se o cabeçalho

			using (ExcelRange rng = ws.Cells[1, 2, 1, allFields.Count + 1])
			{
				rng.Style.Font.Bold = true;
				rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
				rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
				rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
			}

			// acrescenta os títulos das colunas
			int counter = 2;
			foreach (ReportField f in allFields)
				ws.Cells[1, counter++].Value = f.GetTitle();

			int startLine = 2;

			ResultsTableXls(ws, group, query, 0, true, allFields, ref startLine);
		}

		private void ResultsTableXls(ExcelWorksheet ws, ReportReplyGroup group, ReportDefinition query, int nivel, bool first, List<ReportField> allFields, ref int row)
		{
			// se tem filhos, então estamos num grupo
			if (!group.IsLeaf())
			{
				// tem pageBreak ?
				bool pageBreak = false;
				if (nivel <= query.Groups.Count && nivel > 0)
					pageBreak = query.Groups[nivel - 1].PageBreak;

				// o limite é o número de campos agrupados, os restantes valores são valores totalizadores
				// aqui temos de descobrir quantos campos são agregados e subtrair esse valor ao número total de campos
				// caso contrário a opção seria saber quantos campos de groupBy existem a este nível
				// o que requer precorrer todos os grupos até nivel -1 e somar os campos de groupBy que não são totalizadores
				int limite = (nivel > 0 && nivel <= query.Groups.Count) ? group.Values.Count - query.Groups[nivel - 1].Fields.Count(f => !string.IsNullOrEmpty(f.TotalType)) : 0;

				// chamada recursiva para escrever os "filhos" do grupo actual
				for (int i = 0; i < group.Groups.Count; i++)
					ResultsTableXls(ws, group.Groups[i], query, nivel + 1, i == 0, allFields, ref row);

				// os totalizadores passam a ser exibidos no final de cada grupo da seguinte forma
				//           campo1   campo2   ...   campoN   ...   campoX   campoY   campoZ       (linha do cabeçalho da tabela)
				// ....                                                                            (linhas de detalhes e sub-grupos)
				// Grupo N   valor1   valor2   ...   valorN                                        (linha dos valores do grupo)
				// SUM                                                  11       13       17       (linhas dos valores dos totalizadores para cada campo)
				// MAX                                                   3        5        7
				// MIN                                                   -        -        2       (podem não ter sido pedidos totalizadores para todas as colunas, nestes casos deixa um espaço em branco)

				bool escreveNivel = false;

				// se existem totalizadores
				if (group.Values.Count > limite)
				{
					// primeiro descobrimos qual a lista de totalizadores no nível actual onde temos de procurar
					List<ReportField> listaTotalizadores = null;
					escreveNivel = false;

					if (nivel > 0 && nivel <= query.Groups.Count)
						listaTotalizadores = query.Groups[nivel - 1].Fields.FindAll(x => !string.IsNullOrEmpty(x.TotalType));
					else
						listaTotalizadores = query.DetailsGroup.Fields.FindAll(x => !string.IsNullOrEmpty(x.TotalType));

					foreach (string[] totalType in ReportExtensions.totaTypeLabels)
					{
						// totalType[0] - id da função
						// totalType[1] - label com a descrição da função

						// lista de totalizadores para o tipo de função actual
						List<ReportField> totalizadores = listaTotalizadores.FindAll(x => x.TotalType.Equals(totalType[0]));

						// se existem totalizadores deste tipo
						if (totalizadores.Count > 0)
						{
							// se ainda não escreveu o cabeçalho para este conjunto de totalizadores, então escreve-o antes dos valores
							if (!escreveNivel)
							{

								escreveNivel = true;

								// acrescenta uma linha em branco para ajudar na visualização
								row++;

								// escreve o nível do grupo e os valores pelos quais está agrupado nas respectivas colunas
								if (nivel == 0)
									ws.Cells[row, 1].Value = "Totais";
								else
									ws.Cells[row, 1].Value = string.Format("Grupo {0}", nivel);

								for (int i = 0; i < allFields.Count; i++)
									if (i < nivel) // penso que isto esteja mal (verificar!!! - presumo que seja limite em vez de nivel)
										ws.Cells[row, 2 + i].Value = string.IsNullOrEmpty(group.Values[i]) ? "-" : group.Values[i];
								row++;
							}

							ws.Cells[row, 1].Value = totalType[1];

							// coloca os valores nas respectivas colunas
							foreach (ReportField f in totalizadores)
							{
								// isto tem de devolver sempre um valor válido, se não algo correu mal noutro sitio!
								int pos = allFields.FindIndex(x => x.FieldId == f.FieldId);
								int posTotalGroup = listaTotalizadores.FindIndex(x => x.FieldId == f.FieldId && x.TotalType == f.TotalType);
								ws.Cells[row, 2 + pos].Value = group.Values[posTotalGroup + limite];
							}

							row++;
						}
					}
				}

				// se tem totalizadores dá mais 1 enter, para se perceber melhor
				if (escreveNivel)
					row++;
			}
			else
			{
				// se não tem filhos estamos numa linha

				// TODO
				// para obter o pagebreak a este nível é necessário aceder a query.Groups[nivel - 2]
				// aqui podemos verificar se estamos no primeiro elemento do grupo (if (first)) para escrever o cabeçalho, caso pagebreak = true
				// (se calhar este caso pode passar para a parte do if, porque a cada cabeçalho de um grupo é que sabemos se queremos escrever ou não as colunas - neste caso o first torna-se obsoleto)

				// escreve os valores da linha
				// começa na segunda coluna, porque a primeira é para ter os títulos dos totalizadores
				int column = 2;
				foreach (string value in group.Values)
					ws.Cells[row, column++].Value = string.IsNullOrEmpty(value) ? "-" : value;

				// passa para a linha seguinte
				row++;
			}
		}
	}
}
