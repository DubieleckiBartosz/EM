namespace EventManagement.Application.Models.Dto
{
    public class PerformerWithNumberOfPerformancesDto
    {
        public int Id { get; set; }
        public int NumberOfPeople { get; set; }
        public bool VIP { get; set; }
        public string PerformerName { get; set; }
        public string PerformerMail { get; set; }
        public int NumberPerformance { get; set; }
    }
}
