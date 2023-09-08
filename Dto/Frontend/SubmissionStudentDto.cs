using IpDeputyApi.Database.Models;

namespace IpDeputyApi.Dto.Frontend
{
    public class SubmissionStudentDto
    {
        public int Id { get; set; }
        public int SubmissionWorkId { get; set; }
        public int StudentId { get; set; }
        public int SubmissionsConfigId { get; set; }
        public PreferredPosition PreferredPosition { get; set; }
    }
}
