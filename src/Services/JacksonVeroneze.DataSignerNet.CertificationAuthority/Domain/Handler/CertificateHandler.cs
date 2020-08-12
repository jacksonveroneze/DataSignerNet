using System.Threading;
using System.Threading.Tasks;
using JacksonVeroneze.DataSignerNet.CertificationAuthority.Domain.Command;
using JacksonVeroneze.DataSignerNet.CertificationAuthority.Domain.Interfaces;
using MediatR;

namespace JacksonVeroneze.DataSignerNet.CertificationAuthority.Domain.Handler
{
    public class CustomerHandler :
        IRequestHandler<CreateCertificateCommand, CreateCertificateResult>,
        IRequestHandler<InfoCertificateCommand, InfoCertificateResult>
    {
        private readonly IMediator _mediator;
        private readonly ICertificateService _certificateService;

        public CustomerHandler(IMediator mediator, ICertificateService certificateService)
        {
            _mediator = mediator;
            _certificateService = certificateService;
        }

        public async Task<CreateCertificateResult> Handle(CreateCertificateCommand request,
            CancellationToken cancellationToken)
            => _certificateService.Generate(request);

        public async Task<InfoCertificateResult> Handle(InfoCertificateCommand request,
            CancellationToken cancellationToken)
            => _certificateService.Info(request);
    }
}
