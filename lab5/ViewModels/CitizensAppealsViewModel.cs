using System.Linq;
using lab5.Models;

namespace lab5.ViewModels
{
    public class CitizensAppealsViewModel
    {
        public CitizensAppeal CitizensAppealViewModel { get; set; }
        public IQueryable<CitizensAppeal> PageViewModel { get; set; }
        public PageViewModel Pages { get; set; }
        public int PageNumber { get; set; }
    }
}
