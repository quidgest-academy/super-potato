using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Administration.Models
{
    public class NotificationMessageModel : ModelBase
    {
        [Display(Name = "REMETENTE47685", ResourceType = typeof(Resources.Resources))]
        public SelectList TableEmailProperties { get; set; }

		[Display(Name = "DESTINATARIO22298", ResourceType = typeof(Resources.Resources))]
        public SelectList TableAllowedDestinations { get; set; }

        [Display(Name = "TAGS54909", ResourceType = typeof(Resources.Resources))]
        public SelectList TableAllowedTags { get; set; }

        public string ValSelectedTag { get; set; }
        //public string Identifier { get; set; }

		[Display(Name = "ASSINATURA_DE_EMAIL58979", ResourceType = typeof(Resources.Resources))]
        public SelectList TableEmailSignatures { get; set; }

        /// <summary>Campo : "Chave da tabela 'Propriedades de envio de emails'" Tipo: "CE" Formula:  ""</summary>
        public string ValCodsigna { get; set; }

        [Key]
        /// <summary>Campo : "PK da tabela MESGS" Tipo: "+" Formula:  ""</summary>
        public string ValCodmesgs { get; set; }

        /// <summary>Campo : "Chave para a tabela 'Tipos de notificações'" Tipo: "CE" Formula:  ""</summary>
        public string ValCodtpnot { get; set; }

        [Display(Name = "ID_DE_NOTIFICACAO24601", ResourceType = typeof(Resources.Resources))]
        /// <summary>Campo : "ID de notificação" Tipo: "CE" Formula:  ""</summary>
        public string ValIdnotif { get; set; }

        /// <summary>Campo : "Chave para a tabela virtual de Destinatário" Tipo: "CE" Formula:  ""</summary>
        public string ValCoddestn { get; set; }

		[Display(Name = "DESTINATARIO22298", ResourceType = typeof(Resources.Resources))]
        /// <summary>Campo : "Destinatário (formula + manual do campo dos destinatários)"</summary>
        public string ValTo { get; set; }

        [Display(Name = "DESTINATARIO_MANUAL30643", ResourceType = typeof(Resources.Resources))]
		/// <summary>Campo : "Destinatário manual?" Tipo: "L" Formula:  ""</summary>
        public bool ValDestnman { get; set; }

        [Display(Name = "DESTINATARIO_MANUAL30643", ResourceType = typeof(Resources.Resources))]
		/// <summary>Campo : "Destinatário manual"</summary>
        public string ValTomanual { get; set; }

        /// <summary>Campo : "Cc"</summary>
        [Display(Name = "Cc")]
        public string ValCc { get; set; }

        /// <summary>Campo : "Bcc"</summary>
        [Display(Name = "Bcc")]
        public string ValBcc { get; set; }

        /// <summary>Campo : "Chave da tabela 'Propriedades de envio de emails'" Tipo: "CE" Formula:  ""</summary>
        public string ValCodpmail { get; set; }

        [Display(Name = "REMETENTE47685", ResourceType = typeof(Resources.Resources))] 
        /// <summary>Campo : "Nome do remetente (formula + manual do campo das propriedades de envio de emails)"</summary>
        public string ValFrom { get; set; }

		[Display(Name = "NOTIFICACAO_NO_PORTA61717", ResourceType = typeof(Resources.Resources))] 
        /// <summary>Campo : "Disponibiliza notificação no portal" Tipo: "L" Formula:  ""</summary>
        public bool ValNotifica { get; set; }

        [Display(Name = "ENVIA_EMAIL_46551", ResourceType = typeof(Resources.Resources))]
        /// <summary>Campo : "Envia e-mail" Tipo: "L" Formula:  ""</summary>
        public bool ValEmail { get; set; }

        [Display(Name = "ASSUNTO16830", ResourceType = typeof(Resources.Resources))]
        /// <summary>Campo : "Assunto" Tipo: "C" Formula:  ""</summary>
        public string ValAssunto { get; set; }

        /// <summary>Campo : "Agregado" Tipo: "L" Formula:  ""</summary>
        public bool ValAgregado { get; set; }

		[Display(Name = "ENVIA_ANEXO_04515", ResourceType = typeof(Resources.Resources))]
        /// <summary>Campo : "Envia anexo?" Tipo: "L" Formula:  ""</summary>
        public bool ValAnexo { get; set; }

        [Display(Name = "FORMATO_HTML_65194", ResourceType = typeof(Resources.Resources))]
        /// <summary>Campo : "Formato HTML" Tipo: "L" Formula:  ""</summary>
        public bool ValHtml { get; set; }

        [Display(Name = "GRAVA_NA_BD_43814", ResourceType = typeof(Resources.Resources))]
        /// <summary>Campo : "Grava na BD?" Tipo: "L" Formula:  ""</summary>
        public bool ValGravabd { get; set; }

        [Display(Name = "ATIVO_00196", ResourceType = typeof(Resources.Resources))]
        /// <summary>Campo : "Ativo" Tipo: "AC" Formula:  ""</summary>
        public bool ValAtivo { get; set; }
        //public System.Web.Mvc.SelectList ArrayValestado { get { return new System.Web.Mvc.SelectList(CSGenio.business.ArrayAestado.GetDictionary().ToDictionary(p => p.Key, p => ArrayAestado.CodToDescricao(p.Key)), "Key", "Value", ValEstado); } set { ValEstado = value.SelectedValue as string; } }

        [Display(Name = "NOME47814", ResourceType = typeof(Resources.Resources))]
        /// <summary>Campo : "Nome" Tipo: "C" Formula:  ""</summary>
        public string ValDesignac { get; set; }

        //[AllowHtml]
        [Display(Name = "MENSAGEM32641", ResourceType = typeof(Resources.Resources))]
        /// <summary>Campo : "Mensagem" Tipo: "MO" Formula:  ""</summary>
        [DataType(DataType.MultilineText)]
        public string ValMensagem { get; set; }

        [Display(Name = "ALTERACAO__DATA26843", ResourceType = typeof(Resources.Resources))] 
        /// <summary>Campo : "Alteração: Data" Tipo: "ED" Formula:  ""</summary>
        [DataType(DataType.Date)]
        public DateTime? ValDatamuda { get; set; }

        [Display(Name = "CRIACAO__DATA34411", ResourceType = typeof(Resources.Resources))]
        /// <summary>Campo : "Criação: Data" Tipo: "OD" Formula:  ""</summary>
        [DataType(DataType.Date)]

        public DateTime? ValDatacria { get; set; }

        [Display(Name = "CRIACAO__OPERADOR43734", ResourceType = typeof(Resources.Resources))]
        /// <summary>Campo : "Criação: Operador" Tipo: "ON" Formula:  ""</summary>
        public string ValOpercria { get; set; }

        [Display(Name = "ALTERACAO__OPERADOR09152", ResourceType = typeof(Resources.Resources))]
        /// <summary>Campo : "Alteração: Operador" Tipo: "EN" Formula:  ""</summary>
        public string ValOpermuda { get; set; }

        /// <summary>Campo : "ZZSTATE" Tipo: "INT" Formula:  ""</summary>
        public int ValZzstate { get; set; }

        public string FormMode { get; set; }
        public string ResultMsg { get; set; }
        
        #region Class Methods
        public void MapToModel(CSGenioAnotificationmessage m)
        {
            if (m == null)
            {
                Log.Error("Map ViewModel (NotificationMessageModel) to Model (CSGenioAnotificationmessage) - Model is a null reference");
                throw new Exception("Model not found");
            }
            try
            {
                m.ValAgregado = DBConversion.ToLogic(ValAgregado);
                m.ValAnexo = DBConversion.ToInteger(ValAnexo);
                m.ValAssunto = DBConversion.ToString(ValAssunto);
                m.ValCodmesgs = DBConversion.ToString(ValCodmesgs);
                m.ValCodpmail = DBConversion.ToString(ValCodpmail);
                m.ValFrom = DBConversion.ToString(ValFrom);
                m.ValCodtpnot = DBConversion.ToString(ValCodtpnot);
                m.ValIdnotif = DBConversion.ToString(ValIdnotif);
                m.ValDesignac = DBConversion.ToString(ValDesignac);
                m.ValEmail = DBConversion.ToLogic(ValEmail);
                m.ValAtivo = DBConversion.ToLogic(ValAtivo);
                m.ValHtml = DBConversion.ToLogic(ValHtml);
                m.ValMensagem = DBConversion.ToString(ValMensagem);
                m.ValNotifica = DBConversion.ToLogic(ValNotifica);
                m.ValGravabd = DBConversion.ToLogic(ValGravabd);
                m.ValCoddestn = DBConversion.ToString(ValCoddestn);
                m.ValTo = DBConversion.ToString(ValTo);
                m.ValDestnman = DBConversion.ToLogic(ValDestnman);
                m.ValTomanual = DBConversion.ToString(ValTomanual);
                m.ValCc = DBConversion.ToString(ValCc);
                m.ValBcc = DBConversion.ToString(ValBcc);

                m.ValCodsigna = DBConversion.ToString(ValCodsigna);

                m.ValDatamuda = DBConversion.ToDateTime(ValDatamuda);
                m.ValDatacria = DBConversion.ToDateTime(ValDatacria);
                m.ValOpercria = DBConversion.ToString(ValOpercria);
                m.ValOpermuda = DBConversion.ToString(ValOpermuda);

            }
            catch (Exception)
            {
                Log.Error("Map ViewModel (NotificationMessageModel) to Model (CSGenioAnotificationmessage) - Error during mapping");
                throw;
            }
        }

        public void MapFromModel(CSGenioAnotificationmessage m)
        {
            if (m == null)
            {
                Log.Error("Map ViewModel (NotificationMessageModel) from Model (CSGenioAnotificationmessage) - Model is a null reference");
                throw new Exception("Model not found");
            }
            try
            {
                ValAgregado = DBConversion.ToLogic(m.ValAgregado) == 1 ? true : false; 
                ValAnexo = DBConversion.ToLogic(m.ValAnexo) == 1 ? true : false; 
                ValAssunto = DBConversion.ToString(m.ValAssunto);
                ValCodmesgs = DBConversion.ToString(m.ValCodmesgs);
                ValCodpmail = DBConversion.ToString(m.ValCodpmail);
                ValFrom = DBConversion.ToString(m.ValFrom);
                ValCodtpnot = DBConversion.ToString(m.ValCodtpnot);
                ValIdnotif = DBConversion.ToString(m.ValIdnotif);
                ValDesignac = DBConversion.ToString(m.ValDesignac);
                ValEmail = DBConversion.ToLogic(m.ValEmail) == 1 ? true : false; 
                ValAtivo = DBConversion.ToLogic(m.ValAtivo)== 1 ? true : false; 
                ValHtml = DBConversion.ToLogic(m.ValHtml) == 1 ? true : false; 
                ValMensagem = DBConversion.ToString(m.ValMensagem);
                ValNotifica = DBConversion.ToLogic(m.ValNotifica) == 1 ? true : false;
                ValGravabd = DBConversion.ToLogic(m.ValGravabd) == 1 ? true : false;
                ValCoddestn = DBConversion.ToString(m.ValCoddestn);
                ValTo = DBConversion.ToString(m.ValTo);
                ValDestnman = DBConversion.ToLogic(m.ValDestnman) == 1 ? true : false;
                ValTomanual = DBConversion.ToString(m.ValTomanual);
                ValCc = DBConversion.ToString(m.ValCc);
                ValBcc = DBConversion.ToString(m.ValBcc);

                ValCodsigna = DBConversion.ToString(m.ValCodsigna);

                ValDatamuda = DBConversion.ToDateTime(m.ValDatamuda);
                ValDatacria = DBConversion.ToDateTime(m.ValDatacria);
                ValOpercria = DBConversion.ToString(m.ValOpercria);
                ValOpermuda = DBConversion.ToString(m.ValOpermuda);

            }
            catch (Exception)
            {
                Log.Error("Map ViewModel (NotificationMessageModel) to Model (CSGenioAnotificationmessage) - Error during mapping");
                throw;
            }
        }
        #endregion
    }


}

