namespace IpDeputyApi.Dto.Frontend
{
    public class StudentDto
    {
        public int Id { get; set; }
        public int? SubgroupId { get; set; }
        public int Index { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public string TelegramPhone { get; set; } = null!;
        public string? ContactPhone { get; set; }
        public string Email { get; set; } = null!;
        public string FitEmail { get; set; } = null!;
        public string? TelegramNickname { get; set; }
        public DateOnly Birthday { get; set; }
    }
}
