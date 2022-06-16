using System;
using System.Collections.Generic;
using EventManagement.Domain.DDD;
using EventManagement.Domain.Events;
using EventManagement.Domain.Rules;
using EventManagement.Domain.Rules.Common;
using EventManagement.Domain.Rules.PerformerRules;
using EventManagement.Domain.ValueObjects;

namespace EventManagement.Domain.Entities
{
    public class Performer : AggregateRoot
    {
        public int UserId { get; }
        public PerformerName PerformerName { get; }
        public bool VIP { get; private set; }
        public int NumberOfPeople { get; private set; }
        public string PerformerMail { get; private set; }
        public List<EventApplication> Applications { get; private set; }
        public List<PerformanceProposal> PerformanceProposals { get; private set; }

        private Performer(int userId, PerformerName performerName, int numberOfPeople, string performerMail)
        {
            if (performerName == null)
            {
                throw new ArgumentNullException(nameof(performerName));
            }

            if (performerMail == null)
            {
                throw new ArgumentNullException(nameof(performerMail));
            }

            this.UserId = userId;
            this.PerformerName = performerName;
            this.NumberOfPeople = numberOfPeople;
            this.VIP = false;
            this.PerformerMail = performerMail;
            this.Applications = new List<EventApplication>();
            this.PerformanceProposals = new List<PerformanceProposal>();
            this.AddDomainEvent(new PerformerCreated(this.PerformerName.NameValue, userId));
        }

        private Performer(int id, int userId, bool vip, PerformerName performerName, int numberOfPeople,
            string performerMail, List<PerformanceProposal> proposals)
        {
            if (performerName == null)
            {
                throw new ArgumentNullException(nameof(performerName));
            }

            if (performerMail == null)
            {
                throw new ArgumentNullException(nameof(performerMail));
            }

            this.Id = id;
            this.UserId = userId;
            this.PerformerName = performerName;
            this.PerformerMail = performerMail;
            this.NumberOfPeople = numberOfPeople;
            this.VIP = vip;
            this.PerformanceProposals = proposals ?? new List<PerformanceProposal>(); 
        }

        public static Performer Load(int id, int userId, bool vip, PerformerName performerName, int numberOfPeople,
            string performerMail, List<PerformanceProposal> proposals = null)
        {
            return new Performer(id, userId, vip, performerName, numberOfPeople, performerMail, proposals);
        }

        public static Performer Create(int userId, PerformerName performerName, int numberOfPeople,
            string performerMail)
        {
            CheckRule(new IntRules(1, 25, numberOfPeople, "People from the team"));
            return new Performer(userId, performerName, numberOfPeople, performerMail);
        }

        public void UpdateStatusVip()
        {
            if (!this.VIP)
            {
                this.VIP = true;
            }
        }

        public void ChangeData(int? numberOfPeople = null, string performerMail = null)
        {
            if (numberOfPeople.HasValue)
            {
                var value = numberOfPeople.Value;
                CheckRule(new IntRules(1, 25, value, "People from the team"));
                this.NumberOfPeople = value;
            }

            this.PerformerMail = performerMail ?? this.PerformerMail;
        }

        public PerformanceProposal AddNewProposal(int eventId, Message message, DateTime activeTo)
        {
            CheckRule(new OnlyOneProposalForPerformerPerEventRule(this.PerformanceProposals, eventId));
            return PerformanceProposal.Create(this.Id, eventId, message, activeTo);
        }
    }
}