using System.Numerics;

namespace IpDeputyApi.Dto.Frontend
{
    public class DayOfWeekDto
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Name { get; set; } = null!;
    }
}
