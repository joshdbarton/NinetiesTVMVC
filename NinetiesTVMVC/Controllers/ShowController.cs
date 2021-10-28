using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NinetiesTVMVC.Models;
using NinetiesTVMVC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NinetiesTVMVC.Controllers
{
    public class ShowController : Controller
    {

        private IShowRepository _showRepo;

        public ShowController(IShowRepository showRepo)
        {
            _showRepo = showRepo;
        }

        // GET: Show?search=<searchterm>&genreId=<id>&page=<pageNumber>
        public ActionResult Index(string search, int genreId, int page)
        {
            //using the default pageSize of 5
            var vm = _showRepo.Get(search, genreId, page == 0 ? 1 : page);
           // if we had to adjust the page based on total results, redirect using the new page
            if (page != vm.Page)
            {
                return RedirectToAction(nameof(Index), new { search, genreId, page = vm.Page });
            }

            return View(vm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(SearchShowsViewModel vm)
        {
            //use posted form data to redirect to Get method for index,
            // passing the form data as query params
            return RedirectToAction(nameof(Index), new
            {
                search = vm.QueryString,
                genreId = vm.GenreId,
                page = vm.Page
            });
        }

        // GET: Show/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
    }
}
