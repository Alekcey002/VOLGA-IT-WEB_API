using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Hospital.Models
{
    public class HospitalDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactPhone { get; set; }
        public string[] Rooms { get; set; }
    }
}
