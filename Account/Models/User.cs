using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Account.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
