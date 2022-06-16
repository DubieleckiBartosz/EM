using System.Threading.Tasks;
using EventManagement.Application.Features.EventImageFeatures.Commands.AddNewImage;
using EventManagement.Application.Features.EventImageFeatures.Commands.RemoveImage;
using EventManagement.Application.Features.EventImageFeatures.Queries.GetImageById;
using EventManagement.Application.Models.Dto;
using EventManagement.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EventManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : BaseController
    {
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<EventImageDto>), 200)]
        [SwaggerOperation(Summary = "Get image by Id")]
        [Authorize(Roles = "Owner")]
        [HttpGet("[action]/{imageId}")]
        public async Task<IActionResult> GetImageById([FromRoute] int imageId)
        {
            return Ok(await this.Mediator.Send(new GetImageByIdQuery(imageId)));
        }

        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Create a new image to the event")]
        [Authorize(Roles = "Owner")]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateImageForEvent([FromForm] CreateNewImageCommand command)
        {
            return Ok(await this.Mediator.Send(command));
        }

        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Remove image")]
        [Authorize(Roles = "Owner")]
        [HttpDelete("[action]")]
        public async Task<IActionResult> RemoveImage([FromBody]RemoveImageCommand command)
        {
            return Ok(await this.Mediator.Send(command));
        }
    }
}