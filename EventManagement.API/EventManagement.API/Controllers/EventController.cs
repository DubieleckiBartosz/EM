using System.Threading.Tasks;
using EventManagement.Application.Features.EventFeatures.Commands.CancelEvent;
using EventManagement.Application.Features.EventFeatures.Commands.ChangeVisibilityEvent;
using EventManagement.Application.Features.EventFeatures.Commands.CreateEvent;
using EventManagement.Application.Features.EventFeatures.Commands.UpdateEvent;
using EventManagement.Application.Features.EventFeatures.Queries.GetEventDetails;
using EventManagement.Application.Features.EventFeatures.Queries.GetEventsBySearch;
using EventManagement.Application.Features.EventFeatures.Queries.GetEventWithApplications;
using EventManagement.Application.Features.EventFeatures.Queries.GetEventWithOpinions;
using EventManagement.Application.Models.Dto.EventDTOs;
using EventManagement.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EventManagement.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : BaseController
    {
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<EventDetailsDto>), 200)]
        [SwaggerOperation(Summary = "Get event by identifier")]
        [AllowAnonymous]
        [HttpGet("[action]/{eventId}")]
        public async Task<IActionResult> GetEvent([FromRoute] int eventId)
        {
            return Ok(await this.Mediator.Send(new GetEventDetailsQuery(eventId)));
        }

        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<EventWithApplicationsDto>), 200)]
        [SwaggerOperation(Summary = "Get event with applications")]
        [Authorize(Roles = "Owner,Admin")]
        [HttpGet("[action]/{eventId}")]
        public async Task<IActionResult> GetEventWithApplications([FromRoute] int eventId)
        {
            return Ok(await this.Mediator.Send(new GetEventWithApplicationsQuery(eventId)));
        }

        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(ResponseList<EventBaseDto>), 200)]
        [SwaggerOperation(Summary = "Get events by search")]
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetEvents([FromBody] SearchEventsQuery query)
        {
            return Ok(await this.Mediator.Send(query));
        }

        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<EventWithOpinionsDto>), 200)]
        [SwaggerOperation(Summary = "Get event with opinions")]
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetEventWithOpinions([FromBody] GetEventWithOpinionsQuery query)
        {
            return Ok(await this.Mediator.Send(query));
        }

        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<int>), 200)]
        [SwaggerOperation(Summary = "Create new event")]
        [Authorize(Roles = "Owner")]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventCommand command)
        {
            return Ok(await this.Mediator.Send(command));
        }

        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Change visibility")]
        [Authorize(Roles = "Owner")]
        [HttpPut("[action]")]
        public async Task<IActionResult> ChangeVisibilityEvent([FromBody] ChangeVisibilityCommand command)
        {
            return Ok(await this.Mediator.Send(command));
        }

        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Update event")]
        [Authorize(Roles = "Owner")]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateEventInformation([FromBody] UpdateEventCommand command)
        {
            return Ok(await this.Mediator.Send(command));
        }

        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Cancel event")]
        [Authorize(Roles = "Owner")]
        [HttpPut("[action]")]
        public async Task<IActionResult> CancelEvent([FromBody] CancelEventCommand command)
        {
            return Ok(await this.Mediator.Send(command));
        }

        [ProducesResponseType(typeof(Response<List<string>>), 200)]
        [SwaggerOperation(Summary = "Get list of categories")]
        [AllowAnonymous]
        [HttpGet("[action]")]
        public IActionResult GetCategories()
        {
            var response = EnumHelpers.GetStringValuesFromEnum<EventCategory>();

            return Ok(Response<List<string>>.Ok(response));
        }

        [ProducesResponseType(typeof(Response<List<List<string>>>), 200)]
        [SwaggerOperation(Summary = "Get search options")]
        [AllowAnonymous]
        [HttpGet("[action]")]
        public IActionResult GetSearchOptions()
        {
             var categories = EnumHelpers.GetStringValuesFromEnum<EventCategory>();

            return Ok(Response<List<string>>.Ok(response));
        }
    }
}