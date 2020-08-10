using JacksonVeroneze.DataSignerNet.CertificationAuthority.Commands;

namespace JacksonVeroneze.DataSignerNet.CertificationAuthority.Interfaces
{
    public interface ICertificateService
    {
        CreateCertificateResponse Generate(CreateCertificateRequest request);

        ReadCertificateResponse Info(ReadCertificateRequest request);
    }
}
