using System.Net.Mime;
using System.Threading.Tasks;
using ExpiredPassportChecker.Application.Queries.CheckPassport;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpiredPassportChecker.Controllers
{
    [ApiController]
    [Route("expired-passports")]
    public class CheckerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CheckerController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet("series/{series}/numbers/{number}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(CheckPassportResult), StatusCodes.Status200OK)]
        public async Task<CheckPassportResult> CheckExpired(string series, string number)
        {
            var query = new CheckPassportQuery
            {
                PassportSeries = series,
                PassportNumber = number,
            };
            var result = await _mediator.Send(query);
            return result;
        }
    }
}
