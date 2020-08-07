using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using DataSignerNet.Domain.Commands;
using DataSignerNet.Domain.Interfaces;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.X509;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Security;

namespace DataSignerNet.Domain.Services
{
    public class SignatureService : ISignatureService
    {
        //
        // Summary:
        //     /// Method responsible for sign document. ///
        //
        // Parameters:
        //   request:
        //     The request param.
        //
        public string Sign(SignatureSignRequest request)
        {
            PdfLoadedDocument document = new PdfLoadedDocument(Convert.FromBase64String(request.Content));

            using FileStream fs = File.OpenRead(@"/home/jackson/certificate.pfx");

            PdfCertificate certificate = new PdfCertificate(fs, "123456");

            PdfSignature signature = new PdfSignature(document, document.Pages[0], certificate, "DigitalSignature");

            signature.Settings.CryptographicStandard = CryptographicStandard.CADES;
            signature.Settings.DigestAlgorithm = DigestAlgorithm.SHA512;

            //signature.TimeStampServer = new TimeStampServer(new Uri("http://timestamp.digicert.com/"));

            MemoryStream stream = new MemoryStream();

            document.Save(stream);

            document.Close(true);

            string sign = Convert.ToBase64String(stream.ToArray()).ToString();

            Time(sign);

            return sign;
        }

        //
        // Summary:
        //     /// Method responsible for valid document. ///
        //
        // Parameters:
        //   request:
        //     The request param.
        //
        public SignatureVerifyResponse Verify(SignatureVerifyRequest request)
        {
            PdfLoadedDocument document = new PdfLoadedDocument(Convert.FromBase64String(request.Content));

            PdfLoadedSignatureField signatureField = document.Form.Fields[0] as PdfLoadedSignatureField;

            PdfSignature signature = signatureField.Signature;

            return new SignatureVerifyResponse()
            {
                Issuer = signature.Certificate.IssuerName,
                Subject = signature.Certificate.SubjectName,
                SerialNumber = Encoding.UTF8.GetString(signature.Certificate.SerialNumber),
                NotBefore = signature.Certificate.ValidTo,
                NotAfter = signature.Certificate.ValidFrom,
                SignedDate = signature.SignedDate,
                TimeStampServer = signature.TimeStampServer?.ToString(),
                Reason = signature.Reason,
                DigestAlgorithm = signature.Settings.DigestAlgorithm.ToString(),
                CryptographicStandard = signature.Settings.CryptographicStandard.ToString(),
            };
        }

        private bool Time(string data)
        {
            SHA1 sha1 = SHA1CryptoServiceProvider.Create();
            byte[] hash = sha1.ComputeHash(Encoding.ASCII.GetBytes(data));

            TimeStampRequestGenerator reqGen = new TimeStampRequestGenerator();
            reqGen.SetCertReq(true);

            TimeStampRequest tsReq
                = reqGen.Generate(TspAlgorithms.Sha1, hash, BigInteger.ValueOf(100));
            byte[] tsData = tsReq.GetEncoded();
            HttpWebRequest req =
                (HttpWebRequest)WebRequest.Create("http://timestamp.digicert.com");
            req.Method = "POST";
            req.ContentType = "application/timestamp-query";

            req.ContentLength = tsData.Length;

            Stream reqStream = req.GetRequestStream();
            reqStream.Write(tsData, 0, tsData.Length);
            reqStream.Close();

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            Stream resStream = new BufferedStream(res.GetResponseStream());

            TimeStampResponse tsRes = new TimeStampResponse(resStream);
            resStream.Close();

            try
            {
                tsRes.Validate(tsReq);
            }
            catch (TspException e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }
    }
}
