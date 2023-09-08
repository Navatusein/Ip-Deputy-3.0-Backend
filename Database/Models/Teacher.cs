namespace IpDeputyApi.Database.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public string? ContactPhone { get; set; }
        public string? Email { get; set; }
        public string? FitEmail { get; set; }
        public string? TelegramNickname { get; set; }

        public virtual IEnumerable<Couple> Couples { get; } = new List<Couple>();
        public virtual IEnumerable<AdditionalCouple> AdditionalCouples { get; } = new List<AdditionalCouple>();
    }
}
