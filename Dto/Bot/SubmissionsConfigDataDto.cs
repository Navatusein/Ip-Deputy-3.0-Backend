namespace IpDeputyApi.Dto.Bot;

public class SubmissionsConfigDataDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string? Subgroup { get; set; }
    public IEnumerable<SubmissionDto> Submissions { get; set; } = new List<SubmissionDto>();
}