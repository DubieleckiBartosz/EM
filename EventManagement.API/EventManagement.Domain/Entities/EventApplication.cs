using EventManagement.Domain.Base.EnumerationClasses;
using EventManagement.Domain.DDD;
using EventManagement.Domain.Rules;
using EventManagement.Domain.Rules.Common;
using EventManagement.Domain.Rules.EventApplicationRules;

namespace EventManagement.Domain.Entities
{
    public class EventApplication : Entity
    {
        public int PerformerId { get; }
        public int EventId { get; }
        public TypePerformance TypePerformance { get; private set; }
        public int DurationInMinutes { get; private set; }
        public StatusApplication CurrentStatus { get; private set; }
        public bool LastModifiedByApplicant { get; private set; }


        private EventApplication(int performerId, int eventId, TypePerformance typePerformance, int durationInMinutes)
        {
            this.PerformerId = performerId;
            this.EventId = eventId;
            this.TypePerformance = typePerformance;
            this.DurationInMinutes = durationInMinutes;
            this.CurrentStatus = StatusApplication.NotConsidered;
        }

        private EventApplication(int id, int eventId, int performerId, TypePerformance typePerformance,
            int durationInMinutes, bool lastModifiedByApplicant, StatusApplication currentStatus) : this(
            performerId, eventId, typePerformance, durationInMinutes)
        {
            this.Id = id;
            this.CurrentStatus = currentStatus;
            this.LastModifiedByApplicant = lastModifiedByApplicant;
        }

        public static EventApplication Load(int id, int eventId, int performerId, TypePerformance typePerformance,
            int durationInMinutes, bool lastModifiedByApplicant, StatusApplication currentStatus)
        {
            return new EventApplication(id, eventId, performerId, typePerformance, durationInMinutes, lastModifiedByApplicant, currentStatus);
        }

        public static EventApplication Create(int performerId, int eventId, TypePerformance typePerformance,
            int durationInMinutes)
        {
            CheckRule(new IntRules(15, 120, durationInMinutes, nameof(DurationInMinutes)));
            return new EventApplication(performerId, eventId, typePerformance, durationInMinutes);
        }

        public void ChangeStatus(StatusApplication newStatus, bool isApplicant)
        {
            CheckRule(new StatusCannotBeInRejectedDueToEventCancellationRule(this.CurrentStatus));

            this.CurrentStatus = newStatus;
            this.LastModifiedByApplicant = isApplicant;
        }

        public void Update(int? durationInMinutes, TypePerformance typePerformance, bool isApplicant)
        {
            CheckRule(new StatusCannotBeInRejectedDueToEventCancellationRule(this.CurrentStatus));
            CheckRule(new ApplicationStatusMustBeInProgressOrNotConsideredRule(this.CurrentStatus));
            if (durationInMinutes.HasValue)
            {
                CheckRule(new IntRules(15, 120, durationInMinutes.Value, nameof(DurationInMinutes)));
            }

            this.TypePerformance = typePerformance ?? this.TypePerformance;
            this.DurationInMinutes = durationInMinutes ?? this.DurationInMinutes;
            this.LastModifiedByApplicant = isApplicant;

            if ((this.CurrentStatus.Id == StatusApplication.NotConsidered.Id) && !isApplicant)
            {
                this.CurrentStatus = StatusApplication.InProgress;
            }
        }
    }
}