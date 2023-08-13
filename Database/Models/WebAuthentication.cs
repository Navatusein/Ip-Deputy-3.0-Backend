using System.Security.Cryptography;

namespace IpDeputyApi.Database.Models
{
    public class WebAuthentication
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Login { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string PasswordSalt { get; set; } = null!;

        public virtual Student Student { get; set; } = null!;

        public bool VerifyPasswordHash(string password)
        {
            byte[] passwordHashBytes = Convert.FromBase64String(this.PasswordHash);
            byte[] passwordSaltBytes = Convert.FromBase64String(this.PasswordSalt);

            using var hmac = new HMACSHA512(passwordSaltBytes);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHashBytes);
        }

        public void CreatePasswordHash(string password)
        {
            using var hmac = new HMACSHA512();
            this.PasswordSalt = Convert.ToBase64String(hmac.Key);
            this.PasswordHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        }
    }
}
