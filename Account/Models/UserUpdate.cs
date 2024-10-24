using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Account.Models
{
    public class UserUpdate
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Password { get; set; }
    }
}
