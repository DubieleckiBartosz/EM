using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Contracts;
using EventManagement.Application.Helpers;
using EventManagement.Application.Strings.Responses;
using EventManagement.Application.Wrappers;
using EventManagement.Domain.Entities;
using EventManagement.Domain.ValueObjects;
using MediatR;

namespace EventManagement.Application.Features.OpinionFeatures.Commands.CreateOpinion
{
    public class CreateNewOpinionCommandHandler : IRequestHandler<CreateNewOpinionCommand, Response<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CreateNewOpinionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Response<string>> Handle(CreateNewOpinionCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var userId = this._currentUserService.UserId;
            var comment = CommentValue.Create(request.Comment);

            var eventRepository = this._unitOfWork.EventRepository;
            var opinionRepository = this._unitOfWork.OpinionRepository;

            var eventDao = await eventRepository.GetEventBaseDataAsync(request.EventId);
            var eventAggregate = this._mapper.Map<Event>(eventDao);
            var opinion = eventAggregate.AddNewEventOpinion(comment, request.Stars, userId.ToNullableInt());
            await opinionRepository.CreateNewOpinionAsync(opinion);

            return Response<string>.Ok(ResponseStrings.OpinionAdded);
        }
    }
}