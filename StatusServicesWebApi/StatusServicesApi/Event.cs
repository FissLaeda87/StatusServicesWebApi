namespace StatusServicesApi
{
    public class Event
    {
        public int Id { get; set; }
        public string? Service { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsArchived { get; set; }
    }
}
