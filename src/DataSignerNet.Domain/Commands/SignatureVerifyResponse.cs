using System;

namespace DataSignerNet.Domain.Commands
{
    public class SignatureVerifyResponse
    {
        public string Issuer { get; set; }

        public string Subject { get; set; }

        public string SerialNumber { get; set; }

        public DateTime NotBefore { get; set; }

        public DateTime NotAfter { get; set; }

        public DateTime SignedDate { get; set; }

        public string TimeStampServer { get; set; }

        public string Reason { get; set; }

        public string DigestAlgorithm { get; set; }

        public string CryptographicStandard { get; set; }
    }
}