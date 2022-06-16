using EventManagement.Application.Models.Dao.EventDAOs;

namespace EventManagement.Application.Models.Dao.EventApplicationDAOs
{
    public class EventApplicationWithApplicantDataDao : EventApplicationDao
    {
        public int NumberOfPeople { get; set; }
        public string PerformerMail { get; set; }
    }
}