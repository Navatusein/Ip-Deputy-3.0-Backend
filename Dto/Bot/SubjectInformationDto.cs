namespace IpDeputyApi.Dto.Bot;

public class SubjectInformationDto
{
    public string Name { get; set; } = null!;
    public string ShortName { get; set; } = null!;
    public int LaboratoryDaysCount { get; set; }
    public int PracticalDaysCount { get; set; }
    public int LecturesDaysCount { get; set; }
    public int LaboratoryCount { get; set; }
    public int PracticalCount { get; set; }
}