namespace IpDeputyApi.Dto.Frontend
{
    public class UserDto
    {
        public int StudentId { get; set; }
        public string UserName { get; set; } = null!;
        public string JwtToken { get; set; } = null!;
    }
}
