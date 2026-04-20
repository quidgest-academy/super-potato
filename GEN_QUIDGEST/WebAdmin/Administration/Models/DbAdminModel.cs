using Administration.AuxClass;
using ExecuteQueryCore;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Administration.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter<AlertTypeEnum>))]
    public enum AlertTypeEnum
    {
        info,
        success,
        danger,
        warning
    }

    public class DbAdminModel : ModelBase
    {
        [Display(Name = "BASE_DE_DADOS58234", ResourceType = typeof(Resources.Resources))]
        public string DBSchema { get; set; }
        
        [Display(Name = "TAMANHO_DA_BD56664", ResourceType = typeof(Resources.Resources))]
        public decimal DBSize { get; set; }       

        [Display(Name = "VERSAO_DO_SCHEMA11580", ResourceType = typeof(Resources.Resources))]
        public decimal VersionDb { get; set; }

        [Display(Name = "VERSAO_DA_APLICACAO45955", ResourceType = typeof(Resources.Resources))]
        public decimal VersionApp { get; set; }

        [Display(Name = "VERSAO_DOS_SCRIPTS52566", ResourceType = typeof(Resources.Resources))]
        public decimal VersionReIdx { get; set; }

        [Display(Name = "DATABASE_VERSION15344", ResourceType = typeof(Resources.Resources))]
        public int VersionUpgrIndx { get; set; }

        [Display(Name = "APPLICATION_VERSION32207", ResourceType = typeof(Resources.Resources))]
        public int VersionUpgrScripts { get; set; }

        [Display(Name = "ATUALIZACAO_DISPONIV00656", ResourceType = typeof(Resources.Resources))]
        public bool UpgradeIsAvailable { get; set; }

        [Display(Name = "NOME_DE_UTILIZADOR58858", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string DbUser { get; set; }

        public AlertTypeEnum AlertType { get; set; }

        [Display(Name = "PALAVRA_PASSE44126", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string DbPsw { get; set; }

        [Display(Name = "DIRETORIA_DE_FILESTR39886", ResourceType = typeof(Resources.Resources))]
        public string DirFilestream { get; set; }

        [Display(Name = "ESTADO07788", ResourceType = typeof(Resources.Resources))]
        public string ResultMsg { get; set; }

        public List<ReindexFunctionItem> Items { get; set; }

        public int Timeout { get; set; }

        public ReindexOrder reindexMenu { get; set; }

        public bool? ReindexActive { get; set; }

        [Display(Name = "REINDEXACAO_COMPLETA51519", ResourceType = typeof(Resources.Resources))]
        public bool Zero { get; set; }

        public string AppSystem { get; set; }

        public string BaseLang { get; set; }

        public string DSName { get; set; }

        public RdxOperationInfo LastLogInfo { get; set; }
    }

    public class DbBackupModel
    {
        [Display(Name = "NOME_DE_UTILIZADOR58858", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string DbUser { get; set; }

        [Display(Name = "PALAVRA_PASSE44126", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string DbPsw { get; set; }

        public string BackupItem { get; set; }

        public AlertTypeEnum AlertType { get; set; }

        [Display(Name = "ESTADO07788", ResourceType = typeof(Resources.Resources))]
        public string ResultMsg { get; set; }

        public List<BackupFileItem> BackupFiles { get; set; }


        public void Load(string basePath)
        {
            if (Directory.Exists(basePath))
            {
                DirectoryInfo directory = new(basePath);

                BackupFiles = directory.EnumerateFiles("*.bak")
                                       .OrderByDescending(f => f.CreationTime)
                                       .Select(bakFile => new BackupFileItem
                                       {
                                           Filename = bakFile.Name,
                                           Date = bakFile.CreationTime,
                                           Size = Math.Round(bakFile.Length / 1_000_000M, 2)
                                       })
                                       .ToList();
            }
            else
            {
                BackupFiles = [];
            }
        }
    }

    public class BackupFileItem
    {
        [Display(Name = "Ficheiro")]
        public string Filename { get; set; }

        [Display(Name = "Data")]
        public DateTime Date { get; set; }

        [Display(Name = "Tamanho")]
        public decimal Size { get; set; }
    }

    public class DataQualityModel
    {
        [Display(Name = "ESTADO07788", ResourceType = typeof(Resources.Resources))]
        public string ResultMsg { get; set; }
        public AlertTypeEnum AlertType { get; set; }
        public List<IncoherencyModel> Incoherencies { get; set; }
    }

    public class IndexesModel
    {
        [Display(Name = "ESTADO07788", ResourceType = typeof(Resources.Resources))]
        public string ResultMsg { get; set; }
        public AlertTypeEnum AlertType { get; set; }
        public List<IndexModel> Indexes { get; set; }
    }
    public class IndexModel
    {
        //model number
        public int Num { get; set; }

        public int OrderCol { get; set; }

        public string ProgressMessage { get; set; }

        public int ProgressPercent { get; set; }

        public bool Completed { get; set; }

        public bool Active { get; set; }

        public string Year { get; set; }

        public string IndexType { get; set; }

        public AlertTypeEnum AlertType { get; set; }

        public string IndexTitle { get; set; }

        [Display(Name = "ULTIMA_VERIFICACAO35305", ResourceType = typeof(Resources.Resources))]
        public DateTime LastUpdate { get; set; }

        public List<UnusedIndexItem> UnusedIndexesList { get; set; }

        public List<RecommendedIndexItem> RecommendedIndexesList { get; set; }
	
        [Display(Name = "ESTADO07788", ResourceType = typeof(Resources.Resources))]
        public string ResultMsg { get; set; }
    }
    public class IncoherencyModel
    {
        //model number
        public int Num { get; set; }
        public string ProgressMessage { get; set; }

        public int ProgressPercent { get; set; }

        public bool Completed { get; set; }

        public bool Active { get; set; }

        public string Year { get; set; }

        public string IncoherenceType { get; set; }

        public string IncoherenceTitle { get; set; }

        public AlertTypeEnum AlertType { get; set; }

        //description of checkbox 
        [Display(Name = "ULTIMA_VERIFICACAO35305", ResourceType = typeof(Resources.Resources))]
        public DateTime LastUpdate { get; set; }
        public List<IncoherentRelationItem> IncoherentRelations { get; set; }
        public List<OrphanRelation> OrphanRelations { get; set; }
        //Value of use views checkbox 

        [Display(Name = "Tipo de pesquisa")]
        public HardCodedLists.RelationsMode RelationMode { get; set; }

        // public string ViewsText { get; set; }
        //whether the use views checkbox is selected or not
        [Display(Name = "Considerar as Views")]
        public bool ViewsIsChecked { get; set; }

        //whether the use views checkbox is selected or not
        [Display(Name = "Considerar chaves a NULL")]
        public bool NullsIsChecked { get; set; }
    }

    public class UnusedIndexItem
    {
        //ordem n tem display, será omitida
        public decimal OrderCol { get; set; }

        [Display(Name = "Tabela")]
        public string ObjectName { get; set; }

        [Display(Name = "Índice")]
        public string IndexName { get; set; }

        [Display(Name = "Seeks")]
        public int UserSeeks { get; set; }

        [Display(Name = "Scans")]
        public int UserScans { get; set; }

        [Display(Name = "Lookups")]
        public int UserLookups { get; set; }

        [Display(Name = "Updates")]
        public int UserUpdates { get; set; }

        [Display(Name = "Registos")]
        public int TableRows { get; set; }

        [Display(Name = "Colunas")]
        public string ColumnNames { get; set; }

        [Display(Name = "Query Eliminação")]
        public string Drop_Index { get; set; }
    }

    public class RecommendedIndexItem
    {
        //ordem n tem display, será omitida
        public decimal OrderCol { get; set; }

        [Display(Name = "Tabela")]
        public string TableName { get; set; }

        [Display(Name = "Colunas comparadas por igualdade (=)")]
        public string EqualityColumns { get; set; }

        [Display(Name = "Colunas comparadas por não-igualdade (<;>;!=)")]
        public string InequalityColumns { get; set; }

        [Display(Name = "Colunas incluídas na pesquisa")]
        public string IncludedColumns { get; set; }

        [Display(Name = "Último Seek")]
        public DateTime Last_User_Seek { get; set; }

        [Display(Name = "Seeks")]
        public int UserSeeks { get; set; }

        [Display(Name = "Scans")]
        public int UserScans { get; set; }

        [Display(Name = "Melhoria (%)")]
        public decimal Avg_User_Impact { get; set; }

        [Display(Name = "Impacto")]
        public decimal Avg_Estimated_Impact { get; set; }

        [Display(Name = "Query criação")]
        public string Create_Statement { get; set; }
    }

    public class IncoherentRelationItem
    {
        [Display(Name = "Table 1")]
        public string Table1 { get; set; }

        [Display(Name = "Foreign key 1")]
        public string Fk1 { get; set; }

        [Display(Name = "Table 2")]
        public string Table2 { get; set; }

        [Display(Name = "Foreign key 2")]
        public string Fk2 { get; set; }

        [Display(Name = "Destination")]
        public string Destination { get; set; }

        [Display(Name = "Inconsistent Relations")]
        public int CountIR { get; set; }

        [Display(Name = "Path")]
        public string Path { get; set; }

        [Display(Name = "Query")]
        public string Sql { get; set; }
    }

    public class OrphanRelation
    {
        [Display(Name = "Table 1")]
        public string Table1 { get; set; }

        [Display(Name = "Foreign key 1")]
        public string Fk1 { get; set; }

        [Display(Name = "Destination")]
        public string Destination { get; set; }

        [Display(Name = "Orphan Records")]
        public int CountOrphans { get; set; }

        [Display(Name = "Query")]
        public string Sql { get; set; }
    }

    public class ChangeYearModel
    {
        [Display(Name = "NOME_DA_BASE_DE_DADO06329", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string NewDBSchema { get; set; }

        [Display(Name = "NOME_DA_BASE_DE_DADO15982", ResourceType = typeof(Resources.Resources))]
        public string NewAuditDBSchema { get; set; }

        [Display(Name = "CRIAR_A_BASE_DE_DADO55641", ResourceType = typeof(Resources.Resources))]
        public bool CriarBD { get; set; }

        [Display(Name = "NOME_DE_UTILIZADOR58858", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string DbUser { get; set; }

        [Display(Name = "PALAVRA_PASSE44126", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string DbPsw { get; set; }
        
        public AlertTypeEnum AlertType { get; set; }

        [Display(Name = "BASES_DE_DADOS_DISPO02109", ResourceType = typeof(Resources.Resources))]
        public IEnumerable<SelectListItem> Years { get; set; }

        [Display(Name = "Source database")]
        [Required]
        public string SrcYear { get; set; }

        [Display(Name = "ANO33022", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string Year { get; set; }

        [Display(Name = "DIRETORIA_DE_FILESTR39886", ResourceType = typeof(Resources.Resources))]
        public string DirFilestream { get; set; }

        public int Timeout { get; set; }
    }
	
	public enum DisplayEncrypt
    {
        [Display(Name = "AES 128")]
        AES_128,
        [Display(Name = "AES 192")]
        AES_192,
        [Display(Name = "AES 256")]
        AES_256,
        [Display(Name = "Triple DES")]
        TRIPLE_DES_3KEY
    }
	
	public class DbSecurityModel
    {
        [Display(Name = "NOME_DE_UTILIZADOR58858", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string DbUser { get; set; }

        [Display(Name = "PALAVRA_PASSE44126", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string DbPsw { get; set; }

        [Display(Name = "ESTADO07788", ResourceType = typeof(Resources.Resources))]
        public string ResultMsg { get; set; }

        [Display(Name = "CHAVE_MESTRA09773", ResourceType = typeof(Resources.Resources))]
        public string MasterPsw { get; set; }

        [Display(Name = "ALGORITMO_DE_ENCRIPT09649", ResourceType = typeof(Resources.Resources))]
        public DisplayEncrypt Encryption { get; set; }

        [Display(Name = "CRIACAO_DA_CHAVE_MES19380", ResourceType = typeof(Resources.Resources))]
        public bool MasterKey { get; set; }

        public AlertTypeEnum AlertType { get; set; }

        public object SelectLists
        {
            get
            {
                return new
                {
                    DisplayEncrypt = AuxFunctions.ToSelectList<DisplayEncrypt>()
                };
            }
        }
    }

    public class DbMigrateFilesModel
    {
        [Display(Name = "ESTADO07788", ResourceType = typeof(Resources.Resources))]
        public string ResultMsg { get; set; }

        public List<MigrateFileItem> MigrateFiles { get; set; }

        public int FileCount { get; set; }

        public AlertTypeEnum AlertType { get; set; }

        public void Load(string year)
        {
            MigrateFiles = new List<MigrateFileItem>();
            var sp = CSGenio.persistence.PersistentSupport.getPersistentSupport(year);
 
            //Fetch File count
            string sql = @"SELECT COUNT(*) AS cnt FROM docums WITH (NOLOCK) WHERE docpath IS NULL";
            CSGenio.persistence.DataMatrix dm = sp.executeQuery(sql);
            this.FileCount = dm.GetInteger(0, "cnt");
 
            //Fetch top data from DB
            sql = @"SELECT TOP 50 documid, nome, tamanho, tabela, campo FROM docums WITH (NOLOCK) WHERE docpath IS NULL";
            dm = sp.executeQuery(sql);
 
            for (int i = 0; i < dm.NumRows; i++)
            {
                MigrateFileItem file = new MigrateFileItem();
                file.Documid = dm.GetInteger(i, "documid");
                file.Name = dm.GetString(i, "nome");
                file.Size = Convert.ToDecimal(dm.GetNumeric(i, "tamanho") / 1000000);
                file.Table = dm.GetString(i, "tabela");
                file.Field = dm.GetString(i, "campo");
 
                MigrateFiles.Add(file);
            }
        }
    }

    public class MigrateFileItem
    {
        [Display(Name = "Documid")]
        public int Documid { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Size")]
        public decimal Size { get; set; }

        [Display(Name = "Table")]
        public string Table { get; set; }

        [Display(Name = "Field")]
        public string Field { get; set; }
    }

    /// <summary>
    /// Class that contains information about the currently running maintenance job.
    /// </summary>
    public class MaintenanceProgress
    {
        /// <summary>
        /// The maintenance progress percentage.
        /// </summary>
        public double Count { get; set; } = 0;

        /// <summary>
        /// In case of error or success, the message to send to the client.
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// The script that is currently being executed.
        /// </summary>
        public string ActualScript { get; set; } = "";

        /// <summary>
        /// The data system (DbAdminModel) that is currently running the maintenance job.
        /// </summary>
        public string ActualModel { get; set; } = "";

        /// <summary>
        /// The status of the current maintenance job. Is inferred from the RdxProgressStatus enumeration.
        /// </summary>
        public string Status { get; set; } = RdxProgressStatus.NOT_STARTED.ToString();
    }
}
