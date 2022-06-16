using AutoMapper;
using EventManagement.Application.Models.Dao;
using EventManagement.Domain.Entities;
using EventManagement.Domain.ValueObjects;

namespace EventManagement.Application.Mappings.Converters
{
    public class EventOpinionDaoToEventOpinionTypeConverter : ITypeConverter<EventOpinionDao, EventOpinion>
    {
        public EventOpinion Convert(EventOpinionDao source, EventOpinion destination, ResolutionContext context)
        {
            var comment = CommentValue.Create(source.Comment);
            var stars = source.Stars;
            var user = source.UserId;
            var opinionId = source.Id;
            var eventId = source.EventId;

            var opinion = EventOpinion.LoadOpinion(eventId, opinionId, comment, stars, user);
            return opinion;
        }
    }
}