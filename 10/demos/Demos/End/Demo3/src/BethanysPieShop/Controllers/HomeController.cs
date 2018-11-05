using System;
using BethanysPieShop.Filters;
using Microsoft.AspNetCore.Mvc;
using BethanysPieShop.Models;
using BethanysPieShop.Utility;
using BethanysPieShop.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using Newtonsoft.Json;

namespace BethanysPieShop.Controllers
{
    //[RequireHeader]
    [ServiceFilter(typeof(TimerAction))]
    //[TimerAction]
    public class HomeController : Controller
    {
        private readonly IPieRepository _pieRepository;
        private readonly IStringLocalizer<HomeController> _stringLocalizer;
        private readonly ILogger<HomeController> _logger;
        private IMemoryCache _memoryCache;
        private IDistributedCache _distributedCache;

        public HomeController(IPieRepository pieRepository, IStringLocalizer<HomeController> stringLocalizer, ILogger<HomeController> logger, IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            _pieRepository = pieRepository;
            _stringLocalizer = stringLocalizer;
            _logger = logger;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }

        public ViewResult Index()
        {
            //Serilog
            _logger.LogDebug("Loading home page");
            
            //Application Insights
            TelemetryClient tc = new TelemetryClient();
            tc.TrackPageView(new PageViewTelemetry("Insights: Bethany's Home page loaded") { Timestamp = DateTime.UtcNow });
            tc.TrackEvent("HomeControllerLoad");

            //Logic for action method
            ViewBag.PageTitle = _stringLocalizer["PageTitle"];


            //distributed caching
            List<Pie> piesOfTheWeekDistributedCache = LoadPiesFromRepoOrCache(); 

            var homeViewModel = new HomeViewModel
            {
                PiesOfTheWeek = piesOfTheWeekDistributedCache
            };

            return View(homeViewModel);
        }

        private List<Pie> LoadPiesFromRepoOrCache()
        {
            var cachedPiesString = _distributedCache.Get("allPies");
            if (cachedPiesString == null) //we need to load it into cache (again)
            {
                var pies = _pieRepository.PiesOfTheWeek;
                string serializedPiesString = JsonConvert.SerializeObject(pies);
                byte[] encodedPies = Encoding.UTF8.GetBytes(serializedPiesString);
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(1000));
                _distributedCache.Set("allPies", encodedPies, cacheEntryOptions);
                return pies.ToList();
            }
            else //get from cache
            {
                byte[] encodedPies = _distributedCache.Get("allPies");
                string serializedPiesString = Encoding.UTF8.GetString(encodedPies);
                List<Pie> pies = JsonConvert.DeserializeObject<List<Pie>>(serializedPiesString);

                return pies;
            }
        }

        private void FillCacheAgain(object key, object value, EvictionReason reason, object state)
        {
            _logger.LogInformation(LogEventIds.LoadHomepage, "Cache was cleared: reason " + reason.ToString());
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