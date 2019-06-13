using System.Linq;
using lab5.Models;

namespace lab5.ViewModels
{
    public class TVShowsViewModel
    {
        public TVShow TVShowViewModel { get; set; }
        public IQueryable<TVShow> PageViewModel { get; set; }
        public PageViewModel Pages { get; set; }
        public int PageNumber { get; set; }
        public enum SortState
        {
            No,
            NameAsc,
            NameDesc,
        }
    }
}
