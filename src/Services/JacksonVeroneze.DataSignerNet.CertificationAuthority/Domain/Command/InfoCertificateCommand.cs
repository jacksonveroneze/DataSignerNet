using MediatR;

namespace JacksonVeroneze.DataSignerNet.CertificationAuthority.Domain.Command
{
    public class InfoCertificateCommand : IRequest<InfoCertificateResult>
    {
        public string Content { get; set; }
    }
}
