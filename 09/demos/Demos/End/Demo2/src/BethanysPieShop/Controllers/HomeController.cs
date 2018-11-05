using System;
using Microsoft.AspNetCore.Mvc;
using BethanysPieShop.Models;
using BethanysPieShop.Utility;
using BethanysPieShop.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace BethanysPieShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPieRepository _pieRepository;
        private readonly IStringLocalizer<HomeController> _stringLocalizer;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IPieRepository pieRepository, 
            IStringLocalizer<HomeController> stringLocalizer, 
            ILogger<HomeController> logger)
        {
            _pieRepository = pieRepository;
            _stringLocalizer = stringLocalizer;
            _logger = logger;
        }

        public ViewResult Index()
        {
            //Logging
            _logger.LogInformation(LogEventIds.LoadHomepage, "Loading home page");


            //ViewData["PageTitle"] = _stringLocalizer["Welcome to Bethany's Pie Shop"];
            ViewBag.PageTitle = _stringLocalizer["PageTitle"];
            //ViewData["PiesOfTheWeek"] = _stringLocalizer["PiesOfTheWeek"];

            var homeViewModel = new HomeViewModel
            {
                PiesOfTheWeek = _pieRepository.PiesOfTheWeek
            };

            return View(homeViewModel);
        }

        public IActionResult TestUrl()
        {
            // Generates /Pie/Details/1		
            var url =
                Url.Action("Details", "Pie", new { id = 1 });
            //return Content(url);
            return RedirectToAction("Index");
        }


        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            //Logging
            _logger.LogInformation(LogEventIds.ChangeLanguage, "Language changed to {0}", culture);

            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );

            return LocalRedirect(returnUrl);
        }
    }
}