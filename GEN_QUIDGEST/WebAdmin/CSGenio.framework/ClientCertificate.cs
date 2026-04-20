using System;
using System.Security.Cryptography.X509Certificates;

namespace CSGenio.framework
{
    /// <summary>
    /// Certificado enviado pelo cliente através do browser
    /// </summary>
    public class ClientCertificate
    {
        private X509Certificate Qcertificate;
        private String name;
        private String serialNumber;

        /// <summary>
        /// Construtor da class
        /// </summary>
        /// <param name="certificate">Certificado HTTP</param>
        public ClientCertificate(byte[] certificate, string subject) {

            this.Qcertificate = new X509Certificate(certificate);
            this.name = subject.Split(new char []{ '=', ',' })[1]; // Vai guardar o name (o que estiver no CN="")
            this.serialNumber = Qcertificate.GetSerialNumberString();            
        }

		/// <summary>
        /// Construtor da class
        /// </summary>
        /// <param name="numeroSerie">Número de série do Qcertificate</param>
        /// <param name="nome">Name do Qcertificate</param>
        public ClientCertificate(string serialNumber, string name)
        {
            this.Qcertificate = null;
            this.name = name;
            this.serialNumber = serialNumber;
        }
		
        /// <summary>
        /// Devolve o name do user do Qcertificate
        /// </summary>
        /// <returns></returns>
        public String returnName()
        {
            return name;
        }

        /// <summary>
        /// Devolve o number de serie
        /// </summary>
        /// <returns></returns>
        public String returnSerialNumber()
        {
            return serialNumber;
        }

    }

}
