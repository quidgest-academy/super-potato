using System;
using System.Collections.Generic;
using System.Text;
using CSGenio.persistence;
using CSGenio.framework;
using System.Data;
using System.Collections;
using System.Linq;
//using System.Web.UI;
using System.IO;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business
{
    /// <summary>
    /// Classe que representa um pedido gateway do Flash Workflow
    /// </summary>
    public class RequestFlashWFGateway
    {
        //public string[] _args;
        public string Id { get; set; }
        public string IdType { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Duration { get; set; }
        public string CodDepartment { get; set; }
        public string Service { get; set; }
        public string CodEmployee { get; set; }
        public string Employee { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
		public string Greyed {get; set; }
		public string Text {get; set;}

        /// <summary>
		/// Constructor da classe RequestFlashWFGateway
		/// </summary>
		/// <param name="nomeComando"></param>
		/// <param name="parametro"></param>
		/// <param name="campos"></param>
		/// <param name="tipoFlash"></param>
        public RequestFlashWFGateway (string id, string tipoId, string codigo, string name, string duracao, string codservico, string servico,
			string codfuncionario, string funcionario, string x, string y, string greyed, string text) 
        { 
            this.Id = id;
			this.IdType = tipoId;
			this.Code = codigo;
			this.Name = name;
            this.Duration = duracao;
            this.CodDepartment = codservico;
            this.CodEmployee = codfuncionario;
            this.X = x;
            this.Y = y;
			this.Greyed = greyed;
			this.Text = text;
        }

        public RequestFlashWFGateway()
        {
            this.Id = "";
            this.IdType = "";
            this.Code = "";
            this.Name = "";
            this.Duration = "";
            this.CodDepartment = "";
            this.CodEmployee = "";
            this.X = "";
            this.Y = "";
			this.Greyed = "";
			this.Text = "";
        }

		
		public string toXML()
        {
            // Use /[MANUAL FOR GW_GATEW_TOXML]/ to override this
			// USE /[MANUAL FOR GW_GATEW_TOXML]/
			return "";
		}		
    }
}
