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
    public class RequestFlashWFRela
    {
        //public string[] _args;
        public string Codmerel { get; set; }
        public string Label { get; set; }
        public string Fromid { get; set; }
        public string Toid { get; set; }
        public string Fromside { get; set; }
        public string Toside { get; set; }
        public string Fromoffset { get; set; }
        public string Tooffset { get; set; }
        public string Type { get; set; }

        /// <summary>
        /// Constructor da classe RequestFlashWFRela
        /// </summary>
        /// <param name="nomeComando"></param>
        /// <param name="parametro"></param>
        /// <param name="campos"></param>
        /// <param name="tipoFlash"></param>
        public RequestFlashWFRela(string type, string fromid, string toid, string fromside, string toside, string fromoffset, string tooffset) 
        {
            this.Type = type;
            this.Fromid = fromid;
            this.Toid = toid;
            this.Fromside = fromside;
            this.Toside = toside;
            this.Fromoffset = fromoffset;
            this.Tooffset = tooffset;
        }

        public RequestFlashWFRela()
        {
            this.Type = "";
            this.Fromid = "";
            this.Toid = "";
            this.Fromside = "";
            this.Toside = "";
            this.Fromoffset = "";
            this.Tooffset = "";
        }
		
		public string toXML()
        {
            // Use /[MANUAL FOR GW_RELA_TOXML]/ to override this
			// USE /[MANUAL FOR GW_RELA_TOXML]/
			return "";
		}		
    }
}
