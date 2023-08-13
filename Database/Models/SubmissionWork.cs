namespace IpDeputyApi.Database.Models
{
    public class SubmissionWork
    {
        public int Id { get; set; }
        public int SubmissionConfigId { get; set; }
        public string Name { get; set; } = null!;

        public virtual SubmissionsConfig SubmissionsConfig { get; set; } = null!;
        public virtual IEnumerable<SubmissionStudent> SubmissionStudents { get; } = new List<SubmissionStudent>();
    }
}
