using System;
using System.Text;
using System.Collections.Generic;

namespace CSGenio.business
{
	/// <summary>
	/// Classe que representa a definição das arrays como formulas to listagens no Crystal Reports.
	/// </summary>
    public class ArraysCrystalReports
    {
        /// <summary>
        /// variável que vai ter todas as áreas
        /// </summary>
        private static Dictionary<string, string> todasArrays;

        static ArraysCrystalReports()
		{
            todasArrays = new Dictionary<string, string>();

            StringBuilder Qresult = new StringBuilder();
            
			// buildtyp
            Qresult = new StringBuilder();
			            Qresult.AppendLine("if {{{0}}} = \"apartment\" then \"Apartment\" else");
			            Qresult.AppendLine("if {{{0}}} = \"house\" then \"House\" else");
			            Qresult.AppendLine("if {{{0}}} = \"other\" then \"Other\" else");
            Qresult.Append("\"                                              \"");
            todasArrays.Add("buildtyp", Qresult.ToString());
			// s_modpro
            Qresult = new StringBuilder();
			            Qresult.AppendLine("if {{{0}}} = \"INDIV\" then \"Individual\" else");
			            Qresult.AppendLine("if {{{0}}} = \"global\" then \"Global\" else");
			            Qresult.AppendLine("if {{{0}}} = \"unidade\" then \"Unidade orgânica\" else");
			            Qresult.AppendLine("if {{{0}}} = \"horario\" then \"Horário\" else");
            Qresult.Append("\"                                              \"");
            todasArrays.Add("s_modpro", Qresult.ToString());
			// s_module
            Qresult = new StringBuilder();
            Qresult.Append("\"                                              \"");
            todasArrays.Add("s_module", Qresult.ToString());
			// s_prstat
            Qresult = new StringBuilder();
			            Qresult.AppendLine("if {{{0}}} = \"EE\" then \"Em execução\" else");
			            Qresult.AppendLine("if {{{0}}} = \"FE\" then \"Em fila de espera\" else");
			            Qresult.AppendLine("if {{{0}}} = \"AG\" then \"Agendado para execução\" else");
			            Qresult.AppendLine("if {{{0}}} = \"T\" then \"Terminado\" else");
			            Qresult.AppendLine("if {{{0}}} = \"C\" then \"Cancelado\" else");
			            Qresult.AppendLine("if {{{0}}} = \"NR\" then \"Não responde\" else");
			            Qresult.AppendLine("if {{{0}}} = \"AB\" then \"Abortado\" else");
			            Qresult.AppendLine("if {{{0}}} = \"AC\" then \"A cancelar\" else");
            Qresult.Append("\"                                              \"");
            todasArrays.Add("s_prstat", Qresult.ToString());
			// s_resul
            Qresult = new StringBuilder();
			            Qresult.AppendLine("if {{{0}}} = \"ok\" then \"Sucesso\" else");
			            Qresult.AppendLine("if {{{0}}} = \"er\" then \"Erro\" else");
			            Qresult.AppendLine("if {{{0}}} = \"wa\" then \"Aviso\" else");
			            Qresult.AppendLine("if {{{0}}} = \"c\" then \"Cancelado\" else");
            Qresult.Append("\"                                              \"");
            todasArrays.Add("s_resul", Qresult.ToString());
			// s_roles
            Qresult = new StringBuilder();
            Qresult.Append("\"                                              \"");
            todasArrays.Add("s_roles", Qresult.ToString());
			// s_tpproc
            Qresult = new StringBuilder();
            Qresult.Append("\"                                              \"");
            todasArrays.Add("s_tpproc", Qresult.ToString());
			// typology
            Qresult = new StringBuilder();
			Qresult.AppendLine("if {{{0}}} = 0 then \"T0\" else");
			Qresult.AppendLine("if {{{0}}} = 1 then \"T1\" else");
			Qresult.AppendLine("if {{{0}}} = 2 then \"T2\" else");
			Qresult.AppendLine("if {{{0}}} = 3 then \"T3 or more\" else");
            Qresult.Append("\"                                              \"");
            todasArrays.Add("typology", Qresult.ToString());
        }

        /// <summary>
        /// Função que dado o identifier da array e o Qfield da table devolve a string usada no crystal
        /// </summary>
        /// <param name="nomeArray">name da Array</param>
        /// <param name="tabelaCampo">table.Qfield</param>
        /// <param name="ano">Qyear actual da aplicacao</param>
        /// <returns>Area correspondente</returns>
        public static string returnArrayCrystal(string arrayName, string tableField, string Qyear)
        {
            if (todasArrays.ContainsKey(arrayName))
            {
                string formula = string.Format(todasArrays[arrayName], tableField);
                int iano;
                if (formula.Contains("#_ano") && Int32.TryParse(Qyear, out iano))
                {
                    formula = formula.Replace("#_ano#4#", iano.ToString());
                    for (int i = 1; i < 10; i++)
                        formula = formula.Replace("#_ano" + i +"#4#", (iano + i).ToString());
                    for (int i = 1; i < 10; i++)
                        formula = formula.Replace("#_ano_" + i + "#4#", (iano - i).ToString());
                }

                return formula;
            }

            throw new BusinessException(null, "ArrayCrystalReports..devolveArrayCrystal", "Can't find an array with name: " + arrayName);
        }
    }        
}
