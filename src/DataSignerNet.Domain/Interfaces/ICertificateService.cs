using DataSignerNet.Domain.Commands;

namespace DataSignerNet.Domain.Interfaces
{
    public interface ICertificateService
    {
        CreateCertificateResponse Generate(CreateCertificateRequest request);
        
        ReadCertificateResponse Read(ReadCertificateRequest request);
    }
}