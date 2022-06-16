using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Contracts;
using EventManagement.Application.Models.Dto;
using EventManagement.Application.Wrappers;
using MediatR;

namespace EventManagement.Application.Features.EventImageFeatures.Queries.GetImageById
{
    public class GetImageByIdQueryHandler : IRequestHandler<GetImageByIdQuery, Response<EventImageDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetImageByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Response<EventImageDto>> Handle(GetImageByIdQuery request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var imageRepository = this._unitOfWork.EventImageRepository;
            var imageDao = await imageRepository.GetImageByIdAsync(request.ImageId);
            var imageDto = this._mapper.Map<EventImageDto>(imageDao);

            return Response<EventImageDto>.Ok(imageDto);
        }
    }
}