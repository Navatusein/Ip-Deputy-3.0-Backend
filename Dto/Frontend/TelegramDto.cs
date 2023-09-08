namespace IpDeputyApi.Dto.Frontend
{
    public class TelegramDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int TelegramId { get; set; }
        public string Language { get; set; } = null!;
        public bool ScheduleCompact { get; set; }
        public bool RemindDeadlines { get; set; }
        public DateOnly? LastCongratulations { get; set; }
        public DateTime? LastActivity { get; set; }
    }
}
