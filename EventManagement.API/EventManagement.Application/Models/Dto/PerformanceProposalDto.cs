using System;

namespace EventManagement.Application.Models.Dto
{
    public class PerformanceProposalDto
    {
        public int Id { get; set; }
        public int PerformerId { get; set; }
        public int EventId { get; set; }
        public string Message { get; set; }
        public DateTime ActiveTo { get; set; }
        public bool IsOld => ActiveTo < DateTime.Now;
    }
}
