using System;
using System.Data;
using System.Collections.Generic;
using CSGenio.framework;

namespace CSGenio.persistence
{
    /// <summary>
    /// Classe que guarda o Qresult de uma query ad-hoc.
    /// Permite o acesso directo a um Qvalue ou a Qvalues pré-convertidos to os tipos internos do servidor
    /// </summary>
    /// <remarks>
    /// TODO: Criar um wrapper à volta de cada row e retornar um enumerador de rows de forma a poder
    ///  ter a mesma funcionalidade com codigo da seguinte forma:
    /// foreach(DataMatrixRow row in matrix)
    ///     string x = row["descrica"];
    /// TODO: O DataSet é uma classe pesada que tem muita infraestrutura relacionada com manter uma cache de dados
    ///  que é desnecessária a maioria das vezes. Assim seria mais eficiente ler com um IDataReader todos os dados
    ///  to aqui em bruto to depois serem então disponibilizados pelo DataMatrix.
    /// </remarks>
    public class DataMatrix
    {
        private DataSet m_dataSet;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataSet">O DataSet que vamos abstrair</param>
        public DataMatrix(DataSet dataSet)
        {
            m_dataSet = dataSet;
        }

        /// <summary>
        /// Acesso directo ao DataSet subjacente
        /// </summary>
        public DataSet DbDataSet { get { return m_dataSet; } }

        /// <summary>
        /// Numero de linhas retornadas
        /// </summary>
        public int NumRows { get { return m_dataSet.Tables[0].Rows.Count; } }

        /// <summary>
        /// Numero de colunas retornadas
        /// </summary>
        public int NumCols { get { return m_dataSet.Tables[0].Columns.Count; } }

        /// <summary>
        /// Acesso directo a um Qvalue por indice
        /// </summary>
        /// <param name="row">A linha a aceder</param>
        /// <param name="col">A coluna a aceder</param>
        /// <returns>O Qvalue</returns>
        public object GetDirect(int row, int col)
        {
            return m_dataSet.Tables[0].Rows[row][col];
        }

        /// <summary>
        /// Acesso directo a um Qvalue por name
        /// </summary>
        /// <param name="row">A linha a aceder</param>
        /// <param name="col">O name da coluna a aceder</param>
        /// <returns>O Qvalue</returns>
        public object GetDirect(int row, string col)
        {
            return m_dataSet.Tables[0].Rows[row][col];
        }

        /// <summary>
        /// Acesso a um Qvalue convertido to interno por indice
        /// </summary>
        /// <param name="row">A linha a aceder</param>
        /// <param name="col">A coluna a aceder</param>
        /// <returns>O Qvalue interno</returns>
        public string GetString(int row, int col)
        {
            return DBConversion.ToString(m_dataSet.Tables[0].Rows[row][col]);
        }

        /// <summary>
        /// Acesso a um Qvalue convertido to interno por name
        /// </summary>
        /// <param name="row">A linha a aceder</param>
        /// <param name="col">O name da coluna a aceder</param>
        /// <returns>O Qvalue interno</returns>
        public string GetString(int row, string col)
        {
            return DBConversion.ToString(m_dataSet.Tables[0].Rows[row][col]);
        }

        /// <summary>
        /// Acesso a um Qvalue convertido to interno por indice
        /// </summary>
        /// <param name="row">A linha a aceder</param>
        /// <param name="col">A coluna a aceder</param>
        /// <returns>O Qvalue interno</returns>
        public decimal GetNumeric(int row, int col)
        {
            return DBConversion.ToNumeric(m_dataSet.Tables[0].Rows[row][col]);
        }

        /// <summary>
        /// Acesso a um Qvalue convertido to interno por name
        /// </summary>
        /// <param name="row">A linha a aceder</param>
        /// <param name="col">O name da coluna a aceder</param>
        /// <returns>O Qvalue interno</returns>
        public decimal GetNumeric(int row, string col)
        {
            return DBConversion.ToNumeric(m_dataSet.Tables[0].Rows[row][col]);
        }

		/// <summary>
		/// Acesso a um Qvalue convertido to interno por indice
		/// </summary>
		/// <param name="row">A linha a aceder</param>
		/// <param name="col">A coluna a aceder</param>
		/// <returns>O Qvalue interno</returns>
		public int GetInteger(int row, int col)
		{
			return DBConversion.ToInteger(m_dataSet.Tables[0].Rows[row][col]);
		}

		/// <summary>
		/// Acesso a um Qvalue convertido to interno por name
		/// </summary>
		/// <param name="row">A linha a aceder</param>
		/// <param name="col">O name da coluna a aceder</param>
		/// <returns>O Qvalue interno</returns>
		public int GetInteger(int row, string col)
		{
			return DBConversion.ToInteger(m_dataSet.Tables[0].Rows[row][col]);
		}

