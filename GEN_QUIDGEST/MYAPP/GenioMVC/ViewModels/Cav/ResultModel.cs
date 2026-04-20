using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using GenioMVC.Helpers.Cav;

namespace GenioMVC.ViewModels.Cav
{
	public class ResultInformation
	{
		public List<string> Headers { get; set; }

		public List<ResultInformation> Information { get; set; }

		public List<string> Values { get; set; }

		public ResultInformation()
		{
			this.Values = new List<string>();
			this.Headers = new List<string>();
		}
	}

	public class ResultModel
	{
		public ReportReply Result { get; set; }

		public ReportDefinition Query { get; set; }

		public ResultInformation ResultadoFinal { get; set; }

		public string QueryId { get; set; }

		public ResultModel() { }

		public ResultInformation Build_Result(List<string> prefixes, List<string> prefixesHeaders, ReportReplyGroup group, int nivel)
		{
			ResultInformation result = new ResultInformation();

			if (group.Groups != null && group.Groups.Count > 0)
			{
				// Aqui desenha-se o cabeçalho do grupo
				List<string> sub_prefixes = new List<string>(prefixes);

				// construir os prefixos para este nível
				foreach (string value in group.Values)
					sub_prefixes.Add(value);

				// valores do cabeçalho do grupo
				result.Values = new List<string>();
				result.Values.AddRange(sub_prefixes);

				// construir os cabeçalhos
				List<string> subHeaders = new List<string>(prefixesHeaders);

				if (nivel < this.Query.Groups.Count)
				{
					ReportGroup current = this.Query.Groups[nivel];
					foreach (ReportField f in current.Fields)
						subHeaders.Add(f.GetTitle());
				}
				else
				{
					foreach (ReportField f in this.Query.DetailsGroup.Fields)
						subHeaders.Add(f.GetTitle());
				}

				result.Headers.AddRange(subHeaders);

				result.Information = new List<ResultInformation>();
				foreach (ReportReplyGroup sub_group in group.Groups)
					result.Information.Add(Build_Result(sub_prefixes, prefixesHeaders, sub_group, nivel + 1));
			}
			else
			{
				foreach (string value in prefixes)
					result.Values.Add(value);
				foreach (string value in group.Values)
					result.Values.Add(value);
			}

			return result;
		}

		public ResultModel(ReportReply result, ReportDefinition query)
		{
			this.Result = result;
			this.Query = query;
		}
	}
}
