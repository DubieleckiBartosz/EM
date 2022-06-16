using EventManagement.Application.Models.Dto;
using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.EventImageFeatures.Queries.GetImageById
{
    public class GetImageByIdQuery : IRequest<Response<EventImageDto>>
    {
        public int ImageId { get; set; }

        [JsonConstructor]
        public GetImageByIdQuery(int imageId)
        {
            this.ImageId = imageId;
        }
    }
}