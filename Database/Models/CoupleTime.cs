namespace IpDeputyApi.Database.Models
{
    public class CoupleTime
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public TimeOnly TimeStart { get; set; }
        public TimeOnly TimeEnd { get; set; }

        public virtual IEnumerable<Couple> Couples { get; } = new List<Couple>();
    }
}
