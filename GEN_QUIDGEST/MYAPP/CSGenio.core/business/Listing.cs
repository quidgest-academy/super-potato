using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;
using System.Reflection;
using System.Linq;

namespace CSGenio.business
{
    /// <summary>
    /// Esta classe serve to as funções GET, GET+, GET-, GETPOS e GETNIVELTREE.
    /// O atributo :
    /// - fields representa o name dos fields que o user deseja ver
    /// - fieldsvalues representa os Qvalues retornados. Cada entrada é identificada
    /// por um inteiro que corresponde a uma linha de resposta. Isto é se eu quiser ver 50
    /// registos, a hashtable terá 50 registos (ou menos, depende do que existir na BD)
    /// - posicoes é um array de inteiros com 2 posições que tem a posição do name do codigo
    /// interno e o name do Qfield de ordenação no array fields. Serve to construir a clausula
    /// ORDER BY das queries.
    /// - estadoLista serve to introduzir o status do pedido, se correu td bem(OK), se houve
    /// algum erro(E), etc.
    /// - mensagemEstado serve to introuzir a mensagem que vou enviar ao lado cliente.
    /// Por exemplo - Alteração Efectuada ou Erro na abertura da BD, etc.
    /// </summary>
    public class Listing
    {
        /// <summary>
        /// sorting
        /// </summary>
        private IList<ColumnSort> ordenacaoQuery;

        /// <summary>
        /// condição decorrente do uso de EPHs
        /// </summary>
        private CriteriaSet condicoesEphQuery;
        /// <summary>
        /// user em sessão
        /// </summary>
        private User user;
        /// <summary>
        /// dados lidos da BD
        /// </summary>
        private DataSet matrizDados;

        private PersistentSupport sp;
        /// <summary>
        /// Última linha preenchida
        /// </summary>
        private int ultPreenchida;

        /// <summary>
        /// indica se queremos saber o total de registos
        /// </summary>
        public bool obterTotal;
        /// <summary>
        /// total de registos da Qlisting
        /// </summary>
        public int TotalRecords;

        /// <summary>
        /// Lista de fields pedidos to esta Qlisting
        /// </summary>
        public string[] RequestedFields
        {
            get { return fieldsRequested; }
            set { fieldsRequested = value; }
        }
        private string[] fieldsRequested;

        /// <summary>
        /// Area base da Qlisting (null caso não aplicável)
        /// </summary>
        private Area area;

        /// <summary>
        /// Module
        /// </summary>
        protected string module;


        /// <summary>
        /// Construtor
        /// </summary>
        public Listing(Area area, IList<ColumnSort> sorting, string module, string identifier, User user, PersistentSupport sp)
        {
            this.user = user;
            this.module = module;
            this.ordenacaoQuery = sorting;
            bool addPrimaryKeySort = true;
            if (ordenacaoQuery.Count > 0 && ordenacaoQuery[0].Expression is ColumnReference)
            {
                ColumnReference cref = (ColumnReference)ordenacaoQuery[0].Expression;
                if (cref.TableAlias == area.Alias && cref.ColumnName == area.PrimaryKeyName)
                {
                    addPrimaryKeySort = false;
                }
            }

            if (addPrimaryKeySort)
            {
                this.ordenacaoQuery.Add(new ColumnSort(new ColumnReference(area.Alias, area.PrimaryKeyName), SortOrder.Ascending));
            }
            this.sp = sp;
            this.area = area;
            condicoesEphQuery = CalculateConditionsEphGeneric(area, identifier);

            NoLock = true;
        }

		/// <summary>
        /// Aplicar o comando NoLock nas queries
        /// </summary>
		public Boolean NoLock { get; set;}

