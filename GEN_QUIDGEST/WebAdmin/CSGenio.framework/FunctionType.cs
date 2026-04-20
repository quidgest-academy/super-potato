using System.Collections;

namespace CSGenio.framework
{
    /// <summary>
    /// Tipos possíveis de função
    /// </summary>
    public class FunctionType
    {
        public readonly static FunctionType GET = new FunctionType("GET");       // Seleccionar N
        public readonly static FunctionType GET_ALTERNATIVE = new FunctionType("GETN2");		// Seleccionar N alternative
        public readonly static FunctionType GET_NIVELTREE = new FunctionType("GETNIVELTREE");		// Seleccionar ramos duma árvore
        public readonly static FunctionType GET_MAIS = new FunctionType("GET+");	// Seleccionar os próximos N
        public readonly static FunctionType GET_MENOS = new FunctionType("GET-");	// Seleccionar os N anteriores
        public readonly static FunctionType GET_POS = new FunctionType("GETP");  // Seleccionar os próximos apartir da posição indicada
        public readonly static FunctionType GET_UM = new FunctionType("GET1");	// Seleccionar 1
        public readonly static FunctionType GET_UNICO = new FunctionType("GETU");    // Seleccionar quando é único
        public readonly static FunctionType INS = new FunctionType("INS");		// Inserir
        public readonly static FunctionType ELI_INS_M = new FunctionType("ELINSM");      // Inserir muitos
        public readonly static FunctionType ALT = new FunctionType("ALT");       // Alterar
        public readonly static FunctionType DUP = new FunctionType("DUP");       // Duplicar
        public readonly static FunctionType ELI = new FunctionType("ELI");       // Eliminar
        public readonly static FunctionType EXR = new FunctionType("EXR");       // Executar leitura
        public readonly static FunctionType EXW = new FunctionType("EXW");       // Executar escrita
        public readonly static FunctionType CAN = new FunctionType("CAN");		// Cancelar
        public readonly static FunctionType VAZ = new FunctionType("");          // Função vazia
        public readonly static FunctionType FCT = new FunctionType("FCT");           // QFC
        public readonly static FunctionType FCT2 = new FunctionType("FCT2");         // Novos pedidos de file na Bd
        public readonly static FunctionType SRH = new FunctionType("SRH");           // Pesquisas textuais

        public static Hashtable tiposFuncao = InitHash();    // guarda os tipos de função
        private readonly string id;                      // ID do tipo de função

        /// <summary>
        /// Construtor estático to inicialização da classe
        /// </summary>
        private static Hashtable InitHash()
        {
            tiposFuncao = new Hashtable();

            void AddFunc(FunctionType f)
            {
                tiposFuncao.Add(f.id, f);
            }

            AddFunc(GET);
            AddFunc(GET_ALTERNATIVE);
            AddFunc(GET_NIVELTREE);
            AddFunc(GET_MAIS);
            AddFunc(GET_MENOS);
            AddFunc(GET_POS);
            AddFunc(GET_UM);
            AddFunc(GET_UNICO);
            AddFunc(INS);
            AddFunc(ELI_INS_M);
            AddFunc(ALT);
            AddFunc(DUP);
            AddFunc(ELI);
            AddFunc(EXR);
            AddFunc(EXW);
            AddFunc(CAN);
            AddFunc(VAZ);
            AddFunc(FCT);
            AddFunc(FCT2);
            AddFunc(SRH);

            return tiposFuncao;
        }

        /// <summary>
        /// Construtor privado utilizado na inicialização dos membros
        /// </summary>
        /// <param name="anID">String de identificação do tipo de função</param>
        private FunctionType(string anID)
        {
            id = anID;
        }

        /// <summary>
        /// Converte o objecto em string
        /// </summary>
        /// <returns>Identificador do tipo de função</returns>
        public override string ToString()
        {
            return id;
        }

        /// <summary>
        /// Compara dois tipos de função
        /// </summary>
        /// <param name="obj">Objecto a comparar</param>
        /// <returns>true se for igual, false caso contrário</returns>
        public override bool Equals(object obj)
        {
            if (obj is FunctionType tf)
                return tf.id.Equals(id);
            return false;
        }

        /// <summary>
        /// Override obrigatório da classe Object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

    }
}
