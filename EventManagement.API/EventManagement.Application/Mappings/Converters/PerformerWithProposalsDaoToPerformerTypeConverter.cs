using System.Collections.Generic;
using AutoMapper;
using EventManagement.Application.Models.Dao.PerformerDAOs;
using EventManagement.Domain.Entities;
using EventManagement.Domain.ValueObjects;

namespace EventManagement.Application.Mappings.Converters
{
    public class
        PerformerWithProposalsDaoToPerformerTypeConverter : ITypeConverter<PerformerWithProposalsDao, Performer>
    {
        public Performer Convert(PerformerWithProposalsDao source, Performer destination, ResolutionContext context)
        {
            var performerId = source.Id;
            var performerMail = source.PerformerMail;
            var vip = source.VIP;
            var userId = source.UserId;
            var numberOfPeople = source.NumberOfPeople;
            var performerName = PerformerName.Create(source.PerformerName);
            var proposals = context.Mapper.Map<List<PerformanceProposal>>(source.Proposals);

            return Performer.Load(performerId, userId, vip, performerName, numberOfPeople, performerMail, proposals);
        }
    }
}