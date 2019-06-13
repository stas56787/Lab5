using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace lab5.Models
{
    public class CitizensAppeal
    {
        [Key]
        public int CitizensAppealID { get; set; }
        [Display(Name = "ФИО")]
        public string LFO { get; set; }
        [Display(Name = "Организация")]
        public string Organization { get; set; }
        [Display(Name = "Цель запроса")]
        public string GoalOfRequest { get; set; }
        [Display(Name = "ScheduleForWeekID")]
        public int? ScheduleForWeekID { get; set; }

        public virtual ScheduleForWeek ScheduleForWeek { get; set; }

        public CitizensAppeal() { }

        public CitizensAppeal(int CitizensAppealID, string LFO, string Organization, string GoalOfRequest, int? ScheduleForWeekID)
        {
            this.CitizensAppealID = CitizensAppealID;
            this.LFO = LFO;
            this.Organization = Organization;
            this.GoalOfRequest = GoalOfRequest;
            this.ScheduleForWeekID = ScheduleForWeekID;
        }

        public override bool Equals(object obj)
        {
            var item = obj as CitizensAppeal;

            if (obj == null)
            {
                return false;
            }
            if (obj == this)
            {
                return true;
            }

            return this.CitizensAppealID == item.CitizensAppealID;
        }

        public override int GetHashCode()
        {
            return this.CitizensAppealID.GetHashCode();
        }
    }
}
