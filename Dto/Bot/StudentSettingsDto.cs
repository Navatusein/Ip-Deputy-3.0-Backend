namespace IpDeputyApi.Dto.Bot;

public class StudentSettingsDto
{
    public int TelegramId { get; set; }
    public string Language { get; set; } = null!;
    public bool ScheduleCompact { get; set; }
    public bool RemindDeadlines { get; set; }
}