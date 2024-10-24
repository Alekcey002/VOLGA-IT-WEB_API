using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Timetable.Models
{
    public class AppointmentDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int USerId { get; set; }
        [JsonIgnore]
        public int TimetableId { get; set; }
        public DateTime Time { get; set; }
    }
}
