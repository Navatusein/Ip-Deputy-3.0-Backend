namespace IpDeputyApi.Database.Models
{
    public class WorkDeadlineStudent
    {
        public int Id { get; set; }
        public int WorkDeadlineId { get; set; }
        public int StudentId { get; set; }

        public virtual WorkDeadline WorkDeadline { get; set; } = null!;
        public virtual Student Student { get; set; } = null!;
    }
}
