using System;

namespace DataSignerNet.Domain.Commands
{
    public class CreateCertificateResponse
    {
        public string Issuer { get; set; }

        public string Subject { get; set; }

        public string SerialNumber { get; set; }

        public DateTime NotBefore { get; set; }

        public DateTime NotAfter { get; set; }

        public string PublicKey { get; set; }

        public int KeyIdentifier { get; set; }

        public string Algorithm { get; set; }
        
        public string RawData { get; set; }
    }
}