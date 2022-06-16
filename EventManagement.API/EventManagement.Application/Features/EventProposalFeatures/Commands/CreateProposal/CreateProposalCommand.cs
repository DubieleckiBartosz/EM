using System;
using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.EventProposalFeatures.Commands.CreateProposal
{
    public class CreateProposalCommand : IRequest<Response<string>>
    {
        public int PerformerId { get; set; }
        public int EventId { get; set; }
        public string Message { get; set; }
        public DateTime ActiveTo { get; set; }

        [JsonConstructor]
        public CreateProposalCommand(int performerId, int eventId, string message, DateTime activeTo)
        {
            this.PerformerId = performerId;
            this.EventId = eventId;
            this.Message = message;
            this.ActiveTo = activeTo;
        }
    }
}