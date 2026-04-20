using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Administration.Models
{
    /// <summary>
    /// Interface EmailSignature
    /// </summary>
    public class EmailSignatureModel : ModelBase
    {

        /// <summary>
        /// Class to store information on Email Signature to be used when sending messages through email
        /// </summary>
        [Key]
        /// <summary>Campo : "PK da tabela" Tipo: "+" Formula:  ""</summary>
        public string ValCodsigna { get; set; } //Signature identifier

        [Display(Name = "NOME47814", ResourceType = typeof(Resources.Resources))]
        public string ValName { get; set; } //signature name

        /*[DataType(DataType.Upload)]
        [Display(Name = "IMAGEM19513", ResourceType = typeof(Resources.Resources))]
        public byte[] ValImage { get; set; }*/

        //[AllowHtml]
        [Display(Name = "TEXTO_APOS_A_ASSINAT02808", ResourceType = typeof(Resources.Resources))]
        public string ValTextass { get; set; } //signature name


        public DateTime ValDatamuda { get; set; }
        public DateTime ValDatacria { get; set; }
        public string ValOpercria { get; set; }
        public string ValOpermuda { get; set; }
        public int ValZzstate { get; set; }

        public string FormMode { get; set; }
        public string ResultMsg { get; set; }

        public void MapToModel(CSGenioAnotificationemailsignature m)
        {
            if (m == null)
            {
                Log.Error("Map ViewModel (EmailSignatureModel) to Model (CSGenioAnotificationemailsignature) - Model is a null reference");
                throw new Exception("Model not found");
            }
            try
            {
                m.ValName = DBConversion.ToString(ValName);
                //m.ValImage = ValImage;
                m.ValTextass = DBConversion.ToString(ValTextass);
                m.ValDatamuda = DBConversion.ToDateTime(ValDatamuda);
                m.ValDatacria = DBConversion.ToDateTime(ValDatacria);
                m.ValOpercria = DBConversion.ToString(ValOpercria);
                m.ValOpermuda = DBConversion.ToString(ValOpermuda);
                m.ValCodsigna = DBConversion.ToString(ValCodsigna);
            }
            catch (Exception)
            {
                Log.Error("Map ViewModel (EmailSignatureModel) to Model (CSGenioAnotificationemailsignature) - Error during mapping");
                throw;
            }
        }

        public void MapFromModel(CSGenioAnotificationemailsignature m)
        {
            if (m == null)
            {
                Log.Error("Map ViewModel (EmailSignatureModel) to Model (CSGenioAnotificationemailsignature) - Model is a null reference");
                throw new Exception("Model not found");
            }
            try
            {
                ValName = DBConversion.ToString(m.ValName);
                //ValImage = m.ValImage;
                ValTextass = DBConversion.ToString(m.ValTextass);
                ValDatamuda = DBConversion.ToDateTime(m.ValDatamuda);
                ValDatacria = DBConversion.ToDateTime(m.ValDatacria);
                ValOpercria = DBConversion.ToString(m.ValOpercria);
                ValOpermuda = DBConversion.ToString(m.ValOpermuda);
                ValCodsigna = DBConversion.ToString(m.ValCodsigna);
            }
            catch (Exception)
            {
                Log.Error("Map ViewModel (EmailSignatureModel) to Model (CSGenioAnotificationemailsignature) - Error during mapping");
                throw;
            }
        }
    }


}

