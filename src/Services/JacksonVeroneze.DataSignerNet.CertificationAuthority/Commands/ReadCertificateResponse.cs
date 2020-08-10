using System;

namespace JacksonVeroneze.DataSignerNet.CertificationAuthority.Commands
{
    public class ReadCertificateResponse
    {
        public string Issuer { get; set; }

        public string Subject { get; set; }

        public string SerialNumber { get; set; }

        public DateTime NotBefore { get; set; }

        public DateTime NotAfter { get; set; }

        public string PublicKey { get; set; }

        public int KeyIdentifier { get; set; }

        public string Algorithm { get; set; }

        public string Type { get; set; }

        public bool Expired { get; set; }

        public bool Revoged { get; set; }
    }
}
