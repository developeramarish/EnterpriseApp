﻿using System;
using Microsoft.AspNetCore.Mvc;
using BethanysPieShop.Models;
using BethanysPieShop.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;

namespace BethanysPieShop.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly IPieRepository _pieRepository;
        private readonly IStringLocalizer<HomeController> _stringLocalizer;

        public HomeController(IPieRepository pieRepository, IStringLocalizer<HomeController> stringLocalizer)
        {
            _pieRepository = pieRepository;
            _stringLocalizer = stringLocalizer;
        }

        [Route("")]
        public ViewResult Index()
        {
            //ViewData["PageTitle"] = _stringLocalizer["Welcome to Bethany's Pie Shop"];
            ViewBag.PageTitle = _stringLocalizer["PageTitle"];
            //ViewData["PiesOfTheWeek"] = _stringLocalizer["PiesOfTheWeek"];

            var homeViewModel = new HomeViewModel
            {
                PiesOfTheWeek = _pieRepository.PiesOfTheWeek
            };

            return View(homeViewModel);
        }

        [Route("[controller]/SetLanguage")]
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