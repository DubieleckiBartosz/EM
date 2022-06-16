using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.EventImageFeatures.Commands.RemoveImage
{
    public class RemoveImageCommand : IRequest<Response<string>>
    {
        public int ImageId { get; set; }

        [JsonConstructor]
        public RemoveImageCommand(int imageId)
        {
            this.ImageId = imageId;
        }
    }
}