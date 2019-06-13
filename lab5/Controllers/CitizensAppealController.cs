using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using lab5.Data;
using lab5.Models;
using lab5.ViewModels;
using Microsoft.AspNetCore.Authorization;
using lab5.Filters;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace lab5.Controllers
{
    [CatchExceptionFilter]
    [Authorize(Roles = "user, admin")]
    public class CitizensAppealController : Controller
    {
        private int pageSize = 5;
        private Context db;
        private CitizensAppeal _citizensAppeal = new CitizensAppeal
        {
            LFO = "",
            Organization = "",
            GoalOfRequest = ""
        };

        public CitizensAppealController(Context CitizensAppealContext)
        {
            db = CitizensAppealContext;
        }

        [HttpGet]
        public IActionResult Index(int page, SortState sortOrder)
        {
            CitizensAppeal sessionCitizensAppeal = HttpContext.Session.GetObject<CitizensAppeal>("CitizensAppeal");
            string sessionSortState = HttpContext.Session.GetString("SortStateCitizensAppeal");

            if (sessionCitizensAppeal != null)
            {
                _citizensAppeal = sessionCitizensAppeal;
            }

            if (sessionSortState != null)
                if (sortOrder == SortState.No)
                    sortOrder = (SortState)Enum.Parse(typeof(SortState), sessionSortState);

            ViewData["NameSort"] = sortOrder == SortState.NameDesc ? SortState.NameAsc : SortState.NameDesc;
            HttpContext.Session.SetString("SortState", sortOrder.ToString());
            IQueryable<CitizensAppeal> CitizensAppeals = Sort(db.CitizensAppeals, sortOrder,
                _citizensAppeal.LFO, (int)page);

            PageViewModel pageViewModel = new PageViewModel(NoteCount(db.CitizensAppeals,
                _citizensAppeal.LFO), page, pageSize);
            foreach (var item in CitizensAppeals)
            {
                item.ScheduleForWeek = db.SchedulesForWeek.Where(o => o.ScheduleForWeekID == item.ScheduleForWeekID).FirstOrDefault();
            }
            CitizensAppealsViewModel CitizensAppealsView = new CitizensAppealsViewModel
            {
                CitizensAppealViewModel = _citizensAppeal,
                PageViewModel = CitizensAppeals,
                PageNumber = (int)page
            };
            return View(CitizensAppealsView);
        }

        [HttpPost]
        public IActionResult Index(CitizensAppeal citizensAppeal)
        {
            var sessionSortState = HttpContext.Session.GetString("SortStateCitizensAppeal");
            SortState sortOrder = new SortState();
            if (sessionSortState != null)
                sortOrder = (SortState)Enum.Parse(typeof(SortState), sessionSortState);

            int? page = HttpContext.Session.GetInt32("CitizensAppealPage");
            if (page == null)
            {
                page = 0;
                HttpContext.Session.SetInt32("CitizensAppealPage", 0);
            }

            IQueryable<CitizensAppeal> citizensAppeals = Sort(db.CitizensAppeals, sortOrder,
                citizensAppeal.LFO, (int)page);
            HttpContext.Session.SetObject("CitizensAppeal", citizensAppeal);

            CitizensAppealsViewModel citizensAppealsView = new CitizensAppealsViewModel
            {
                CitizensAppealViewModel = citizensAppeal,
                PageViewModel = citizensAppeals,
                PageNumber = (int)page
            };

            return View(citizensAppealsView);
        }

        private IQueryable<CitizensAppeal> Sort(IQueryable<CitizensAppeal> CitizensAppeals,
            SortState sortOrder, string name, int page)
        {
            switch (sortOrder)
            {
                case SortState.NameAsc:
                    CitizensAppeals = CitizensAppeals.OrderBy(s => s.LFO);
                    break;
                case SortState.NameDesc:
                    CitizensAppeals = CitizensAppeals.OrderByDescending(s => s.LFO);
                    break;
            }
            CitizensAppeals = CitizensAppeals.Where(o => o.LFO.Contains(name ?? ""))
                .Skip(page * pageSize).Take(pageSize);
            return CitizensAppeals;
        }
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = db.CitizensAppeals.Where(p => p.CitizensAppealID == id).Single();
            if (treatment == null)
            {
                return NotFound();
            }

            return View(treatment);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var citizensAppeal = db.CitizensAppeals.Where(p => p.CitizensAppealID == id).Single();
            if (citizensAppeal == null)
            {
                return NotFound();
            }

            return View(citizensAppeal);
        }

        [HttpPost]
        public IActionResult Edit(CitizensAppeal citizensAppeal)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(citizensAppeal);
                    db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CitizensAppealExists(citizensAppeal.CitizensAppealID))
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
            return View(citizensAppeal);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var citizensAppeal = db.CitizensAppeals.Where(p => p.CitizensAppealID == id).FirstOrDefault(); ;
            if (citizensAppeal == null)
            {
                return NotFound();
            }

            try
            {
                db.CitizensAppeals.Remove(citizensAppeal);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CitizensAppealExists(citizensAppeal.CitizensAppealID))
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
        public async Task<IActionResult> Add(CitizensAppeal citizensAppeal, ScheduleForWeek scheduleForWeek)
        {
            citizensAppeal.ScheduleForWeekID = db.SchedulesForWeek.Where(o => o.StartTime == scheduleForWeek.StartTime).FirstOrDefault().ScheduleForWeekID;
            if (citizensAppeal == null)
            {
                return View();
            }

            try
            {
                db.CitizensAppeals.Add(citizensAppeal);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CitizensAppealExists(citizensAppeal.CitizensAppealID))
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

        private bool CitizensAppealExists(int id)
        {
            return db.CitizensAppeals.Any(e => e.CitizensAppealID == id);
        }

        private int NoteCount(IQueryable<CitizensAppeal> citizensAppeals, string name)
        {
            return citizensAppeals.Where(o => o.LFO.Contains(name ?? ""))
                .Count();
        }
    }
}