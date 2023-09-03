namespace IpDeputyApi.Dto.Bot;

public class ScheduleDayDto
{
    public string Date { get; set; } = null!;
    public IEnumerable<CoupleDataDto> Couples { get; set; } = new List<CoupleDataDto>();
}