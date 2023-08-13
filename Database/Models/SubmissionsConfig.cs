namespace IpDeputyApi.Database.Models
{
    public class SubmissionsConfig
    {
        public int Id { get; set; }
        public int? SubgroupId { get; set; }
        public int? SubjectId { get; set; }
        public int? SubjectTypeId { get; set; }
        public string? CustomType { get; set; }
        public string? CustomName { get; set; }

        public virtual Subgroup? Subgroup { get; set; }
        public virtual Subject? Subject { get; set; }
        public virtual SubjectType? SubjectType { get; set; }
        public virtual IEnumerable<SubmissionWork> SubmissionWorks { get; } = new List<SubmissionWork>();
        public virtual IEnumerable<SubmissionStudent> SubmissionStudents { get; } = new List<SubmissionStudent>();
    }
}
