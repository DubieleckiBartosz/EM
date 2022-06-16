namespace EventManagement.Application.Models.Dto
{
    public class EventImageDto
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string ImageTitle { get; set; }
        public bool IsMain { get; set; }
        public string ImageDescription { get; set; }
    }
}
