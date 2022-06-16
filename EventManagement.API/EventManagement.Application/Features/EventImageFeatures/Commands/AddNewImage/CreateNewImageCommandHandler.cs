using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Attributes;
using EventManagement.Application.Contracts;
using EventManagement.Application.Helpers;
using EventManagement.Application.Strings.Responses;
using EventManagement.Application.Wrappers;
using EventManagement.Domain.Entities;
using EventManagement.Domain.ValueObjects;
using MediatR;

namespace EventManagement.Application.Features.EventImageFeatures.Commands.AddNewImage
{
    [WithTransaction]
    public class CreateNewImageCommandHandler : IRequestHandler<CreateNewImageCommand, Response<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public CreateNewImageCommandHandler(IUnitOfWork unitOfWork, IFileService fileService,
            IMapper mapper)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            this._mapper = mapper;
        }
        public async Task<Response<string>> Handle(CreateNewImageCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var eventRepository = this._unitOfWork.EventRepository;
            var imageRepository = this._unitOfWork.EventImageRepository;
            var eventDao = await eventRepository.GetEventWithImagesAsync(request.EventId);
            var eventAggregate = this._mapper.Map<Event>(eventDao);
            var description = ImageDescription.Create(request.Description);

            var finalPath = request.ImagePath.CreatePath(eventDao.EventName, request.Image.FileName);

            var image = eventAggregate.AddNewImage(finalPath, request.ImageTitle, request.IsMain, description);
            await imageRepository.AddNewImageAsync(image, eventAggregate.Id);
            await _fileService.SaveFileAsync(request.Image, request.ImagePath, eventDao.EventName);
          
            await this._unitOfWork.CompleteAsync(eventAggregate);
   
            
            return Response<string>.Ok(ResponseStrings.NewImageCreated(eventAggregate.Id));
        }
    }
}