using System;
using System.Collections;

/// <summary>
/// Classe que representa o level de uma arvore.
/// </summary>
namespace CSGenio.framework
{
    public class TreeLevel
    {
        private readonly int n_nodos_pais; //number de nodos pai. 
        private Hashtable nodosNivel;//nodos do level. 

        /// <summary>
        /// Constructor da classe
        /// </summary>
        /// <param name="n_nodos_pais">number de nodos pai</param>
        /// <param name="nivel">nodos do level</param>
        public TreeLevel(int n_nodos_pais, int level)
        {
            this.n_nodos_pais = n_nodos_pais;
            this.QLevel = level;
            this.nodosNivel = new Hashtable();
        }

        public int CountryNumber
        {
            get { return n_nodos_pais; }
        }

        public int TotalNodes
        {
            get { return nodosNivel.Count; }
        }
        // Nivel da arvore
        public int QLevel { get; set; }
        public Hashtable LevelNodes { get; }
        public void AddNode(Nodes node, int num)
        {
            nodosNivel.Add(num, node);
        }

        public Nodes SearchNode(int num)
        {
            return (Nodes)nodosNivel[num];
        }

        public void sortLevel()
        {
            int[] aFilhos = new int[this.TotalNodes];
            int[] aPosicao = new int[this.TotalNodes];

            int max = 0;
            for (int k = 0; k < this.TotalNodes; k++)
            {
                Nodes node = SearchNode(k);
                aFilhos[k] = node.TotalBranches;
                aPosicao[k] = k;
                if (aFilhos[k] > max)
                {
                    max = aFilhos[k];
                }
            }
            Array.Sort(aFilhos, aPosicao);
            if (aFilhos[aPosicao[0]] < max)
                Array.Reverse(aPosicao);

            Hashtable ordenado = new Hashtable(this.TotalNodes);

            for (int i = 0; i < this.TotalNodes; i++)
            {
                ordenado.Add(i, this.nodosNivel[aPosicao[i]]);
            }
            this.nodosNivel.Clear();
            this.nodosNivel = ordenado;

        }
        /// <summary>
        /// Implementa��o do m�todo equals to o tipo TreeLevel
        /// </summary>
        /// <param name="obj">objecto a comparar</param>
        /// <returns>true se forem iguais e false caso contr�rio</returns>
        public override bool Equals(Object obj)
        {
            if (obj is TreeLevel arg)
            {
                if (arg.QLevel == this.QLevel &&
                    arg.CountryNumber == this.CountryNumber &&
                    arg.LevelNodes.Equals(this.LevelNodes))
                    return true;
                else
                    return false;

            }
            else
                return true;
        }

        public override int GetHashCode()
        {
            string[] conbineHash = { QLevel.ToString(), CountryNumber.ToString() };
            return string.Join("", conbineHash).GetHashCode() + LevelNodes.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

        
}
