using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Administration.Models
{
    public class ManageReportsModel : ModelBase
    {
        public ManageReportsModel()
        {
        }

        [Display(Name = "ESTADO07788", ResourceType = typeof(Resources.Resources))]
        public string ResultMsg { get; set; }

        [Display(Name = "DIRECTORIA_DE_RELATO59580", ResourceType = typeof(Resources.Resources))]
        public string ReportsPath { get; set; }

        [Display(Name = "URL_DO_SERVIDOR_DE_R44145", ResourceType = typeof(Resources.Resources))]
        public string ReportsServerUrl { get; set; }

        [Display(Name = "SUBPATH_NO_SERVIDOR_61718", ResourceType = typeof(Resources.Resources))]
        public string ReportsServerPath { get; set; }

        [Display(Name = "LISTA_DE_RELATORIOS46856", ResourceType = typeof(Resources.Resources))]
        public List<ReportItem> ReportList { get; set; }

        public bool DeployActive { get; set; }
    }


    public class ReportItem
    {
        [Display(Name = "TIPO55111", ResourceType = typeof(Resources.Resources))]
        public string ReportType { get; set; }
        [Display(Name = "NOME47814", ResourceType = typeof(Resources.Resources))]
        public string Name { get; set; }
        [Display(Name = "DATA_INSTALACAO03103", ResourceType = typeof(Resources.Resources))]
        public DateTime? DateInstall { get; set; }
        [Display(Name = "DATA_FICHEIRO58453", ResourceType = typeof(Resources.Resources))]
        public DateTime? DateFile { get; set; }
        public string Hash { get; set; }
        [Display(Name = "DINAMICO34700", ResourceType = typeof(Resources.Resources))]
        public bool Dynamic { get; set; }
        [Display(Name = "ESTADO07788", ResourceType = typeof(Resources.Resources))]
        public string Status { get; set; }
        [Display(Name = "ERRO38355", ResourceType = typeof(Resources.Resources))]
        public string Error { get; set; }
    }

    public class ReportInstallXml
    {
        public string Report { get; set; }
        public DateTime Date { get; set; }
        public string Hash { get; set; }
        public bool Dynamic { get; set; }
        public string Error { get; set; }

        public static List<ReportInstallXml> Load(string filename)
        {
            List<ReportInstallXml> res = new List<ReportInstallXml>();
            try
            {
                using (System.IO.StreamReader input = new System.IO.StreamReader(filename))
                {
                    System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof(List<ReportInstallXml>));
                    res = (List<ReportInstallXml>)s.Deserialize(input);
                }
            }
            catch (FileNotFoundException)
            {
                //Return empty list
            }
            return res;
        }

        public static void Save(List<ReportInstallXml> list, string filename)
        {
            using (System.IO.StreamWriter output = new System.IO.StreamWriter(filename))
            {
                System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(list.GetType());
                s.Serialize(output, list);
            }
        }
    }
		
	
	/// <summary>
    /// Define the ViewModel for slot report form
    /// </summary>
    public class SlotReportsModel : ModelBase
    {
        public SlotReportsModel(){ }

        /// <summary>
        /// Record key 
        /// </summary>
        [Key]        
        public string ValCodreport { get; set; } 

        /// <summary>
        /// Slot title/description 
        /// </summary>
        [Display(Name = "Título")]
        public string ValTitulo { get; set; }

        /// <summary>
        /// Slot identifier
        /// </summary>
        [Display(Name = "ID")]
        public string ValSlotid { get; set; }

        /// <summary>
        /// Report file name
        /// </summary>
        [Display(Name = "Report")]
        public string ValReport { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime ValDatacria { get; set; }

        /// <summary>
        /// Creation user
        /// </summary>
        public string ValOpercria { get; set; }

        /// <summary>
        /// Pseud record status flag
        /// </summary>
        public int ValZzstate { get; set; }

        /// <summary>
        /// Define forme mode ('new', 'edit', 'delete')
        /// </summary>
        public string FormMode { get; set; }

        /// <summary>
        /// Map the ViewModel fields to the Model.
        /// </summary>
        /// <param name="m">Model</param>
        public void MapToModel(CSGenioAreportlist m)
        {
            if (m == null)
            {
                Log.Error("Map ViewModel (SlotReportsModel) to Model (CSGenioAreportlist) - Model is a null reference");
                throw new Exception("Model not found");
            }
            try
            {
                m.ValTitulo = DBConversion.ToString(ValTitulo);
                m.ValSlotid = DBConversion.ToString(ValSlotid);
                m.ValReport = DBConversion.ToString(ValReport);
                m.ValDatacria = DBConversion.ToDateTime(ValDatacria);
                m.ValOpercria = DBConversion.ToString(ValOpercria);                
                m.ValCodreport = DBConversion.ToString(ValCodreport);
            }
            catch (Exception)
            {
                Log.Error("Map ViewModel (SlotReportsModel) to Model (CSGenioAreportlist) - Error during mapping");
                throw;
            }
        }

        /// <summary>
        /// Map Model fields to the ViewModel
        /// </summary>
        /// <param name="m">Model</param>
        public void MapFromModel(CSGenioAreportlist m)
        {
            if (m == null)
            {
                Log.Error("Map model (CSGenioAreportlist) to ViewModel (SlotReportsModel) - Model is a null reference");
                throw new Exception("Model not found");
            }
            try
            {
                ValTitulo = DBConversion.ToString(m.ValTitulo);
                ValSlotid = DBConversion.ToString(m.ValSlotid);
                ValReport = DBConversion.ToString(m.ValReport);
                ValDatacria = DBConversion.ToDateTime(m.ValDatacria);
                ValOpercria = DBConversion.ToString(m.ValOpercria);
                ValCodreport = DBConversion.ToString(m.ValCodreport);
            }
            catch (Exception)
            {
                Log.Error("Map model (CSGenioAreportlist) to ViewModel (SlotReportsModel) - Error during mapping");
                throw;
            }
        }

    }

}