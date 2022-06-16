using System;
using System.Threading;
using System.Threading.Tasks;
using EventManagement.Application.Attributes;
using EventManagement.Application.Contracts;
using EventManagement.Application.Helpers;
using EventManagement.Application.Strings.Responses;
using EventManagement.Application.Wrappers;
using EventManagement.Domain.Entities;
using EventManagement.Domain.ValueObjects;
using MediatR;

namespace EventManagement.Application.Features.PerformerFeatures.Commands.CreatePerformer
{
    [WithTransaction]
    public class CreatePerformerCommandHandler : IRequestHandler<CreatePerformerCommand, Response<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreatePerformerCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        public async Task<Response<string>> Handle(CreatePerformerCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var currentUser = this._currentUserService.UserId.ToInt();
            var mail = this._currentUserService.UserMail;
            var performerMail = request.PerformerMail ?? mail;
            var name = PerformerName.Create(request.PerformerName);
            var performerRepository = this._unitOfWork.Performer;
            var performer = Performer.Create(currentUser, name, request.NumberOfPeople, performerMail);
            await performerRepository.CreatePerformerAsync(performer);
            await this._unitOfWork.CompleteAsync(performer);
            return Response<string>.Ok(ResponseStrings.NewPerformerRegistered);
        }
    }
}