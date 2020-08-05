using System.Net.Mime;
using DataSignerNet.Domain;
using DataSignerNet.Domain.Commands;
using DataSignerNet.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DataSignerNet.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SignaturesController : ControllerBase
    {
        private readonly ILogger<SignaturesController> _logger;
        private readonly ISignatureService _signatureService;

        //
        // Summary:
        //     /// Method responsible for initializing the controller. ///
        //
        // Parameters:
        //   logger:
        //     The logger param.
        //
        //   signService:
        //     The signService param.
        //
        public SignaturesController(ILogger<SignaturesController> logger, ISignatureService signatureService)
        {
            _logger = logger;
            _signatureService = signatureService;
        }

        //
        // Summary:
        //     /// Method responsible for action: New (POST). ///
        //
        // Parameters:
        //   command:
        //     The command param.
        //
        [HttpPost("sign")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Sign([FromBody] SignatureSignRequest request)
        {
            _logger.LogInformation("Request: {0}", "Sign document");

            return Ok(_signatureService.Sign(request));
        }


        //
        // Summary:
        //     /// Method responsible for action: New (POST). ///
        //
        // Parameters:
        //   command:
        //     The command param.
        //
        [HttpPost("verify")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<SignatureVerifyResponse> Verify([FromBody] SignatureVerifyRequest request)
        {
            _logger.LogInformation("Request: {0}", "Verify document");

            return Ok(_signatureService.Verify(request));
        }
    }
}