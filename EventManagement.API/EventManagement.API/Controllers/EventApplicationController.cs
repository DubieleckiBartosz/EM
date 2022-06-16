using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagement.Application.Features.EventApplicationFeatures.Commands.ConsiderEventApplication;
using EventManagement.Application.Features.EventApplicationFeatures.Commands.CreateEventApplication;
using EventManagement.Application.Features.EventApplicationFeatures.Commands.UpdateEventApplication;
using EventManagement.Application.Features.EventApplicationFeatures.Queries.GetEventApplicationsBySearch;
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
    public class EventApplicationController : BaseController
    {
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<List<EventApplicationDto>>), 200)]
        [SwaggerOperation(Summary = "Get events applications by search")]
        [Authorize(Roles = "Owner,Admin,Performer")]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetEventApplicationsBySearch([FromBody] GetEventApplicationsQuery query)
        {
            return Ok(await this.Mediator.Send(query));
        }

        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Create new application")]
        [Authorize(Roles = "Performer")]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateEventApplication([FromBody] CreateApplicationCommand command)
        {
            return Ok(await this.Mediator.Send(command));
        }

        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Update application")]
        [Authorize(Roles = "Owner,Admin,Performer")]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateEventApplication([FromBody] UpdateEventApplicationCommand command)
        {
            return Ok(await this.Mediator.Send(command));
        }

        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Consider application")]
        [Authorize(Roles = "Owner,Admin,Performer")]
        [HttpPut("[action]")]
        public async Task<IActionResult> ConsiderEventApplication([FromBody] ConsiderApplicationCommand command)
        {
            return Ok(await this.Mediator.Send(command));
        }
    }
}