using System.ComponentModel.DataAnnotations;

namespace AlicjowyBackendv3.Models
{
    public class UserModel
    {
        [Key]
        public int user_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public int age { get; set; }
    }
}
