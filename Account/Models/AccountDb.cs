using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Account.Models
{
    public class AccountDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string[] Roles { get; set; }
    }
}
