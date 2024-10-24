using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Document.Models
{
    public class DocumentDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int PacientId { get; set; }
        public int HospitalId { get; set; }
        public int DoctorId { get; set; }
        public string Room { get; set; }
        public string Data { get; set; }
    }
}
