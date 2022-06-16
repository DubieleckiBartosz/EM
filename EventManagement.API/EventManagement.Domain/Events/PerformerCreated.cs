using System.Text.Json.Serialization;
using EventManagement.Domain.Base;

namespace EventManagement.Domain.Events
{
    public class PerformerCreated : IDomainNotification
    {
        public int UserId { get; }
        public string PerformerName { get; }
        [JsonConstructor]
        public PerformerCreated(string performerName, int userId)
        {
            this.PerformerName = performerName;
            this.UserId = userId;
        }
    }
}
