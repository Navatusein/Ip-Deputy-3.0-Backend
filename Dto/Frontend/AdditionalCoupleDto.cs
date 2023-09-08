namespace IpDeputyApi.Dto.Frontend
{
    public class AdditionalCoupleDto
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int SubjectTypeId { get; set; }
        public int DayOfWeekId { get; set; }
        public int CoupleTimeId { get; set; }
        public int? SubgroupId { get; set; }
        public int TeacherId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string? Cabinet { get; set; }
        public string? AdditionalInformation { get; set; }
        public string? Link { get; set; }
    }
}