        /// <summary>
        /// Calcula uma condição em SQL que vai aplicar a uma area os filtros indicados nos EPH
        /// </summary>
        /// <param name="area">A area base</param>
        /// <param name="identificador">O identifier to aplicar overrides de eph</param>
        /// <returns>A condição de SQL</returns>
        public static CriteriaSet CalculateConditionsEphGeneric(Area area, string identifier)
        {
            CriteriaSet res = CriteriaSet.And();

            List<EPHOfArea> ephsDaArea = area.CalculateAreaEphs(area.User.Ephs, identifier, false);

            foreach (EPHOfArea eph in ephsDaArea)
            {
                if (eph.Relation2 != null)
                {
                    CriteriaSet inner_cs = new CriteriaSet(CriteriaSetOperator.And);
                    if(eph.Eph.OR_EPH1_EPH2)
                        inner_cs = new CriteriaSet(CriteriaSetOperator.Or);

                    // First relation can be empty
                    if (eph.Relation != null)
                        AuxAdicionaCondicaoOutraArea(inner_cs, area, eph.Eph, eph.ValuesList, eph.Relation);

                    // In order to reuse code, we create a second EPH field from data in area's EPH
                    EPHField EPH2 = new EPHField(eph.Eph.Name, eph.Eph.Table2, eph.Eph.Field2, eph.Eph.Operator2, eph.Eph.Propagate);
                    AuxAdicionaCondicaoOutraArea(inner_cs, area, EPH2, eph.ValuesList, eph.Relation2);
                    res.SubSet(inner_cs);
                }
                else if (eph.Relation != null)
                {
                    AuxAdicionaCondicaoOutraArea(res, area, eph.Eph, eph.ValuesList, eph.Relation);
                }
                else
                {
                    AuxAdicionaCondicaoMesmaArea(res, area, eph.Eph, eph.ValuesList);
                }
            }

            return res;
        }

