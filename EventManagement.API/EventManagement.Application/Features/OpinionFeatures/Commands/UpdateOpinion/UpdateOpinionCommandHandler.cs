using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Contracts;
using EventManagement.Application.Exceptions;
using EventManagement.Application.Helpers;
using EventManagement.Application.Strings.Responses;
using EventManagement.Application.Wrappers;
using EventManagement.Domain.Entities;
using EventManagement.Domain.ValueObjects;
using MediatR;

namespace EventManagement.Application.Features.OpinionFeatures.Commands.UpdateOpinion
{
    public class UpdateOpinionCommandHandler : IRequestHandler<UpdateOpinionCommand,Response<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public UpdateOpinionCommandHandler(IUnitOfWork unitOfWork,IMapper mapper, ICurrentUserService currentUserService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._currentUserService = currentUserService;
        }

        public async Task<Response<string>> Handle(UpdateOpinionCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var currentUserId = this._currentUserService.UserId.ToInt();
            var opinionRepository = this._unitOfWork.OpinionRepository;
            var opinionDao = await opinionRepository.GetOpinionAsync(request.OpinionId);

            if (opinionDao.UserId != currentUserId)
            {
                throw new AuthException(ResponseStrings.NoPermission);
            }

            var comment = request.Comment != null ? CommentValue.Create(request.Comment) : null;
            var opinionEntity = this._mapper.Map<EventOpinion>(opinionDao);
            opinionEntity.Update(request.Stars, comment);
            await opinionRepository.UpdateOpinionAsync(opinionEntity);
            return Response<string>.Ok(ResponseStrings.OpinionUpdated);
        }
    }
}
