namespace IpDeputyApi.Database.Models
{
    public class WorkDeadline
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int SubjectTypeId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? Deadline { get; set; }

        public virtual Subject Subject { get; set; } = null!;
        public virtual SubjectType SubjectType { get; set; } = null!;
        public virtual IEnumerable<WorkDeadline> WorkDeadlines { get; } = new List<WorkDeadline>();
    }
}
