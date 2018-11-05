using System;
using Microsoft.AspNetCore.Mvc;
using BethanysPieShop.Models;
using BethanysPieShop.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;

namespace BethanysPieShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPieRepository _pieRepository;
        private readonly IStringLocalizer<HomeController> _stringLocalizer;

        public HomeController(IPieRepository pieRepository, IStringLocalizer<HomeController> stringLocalizer)
        {
            _pieRepository = pieRepository;
            _stringLocalizer = stringLocalizer;
        }

        public ViewResult Index()
        {
            throw new Exception("test");
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
                Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );

                return LocalRedirect(returnUrl);
            }
        }
    }