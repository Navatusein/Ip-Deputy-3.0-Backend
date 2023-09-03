namespace IpDeputyApi.Dto.Bot;

public class StudentInformationDto
{
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Patronymic { get; set; } = null!;
    public string? Subgroup { get; set; }
    public string? ContactPhone { get; set; }
    public string TelegramPhone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FitEmail { get; set; } = null!;
    public string? TelegramNickname { get; set; }
    public string Birthday { get; set; } = null!;
}