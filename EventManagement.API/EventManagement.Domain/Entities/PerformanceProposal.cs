using System;
using EventManagement.Domain.DDD;
using EventManagement.Domain.ValueObjects;

namespace EventManagement.Domain.Entities
{
    public class PerformanceProposal : Entity
    {
        public int PerformerId { get; private set; }
        public int EventId { get; private set; }
        public Message Message { get; private set; }
        public DateTime ActiveTo { get; private set; }

        private PerformanceProposal(int performerId, int eventId, Message message, DateTime activeTo)
        {
            this.PerformerId = performerId;
            this.EventId = eventId;
            this.Message = message;
            this.ActiveTo = activeTo;
        }

        private PerformanceProposal(int id, int performerId, int eventId, Message message, DateTime activeTo) : this(
            performerId, eventId, message, activeTo)
        {
            this.Id = id;
        }

        public static PerformanceProposal Load(int id, int performerId, int eventId, Message message, DateTime activeTo)
        {
            return new PerformanceProposal(id, performerId, eventId, message, activeTo);
        }

        public static PerformanceProposal Create(int performerId, int eventId, Message message, DateTime activeTo)
        {
            return new PerformanceProposal(performerId, eventId, message, activeTo);
        }
    }
}