using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace lab5.Models
{
    public class ScheduleForWeek
    {
        [Key]
        public int ScheduleForWeekID { get; set; }
        [Display(Name = "Время начала")]
        public string StartTime { get; set; }
        [Display(Name = "Приглашенные гости")]
        public string GuestsEmployees { get; set; }
        [Display(Name = "TVShowID")]
        public int? TVShowID { get; set; }

        public virtual TVShow TVShow { get; set; }
        public virtual ICollection<CitizensAppeal> CitizensAppeals { get; set; }

        public ScheduleForWeek() { }

        public ScheduleForWeek(int ScheduleForWeekID, string StartTime, string GuestsEmployees, int? TVShowID)
        {
            this.ScheduleForWeekID = ScheduleForWeekID;
            this.StartTime = StartTime;
            this.GuestsEmployees = GuestsEmployees;
            this.TVShowID = TVShowID;
        }

        public override bool Equals(object obj)
        {
            var item = obj as ScheduleForWeek;

            if (obj == null)
            {
                return false;
            }
            if (obj == this)
            {
                return true;
            }

            return this.ScheduleForWeekID == item.ScheduleForWeekID;
        }

        public override int GetHashCode()
        {
            return this.ScheduleForWeekID.GetHashCode();
        }
    }
}
