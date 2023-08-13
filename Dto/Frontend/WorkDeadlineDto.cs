namespace IpDeputyApi.Dto.Frontend
{
    public class WorkDeadlineDto
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int SubjectTypeId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? Deadline { get; set; }
    }
}
