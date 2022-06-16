using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Contracts;
using EventManagement.Application.Exceptions;
using EventManagement.Application.Helpers;
using EventManagement.Application.Strings.Responses;
using EventManagement.Application.Wrappers;
using EventManagement.Domain.Entities;
using MediatR;

namespace EventManagement.Application.Features.PerformerFeatures.Commands.UpdatePerformer
{
    public class UpdatePerformerCommandHandler : IRequestHandler<UpdatePerformerCommand, Response<string>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdatePerformerCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
            IMapper mapper)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._currentUserService =
                currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        public async Task<Response<string>> Handle(UpdatePerformerCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var currentUser = this._currentUserService.UserId.ToInt();
            var superAccess = this._currentUserService.HasSuperAccess();
            var repository = this._unitOfWork.Performer;
            var performerDao = await repository.GetPerformerByIdAsync(currentUser);

            if (!superAccess && currentUser != performerDao.UserId)
            {
                throw new AuthException(ResponseStrings.NoPermission);
            }

            var performerEntity = this._mapper.Map<Performer>(performerDao);
            performerEntity.ChangeData(request.NumberOfPeople, request.PerformerMail);
            await repository.UpdatePerformerAsync(performerEntity);

            return Response<string>.Ok(ResponseStrings.OperationSuccess);
        }
    }
}