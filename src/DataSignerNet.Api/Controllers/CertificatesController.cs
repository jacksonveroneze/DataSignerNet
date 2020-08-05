using System.Net.Mime;
using DataSignerNet.Domain.Commands;
using DataSignerNet.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DataSignerNet.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CertificatesController : ControllerBase
    {
        private readonly ILogger<CertificatesController> _logger;
        private readonly ICertificateService _certificateService;

        //
        // Summary:
        //     /// Method responsible for initializing the controller. ///
        //
        // Parameters:
        //   logger:
        //     The logger param.
        //
        //   certificateService:
        //     The certificateService param.
        //
        public CertificatesController(ILogger<CertificatesController> logger, ICertificateService certificateService)
        {
            _logger = logger;
            _certificateService = certificateService;
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
        [ProducesResponseType(typeof(CreateCertificateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<CreateCertificateResponse> Create([FromBody] CreateCertificateRequest request)
        {
            _logger.LogInformation("Request: {0}", "Generate new certificate");

            return _certificateService.Generate(request);
        }
        
        
        //
        // Summary:
        //     /// Method responsible for action: New (POST). ///
        //
        // Parameters:
        //   command:
        //     The command param.
        //
        [HttpPost("read")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(CreateCertificateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ReadCertificateResponse> Read([FromBody] ReadCertificateRequest request)
        {
            _logger.LogInformation("Request: {0}", "Generate new certificate");

            return _certificateService.Read(request);
        }
    }
}