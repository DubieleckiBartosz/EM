using EventManagement.Application.Models.Dao.PerformerDAOs;

namespace EventManagement.Application.Models.Dao.EventApplicationDAOs
{
    public class EventApplicationDetailsDao 
    {
        public EventApplicationDao EventApplicationDao { get; set; }
        public PerformerDao PerformerDao { get; set; }
    }
}
