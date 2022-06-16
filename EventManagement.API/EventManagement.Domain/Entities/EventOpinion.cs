using System;
using EventManagement.Domain.DDD;
using EventManagement.Domain.ValueObjects;

namespace EventManagement.Domain.Entities
{
    public class EventOpinion : Entity
    {
        public int EventId { get; private set; }
        public CommentValue Comment { get; private set; }
        public int Stars { get; private set; }
        public int? UserId { get; private set; }

        private EventOpinion(int eventId, CommentValue comment, int stars, int? userId = null)
        {
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            this.EventId = eventId;
            this.Comment = comment;
            this.Stars = stars;
            this.UserId = userId;
        }

        private EventOpinion(int eventId, int id, CommentValue comment, int stars, int? userId = null) : this(eventId,
            comment, stars, userId)
        {
            this.Id = id;
            this.Comment = comment;
            this.Stars = stars;
            this.UserId = userId;
        }

        public static EventOpinion LoadOpinion(int eventId, int id, CommentValue comment, int stars, int? userId)
        {
            return new EventOpinion(eventId, id, comment, stars, userId);
        }

        public static EventOpinion Create(int eventId, CommentValue comment, int stars, int? userId)
        {
            return new EventOpinion(eventId, comment, stars, userId);
        }

        public void Update(int? stars, CommentValue comment)
        {
            this.Stars = stars ?? this.Stars;
            this.Comment = comment == null ? this.Comment : comment;
        }
    }
}