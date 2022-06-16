namespace EventManagement.Application.Models.Dao.PerformerDAOs
{
    public class PerformerDao
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PerformerName { get; set; }
        public string PerformerMail { get; set; }
        public bool VIP { get; set; }
        public int NumberOfPeople { get; set; }
    }
}