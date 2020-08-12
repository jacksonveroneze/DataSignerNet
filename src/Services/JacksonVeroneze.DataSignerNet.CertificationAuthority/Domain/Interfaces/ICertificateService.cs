using JacksonVeroneze.DataSignerNet.CertificationAuthority.Domain.Command;

namespace JacksonVeroneze.DataSignerNet.CertificationAuthority.Domain.Interfaces
{
    public interface ICertificateService
    {
        CreateCertificateResult Generate(CreateCertificateCommand request);

        InfoCertificateResult Info(InfoCertificateCommand request);
    }
}
