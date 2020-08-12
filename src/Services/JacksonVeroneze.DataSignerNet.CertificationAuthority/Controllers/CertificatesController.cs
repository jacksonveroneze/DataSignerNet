using System.Net.Mime;
using System.Threading.Tasks;
using JacksonVeroneze.DataSignerNet.CertificationAuthority.Domain.Command;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JacksonVeroneze.DataSignerNet.CertificationAuthority.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CertificatesController : ControllerBase
    {
        private readonly ILogger<CertificatesController> _logger;
        private readonly IMediator _mediator;
        //
        // Summary:
        //     /// Method responsible for initializing the controller. ///
        //
        // Parameters:
        //   logger:
        //     The logger param.
        //
        //   mediator:
        //     The mediator param.
        //
        public CertificatesController(ILogger<CertificatesController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        //
        // Summary:
        //     /// Method responsible for action: New (POST). ///
        //
        // Parameters:
        //   command:
        //     The command param.
        //
        [HttpPost("new")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(CreateCertificateCommand), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateCertificateCommand request)
        {
            _logger.LogInformation("Request: {0}", "Generate new certificate");

            return Ok(await _mediator.Send(request));
        }


        //
        // Summary:
        //     /// Method responsible for action: New (POST). ///
        //
        // Parameters:
        //   command:
        //     The command param.
        //
        [HttpPost("info")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(InfoCertificateResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Info([FromBody] InfoCertificateCommand request)
        {
            _logger.LogInformation("Request: {0}", "Info certificate");

            return Ok(await _mediator.Send(request));
        }
    }
}
