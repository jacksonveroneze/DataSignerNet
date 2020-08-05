using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DataSignerNet.Domain.Commands;
using DataSignerNet.Domain.Interfaces;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Pkcs;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace DataSignerNet.Domain.Services
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
        public CreateCertificateResponse Generate(CreateCertificateRequest request)
        {
            SecureRandom random = new SecureRandom();

            RsaKeyPairGenerator keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(new KeyGenerationParameters(random, 2048));

            AsymmetricCipherKeyPair subjectKeyPair = keyPairGenerator.GenerateKeyPair();

            IDictionary issuerAttrs = FactoryIssuerAttrs();
            IDictionary subjectAttrs = FactorySubjectAttrs(request);

            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();
            certificateGenerator.SetSerialNumber(BigInteger.ProbablePrime(120, new Random()));
            certificateGenerator.SetIssuerDN(new X509Name(new ArrayList(issuerAttrs.Keys), issuerAttrs));
            certificateGenerator.SetSubjectDN(new X509Name(new ArrayList(subjectAttrs.Keys), subjectAttrs));
            certificateGenerator.SetNotBefore(DateTime.UtcNow.Date);
            certificateGenerator.SetNotAfter(DateTime.UtcNow.Date.AddYears(3));
            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            certificateGenerator.AddExtension(X509Extensions.KeyUsage, true,
                new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.KeyAgreement | KeyUsage.NonRepudiation));

            certificateGenerator.AddExtension(X509Extensions.ExtendedKeyUsage.Id, false,
                new ExtendedKeyUsage(new[] {KeyPurposeID.IdKPServerAuth}));

            GeneralNames subjectAltName = new GeneralNames(new GeneralName(GeneralName.DnsName, "SAN"));
            certificateGenerator.AddExtension(X509Extensions.SubjectAlternativeName, false, subjectAltName);

            Asn1SignatureFactory signatureFactory =
                new Asn1SignatureFactory(HashType.SHA256WithRSA.ToString(), subjectKeyPair.Private);

            X509Certificate certificate = certificateGenerator.Generate(signatureFactory);

            X509CertificateEntry certEntry = new X509CertificateEntry(certificate);

            Pkcs12Store store = new Pkcs12StoreBuilder().Build();

            store.SetCertificateEntry(certificate.SubjectDN.ToString(), certEntry);
            store.SetKeyEntry(certificate.SubjectDN + "_key", new AsymmetricKeyEntry(subjectKeyPair.Private),
                new[] {certEntry});

            using FileStream filestream = new FileStream(@$"/home/jackson/{certificate.SerialNumber}.pfx", FileMode.Create,
                FileAccess.ReadWrite);
            store.Save(filestream, request.Pin.ToCharArray(), random);

            MemoryStream p12Stream = new MemoryStream();

            store.Save(p12Stream, request.Pin.ToCharArray(), random);

            X509Certificate2 x509Certificate2 =
                new X509Certificate2(p12Stream.ToArray(), request.Pin, X509KeyStorageFlags.DefaultKeySet);

            return FactoryResponse(x509Certificate2);
        }

        public ReadCertificateResponse Read(ReadCertificateRequest request)
        {
            X509CertificateParser parser = new X509CertificateParser();
            
            X509Certificate certificate = parser.ReadCertificate(Encoding.UTF8.GetBytes(request.Content));
            
            return new ReadCertificateResponse()
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
        private CreateCertificateResponse FactoryResponse(X509Certificate2 x509Certificate2)
        {
            return new CreateCertificateResponse()
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
        private IDictionary FactorySubjectAttrs(CreateCertificateRequest request)
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
    }
}