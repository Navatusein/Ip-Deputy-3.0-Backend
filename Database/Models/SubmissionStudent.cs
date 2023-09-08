namespace IpDeputyApi.Database.Models
{
    public enum PreferredPosition
    {
        InBegin,
        DoesNotMatter,
        InEnd
    }

    public class SubmissionStudent
    {
        public int Id { get; set; }
        public int SubmissionWorkId { get; set; }
        public int StudentId { get; set; }
        public int SubmissionsConfigId { get; set; }
        public PreferredPosition PreferredPosition { get; set; }


        public virtual Student Student { get; set; } = null!;
        public virtual SubmissionWork SubmissionWork { get; set; } = null!;
        public virtual SubmissionsConfig SubmissionsConfig { get; set; } = null!;
    }
}
