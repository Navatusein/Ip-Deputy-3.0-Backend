namespace IpDeputyApi.Dto.Frontend
{
    public class SubmissionsConfigDto
    {
        public int Id { get; set; }
        public int? SubgroupId { get; set; }
        public int? SubjectId { get; set; }
        public int? SubjectTypeId { get; set; }
        public string? CustomType { get; set; } = null!;
        public IEnumerable<SubmissionWorkDto> SubmissionWorks { get; set; } = new List<SubmissionWorkDto>();
        public IEnumerable<SubmissionStudentDto> SubmissionStudents { get; set; } = new List<SubmissionStudentDto>();
    }
}
