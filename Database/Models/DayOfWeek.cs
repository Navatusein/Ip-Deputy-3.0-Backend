using System.Numerics;

namespace IpDeputyApi.Database.Models
{
    public class DayOfWeek
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Name { get; set; } = null!;

        public virtual IEnumerable<Couple> Couples { get; } = new List<Couple>();
        public virtual IEnumerable<AdditionalCouple> AdditionalCouples { get; } = new List<AdditionalCouple>();
    }
}
