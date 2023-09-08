namespace IpDeputyApi.Database.Models
{
    public class CoupleDate
    {
        public int Id { get; set; }
        public int CoupleId { get; set; }
        public DateOnly Date { get; set; }
        public bool IsRemovedDate { get; set; }

        public virtual Couple Couple { get; set; } = null!;
    }
}
