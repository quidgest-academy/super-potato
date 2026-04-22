using CSGenio.framework;
using CSGenio.persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;

namespace CSGenio.business
{
	public abstract class ManualQuery
	{
		protected String m_query;
		protected String m_id;
		protected IDictionary<String, ParameterQuery> m_parameters;
		protected String m_tiporesultado;
		protected String m_separadorcol;
		protected String m_separadorrow;
        protected String m_ignorarresultadosvazios;

	    public String Id
	    {
	        get { return m_id; }
		}
        public String Query
        {
            get { return m_query; }
            set { m_query = value; }
        }

		public String TipoResultado
        {
            get { return m_tiporesultado; }
            set { m_tiporesultado = value; }
        }

        public String SeparadorColuna
        {
            get { return m_separadorcol; }
            set { m_separadorcol = value; }
        }

        public String SeparadorLinha
        {
            get { return m_separadorrow; }
            set { m_separadorrow = value; }
        }

        public String IgnorarResultadosVazios
        {
            get { return m_ignorarresultadosvazios; }
            set { m_ignorarresultadosvazios = value; }
        }

   		abstract public DataMatrix Run(PersistentSupport sp);
        abstract public DataMatrix Run(IDictionary<String, ParameterQuery> parameters, PersistentSupport sp);

        protected virtual DataMatrix ExecuteQuery(PersistentSupport sp) {
            return sp.executeQuery(m_query, m_parameters);
        }

        public void setParams(Hashtable dados)
        {
            foreach (String id in m_parameters.Keys)
            { 
                ParameterQuery param = m_parameters[id];
                String dado = param.TabelaBase;
                
                if(dados.ContainsKey(dado))
				{
                    if (!String.IsNullOrEmpty(dados[dado].ToString()))
                    {
                        param.Value = dados[dado];
                        continue;
                    }
				}
                    
                dado = param.TabelaBase + "." + param.Field;

                if (dados.ContainsKey(dado))
				{
                    if (!String.IsNullOrEmpty(dados[dado].ToString()))
                    {
                        param.Value = dados[dado];
                        continue;
                    }
				}

                dado = param.Field;

                if (dados.ContainsKey(dado))
				{
                    if (!String.IsNullOrEmpty(dados[dado].ToString()))
                    {
                        param.Value = dados[dado];
                        continue;
                    }
				}
            }
        }
	}

    public class ParameterQuery
    {
        protected String m_id;
        protected String m_tabelabase;
        protected String m_campo;
        protected object m_valor;

        public ParameterQuery(String id)
        {
        	this.m_id = id;
        }

        public String Id
        {
            get { return m_id; }
        }

        public String TabelaBase
        {
            get { return m_tabelabase; }
            set { m_tabelabase = value; }
        }

        public String Field
        {
            get { return m_campo; }
            set { m_campo = value; }
        }

        public object Value
        {
            get { return m_valor; }
            set { m_valor = value; }
        }    
    }

	class Q_view_stats : ManualQuery
	{

		public Q_view_stats()
		{
			this.m_parameters = new Dictionary<String, ParameterQuery>();
			this.m_id = "#_QDG_VIEW_STATS____QDG_#";
			this.m_query = @"with view_stats (COUNTRY, NOTSOLD, PROFIT)
AS (
	SELECT c.COUNTRY, COUNT(CASE WHEN p.SOLD = 0 THEN 1 END) as NOTSOLD, SUM(p.PROFIT)
	FROM FORCOUNT c
	LEFT JOIN FORCITY ct ON c.CODCOUNT = ct.CODCOUNT
	LEFT JOIN FORPROPERTY p ON p.CODCITY = ct.CODCITY
	group by c.COUNTRY)

SELECT NEWID() CODSTATS, COUNTRY, NOTSOLD, PROFIT, 0 ZZSTATE

FROM view_stats
";
		}


		private Q_view_stats SetParams()
		{
			return this;
		}

		public override DataMatrix Run(PersistentSupport sp)
		{
			return this.SetParams().ExecuteQuery(sp);
		}

		public override DataMatrix Run(IDictionary<String, ParameterQuery> parameters,PersistentSupport sp)
		{
			return new Q_view_stats().SetParams().ExecuteQuery(sp);
		}
	}
}
