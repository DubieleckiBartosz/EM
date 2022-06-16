using System;
using System.Collections.Generic;
using System.Linq;
using EventManagement.Domain.Base.EnumerationClasses;
using EventManagement.Domain.DDD;
using EventManagement.Domain.Events;
using EventManagement.Domain.Rules.EventRules;
using EventManagement.Domain.ValueObjects;

namespace EventManagement.Domain.Entities
{
    public class Event : AggregateRoot
    {
        public EventName EventName { get; private set; }
        public EventDescription Description { get; private set; }
        public EventTime EventTime { get; private set; }
        public PlaceType PlaceType { get; private set; }
        public bool RecurringEvent { get; private set; }
        public Address EventAddress { get; private set; }
        public EventCategory Category { get; private set; }
        public EventType EventType { get; private set; }
        public List<EventImage> Images { get; private set; }
        public List<EventOpinion> Opinions { get; private set; }
        public List<EventApplication> Applications { get; private set; }
        public EventCurrentStatus CurrentStatus { get; private set; }

        private Event(EventName eventName, EventDescription description, EventTime time, PlaceType placeType,
            bool recurringEvent, Address eventAddress, EventCategory category, EventType eventType)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (eventName == null)
            {
                throw new ArgumentNullException(nameof(eventName));
            }

            if (time == null)
            {
                throw new ArgumentNullException(nameof(time));
            }

            if (eventAddress == null)
            {
                throw new ArgumentNullException(nameof(eventAddress));
            }

            if (eventType == null)
            {
                throw new ArgumentNullException(nameof(eventType));
            }

            this.EventName = eventName;
            this.Description = description;
            this.EventTime = time;
            this.PlaceType = placeType;
            this.RecurringEvent = recurringEvent;
            this.EventAddress = eventAddress;
            this.Category = category;
            this.EventType = eventType;
            this.CurrentStatus = EventCurrentStatus.Active;
            this.Images = new List<EventImage>();
            this.Opinions = new List<EventOpinion>();
            this.Applications = new List<EventApplication>();
            this.AddDomainEvent(new EventCreated(this));
        }

        private Event(int id, List<EventImage> images, List<EventOpinion> opinions,
            List<EventApplication> applications, EventName eventName,
            EventDescription description, EventTime time,
            PlaceType placeType, EventCurrentStatus currentStatus,
            bool recurringEvent, Address eventAddress, EventCategory category, EventType eventType)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (eventName == null)
            {
                throw new ArgumentNullException(nameof(eventName));
            }

            if (time == null)
            {
                throw new ArgumentNullException(nameof(time));
            }

            if (eventAddress == null)
            {
                throw new ArgumentNullException(nameof(eventAddress));
            }

            if (eventType == null)
            {
                throw new ArgumentNullException(nameof(eventType));
            }

            if (currentStatus == null)
            {
                throw new ArgumentNullException(nameof(currentStatus));
            }

