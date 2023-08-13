namespace IpDeputyApi.Dto.Frontend
{
    public class SubjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ShortName { get; set; } = null!;
        public int LaboratoryCount { get; set; }
        public int PracticalCount { get; set; }
    }
}
