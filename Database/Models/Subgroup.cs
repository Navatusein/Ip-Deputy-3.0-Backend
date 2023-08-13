namespace IpDeputyApi.Database.Models
{
    public class Subgroup
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Name { get; set; } = null!;

        public virtual IEnumerable<Student> Students { get; } = new List<Student>();
        public virtual IEnumerable<SubmissionsConfig> SubmissionsConfigs { get; } = new List<SubmissionsConfig>();
    }
}
