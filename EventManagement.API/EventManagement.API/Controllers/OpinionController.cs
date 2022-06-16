using System.Threading.Tasks;
using EventManagement.Application.Features.OpinionFeatures.Commands.CreateOpinion;
using EventManagement.Application.Features.OpinionFeatures.Commands.RemoveOpinion;
using EventManagement.Application.Features.OpinionFeatures.Commands.UpdateOpinion;
using EventManagement.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EventManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpinionController : BaseController
    {
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Create opinion")]
        [HttpPost("[action]")] 
        public async Task<IActionResult> CreateOpinion([FromBody] CreateNewOpinionCommand command)
        {
            var result = await this.Mediator.Send(command);
            return Ok(result);
        }

        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Update opinion")]
        [Authorize(Roles = "User")]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateOpinion([FromBody] UpdateOpinionCommand command)
        {
            var result = await this.Mediator.Send(command);
            return Ok(result);
        }

        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Remove opinion")]
        [Authorize(Roles = "User")]
        [HttpDelete("[action]")]
        public async Task<IActionResult> RemoveOpinion([FromBody] RemoveOpinionCommand command)
        {
            var result = await this.Mediator.Send(command);
            return Ok(result);
        }
    }
}