        /// <summary>
        /// Acesso a um Qvalue convertido to interno por indice
        /// </summary>
        /// <param name="row">A linha a aceder</param>
        /// <param name="col">A coluna a aceder</param>
        /// <returns>O Qvalue interno</returns>
        public int GetLogic(int row, int col)
        {
            return DBConversion.ToLogic(m_dataSet.Tables[0].Rows[row][col]);
        }

        /// <summary>
        /// Acesso a um Qvalue convertido to interno por name
        /// </summary>
        /// <param name="row">A linha a aceder</param>
        /// <param name="col">O name da coluna a aceder</param>
        /// <returns>O Qvalue interno</returns>
        public int GetLogic(int row, string col)
        {
            return DBConversion.ToLogic(m_dataSet.Tables[0].Rows[row][col]);
        }

        /// <summary>
        /// Acesso a um Qvalue convertido to interno por indice
        /// </summary>
        /// <param name="row">A linha a aceder</param>
        /// <param name="col">A coluna a aceder</param>
        /// <returns>O Qvalue interno</returns>
        public DateTime GetDate(int row, int col)
        {
            return DBConversion.ToDateTime(m_dataSet.Tables[0].Rows[row][col]);
        }

        /// <summary>
        /// Acesso a um Qvalue convertido to interno por name
        /// </summary>
        /// <param name="row">A linha a aceder</param>
        /// <param name="col">O name da coluna a aceder</param>
        /// <returns>O Qvalue interno</returns>
        public DateTime GetDate(int row, string col)
        {
            return DBConversion.ToDateTime(m_dataSet.Tables[0].Rows[row][col]);
        }

        /// <summary>
        /// Acesso a um Qvalue convertido to interno por indice
        /// </summary>
        /// <param name="row">A linha a aceder</param>
        /// <param name="col">A coluna a aceder</param>
        /// <returns>O Qvalue interno</returns>
        public string GetKey(int row, int col)
        {
            return DBConversion.ToKey(m_dataSet.Tables[0].Rows[row][col]);
        }

        /// <summary>
        /// Acesso a um Qvalue convertido to interno por name
        /// </summary>
        /// <param name="row">A linha a aceder</param>
        /// <param name="col">O name da coluna a aceder</param>
        /// <returns>O Qvalue interno</returns>
        public string GetKey(int row, string col)
        {
            return DBConversion.ToKey(m_dataSet.Tables[0].Rows[row][col]);
        }

        /// <summary>
        /// Acesso a um Qvalue convertido to interno por indice
        /// </summary>
        /// <param name="row">A linha a aceder</param>
        /// <param name="col">A coluna a aceder</param>
        /// <returns>O Qvalue interno</returns>
        public byte[] GetBinary(int row, int col)
        {
            return DBConversion.ToBinary(m_dataSet.Tables[0].Rows[row][col]);
        }

        /// <summary>
        /// Acesso a um Qvalue convertido to interno por name
        /// </summary>
        /// <param name="row">A linha a aceder</param>
        /// <param name="col">O name da coluna a aceder</param>
        /// <returns>O Qvalue interno</returns>
        public byte[] GetBinary(int row, string col)
        {
            return DBConversion.ToBinary(m_dataSet.Tables[0].Rows[row][col]);
        }

        /// <summary>
        /// Gets the datamatrix in form of a list
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>The list of objects A</returns>
        public List<A> GetList<A>(User user) where A : IArea
        {
            int numRows = this.NumRows;
            int numCols = this.NumCols;

            List<A> Qresult = new List<A>(numRows);
            Type type = typeof(A);

            for (int i = 0; i < numRows; i++)
            {
                A area = (A)Activator.CreateInstance(type, user);
                for (int j = 0; j < numCols; j++)
                {
                    area.insertNameValueField(this.DbDataSet.Tables[0].Columns[j].ColumnName, this.GetDirect(i, j));
                }
                Qresult.Add(area);
            }

            return Qresult;
        }

        /// <summary>
        /// Gets the datamatrix in form of a list.
        /// Function that returns the new instance of the object.
        /// It is more efficient than Activator or Reflection.
        /// </summary>
        /// <param name="constructor">Function to get a new instance</param>
        /// <returns>The list of objects A</returns>
        public List<A> GetList<A>(Func<A> constructor) where A : IArea
        {
            int numRows = this.NumRows;
            int numCols = this.NumCols;

            var Qresult = new List<A>(numRows);
            for (int i = 0; i < numRows; i++)
            {
                A area = constructor();
                for (int j = 0; j < numCols; j++)
                {
                    area.insertNameValueField(this.DbDataSet.Tables[0].Columns[j].ColumnName, this.GetDirect(i, j));
                }
                Qresult.Add(area);
            }

            return Qresult;
        }

    }
}
