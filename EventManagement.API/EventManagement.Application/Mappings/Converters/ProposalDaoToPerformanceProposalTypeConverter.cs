using AutoMapper;
using EventManagement.Application.Models.Dao.ProposalDAOs;
using EventManagement.Domain.Entities;
using EventManagement.Domain.ValueObjects;

namespace EventManagement.Application.Mappings.Converters
{
    public class ProposalDaoToPerformanceProposalTypeConverter : ITypeConverter<ProposalDao, PerformanceProposal>
    {
        public PerformanceProposal Convert(ProposalDao source, PerformanceProposal destination, ResolutionContext context)
        {
            var proposalId = source.Id;
            var performerId = source.PerformerId;
            var eventId = source.EventId;
            var activeTo = source.ActiveTo;
            var message = Message.CreateMessage(source.Message);

            return PerformanceProposal.Load(proposalId, performerId, eventId, message, activeTo);
        }
    }
}
