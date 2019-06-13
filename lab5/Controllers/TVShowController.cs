using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using lab5.Data;
using lab5.Models;
using lab5.ViewModels;
using lab5.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace lab5.Controllers
{
    [CatchExceptionFilter]
    [Authorize(Roles = "user, admin")]
    public class TVShowController : Controller
    {
        private int pageSize = 5;
        private Context _context;
        private TVShow _tvShow = new TVShow
        {
            NameShow = "",
            Duration = "",
            Rating = "",
            DescriptionShow = "",
            GenreID = null
        };

        public TVShowController(Context tvShowContext)
        {
            _context = tvShowContext;
        }

        [HttpGet]
        public IActionResult Index(int page, SortState sortOrder)
        {
            TVShow sessionTVShow = HttpContext.Session.GetObject<TVShow>("TVShow");
            string sessionSortState = HttpContext.Session.GetString("SortStateTVShow");

            if (sessionTVShow != null)
            {
                _tvShow = sessionTVShow;
            }

            if (sessionSortState != null)
                if (sortOrder == SortState.No)
                    sortOrder = (SortState)Enum.Parse(typeof(SortState), sessionSortState);
            ViewData["NameSort"] = sortOrder == SortState.NameDesc ? SortState.NameAsc : SortState.NameDesc;
            HttpContext.Session.SetString("SortStateTVShow", sortOrder.ToString());

            IQueryable<TVShow> tvShows = Sort(_context.TVShows, sortOrder,
                _tvShow.NameShow, (int)page);
            PageViewModel pageViewModel = new PageViewModel(NoteCount(_context.TVShows,
                _tvShow.NameShow), page, pageSize);

            foreach (var item in tvShows)
            {
                item.Genre = _context.Genres.Where(o => o.GenreID == item.GenreID).FirstOrDefault();
            }

            TVShowsViewModel tvShowsView = new TVShowsViewModel
            {
                TVShowViewModel = _tvShow,
                PageViewModel = tvShows,
                Pages = pageViewModel
            };

            return View(tvShowsView);
        }

        [HttpPost]
        public IActionResult Index(TVShow tvShow, int page)
        {
            var sessionSortState = HttpContext.Session.GetString("SortStateTVShow");
            SortState sortOrder = new SortState();
            if (sessionSortState != null)
                sortOrder = (SortState)Enum.Parse(typeof(SortState), sessionSortState);
            
            IQueryable<TVShow> tvShows = Sort(_context.TVShows, sortOrder,
                tvShow.NameShow, (int)page);
            HttpContext.Session.SetObject("TVShow", tvShow);

            PageViewModel pageViewModel = new PageViewModel(NoteCount(_context.TVShows,
                   _tvShow.NameShow), page, pageSize);

            TVShowsViewModel tvShowsView = new TVShowsViewModel
            {
                TVShowViewModel = tvShow,
                PageViewModel = tvShows,
                Pages = pageViewModel
            };

            return View(tvShowsView);
        }

        private IQueryable<TVShow> Sort(IQueryable<TVShow> tvShows,
            SortState sortOrder, string name, int page)
        {
            switch (sortOrder)
            {
                case SortState.NameAsc:
                    tvShows = tvShows.OrderBy(s => s.NameShow);
                    break;
                case SortState.NameDesc:
                    tvShows = tvShows.OrderByDescending(s => s.NameShow);
                    break;
            }
            tvShows = tvShows.Where(o => o.NameShow.Contains(name ?? ""))
                .Skip(page * pageSize).Take(pageSize);
            return tvShows;
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tvShow = _context.TVShows.Where(p => p.TVShowID == id).Single();
            if (tvShow == null)
            {
                return NotFound();
            }

            return View(tvShow);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tvShow = _context.TVShows.Where(p => p.TVShowID == id).Single();
            if (tvShow == null)
            {
                return NotFound();
            }

            return View(tvShow);
        }

        [HttpPost]
        public IActionResult Edit(TVShow tvShow)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tvShow);
                    _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TVShowExists(tvShow.TVShowID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(tvShow);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var tvShow = _context.TVShows.Where(p => p.TVShowID == id).FirstOrDefault(); ;
            if (tvShow == null)
            {
                return NotFound();
            }

            try
            {
                _context.TVShows.Remove(tvShow);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TVShowExists(tvShow.TVShowID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(TVShow tvShow, Genre genre)
        {
            tvShow.GenreID = _context.Genres.Where(o => o.NameGenre == genre.NameGenre).FirstOrDefault().GenreID;
            if (tvShow == null)
            {
                return View();
            }

            try
            {
                _context.TVShows.Add(tvShow);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TVShowExists(tvShow.TVShowID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Index");
        }

        private bool TVShowExists(int id)
        {
            return _context.TVShows.Any(e => e.TVShowID == id);
        }

        private int NoteCount(IQueryable<TVShow> tvShows, string name)
        {
            return tvShows.Where(o => o.NameShow.Contains(name ?? ""))
                .Count();
        }
    }
}