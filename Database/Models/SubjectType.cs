namespace IpDeputyApi.Database.Models
{
    public class SubjectType
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ShortName { get; set; } = null!;

        public virtual IEnumerable<Couple> Couples { get; } = new List<Couple>();
        public virtual IEnumerable<AdditionalCouple> AdditionalCouples { get; } = new List<AdditionalCouple>();
        public virtual IEnumerable<SubmissionsConfig> SubmissionsConfigs { get; } = new List<SubmissionsConfig>();
        public virtual IEnumerable<WorkDeadline> WorkDeadlines { get; } = new List<WorkDeadline>();
    }
}
