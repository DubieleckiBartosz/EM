using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagement.Application.Features.EventProposalFeatures.Commands.CreateProposal;
using EventManagement.Application.Features.EventProposalFeatures.Commands.RemoveProposal;
using EventManagement.Application.Features.EventProposalFeatures.Queries.GetProposals;
using EventManagement.Application.Models.Dto;
using EventManagement.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EventManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceProposalController : BaseController
    {
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<List<PerformanceProposalDto>>), 200)]
        [SwaggerOperation(Summary = "Get proposals")]
        [Authorize(Roles = "Performer,Owner,Admin")]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetProposals()
        {
            var result = await this.Mediator.Send(new GetProposalsQuery());
            return Ok(result);
        }

        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Create proposal")]
        [Authorize(Roles = "Owner")]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateProposal([FromBody] CreateProposalCommand command)
        {
            var result = await this.Mediator.Send(command);
            return Ok(result);
        }

        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Remove proposal")]
        [Authorize(Roles = "Owner,Admin,Performer")]
        [HttpDelete("[action]")]
        public async Task<IActionResult> RemoveProposal([FromBody] RemoveProposalCommand command)
        {
            var result = await this.Mediator.Send(command);
            return Ok(result);
        }
    }
}
