using IpDeputyApi.Dto.Frontend;

namespace IpDeputyApi.Dto.Bot;

public class ScheduleWeekDto
{
    public IEnumerable<string> CoupleTimes { get; set; } = new List<string>();
    public IEnumerable<ScheduleDayDto> ScheduleDays { get; set; } = new List<ScheduleDayDto>();
}