		private static void AuxAdicionaCondicaoOutraArea(CriteriaSet cs, Area area, EPHField ephArea, object[] listaValores, Relation myrelacao)
        {
            AreaInfo tabelaEPH = Area.GetInfoArea(ephArea.Table);
            Field campoEPH = tabelaEPH.DBFields[ephArea.Field];

            string arorigem = myrelacao.AliasSourceTab;
            string crorigem = myrelacao.SourceRelField;
            string fieldKey = myrelacao.SourceRelField;

            // EPH is set on a primary-key field
            if (ephArea.Field.ToLower().Equals(myrelacao.TargetRelField))
            {
                object[] safeValues = new object[listaValores.Length];
                for (int i = 0; i < listaValores.Length; i++)
                {
                    safeValues[i] = QueryUtils.ToValidDbValue(listaValores[i], campoEPH);
                }

                if (ephArea.Operator == "EN")
                {
                    CriteriaSet mda = new CriteriaSet(CriteriaSetOperator.Or);

                    if (campoEPH.isKey())
                    {
                        mda.Equal(new ColumnReference(arorigem, crorigem), null);
                    }
                    else
                    {
                        string funcaoSQL = campoEPH.FieldType.GetEPHFunction();
                        mda.Equal(SqlFunctions.Custom(funcaoSQL, new ColumnReference(arorigem, crorigem)), 1);
                    }
                    mda.In(arorigem, crorigem, safeValues);
                    cs.SubSet(mda);
                }
                else
                    cs.In(arorigem, crorigem, safeValues);
            }
            // EPH is set on value field (non primary-key)
            else
            {
                if (ephArea.Propagate)
                {
                    // Assume the path between current area and the EPH's area exists
                    arorigem = ephArea.Table;
                    crorigem = ephArea.Field;
                    fieldKey = tabelaEPH.PrimaryKeyName;
                }

                if (ephArea.Operator == "EN")
                {
                    object[] safeValues = new object[listaValores.Length];
                    for (int i = 0; i < listaValores.Length; i++)
                    {
                        safeValues[i] = QueryUtils.ToValidDbValue(listaValores[i], campoEPH);
                    }

                    CriteriaSet mda = new CriteriaSet(CriteriaSetOperator.Or);

                    if (campoEPH.isKey())
                    {
                        mda.Equal(new ColumnReference(arorigem, crorigem), null);
                    }
                    else
                    {
                        crorigem = campoEPH.Name;
                        string funcaoSQL = campoEPH.FieldType.GetEPHFunction();
                        
						mda.Equal(SqlFunctions.Custom(funcaoSQL, new ColumnReference(myrelacao.AliasTargetTab, crorigem)), 1);
                    }
                    
                    mda.In(myrelacao.AliasTargetTab, crorigem, safeValues);
					cs.SubSet(mda);
                }
                else
                {
                    //condições de OR (Valores ou EMPTY)
                    CriteriaSet mda = new CriteriaSet(CriteriaSetOperator.Or);

                    SelectQuery innerQuery = new SelectQuery()
                        .Select(tabelaEPH.TableName, tabelaEPH.PrimaryKeyName)
                        .From(tabelaEPH.QSystem, tabelaEPH.TableName, tabelaEPH.TableName);

                    CriteriaSet innerConditions = CriteriaSet.Or();

                    // Tree structure EPH
                    if (ephArea.Operator == "L" || ephArea.Operator == "LN")
                    {
                        for (int i = 0; i < listaValores.Length; i++)
                        {
                            // Use the EPH field as a prefix
                            innerConditions.Like(tabelaEPH.TableName, ephArea.Field,
                                listaValores[i].ToString() + "%");
                        }
                        // MH - Eph em árvore ou NULL
                        if (ephArea.Operator == "LN")
                        {
                            if (campoEPH.isKey())
                            {
                                mda.Equal(new ColumnReference(arorigem, crorigem), null);
                            }
                            else
                            {
                                string funcaoSQL = campoEPH.FieldType.GetEPHFunction();
                                mda.Equal(SqlFunctions.Custom(funcaoSQL, new ColumnReference(arorigem, crorigem)), 1);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < listaValores.Length; i++)
                        {
                            innerConditions.Criterias.Add(new Criteria(
                                new ColumnReference(tabelaEPH.TableName, ephArea.Field),
                                QueryUtils.ParseEphOperator(ephArea.Operator),
                                listaValores[i]));
                        }
                    }

                    innerQuery.Where(innerConditions);
                    // The inner query fetches the PKey, not the value field
                    mda.In(arorigem, fieldKey, innerQuery);
                    cs.SubSet(mda);
                }
            }
        }

        private static void AuxAdicionaCondicaoMesmaArea(CriteriaSet cs, Area area, EPHField ephArea, object[] listaValores)
        {
            if (ephArea.Operator == "=" || ephArea.Operator == "EN" )
            {
                //se o operador for "=" podemos usar o IN
                object[] safeValues = new object[listaValores.Length];
                for (int i = 0; i < listaValores.Length; i++)
                {
                    safeValues[i] = QueryUtils.ToValidDbValue(listaValores[i], area.DBFields[ephArea.Field]);
                }
                if (ephArea.Operator == "=")
                    cs.In(area.Alias, ephArea.Field, safeValues);
                else
                {
                    CriteriaSet mda = new CriteriaSet(CriteriaSetOperator.Or);

                    Field campoEPH = area.DBFields[ephArea.Field];
                    if (campoEPH.isKey())
                    {
                        mda.Equal(new ColumnReference(area.Alias, ephArea.Field), null);
                    }
                    else
                    {
                        string funcaoSQL = campoEPH.FieldType.GetEPHFunction();
                        mda.Equal(SqlFunctions.Custom(funcaoSQL, new ColumnReference(area.Alias, ephArea.Field)), 1);
                    }
                    mda.In(area.Alias, ephArea.Field, safeValues);
                    cs.SubSet(mda);
                }
            }
            else
            {
                CriteriaSet innerConditions = CriteriaSet.Or();

                if (ephArea.Operator == "L" || ephArea.Operator == "LN")
                {
                    // Use the EPH field as a prefix
                    for (int i = 0; i < listaValores.Length; i++)
                    {
                        innerConditions.Like(area.Alias, ephArea.Field,
                            listaValores[i].ToString() + "%");
                    }
                    // MH - Eph em árvore ou NULL
                    if(ephArea.Operator == "LN")
                    {
                        Field campoEPH = area.DBFields[ephArea.Field];
                        if (campoEPH.isKey())
                        {
                            innerConditions.Equal(new ColumnReference(area.Alias, ephArea.Field), null);
                        }
                        else
                        {
                            string funcaoSQL = campoEPH.FieldType.GetEPHFunction();
                            innerConditions.Equal(SqlFunctions.Custom(funcaoSQL, new ColumnReference(area.Alias, ephArea.Field)), 1);
                        }
                    }
                }
				else if (ephArea.Operator == "EN")
                {
                    object[] safeValues = new object[listaValores.Length];
                    for (int i = 0; i < listaValores.Length; i++)
                    {
                        safeValues[i] = QueryUtils.ToValidDbValue(listaValores[i], area.DBFields[ephArea.Field]);
                    }
                    Field campoEPH = area.DBFields[ephArea.Field];
                    if (campoEPH.isKey())
                    {
                        innerConditions.Equal(new ColumnReference(area.Alias, ephArea.Field), null);
                    }
                    else
                    {
                        string funcaoSQL = campoEPH.FieldType.GetEPHFunction();
                        innerConditions.Equal(SqlFunctions.Custom(funcaoSQL, new ColumnReference(area.Alias, ephArea.Field)), 1);
                    }
                    innerConditions.In(area.Alias, ephArea.Field, safeValues);
                }
                else
                {
                    for (int i = 0; i < listaValores.Length; i++)
                    {
                        innerConditions.Criterias.Add(new Criteria(
                            new ColumnReference(area.Alias, ephArea.Field),
                            QueryUtils.ParseEphOperator(ephArea.Operator),
                            listaValores[i]));
                    }
                }

                cs.SubSet(innerConditions);
            }
        }

        /// <summary>
        /// Método que preenche o name das colunas da matriz
        /// </summary>
        /// <param name="nomesColunas">string com o name das colunas</param>
        /// <returns>Devolve uma arraylist com os pares (Qfield,area)</returns>
        public Dictionary<string, Area> preencheColunasMatriz_Areas(string columnNames, string module)
        {
            Dictionary<string, Area> areas = new Dictionary<string, Area>();
            Area area = null;
            string[] arrayCampos = columnNames.Split(',');
            for (int i = 0; i < arrayCampos.Length; i++)
            {
                string[] tableField = arrayCampos[i].Split('.');
                if (tableField.Length == 2)
                {

                    if (!areas.ContainsKey(tableField[0]))//se a área ainda não foi adicionada
                    {
                        //SO 2007.05.29
                        area = Area.createArea(tableField[0], User, module);
                        areas.Add(tableField[0], area);//adicionar a área
                    }
                    else
                        area = (Area)areas[tableField[0]];
                }
            }
            return areas;
        }

        /// <summary>
        /// Função to select vários registos da BD -GET
        /// </summary>
        /// <param name="identificador"></param>
        /// <param name="condicao"></param>
        /// <returns>uma listagens com os fields</returns>
        public Listing select(string identifier, CriteriaSet condition, int nrRecords)
        {
            try
            {
                Listing Qlisting;
                Type funcObj = typeof(GenioServer.framework.OverrideQuery);
                //public static Listing OVERRIDE_IDENTIFICADOR(Listing Qlisting, CriteriaSet condition, User user, int nrRecords, PersistentSupport sp)
                MethodInfo funcOver = funcObj.GetMethod(identifier);
                if (funcOver != null)
                {
                    object[] parameters = new object[5];
                    parameters[0] = condition;//CriteriaSet
                    parameters[1] = user;//User
                    parameters[2] = sp;//PersistentSupport
                    parameters[3] = nrRecords;//int
                    parameters[4] = this;//Listing

                    Qlisting = (Listing)funcOver.Invoke(this, parameters);
                }
                else
                {
                    if (area == null || area.Information.PersistenceType == PersistenceType.Database || area.Information.PersistenceType == PersistenceType.View)
                    {
                        Qlisting = sp.select(identifier, this, condition, nrRecords, NoLock);
                    }
                    else //if (area.Information.PersistenceType == PersistenceType.Codebase)
                    {
                        //invocar o metodo estatico searchList por reflection
                        IDictionary<string, PersistentSupport.ControlQueryDefinition> controlos = PersistentSupport.getControlQueries();
                        PersistentSupport.ControlQueryDefinition aux = controlos[identifier];
                        condition.SubSet(aux.WhereConditions);
                        IList res = InvokeSearchList(condition);
                        if (res == null)
                            return null;

                        Qlisting = AreaList2Listagem(res);
                    }
                }
                return Qlisting;
            }
            catch (PersistenceException ex)
            {
                throw new BusinessException(ex.UserMessage, "Listing.seleccionar", "Error selecting database records: " + ex.Message, ex);
            }
            catch (TargetInvocationException ex)
            {
                throw new BusinessException(null, "Listing.seleccionar", "Error selecting database records: " + ex.Message, ex);
                /*if (ex.InnerException is FrameworkException)
                    throw (FrameworkException)ex.InnerException;
                else if (ex.InnerException is BusinessException)
                    throw (BusinessException)ex.InnerException;
                else if (ex.InnerException is PersistenceException)
                    throw (PersistenceException)ex.InnerException;
                else
                    throw ex.InnerException;*/
            }
        }

		public Listing anotherSelect(string identifier, string[] fieldsRequested, IList<ColumnSort> sorts, CriteriaSet condition, int nrRecords,  int offset)
        {
            try
            {
                Listing Qlisting;
                Type funcObj = typeof(GenioServer.framework.OverrideQuery);
                //public static Listing OVERRIDE_IDENTIFICADOR(Listing Qlisting, CriteriaSet condition, User user, int nrRecords, PersistentSupport sp)
                MethodInfo funcOver = funcObj.GetMethod(identifier);
                if (funcOver != null)
                {
                    object[] parameters = new object[5];
                    parameters[0] = condition;//CriteriaSet
                    parameters[1] = user;//User
                    parameters[2] = sp;//PersistentSupport
                    parameters[3] = nrRecords;//int
                    parameters[4] = this;//Listing

                    Qlisting = (Listing)funcOver.Invoke(this, parameters);
                }
                else
                {
                    if (area == null || area.Information.PersistenceType == PersistenceType.Database || area.Information.PersistenceType == PersistenceType.View)
                    {
                        sp.PrepareQuerySelect(identifier, fieldsRequested, sorts, false, nrRecords, offset, area);

                        Qlisting = sp.anotherSelect(identifier, this, condition, nrRecords, offset, area);
                    }
                    else //if (area.Information.PersistenceType == PersistenceType.Codebase)
                    {
                        //invocar o metodo estatico searchList por reflection
                        IDictionary<string, PersistentSupport.ControlQueryDefinition> controlos = PersistentSupport.getControlQueries();
                        PersistentSupport.ControlQueryDefinition aux = controlos[identifier];
                        condition.SubSet(aux.WhereConditions);
                        IList res = InvokeSearchList(condition);
                        if (res == null)
                            return null;

                        Qlisting = AreaList2Listagem(res);
                    }
                }
                return Qlisting;
            }
            catch (PersistenceException ep)
            {
                throw new BusinessException(null, "Listing.anotherSelect", "Error selecting database records: " + ep.Message, ep);
            }
            catch (TargetInvocationException ex)
            {
                throw new BusinessException(null, "Listing.anotherSelect", "Error selecting database records: " + ex.Message, ex);
                /*if (ex.InnerException is FrameworkException)
                    throw (FrameworkException)ex.InnerException;
                else if (ex.InnerException is BusinessException)
                    throw (BusinessException)ex.InnerException;
                else if (ex.InnerException is PersistenceException)
                    throw (PersistenceException)ex.InnerException;
                else
                    throw ex.InnerException;*/
            }
        }

        private IList InvokeSearchList(CriteriaSet condition)
        {
            Type areaType = area.GetType();
            MethodInfo listMethod = areaType.GetMethod("searchList",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { typeof(PersistentSupport), typeof(User), typeof(CriteriaSet), typeof(string[]), typeof(bool), typeof(bool) },
                null
                );
            IList res = listMethod.Invoke(null, new object[] {
                            sp,
                            user,
                            condition,
                            fieldsRequested, false, false
                        }) as IList;
            return res;
        }

        private Listing AreaList2Listagem(IList res)
        {
            Listing Qlisting;
            DataTable dt = new DataTable();
            foreach (string campopedido in fieldsRequested)
            {
                string[] tabelacampo = campopedido.Split('.');
                string table = tabelacampo[0];
                string Qfield = tabelacampo[1];

                var cp = Area.GetInfoArea(table).DBFields[Qfield];
                dt.Columns.Add(campopedido, cp.FieldType.GetExternalType());
            }

            foreach (IArea row in res)
            {
                DataRow dr = dt.NewRow();
                for (int col = 0; col < fieldsRequested.Length; col++)
                {
                    var cp = row.Fields[fieldsRequested[col]];
                    dr[col] = cp.Value;
                }
                dt.Rows.Add(dr);
            }

            //ordernar o dataset
            if (ordenacaoQuery.Count > 0)
            {
                DataView dv = new DataView(dt);
                StringBuilder sort = new StringBuilder(dv.Sort);

                foreach (ColumnSort sortCol in QuerySort)
                {
                    string colname = "";
                    if (sortCol.ColumnIndex != null)
                    {
                        int index = (int)sortCol.ColumnIndex - 1;
                        colname = dt.Columns[index].ColumnName;
                    }
                    else if (sortCol.Expression is ColumnReference)
                    {
                        ColumnReference col = (ColumnReference)sortCol.Expression;
                        colname = col.TableAlias + "." + col.ColumnName;
                    }
                    else
                        continue;

                    if (!string.IsNullOrEmpty(sort.ToString()))
                        sort.Append(", ");
                    sort.AppendFormat("{0} {1}", colname, sortCol.Order == SortOrder.Descending ? " DESC" : " ASC");
                }
                dv.Sort = sort.ToString();
                dt = dv.ToTable();
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            DataMatrix = ds;
            LastFilled = dt.Rows.Count;

            if (obterTotal)
                TotalRecords = dt.Rows.Count;

            Qlisting = this;
            return Qlisting;
        }


        public Listing selectLevel(Area areaLevel, IList<SelectField> fieldsRequested, CriteriaSet condition, string parentKeyCond)
        {
            try
            {
                Listing Qlisting = sp.selectLevel(areaLevel, fieldsRequested, this, condition, parentKeyCond);
                return Qlisting;
            }
            catch (PersistenceException ex)
            {
                throw new BusinessException(ex.UserMessage, "Listing.seleccionarNivel", "Error selecting database records: " + ex.Message, ex);
            }
        }

        public Listing selectMore(string identifier, CriteriaSet condition, int nrRecords, int lastRead, string primaryKey)
        {
            try
            {
                Listing Qlisting;
                Type funcObj = typeof(GenioServer.framework.OverrideQuery);
                MethodInfo funcOver = funcObj.GetMethod(identifier + "_MAIS");
                if (funcOver != null)
                {
                    object[] parameters = new object[7];
                    parameters[0] = condition;//CriteriaSet
                    parameters[1] = user;//User
                    parameters[2] = sp;//PersistentSupport
                    parameters[3] = nrRecords;//int
                    parameters[4] = this;//Listing
                    parameters[5] = lastRead;
                    parameters[6] = primaryKey;

                    Qlisting = (Listing)funcOver.Invoke(this, parameters);
                }
                else
                {
                    if (area == null || area.Information.PersistenceType == PersistenceType.Database || area.Information.PersistenceType == PersistenceType.View)
                    {
                        Qlisting = sp.selectMore(identifier, this, condition, nrRecords, lastRead, primaryKey);
                    }
                    else //if (area.Information.PersistenceType == PersistenceType.Codebase)
                    {
                        //invocar o metodo estatico searchList por reflection
                        IDictionary<string, PersistentSupport.ControlQueryDefinition> controlos = PersistentSupport.getControlQueries();
                        PersistentSupport.ControlQueryDefinition aux = controlos[identifier];
                        condition.SubSet(aux.WhereConditions);
                        IList res = InvokeSearchList(condition);
                        if (res == null)
                            return null;

                        Qlisting = AreaList2Listagem(res);
                    }
                }
                return Qlisting;

            }
            catch (PersistenceException ex)
            {
                throw new BusinessException(ex.UserMessage, "Listing.seleccionarMais", "Error selecting database records: " + ex.Message, ex);
            }
        }


        public int getRecordPos(IArea area, string primaryKeyValue, CriteriaSet conditions, string identifier)
        {
            try
            {
                int posRegisto = sp.getRecordPos(User, Module, area, ordenacaoQuery, primaryKeyValue, conditions, identifier);
                return posRegisto;
            }
            catch (PersistenceException ex)
            {
                throw new BusinessException(ex.UserMessage, "Listing.getRecordPos", "Error getting previous record: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Devolve o elemento que está a servir de ordenação
        /// </summary>
        /// <param name="campoOrdenacao">Field de ordenação</param>
        /// <param name="nrLinha">Número da linha pretendida</param>
        /// <returns>Devolve o elemento ordenação da linha pretendida</returns>
        public object returnElementOrder(string sortingField, int nrLine)
        {
            //verificar se a coluna exists
            if (matrizDados.Tables[0].Columns[sortingField] != null)
            {
                return matrizDados.Tables[0].Rows[nrLine][sortingField];
            }
            else//significa que nao exists coluna de ordenação
                return null;
        }

        public IList<ColumnSort> QuerySort
        {
            get { return ordenacaoQuery; }
            set { ordenacaoQuery = value; }
        }


        public User User
        {
            get { return user; }

        }


        public CriteriaSet EphQueryConditions
        {
            get { return condicoesEphQuery; }
            set { condicoesEphQuery = value; }
        }

        public DataSet DataMatrix
        {
            get { return matrizDados; }
            set { matrizDados = value; }
        }

        /// <summary>
        /// Método que devolve ou coloca o número da última linha preenchida
        /// </summary>
        public int LastFilled
        {
            get { return ultPreenchida; }
            set { ultPreenchida = value; }
        }

        /// <summary>
        /// Método que devolve o module
        /// </summary>
        public string Module
        {
            get { return module; }
        }

//---------------------------------------
// WS LEGACY CODE PLACEHOLDER
// (do not delete or change this comment)
//---------------------------------------

    }

    public class ListingMVC<A>
    {
        private const int DefaultNumRegs = 10;

        private List<A> elements;

        private int numRegs;

        private int offset;

        private FieldRef[] fields;

        private bool distinct;

        private bool noLock;

        private User user;

        private string rowselect;

		private CriteriaSet pagingposEPHs;

        private int currentpage;

        public string identifier;

        public int totalRegistos;

        public IList<TableJoin> Joins { get; set; }

        public User User
        {
            get { return user; }
        }

        public bool Distinct
        {
            get { return distinct; }
        }

        public bool NoLock
        {
            get { return noLock; }
            set { noLock = value; }
        }

        public FieldRef[] RequestFields
        {
            get { return fields; }
        }

        public string[] RequestFieldsAsStringArray
        {
            get {
                string[] array = new string[fields.Length];
                int index = 0;
                foreach(FieldRef f in fields)
                    array[index++] = f.FullName;
                return array;
            }
        }

        private IList<ColumnSort> sorts;

        public IList<ColumnSort> Sorts
        {
            get { return sorts; }
        }

        public CriteriaSet PagingPosEPHs
        {
            get { return pagingposEPHs; }
            set { pagingposEPHs = value; }
        }

        public ListingMVC(FieldRef[] fields, IList<ColumnSort> sorts, int offset, int numRegs, bool distinct, User user, bool noLock, string identifier = "", bool getTotal = false, string selectrow = "", CriteriaSet pagingPosEPHs = null, List<FieldRef> fieldsWithTotalizer = null, List<string> selectedRecords = null)
        {
            this.fields = fields;
            this.sorts = sorts;
            this.offset = offset;
            if (numRegs == 0)
                this.numRegs = DefaultNumRegs;
            else
                this.numRegs = numRegs;
            this.distinct = distinct;
            this.user = user;
            this.noLock = noLock;
            this.identifier = identifier;
            this.getTotal = getTotal;
            this.FieldsWithTotalizer = fieldsWithTotalizer ?? new List<FieldRef>();
            this.SelectedRecords = selectedRecords ?? new List<string>();
            this.rowselect = selectrow;
            this.pagingposEPHs = pagingPosEPHs;
        }

        public int Offset
        {
            get { return offset; }
            set { this.offset = value; }
        }

        public string RowSelect
        {
            get { return rowselect; }
            set { this.rowselect = value; }
        }

        public int CurrentPage
        {
            get { return currentpage; }
            set { this.currentpage = value; }
        }

        public int NumRegs
        {
            get { return numRegs; }
            set { this.numRegs = value; }
        }

        public List<A> Rows
        {
            get { return elements; }
            set { this.elements = value; }
        }

        protected bool getTotal;
        /// <summary>
        /// Indicates whether we want to know the total number of records
        /// </summary>
        public bool GetTotal
        {
            get { return getTotal; }
        }

        public int TotalRecords
        {
            get { return totalRegistos; }
            set { this.totalRegistos = value; }
        }

        /// <summary>
        /// Fields whose columns will have totalizers.
        /// </summary>
        public List<FieldRef> FieldsWithTotalizer { get; set; }

        /// <summary>
        /// The list of records that are currently selected - used in totalizers for multiple selection lists.
        /// </summary>
        public List<string> SelectedRecords { get; set; }

        /// <summary>
        /// The list of totalizers for the listing columns that have them enabled.
        /// </summary>
        public List<Totalizer> Totalizers { get; set; }

        /// <summary>
        /// Indicates if exist more pages
        /// One more record is always selected to check if exist more pages
        /// </summary>
        public bool HasMore {
            get {
				return (Rows != null && numRegs != -1) ? Rows.Count > NumRegs : false;
            }
        }

        /// <summary>
        /// Based on the buildQueryCount function (QueryUtils), sets the listing's totalizers and record count values.
        /// For this method to work, the DataMatrix must have a set shape:
        /// Row 0 - Cell 0 is the record count, the remaining are the Total values of the totalizers.
        /// Row 1, Cell 0 is discardable, the remaining are the Selected values of the totalizers.
        /// <param name="data"> The result of the buildQueryCount query.<param/>
        /// </summary>
        public void SetCountAndTotalizers(DataMatrix data)
        {

            List<Totalizer> totalizers = FieldsWithTotalizer
                .Select(field => new Totalizer(field.FullName, 0.0, 0.0))
                .ToList();

            if (data.NumRows == 0)
            {
                Totalizers = totalizers;
                return;
            }

            DataRow totalRow = data.DbDataSet.Tables[0].Select("RowId = 'Total'").FirstOrDefault();
            DataRow selectedRow = data.DbDataSet.Tables[0].Select("RowId = 'Selected'").FirstOrDefault();

            if (totalRow == null) 
            {
                Totalizers = totalizers;
                return;
            }

            foreach (Totalizer tot in totalizers)
            {
                if (totalRow[tot.Column] != DBNull.Value)
                    tot.Total = Convert.ToDouble(totalRow[tot.Column]); // Get total value from row with RowId "Total"
                if (selectedRow != null && selectedRow[tot.Column] != DBNull.Value)
                    tot.Selected = Convert.ToDouble(selectedRow[tot.Column]); // get selected value from row with RowId "Selected"
            }

            // The count and totalizer queries are merged into one, so we can set both the totalizers and the total records value
            Totalizers = totalizers;
            if (GetTotal)
                TotalRecords = DBConversion.ToInteger(totalRow["count"]);
        }

        public List<T> RowsForViewModel<T>() where T : new()
        {
            return RowsForViewModel<T>(false);
        }

        public List<T> RowsForViewModel<T> (bool fillAreasRel) where T: new() {
            return RowsForViewModel<T>(fillAreasRel, null);
        }

        public List<T> RowsForViewModel<T>(bool fillAreasRel, string[] _fieldsToSerialize) where T : new()
        {
            int count = (numRegs > 0 && elements.Count > numRegs) ? numRegs : elements.Count;

            List<T> list = new List<T>(count);
            Type type = typeof(T);

            T[] array = new T[count];
            for (int i = 0; i < count; i++)
            {
               var args = new object[] { elements[i] };
                if (fillAreasRel)
                    args = new object[] { elements[i], true };
                if (_fieldsToSerialize != null)
                    args = new object[] { elements[i], true, _fieldsToSerialize };
                array[i] = (T)Activator.CreateInstance(type, args);
            }
            list.AddRange(array);

            return list;
        }

        public List<T> RowsForViewModel<T>(Func<A, T> constructor)
        {
			int count = (elements != null) ? ((numRegs > 0 && elements.Count > numRegs) ? numRegs : elements.Count) : 0;
            List<T> list = new List<T>(count);

            for (int i = 0; i < count; i++)
                list.Add(constructor(elements[i]));

            return list;
        }
    }

    /// <summary>
    /// Class used to store aggregator values of specific listing columns.
    /// Must be outside of the ListingMVC class, to ensure it can be used in the table and menu list ViewModels.
    /// </summary>
    public class Totalizer
    {
        /// <summary>
        /// The column name, represented as "area.field".
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// The sum of all records of a given listing.
        /// </summary>
        public double Total { get; set; }

        /// <summary>
        /// The sum of all selected records of a given listing - only for multiple selection lists.
        /// </summary>
        public double Selected { get; set; }

        public Totalizer(string columnName, double totalValue, double selectedValue)
        {
            Column = columnName;
            Total = totalValue;
            Selected = selectedValue;
        }
    }
}
