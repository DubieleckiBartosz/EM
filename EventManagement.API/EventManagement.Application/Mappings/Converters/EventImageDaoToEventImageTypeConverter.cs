using AutoMapper;
using EventManagement.Application.Models.Dao;
using EventManagement.Domain.Entities;
using EventManagement.Domain.ValueObjects;

namespace EventManagement.Application.Mappings.Converters
{
    public class EventImageDaoToEventImageTypeConverter : ITypeConverter<EventImageDao, EventImage>
    {
        public EventImage Convert(EventImageDao source, EventImage destination, ResolutionContext context)
        {
            var id = source.Id;
            var eventId = source.EventId;
            var isMain = source.IsMain;
            var imageTitle = source.ImageTitle;
            var imagePath = source.ImagePath;
            var description = ImageDescription.Create(source.ImageDescription);

            return EventImage.LoadImage(id, eventId, imagePath, imageTitle, isMain, description);
        }
    }
}