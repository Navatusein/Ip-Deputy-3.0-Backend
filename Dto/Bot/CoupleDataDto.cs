namespace IpDeputyApi.Dto.Bot;

public class CoupleDataDto
{
    public string Subject { get; set; } = null!;
    public string SubjectType { get; set; } = null!;
    public int? CoupleIndex { get; set; }
    public bool IsMySubgroup { get; set; }
    public string? Time { get; set; }
    public string? Link { get; set; }
    public string? Cabinet { get; set; }
    public string? AdditionalInformation { get; set; }
}