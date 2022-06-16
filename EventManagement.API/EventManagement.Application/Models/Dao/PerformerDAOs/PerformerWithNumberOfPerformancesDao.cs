namespace EventManagement.Application.Models.Dao.PerformerDAOs
{
    public class PerformerWithNumberOfPerformancesDao
    {
        public int Id { get; set; }
        public int NumberOfPeople { get; set; }
        public bool VIP { get; set; }
        public string PerformerName { get; set; }
        public string PerformerMail { get; set; }
        public int NumberPerformance { get; set; }
    }
}
