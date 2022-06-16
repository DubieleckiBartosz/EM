using System;
using System.Threading;
using System.Threading.Tasks;
using EventManagement.Application.Attributes;
using EventManagement.Application.Contracts;
using EventManagement.Application.Strings.Responses;
using EventManagement.Application.Wrappers;
using MediatR;

namespace EventManagement.Application.Features.EventImageFeatures.Commands.RemoveImage
{
    [WithTransaction]

    public class RemoveImageCommandHandler : IRequestHandler<RemoveImageCommand, Response<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public RemoveImageCommandHandler(IUnitOfWork unitOfWork, IFileService fileService)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._fileService = fileService;
        }

        public async Task<Response<string>> Handle(RemoveImageCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var imageRepository = this._unitOfWork.EventImageRepository;
            var imageDao = await imageRepository.GetImageByIdAsync(request.ImageId);
            await imageRepository.RemoveImageAsync(imageDao.Id);
            this._fileService.RemoveFile(imageDao.ImagePath);
            return Response<string>.Ok(ResponseStrings.OperationSuccess);
        }
    }
}