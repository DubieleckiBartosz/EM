using System;

namespace EventManagement.Application.Models.Dao.ProposalDAOs
{
    public class ProposalDao
    {
        public int Id { get; set; }
        public int PerformerId { get; set; }
        public int EventId { get; set; }
        public string Message { get; set; }
        public DateTime ActiveTo { get; set; }
    }
}
