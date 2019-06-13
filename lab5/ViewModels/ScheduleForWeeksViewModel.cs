using System.Linq;
using lab5.Models;

namespace lab5.ViewModels
{
    public class ScheduleForWeeksViewModel
    {
        public ScheduleForWeek ScheduleForWeekViewModel { get; set; }
        public IQueryable<ScheduleForWeek> PageViewModel { get; set; }
        public PageViewModel Pages { get; set; }
        public int PageNumber { get; set; }     
    }
}
