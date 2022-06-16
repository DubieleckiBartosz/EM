using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Swashbuckle.AspNetCore.Annotations;

namespace EventManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentationController : ControllerBase
    {

        [SwaggerOperation(Summary = "Get application documentation")]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetApplicationDocumentation()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documentation", "doc-pl.txt");
            var fileExist = System.IO.File.Exists(filePath);
            if (!fileExist)
            {
                return NotFound();
            }

            var contentProvider = new FileExtensionContentTypeProvider();
            contentProvider.TryGetContentType(filePath, out string contentType);

            var content = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(content, contentType);
        }
    }
}
