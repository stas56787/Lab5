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
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace lab5.Controllers
{
    [CatchExceptionFilter]
    [Authorize(Roles = "user, admin")]
    public class ScheduleForWeekController : Controller
    {
        private int pageSize = 5;
        private Context _context;
        private ScheduleForWeek _scheduleForWeek = new ScheduleForWeek
        {
            StartTime = "",
            GuestsEmployees = "",
            TVShowID = null
        };

        public ScheduleForWeekController(Context ScheduleForWeekContext)
        {
            _context = ScheduleForWeekContext;
        }

        [HttpGet]
        public IActionResult Index(SortState sortOrder)
        {
            ScheduleForWeek sessionScheduleForWeek = HttpContext.Session.GetObject<ScheduleForWeek>("ScheduleForWeek");
            string sessionSortState = HttpContext.Session.GetString("SortStateScheduleForWeek");
            int? page = HttpContext.Session.GetInt32("ScheduleForWeekPage");
            if (page == null)
            {
                page = 0;
                HttpContext.Session.SetInt32("ScheduleForWeekPage", 0);
            }

            if (sessionScheduleForWeek != null)
            {
                _scheduleForWeek = sessionScheduleForWeek;
            }

            if (sessionSortState != null)
                if (sortOrder == SortState.No)
                    sortOrder = (SortState)Enum.Parse(typeof(SortState), sessionSortState);

            ViewData["NameSort"] = sortOrder == SortState.NameDesc ? SortState.NameAsc : SortState.NameDesc;
            HttpContext.Session.SetString("SortState", sortOrder.ToString());
            IQueryable<ScheduleForWeek> SchedulesForWeek = Sort(_context.SchedulesForWeek, sortOrder,
                _scheduleForWeek.StartTime, (int)page);

            foreach (var item in SchedulesForWeek)
            {
                item.TVShow = _context.TVShows.Where(o => o.TVShowID == item.TVShowID).FirstOrDefault();
            }

            ScheduleForWeeksViewModel SchedulesForWeekView = new ScheduleForWeeksViewModel
            {
                ScheduleForWeekViewModel = _scheduleForWeek,
                PageViewModel = SchedulesForWeek,
                PageNumber = (int)page
            };

            return View(SchedulesForWeekView);
        }

        [HttpPost]
        public IActionResult Index(ScheduleForWeek scheduleForWeek)
        {
            var sessionSortState = HttpContext.Session.GetString("SortStateScheduleForWeek");
            SortState sortOrder = new SortState();
            if (sessionSortState != null)
                sortOrder = (SortState)Enum.Parse(typeof(SortState), sessionSortState);

            int? page = HttpContext.Session.GetInt32("ScheduleForWeekPage");
            if (page == null)
            {
                page = 0;
                HttpContext.Session.SetInt32("ScheduleForWeekPage", 0);
            }

            IQueryable<ScheduleForWeek> schedulesForWeek = Sort(_context.SchedulesForWeek, sortOrder,
                 scheduleForWeek.StartTime, (int)page);
            HttpContext.Session.SetObject("Patient", scheduleForWeek);

            ScheduleForWeeksViewModel schedulesForWeekView = new ScheduleForWeeksViewModel
            {
                ScheduleForWeekViewModel = scheduleForWeek,
                PageViewModel = schedulesForWeek,
                PageNumber = (int)page
            };

            return View(schedulesForWeekView);
        }

        private IQueryable<ScheduleForWeek> Sort(IQueryable<ScheduleForWeek> SchedulesForWeek,
            SortState sortOrder, string name, int page)
        {
            switch (sortOrder)
            {
                case SortState.NameAsc:
                    SchedulesForWeek = SchedulesForWeek.OrderBy(s => s.StartTime);
                    break;
                case SortState.NameDesc:
                    SchedulesForWeek = SchedulesForWeek.OrderByDescending(s => s.StartTime);
                    break;
            }
            SchedulesForWeek = SchedulesForWeek.Where(o => o.StartTime.Contains(name ?? ""))
                .Skip(page * pageSize).Take(pageSize);
            return SchedulesForWeek;
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleForWeek = _context.SchedulesForWeek.Where(p => p.ScheduleForWeekID == id).Single();
            if (scheduleForWeek == null)
            {
                return NotFound();
            }

            return View(scheduleForWeek);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleForWeek = _context.SchedulesForWeek.Where(p => p.ScheduleForWeekID == id).Single();
            if (scheduleForWeek == null)
            {
                return NotFound();
            }

            return View(scheduleForWeek);
        }

        [HttpPost]
        public IActionResult Edit(ScheduleForWeek scheduleForWeek)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scheduleForWeek);
                    _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleForWeekExists(scheduleForWeek.ScheduleForWeekID))
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
            return View(scheduleForWeek);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var scheduleForWeek = _context.SchedulesForWeek.Where(p => p.ScheduleForWeekID == id).FirstOrDefault();
            if (scheduleForWeek == null)
            {
                return NotFound();
            }

            try
            {
                _context.SchedulesForWeek.Remove(scheduleForWeek);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScheduleForWeekExists(scheduleForWeek.ScheduleForWeekID))
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
        public async Task<IActionResult> Add(ScheduleForWeek scheduleForWeek, TVShow tvShow)
        {
            scheduleForWeek.TVShowID = _context.TVShows.Where(o => o.NameShow == tvShow.NameShow).FirstOrDefault().TVShowID;
            if (scheduleForWeek == null)
            {
                return View();
            }

            try
            {
                _context.SchedulesForWeek.Add(scheduleForWeek);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScheduleForWeekExists(scheduleForWeek.ScheduleForWeekID))
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

        private bool ScheduleForWeekExists(int id)
        {
            return _context.SchedulesForWeek.Any(e => e.ScheduleForWeekID == id);
        }

        private int NoteCount(IQueryable<ScheduleForWeek> scheduleForWeeks, string name)
        {
            return scheduleForWeeks.Where(o => o.StartTime.Contains(name ?? ""))
                .Count();
        }
    }
}