using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CSGenio.framework
{
    /// <summary>
    /// Funções to serializar e desserializar objectos de forma binária
    /// </summary>
    public static class SerializationFunctions
    {
        /// <summary>
        /// Serializa todos os objectos passados por argumento to um byte array
        /// </summary>
        /// <param name="objs">Objectos a serializar</param>
        /// <returns>byte array com a representação binária dos objectos</returns>
        public static byte[] SerializeObjectsToByteArray(params object[] objs)
        {
            MemoryStream stream = new MemoryStream();
            SerializeObjects(stream, objs);
            byte[] result = stream.ToArray();
            stream.Close();
            return result;
        }

        /// <summary>
        /// Serializa todos os objectos passados como argumento to a stream
        /// </summary>
        /// <param name="stream">Stream to a qual os objectos vão ser serializados</param>
        /// <param name="objs">Objectos a serializar</param>
        public static void SerializeObjects(Stream stream, params object[] objs)
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, objs);
        }

        /// <summary>
        /// Desserializa um array de objectos da strem
        /// </summary>
        /// <param name="stream">Stream da qual os objectos vão ser desserializados</param>
        /// <returns>Array de objectos que foram desserializados da stream</returns>
        public static object[] DeserializeObjects(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            return (object[])formatter.Deserialize(stream);
        }

        /// <summary>
        /// Desserializa um array de objectos de um byte array
        /// </summary>
        /// <param name="objs">Byte array com a representação binária dos objectos</param>
        /// <returns>Array de objectos que foram desserializados do byte array</returns>
        public static object[] DeserializeObjectsFromByteArray(byte[] objs)
        {
            MemoryStream stream = new MemoryStream(objs);
            object[] objects = DeserializeObjects(stream);
            stream.Close();
            return objects;
        }
    }
}
