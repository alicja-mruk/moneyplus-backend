using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AlicjowyBackendv3.Models
{
    public class UserModel
    {
        [Key]
        public int userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        [JsonIgnore]
        public byte[] passwordHash { get; set; }
        [JsonIgnore]
        public byte[] passwordSalt { get; set; }
        public int age { get; set; }
        public string email { get; set; }
        [JsonIgnore]
        public string refreshToken { get; set; }
        [JsonIgnore]
        public DateTime tokenExpires { get; set; }
    }
}
