using AlicjowyBackendv3.Models;
using System.Text.Json.Serialization;

namespace AlicjowyBackendv3.Helpers
{
    public class RegisterDataTransferObject : User
    {
        public string password { get; set; }
        [JsonIgnore]
        public string id { get; set; }
    }
}
