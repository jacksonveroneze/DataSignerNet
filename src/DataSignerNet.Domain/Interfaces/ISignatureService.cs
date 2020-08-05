using DataSignerNet.Domain.Commands;

namespace DataSignerNet.Domain.Interfaces
{
    public interface ISignatureService
    {
        string Sign(SignatureSignRequest request);

        SignatureVerifyResponse Verify(SignatureVerifyRequest request);
    }
}