            this.Id = id;
            this.EventName = eventName;
            this.Description = description;
            this.EventTime = time;
            this.PlaceType = placeType;
            this.RecurringEvent = recurringEvent;
            this.EventAddress = eventAddress;
            this.Category = category;
            this.EventType = eventType;
            this.CurrentStatus = currentStatus;
            this.Images = images ?? new List<EventImage>();
            this.Opinions = opinions ?? new List<EventOpinion>();
            this.Applications = applications ?? new List<EventApplication>();
        }

        public static Event LoadEvent(int id, List<EventImage> images, List<EventOpinion> opinions,
            List<EventApplication> applications, EventName eventName, EventDescription description, EventTime time,
            PlaceType placeType, EventCurrentStatus currentStatus,
            bool recurringEvent, Address eventAddress, EventCategory category, EventType eventType)
        {
            return new Event(id, images, opinions, applications, eventName, description, time, placeType, currentStatus,
                recurringEvent,
                eventAddress, category, eventType);
        }

        public static Event Create(EventName eventName, EventDescription description, EventTime time,
            PlaceType placeType,
            bool recurringEvent, Address eventAddress, EventCategory category, EventType eventType)
        {
            return new Event(eventName, description, time, placeType, recurringEvent, eventAddress, category,
                eventType);
        }

        public void Update(EventTime eventTime, bool? recurringEvent = null,
            EventDescription eventDescription = null,
            PlaceType placeType = null, EventCategory categoryType = null,
            EventType eventType = null)
        {
            CheckRule(new EventStatusMustBeActiveRule(this.CurrentStatus));
            if (eventTime.StartDate < this.EventTime.StartDate)
            {
                CheckRule(new TimeAllowsModificationRule(this.EventTime.StartDate, 14));
            }

            CheckRule(new UpdatingEventTimeShouldBeEarlyEnoughRule(eventTime, this.EventTime.StartDate));

            this.Description = eventDescription ?? this.Description;
            this.EventTime = eventTime ?? this.EventTime;
            this.RecurringEvent = recurringEvent ?? this.RecurringEvent;
            this.PlaceType = placeType ?? this.PlaceType;
            this.Category = categoryType ?? this.Category;
            this.EventType = eventType ?? this.EventType;

            this.AddDomainEvent(new EventChanged(this.Id));
        }

        public void ChangeVisibility()
        {
            CheckRule(new EventCannotBeCanceledRule(this.CurrentStatus));
            CheckRule(new TimeAllowsModificationRule(this.EventTime.StartDate, 14));

            this.CurrentStatus = this.CurrentStatus == EventCurrentStatus.Suspended
                ? EventCurrentStatus.Active
                : EventCurrentStatus.Suspended;

            this.AddDomainEvent(new EventStatusChanged(this.Id, this.CurrentStatus, this.EventName.Name));
        }

        public void ChangeStatusToRealized()
        {
            CheckRule(new EventCannotBeCanceledRule(this.CurrentStatus));
            CheckRule(new EventMustBeCompletedRule(this.EventTime));

            this.CurrentStatus = EventCurrentStatus.Realized;
        }

        public bool IsCanceled() => this.CurrentStatus.Id == EventCurrentStatus.Cancelled.Id;
        public bool IsActive() => this.CurrentStatus.Id == EventCurrentStatus.Active.Id;

        public void Cancel()
        {
            CheckRule(new EventCannotBeCanceledRule(this.CurrentStatus));

            var daysWithoutCancellation = this.CurrentStatus.Id == EventCurrentStatus.Active.Id ? 14 : 0;
            
            CheckRule(new EventTimeCancelRule(this.EventTime.StartDate, daysWithoutCancellation));

            this.CurrentStatus = EventCurrentStatus.Cancelled;

            this.AddDomainEvent(new EventCanceled(this.Id, this.EventName.Name));
        }

        public EventImage AddNewImage(string path, string title, bool isMain, ImageDescription imageDescription)
        {
            CheckRule(new EventStatusMustBeActiveRule(this.CurrentStatus));

            var newImage = EventImage.Create(this.Id, path, title, isMain, imageDescription);
            if (newImage == null)
            {
                throw new ArgumentNullException(nameof(newImage));
            }

            CheckRule(new ImageMainRule(this.Images, isMain));

            if (this.Images.Any())
            {
                var titleList = this.Images.Select(_ => _.ImageTitle).ToList();
                CheckRule(new RepeatImagesRule(titleList, title));
            }

            if (this.Images.Any() && isMain)
            {
                var currentMainImage = this.Images.First(_ => _.IsMain);
                currentMainImage?.ChangeStatusMain();
                this.AddDomainEvent(new MainImageChanged(this.Id, currentMainImage.Id, currentMainImage.IsMain));
            }

            this.Images.Add(newImage);

            return newImage;
        }

        public EventOpinion AddNewEventOpinion(CommentValue comment, int stars, int? userId)
        {
            var opinion = EventOpinion.Create(this.Id, comment, stars, userId);
            if (opinion == null)
            {
                throw new ArgumentNullException(nameof(opinion));
            }

            return opinion;
        }

        public void RemoveOpinion(int opinionId, int attemptToRemoveBy, bool hasSuperAccess)
        {
            var opinion = this.GetOpinionById(opinionId);
            if (opinion == null)
            {
                throw new ArgumentNullException(nameof(opinion));
            }

            CheckRule(new OpinionCanBeRemoveRule(opinion.UserId, attemptToRemoveBy, hasSuperAccess));

            this.Opinions.Remove(opinion);

            this.AddDomainEvent(new OpinionRemoved(opinion.Id));
        }

        public void UpdateApplication(EventApplication application)
        {
            CheckRule(new EventCannotBeCanceledRule(this.CurrentStatus));

            var applications =
                this.Applications.Where(_ => _.CurrentStatus.Id == StatusApplication.ConsideredPositively.Id)?.ToList();

            CheckRule(new ApplicationCanBeAcceptedRules(applications, application.DurationInMinutes,
                this.EventTime.StartDate, this.EventTime.EndDate));

            var applicationToUpdate = GetApplicationById(application.Id);

            if (applicationToUpdate == null)
            {
                throw new ArgumentException($"Application {application.Id} was not found for the event.");
            }

            applicationToUpdate = application;

            this.AddDomainEvent(new ApplicationChanged(applicationToUpdate));
        }

        public void AcceptApplication(int applicationId,
            bool isApplicant, Performer performer)
        {
            CheckRule(new EventCannotBeCanceledRule(this.CurrentStatus));

            var application = this.GetApplicationById(applicationId);
            if (application == null || (isApplicant && performer.Id != application.PerformerId))
            {
                throw new ArgumentException($"Application {applicationId} not found.");
            }

            var applications =
                this.Applications.Where(_ => _.CurrentStatus.Id == StatusApplication.ConsideredPositively.Id)?.ToList();

            CheckRule(new ApplicationCanBeAcceptedRules(applications, application.DurationInMinutes,
                this.EventTime.StartDate, this.EventTime.EndDate));
            CheckRule(new TimeAllowsModificationRule(this.EventTime.StartDate, 3));

            if (isApplicant)
            {
                CheckRule(new ApplicantHasAccessWhenStatusIsInProgressRule(
                    currentStatusApplication: application.CurrentStatus));
            }

            var newStatus = StatusApplication.ConsideredPositively;
            application?.ChangeStatus(newStatus, isApplicant);
            this.AddDomainEvent(new ApplicationAccepted(this.Id, application));
        }

        public void RejectApplication(int applicationId, bool isApplicant, Performer performer)
        {
            CheckRule(new EventCannotBeCanceledRule(this.CurrentStatus));

            var application = this.GetApplicationById(applicationId);
            if (application == null || (isApplicant && performer.Id != application.PerformerId))
            {
                throw new ArgumentException($"Application {applicationId} not found.");
            }

            CheckRule(new RejectApplicationRule(application));
            CheckRule(new TimeAllowsModificationRule(this.EventTime.StartDate, 3));

            if (isApplicant)
            {
                CheckRule(new ApplicantHasAccessWhenStatusIsInProgressRule(application.CurrentStatus));
            }

            var newStatus = StatusApplication.Rejected;
            application?.ChangeStatus(newStatus, isApplicant);

            this.AddDomainEvent(new ApplicationRejected(application));
        }

        public void CreateNewApplication(Performer performer, TypePerformance typePerformance, int durationInMinutes)
        {
            CheckRule(new EventCannotBeCanceledRule(this.CurrentStatus));
            CheckRule(new TimeAllowsModificationRule(this.EventTime.StartDate, 7));
            CheckRule(new OnlyOneApplicationPerEventRule(performer.Id, this.Applications));

            var eventApplication = EventApplication.Create(performer.Id, this.Id, typePerformance, durationInMinutes);
            this.Applications.Add(eventApplication);

            this.AddDomainEvent(new ApplicationCreated(eventApplication, performer));
        }

        private EventOpinion GetOpinionById(int opinionId) => this.Opinions.FirstOrDefault(_ => _.Id == opinionId);

        private EventApplication GetApplicationById(int applicationId) =>
            this.Applications.FirstOrDefault(_ => _.Id == applicationId);
    }
}