namespace IpDeputyApi.Dto.Frontend
{
    public class TeacherDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public string? ContactPhone { get; set; }
        public string? Email { get; set; }
        public string? FitEmail { get; set; }
        public string? TelegramNickname { get; set; }
    }
}
