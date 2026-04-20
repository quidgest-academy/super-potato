using System;

namespace CSGenio.framework
{
    public class History
    {
        private string tabelaCriaHist;	// name da table de histórico
        private string[] camposCriaHist;	// lista de fields em histórico

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="tabelaCriaHist">name da table de histórico</param>
        /// <param name="camposCriaHist">lista de fields em histórico</param>
        public History(string tabelaCriaHist, string[] camposCriaHist)
        {
            this.tabelaCriaHist = tabelaCriaHist;
            this.camposCriaHist = camposCriaHist;
        }

        /// <summary>
        /// Name do alias da table dos fields que guardam o histórico
        /// </summary>
        public string CreateHistTables
        {
            get { return tabelaCriaHist; }
            set { tabelaCriaHist = value; }
        }

        /// <summary>
        /// Name dos fields que guardam o histórico
        /// </summary>
        public string[] CreateHistFields
        {
            get { return camposCriaHist; }
            set { camposCriaHist = value; }
        }
    }
}
