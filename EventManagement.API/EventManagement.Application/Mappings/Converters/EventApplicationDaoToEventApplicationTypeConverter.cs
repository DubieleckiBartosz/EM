using AutoMapper;
using EventManagement.Application.Models.Dao.EventApplicationDAOs;
using EventManagement.Domain.Base.EnumerationClasses;
using EventManagement.Domain.Entities;

namespace EventManagement.Application.Mappings.Converters
{
    public class
        EventApplicationDaoToEventApplicationTypeConverter : ITypeConverter<EventApplicationDao, EventApplication>
    {
        public EventApplication Convert(EventApplicationDao source, EventApplication destination,
            ResolutionContext context)
        {
            var applicationId = source.Id;
            var performerId = source.PerformerId;
            var eventId = source.EventId;
            var durationInMinutes = source.DurationInMinutes;
            var typePerformance = Enumeration.GetById<TypePerformance>((int) source.TypePerformance);
            var currentStatus = Enumeration.GetById<StatusApplication>((int) source.CurrentStatus);
            var lastModifiedByApplicant = source.LastModifiedByApplicant;

            return EventApplication.Load(applicationId, eventId, performerId, typePerformance, durationInMinutes,
                lastModifiedByApplicant, currentStatus);
        }
    }
}