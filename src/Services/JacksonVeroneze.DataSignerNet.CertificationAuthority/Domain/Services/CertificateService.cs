using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DataSignerNet.Domain;
using JacksonVeroneze.DataSignerNet.CertificationAuthority.Domain.Command;
using JacksonVeroneze.DataSignerNet.CertificationAuthority.Domain.Interfaces;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Utilities;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;
using Org.BouncyCastle.X509.Extension;

namespace JacksonVeroneze.DataSignerNet.CertificationAuthority.Domain.Services
{
    public class CertificateService : ICertificateService
    {
        //
        // Summary:
        //     /// Method responsible for generate certificate. ///
        //
        // Parameters:
        //   request:
        //     The request param.
        //
        public CreateCertificateResult Generate(CreateCertificateCommand request)
        {
            AsymmetricKeyParameter caPrivateKey = null;

            var caCert = GenerateCACertificate("CN=MyROOTCA", ref caPrivateKey);


            SecureRandom random = new SecureRandom();

            RsaKeyPairGenerator keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(new KeyGenerationParameters(random, 2048));

            AsymmetricCipherKeyPair subjectKeyPair = keyPairGenerator.GenerateKeyPair();

            IDictionary issuerAttrs = FactoryIssuerAttrs();
            IDictionary subjectAttrs = FactorySubjectAttrs(request);

            BigInteger serialNumber = BigInteger.ProbablePrime(120, new Random());

            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();
            certificateGenerator.SetSerialNumber(serialNumber);
            certificateGenerator.SetIssuerDN(new X509Name(new ArrayList(issuerAttrs.Keys), issuerAttrs));
            certificateGenerator.SetSubjectDN(new X509Name(new ArrayList(subjectAttrs.Keys), subjectAttrs));
            certificateGenerator.SetNotBefore(DateTime.UtcNow.Date);
            certificateGenerator.SetNotAfter(DateTime.UtcNow.Date.AddYears(3));
            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            certificateGenerator.AddExtension(X509Extensions.KeyUsage, true,
                new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.KeyAgreement | KeyUsage.NonRepudiation));

            certificateGenerator.AddExtension(X509Extensions.ExtendedKeyUsage.Id, false,
                new ExtendedKeyUsage(new[] { KeyPurposeID.IdKPServerAuth }));

            GeneralNames subjectAltName = new GeneralNames(new GeneralName(GeneralName.DnsName, "SAN"));
            certificateGenerator.AddExtension(X509Extensions.SubjectAlternativeName, false, subjectAltName);

            Asn1SignatureFactory signatureFactory =
                new Asn1SignatureFactory(HashType.SHA256WithRSA.ToString(), subjectKeyPair.Private);

            X509Certificate certificate = certificateGenerator.Generate(signatureFactory);

            X509CertificateEntry certEntry = new X509CertificateEntry(certificate);

            Pkcs12Store store = new Pkcs12StoreBuilder().Build();

            store.SetCertificateEntry(certificate.SubjectDN.ToString(), certEntry);
            store.SetKeyEntry(certificate.SubjectDN + "_key", new AsymmetricKeyEntry(subjectKeyPair.Private),
                new[] { certEntry });

            MemoryStream p12Stream = new MemoryStream();

            store.Save(p12Stream, request.Pin.ToCharArray(), random);

            byte[] pfx = Pkcs12Utilities.ConvertToDefiniteLength(p12Stream.ToArray(), request.Pin.ToCharArray());

            X509Certificate2 x509Certificate2 =
                new X509Certificate2(pfx, request.Pin, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);


            CreateCrl(caCert, caPrivateKey, serialNumber);

            return FactoryResponse(x509Certificate2);
        }

        public InfoCertificateResult Info(InfoCertificateCommand request)
        {
            X509CertificateParser parser = new X509CertificateParser();

            X509Certificate certificate = parser.ReadCertificate(Encoding.UTF8.GetBytes(request.Content));

            return new InfoCertificateResult()
            {
                Issuer = certificate.IssuerDN.ToString(),
                Subject = certificate.SubjectDN.ToString(),
                SerialNumber = certificate.SerialNumber.ToString(),
                NotAfter = certificate.NotAfter,
                NotBefore = certificate.NotBefore,
                Algorithm = certificate.SigAlgName,
                Type = certificate.Version.ToString(),
                Expired = DateTime.Now > certificate.NotAfter,
                Revoged = false,
            };
        }

        //
        // Summary:
        //     /// Method responsible for generate raw data certificate. ///
        //
        // Parameters:
        //   request:
        //     The request param.
        //
        private CreateCertificateResult FactoryResponse(X509Certificate2 x509Certificate2)
        {
            return new CreateCertificateResult()
            {
                Issuer = x509Certificate2.IssuerName.Name,
                Subject = x509Certificate2.SubjectName.Name,
                SerialNumber = x509Certificate2.SerialNumber,
                NotBefore = x509Certificate2.NotBefore,
                NotAfter = x509Certificate2.NotAfter,
                PublicKey = x509Certificate2.GetPublicKeyString(),
                KeyIdentifier = x509Certificate2.PublicKey.Key.KeySize,
                Algorithm = x509Certificate2.PublicKey.Key.SignatureAlgorithm,
                RawData = FactoryRawData(x509Certificate2)
            };
        }

