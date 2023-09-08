namespace IpDeputyApi.Dto.Frontend
{
    public class CoupleDto
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int SubjectTypeId { get; set; }
        public int DayOfWeekId { get; set; }
        public int CoupleTimeId { get; set; }
        public int? SubgroupId { get; set; }
        public int TeacherId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool IsRolling { get; set; }
        public string? Cabinet { get; set; }
        public string? AdditionalInformation { get; set; }
        public string? Link { get; set; }


        public IEnumerable<CoupleDateDto> AdditionalDates { get; set; } = new List<CoupleDateDto>();
        public IEnumerable<CoupleDateDto> RemovedDates { get; set; } = new List<CoupleDateDto>();
    }
}
