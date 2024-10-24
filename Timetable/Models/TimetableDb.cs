using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Timetable.Models
{
    public class TimetableDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int Id { get; set; }
        public int HospitalId { get; set; }
        public int DoctorId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Room { get; set; }
    }
}