        //
        // Summary:
        //     /// Method responsible for generate raw data certificate. ///
        //
        // Parameters:
        //   request:
        //     The request param.
        //
        private string FactoryRawData(X509Certificate2 x509Certificate2)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(x509Certificate2.Export(X509ContentType.Cert),
                Base64FormattingOptions.None));
            builder.AppendLine("-----END CERTIFICATE-----");

            return builder.ToString();
        }

        //
        // Summary:
        //     /// Method responsible for generate issuer attribs. ///
        //
        // Parameters:
        //   request:
        //     The request param.
        //
        private IDictionary FactoryIssuerAttrs()
        {
            return new Dictionary<DerObjectIdentifier, string>()
            {
                {X509Name.C, "BR"},
                {X509Name.O, "Jackson Veroneze - CA"},
                {X509Name.OU, "Jackson Veroneze - CA"},
                {X509Name.T, "Jackson Veroneze - CA"},
                {X509Name.CN, "Jackson Veroneze - CA"},
                {X509Name.L, "Capinzal"},
                {X509Name.ST, "SC"},
                {X509Name.E, "jackson@jacksonveroneze.com"},
                {X509Name.PostalCode, "89665-000"},
                {X509Name.Street, "Capinzal - SC"},
            };
        }

        //
        // Summary:
        //     /// Method responsible for generate subject attribs. ///
        //
        // Parameters:
        //   request:
        //     The request param.
        //
        private IDictionary FactorySubjectAttrs(CreateCertificateCommand request)
        {
            return new Dictionary<DerObjectIdentifier, string>()
            {
                {X509Name.C, request.CountryCode},
                {X509Name.O, request.Organization},
                {X509Name.OU, request.OrganizationalUnitName},
                {X509Name.T, request.CommonName},
                {X509Name.CN, request.CommonName},
                {X509Name.L, request.LocalityName},
                {X509Name.ST, request.StateOrProvinceName},
                {X509Name.E, request.EmailAddress},
                {X509Name.PostalCode, request.PostalCode},
                {X509Name.Street, request.Street},
                {X509Name.Surname, request.Surname},
                {X509Name.GivenName, request.GivenName},
                {X509Name.Gender, request.Gender},
            };
        }

        public X509Crl CreateCrl(
            X509Certificate caCert,
            AsymmetricKeyParameter caKey,
            BigInteger serialNumber)
        {
            X509V2CrlGenerator crlGen = new X509V2CrlGenerator();
            DateTime now = DateTime.UtcNow;

            crlGen.SetIssuerDN(PrincipalUtilities.GetSubjectX509Principal(caCert));

            crlGen.SetThisUpdate(now);
            crlGen.SetNextUpdate(now.AddMinutes(30));
            crlGen.SetSignatureAlgorithm("SHA256WithRSAEncryption");

            crlGen.AddCrlEntry(serialNumber, now, CrlReason.PrivilegeWithdrawn);

            crlGen.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(caCert));
            crlGen.AddExtension(X509Extensions.CrlNumber, false, new CrlNumber(BigInteger.One));

            SecureRandom random = new SecureRandom();

            return crlGen.Generate(caKey, random);
        }

        public X509Certificate GenerateCACertificate(string subjectName, ref AsymmetricKeyParameter CaPrivateKey)
        {
            const int keyStrength = 2048;

            // Generating Random Numbers
            SecureRandom random = new SecureRandom();

            // The Certificate Generator
            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();

            // Serial Number
            BigInteger serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);

            // Signature Algorithm
            const string signatureAlgorithm = "SHA256WithRSA";
            certificateGenerator.SetSignatureAlgorithm(signatureAlgorithm);

            // Issuer and Subject Name
            X509Name subjectDN = new X509Name(subjectName);
            X509Name issuerDN = subjectDN;
            certificateGenerator.SetIssuerDN(issuerDN);
            certificateGenerator.SetSubjectDN(subjectDN);

            // Valid For
            DateTime notBefore = DateTime.UtcNow.Date;
            DateTime notAfter = notBefore.AddYears(2);

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);

            // Subject Public Key
            AsymmetricCipherKeyPair subjectKeyPair;
            KeyGenerationParameters keyGenerationParameters = new KeyGenerationParameters(random, keyStrength);
            RsaKeyPairGenerator keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            subjectKeyPair = keyPairGenerator.GenerateKeyPair();

            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            // Generating the Certificate
            AsymmetricCipherKeyPair issuerKeyPair = subjectKeyPair;

            // Selfsign certificate
            Org.BouncyCastle.X509.X509Certificate certificate = certificateGenerator.Generate(issuerKeyPair.Private, random);
            X509Certificate2 x509 = new X509Certificate2(certificate.GetEncoded());

            CaPrivateKey = issuerKeyPair.Private;

            return certificate;
        }
    }
}
