using EventManagement.Application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EventManagement.Application.Features.EventImageFeatures.Commands.AddNewImage
{
    public class CreateNewImageCommand : IRequest<Response<string>>
    {
        public int EventId { get; set; }
        public bool IsMain { get; set; }
        public string ImagePath { get; set; }
        public string ImageTitle { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
    }
}