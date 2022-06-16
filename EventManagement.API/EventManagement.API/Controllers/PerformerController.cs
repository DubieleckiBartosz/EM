using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagement.Application.Features.PerformerFeatures.Commands.CreatePerformer;
using EventManagement.Application.Features.PerformerFeatures.Commands.UpdatePerformer;
using EventManagement.Application.Features.PerformerFeatures.Queries.GetPerformersBySearch;
using EventManagement.Application.Models.Dto;
using EventManagement.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EventManagement.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PerformerController : BaseController
    {
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<List<PerformerWithNumberOfPerformancesDto>>), 200)]
        [SwaggerOperation(Summary = "Get performers by search")]
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPerformersWithNumberPerformancesBySearch([FromBody] GetPerformersBySearchQuery query)
        {
            var result = await this.Mediator.Send(query);
            return Ok(result);
        }

        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Register new performer")]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreatePerformer([FromBody] CreatePerformerCommand command)
        {
            var result = await this.Mediator.Send(command);
            return Ok(result);
        }

        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Update performer")]
        [Authorize(Roles = "Performer")]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdatePerformer([FromBody] UpdatePerformerCommand command)
        {
            var result = await this.Mediator.Send(command);
            return Ok(result);
        }
    }
}
