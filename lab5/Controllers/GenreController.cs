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
    public class GenreController : Controller
    {
        private int pageSize = 5;
        private Context _context;
        private Genre _genre = new Genre
        {
            NameGenre = "",
            DescriptionOfGenre = ""
        };

        public GenreController(Context genreContext) {
            _context = genreContext;
        }

        [HttpGet]
        public IActionResult Index(int page, SortState sortOrder)
        {
            Genre sessionGenre = HttpContext.Session.GetObject<Genre>("Genre");
            string sessionSortState = HttpContext.Session.GetString("SortStateGenre");

            if (sessionGenre != null)
            {
                _genre = sessionGenre;
            }

            if (sessionSortState != null)
                if (sortOrder == SortState.No)
                    sortOrder = (SortState)Enum.Parse(typeof(SortState), sessionSortState);
            ViewData["NameSort"] = sortOrder == SortState.NameDesc ? SortState.NameAsc : SortState.NameDesc;
            HttpContext.Session.SetString("SortStateGenre", sortOrder.ToString());

            IQueryable<Genre> genres = Sort(_context.Genres, sortOrder,
                _genre.NameGenre, (int)page);
            PageViewModel pageViewModel = new PageViewModel(NoteCount(_context.Genres,
                _genre.NameGenre), page, pageSize);
            GenresViewModel genresView = new GenresViewModel
            {
                GenreViewModel = _genre,
                PageViewModel = genres,
                Pages = pageViewModel
            };

            return View(genresView);
        }

        [HttpPost]
        public IActionResult Index(Genre genre, int page)
        {
            var sessionSortState = HttpContext.Session.GetString("SortStateGenre");
            SortState sortOrder = new SortState();
            if (sessionSortState != null)
                sortOrder = (SortState)Enum.Parse(typeof(SortState), sessionSortState);

            IQueryable<Genre> genres = Sort(_context.Genres, sortOrder,
                _genre.NameGenre, (int)page);
            PageViewModel pageViewModel = new PageViewModel(NoteCount(_context.Genres,
                _genre.NameGenre), page, pageSize);
            GenresViewModel genresView = new GenresViewModel
            {
                GenreViewModel = _genre,
                PageViewModel = genres,
                PageNumber = page
            };

            return View(genresView);
        }

        private IQueryable<Genre> Sort(IQueryable<Genre> genres,
            SortState sortOrder, string name, int page)
        {
            switch (sortOrder)
            {
                case SortState.NameAsc:
                    genres = genres.OrderBy(s => s.NameGenre);
                    break;
                case SortState.NameDesc:
                    genres = genres.OrderByDescending(s => s.NameGenre);
                    break;
            }
            genres = genres.Where(o => o.NameGenre.Contains(name ?? "")).Skip(page * pageSize).Take(pageSize);
            return genres;
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = _context.Genres.Where(p => p.GenreID == id).Single();
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = _context.Genres.Where(p => p.GenreID == id).Single();
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        [HttpPost]
        public IActionResult Edit(Genre genre)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genre);
                    _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenreExists(genre.GenreID))
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
            return View(genre);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var genre = _context.Genres.Where(p => p.GenreID == id).FirstOrDefault(); ;
            if (genre == null)
            {
                return NotFound();
            }

            try
            {
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(genre.GenreID))
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
        public async Task<IActionResult> Add(Genre genre)
        {
            if (genre == null)
            {
                return View();
            }

            try
            {
                _context.Genres.Add(genre);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(genre.GenreID))
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

        private bool GenreExists(int id)
        {
            return _context.Genres.Any(e => e.GenreID == id);
        }

        private int NoteCount(IQueryable<Genre> genres, string name)
        {
            return genres.Where(o => o.NameGenre.Contains(name ?? ""))
                .Count();
        }
    }
}