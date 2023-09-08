namespace IpDeputyApi.Dto.Frontend
{
    public class CoupleTimeDto
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public TimeOnly TimeStart { get; set; }
        public TimeOnly TimeEnd { get; set; }
    }
}
