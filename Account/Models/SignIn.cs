using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Account.Models
{
    public class SignIn
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Tokens { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiry { get; set; }
    }
}
