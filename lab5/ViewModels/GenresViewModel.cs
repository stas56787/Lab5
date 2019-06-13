using System.Linq;
using lab5.Models;

namespace lab5.ViewModels
{
    public class GenresViewModel
    {
        public Genre GenreViewModel { get; set; }
        public IQueryable<Genre> PageViewModel { get; set; }
        public PageViewModel Pages { get; set; }
        public int PageNumber { get; set; }
    }
    public enum SortState
    {
        No,
        NameAsc,
        NameDesc
    }
}
